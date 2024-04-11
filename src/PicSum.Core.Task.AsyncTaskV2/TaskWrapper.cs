using NLog;
using System;
using System.Collections.Concurrent;

namespace PicSum.Core.Task.AsyncTaskV2
{
    public class TwoWayTask<TTask, TTaskParameter, TTaskResult>
        : IDisposable
        where TTask : AbstractTwoWayTask<TTaskParameter, TTaskResult>, new()
        where TTaskParameter : ITaskParameter
        where TTaskResult : ITaskResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;
        private readonly ThreadID threadID;
        private readonly Type taskType;
        private readonly string taskInfo;
        private readonly SynchronizationContext context;
        private readonly CancellationTokenSource source = new();
        private readonly ConcurrentQueue<TTask> taskQueue = new();
        private System.Threading.Tasks.Task? thread;
        private Action<TTaskResult>? callbackAction;
        private Action<TaskException>? catchAction;
        private Action? completeAction;

        public TwoWayTask()
        {
            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }

            if (SynchronizationContext.Current != null)
            {
                this.context = SynchronizationContext.Current;
            }
            else
            {
                throw new NullReferenceException("コンテキストがNullです。");
            }

            this.threadID = ThreadID.GetNew();
            this.taskType = typeof(TTask);
            this.taskInfo = $"{this.taskType.Name} {this.threadID}";
        }

        ~TwoWayTask()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.ClearQueue();

                    if (this.thread != null)
                    {
                        this.source.Cancel();
                        this.thread.Wait();
                        Logger.Debug($"{this.taskInfo} タスク実行スレッドが終了しました。");
                    }

                    this.source.Dispose();
                }

                this.thread = null;

                this.disposed = true;
            }
        }

        public TwoWayTask<TTask, TTaskParameter, TTaskResult> Callback(Action<TTaskResult> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.callbackAction != null)
                throw new InvalidOperationException($"{this.taskInfo} 既にコールバックアクションが設定されています。");

            this.callbackAction = action;
            return this;
        }

        public TwoWayTask<TTask, TTaskParameter, TTaskResult> Catch(Action<TaskException> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.catchAction != null)
                throw new InvalidOperationException($"{this.taskInfo} 既に例外アクションが設定されています。");

            this.catchAction = action;
            return this;
        }

        public TwoWayTask<TTask, TTaskParameter, TTaskResult> Complete(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.completeAction != null)
                throw new InvalidOperationException($"{this.taskInfo} 既に完了アクションが設定されています。");

            this.completeAction = action;
            return this;
        }

        public void StartThread()
        {
            if (this.thread != null)
                throw new InvalidOperationException($"{this.taskInfo} 既にタスク実行スレッドが開始されています。");

            this.thread = System.Threading.Tasks.Task.Run(() => this.DoWork(this.source.Token));
        }

        public void StartTask(TTaskParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            this.ClearQueue();

            var task = new TTask
            {
                ID = TaskID.GetNew(),
                Parameter = parameter
            };
            this.taskQueue.Enqueue(task);
        }

        public void StartTask()
        {
            this.ClearQueue();

            var task = new TTask
            {
                ID = TaskID.GetNew()
            };
            this.taskQueue.Enqueue(task);
        }

        public void BeginCancel()
        {
            this.ClearQueue();
        }

        private void ClearQueue()
        {
            while (this.taskQueue.TryDequeue(out var t))
            {
                t.BeginCancel();
            }
        }

        private void DoWork(CancellationToken token)
        {
            Thread.CurrentThread.Name = this.taskInfo;

            Logger.Debug("タスク実行スレッドが開始されました。");

            TTask? previewTask = null;

            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("タスク実行スレッドにキャンセルリクエストがありました。");
                        token.ThrowIfCancellationRequested();
                    }

                    if (this.taskQueue.TryPeek(out var task))
                    {
                        if (previewTask == task)
                        {
                            token.WaitHandle.WaitOne(1);
                            continue;
                        }

                        previewTask = task;

                        if (task.ID == null)
                            throw new NullReferenceException(nameof(task.ID));

                        Logger.Debug($"{task.ID} タスクを実行します。");

                        if (this.callbackAction != null)
                        {
                            task.CallbackAction = r =>
                            {
                                this.context.Post(state =>
                                {
                                    ArgumentNullException.ThrowIfNull(state, nameof(state));
                                    var result = (TTaskResult)state;
                                    this.callbackAction(result);
                                }, r);
                            };
                        }

                        if (this.catchAction != null)
                        {
                            task.CatchAction = e =>
                            {
                                this.context.Post(state =>
                                {
                                    ArgumentNullException.ThrowIfNull(state, nameof(state));
                                    var ex = (TaskException)state;
                                    this.catchAction(ex);
                                }, e);
                            };
                        }

                        if (this.completeAction != null)
                        {
                            task.CompleteAction = () =>
                            {
                                this.context.Post(state =>
                                {
                                    this.completeAction();
                                }, null);
                            };
                        }

                        try
                        {
                            task.ExecuteWrapper();
                        }
                        catch (TaskCancelException)
                        {
                            Logger.Debug($"{task.ID}  タスクがキャンセルされました。");
                        }
                        finally
                        {
                            Logger.Debug($"{task.ID}  タスクが終了しました。");
                        }
                    }

                    token.WaitHandle.WaitOne(1);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Debug($"{this.taskInfo} タスク実行スレッドをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Debug($"{this.taskInfo} タスク実行スレッドで補足されない例外が発生しました。: {ex.Message}");
            }
            finally
            {
                Logger.Debug($"{this.taskInfo} タスク実行スレッドが終了します。");
            }
        }
    }

    public sealed class TwoWayTask<TTask, TTaskResult>
        : TwoWayTask<TTask, EmptyParameter, TTaskResult>
        where TTask : AbstractTwoWayTask<TTaskResult>, new()
        where TTaskResult : ITaskResult
    {
        public TwoWayTask()
            : base()
        {

        }
    }

    public sealed class OneWayTask<TTask>
        : TwoWayTask<TTask, EmptyParameter, EmptyResult>
        where TTask : AbstractOneWayTask, new()
    {
        public OneWayTask()
            : base()
        {

        }
    }

    public sealed class OneWayTask<TTask, TTaskParameter>
        : TwoWayTask<TTask, TTaskParameter, EmptyResult>
        where TTask : AbstractOneWayTask<TTaskParameter>, new()
        where TTaskParameter : ITaskParameter
    {
        public OneWayTask()
            : base()
        {

        }
    }
}
