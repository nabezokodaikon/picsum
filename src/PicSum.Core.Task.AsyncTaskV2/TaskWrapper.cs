using NLog;
using System.Collections.Concurrent;
using System.Windows.Forms;

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
        private long waitUIThread = 0;

        private bool WaitUIThread
        {
            get
            {
                return Interlocked.Read(ref this.waitUIThread) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this.waitUIThread, Convert.ToInt64(value));
            }
        }

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
            {
                throw new InvalidOperationException($"{this.taskInfo} 既にコールバックアクションが設定されています。");
            }

            this.callbackAction = action;
            return this;
        }

        public TwoWayTask<TTask, TTaskParameter, TTaskResult> Catch(Action<TaskException> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.catchAction != null)
            {
                throw new InvalidOperationException($"{this.taskInfo} 既に例外アクションが設定されています。");
            }

            this.catchAction = action;
            return this;
        }

        public TwoWayTask<TTask, TTaskParameter, TTaskResult> Complete(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.completeAction != null)
            {
                throw new InvalidOperationException($"{this.taskInfo} 既に完了アクションが設定されています。");
            }

            this.completeAction = action;
            return this;
        }

        public void StartThread()
        {
            if (this.thread != null)
            {
                throw new InvalidOperationException($"{this.taskInfo} 既にタスク実行スレッドが開始されています。");
            }

            this.thread = System.Threading.Tasks.Task.Run(() => this.DoWork(this.source.Token));
        }

        public void StartTask(TTaskParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

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

        public void Wait()
        {
            this.WaitUIThread = true;

            Logger.Debug("UIスレッドを待機します。");
            while (this.WaitUIThread)
            {
                Application.DoEvents();
            }
            Logger.Debug("UIスレッドの待機が解除されました。");
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

                        Logger.Debug($"{task.ID} を実行します。");

                        if (this.callbackAction != null)
                        {
                            task.CallbackAction = r =>
                            {
                                this.context.Post(state =>
                                {
                                    ArgumentNullException.ThrowIfNull(state, nameof(state));
                                    var result = (TTaskResult)state;
                                    this.callbackAction(result);
                                    this.WaitUIThread = false;

                                    if (this.WaitUIThread)
                                    {
                                        Logger.Debug($"{task.ID} UIスレッドの待機を解除します。");
                                        this.WaitUIThread = false;
                                    }
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

                                    if (this.WaitUIThread)
                                    {
                                        Logger.Debug($"{task.ID} UIスレッドの待機を解除します。");
                                        this.WaitUIThread = false;
                                    }
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

                                    if (this.WaitUIThread)
                                    {
                                        Logger.Debug($"{task.ID} UIスレッドの待機を解除します。");
                                        this.WaitUIThread = false;
                                    }
                                }, null);
                            };
                        }

                        try
                        {
                            task.ExecuteWrapper();

                            if (this.WaitUIThread)
                            {
                                Logger.Debug($"{task.ID} が終了するまで待機します。");
                                while (this.WaitUIThread)
                                {
                                    token.WaitHandle.WaitOne(1);
                                }

                                return;
                            }
                        }
                        catch (TaskCancelException)
                        {
                            Logger.Debug($"{task.ID} がキャンセルされました。");
                        }
                        finally
                        {
                            Logger.Debug($"{task.ID} が終了しました。");
                        }
                    }

                    token.WaitHandle.WaitOne(1);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Debug($"タスク実行スレッドをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "タスク実行スレッドで補足されない例外が発生しました。");
            }
            finally
            {
                Logger.Debug($"タスク実行スレッドが終了します。");
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
