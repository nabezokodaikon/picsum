using NLog;
using System.Collections.Concurrent;

namespace PicSum.Core.Task.AsyncTaskV2
{
    public class TaskWrapper<TTask, TTaskParameter, TTaskResult>
        : IDisposable
        where TTask : AbstractAsyncTask<TTaskParameter, TTaskResult>, new()
        where TTaskParameter : AbstractTaskParameter
        where TTaskResult : AbstractTaskResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;
        private readonly SynchronizationContext context;
        private readonly CancellationTokenSource source = new();
        private readonly ConcurrentQueue<TTask> taskQueue = new();
        private System.Threading.Tasks.Task? thread;
        private Action<TTaskResult>? callbackAction;
        private Action<Exception>? catchAction;
        private Action? completeAction;

        public TaskWrapper()
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
        }

        ~TaskWrapper()
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
                        Logger.Debug("スレッドが終了しました。");
                    }

                    this.source.Dispose();
                }

                this.thread = null;

                this.disposed = true;
            }
        }

        public TaskWrapper<TTask, TTaskParameter, TTaskResult> Callback(Action<TTaskResult> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (this.callbackAction != null)
                throw new InvalidOperationException("既にコールバックアクションが設定されています。");

            this.callbackAction = action;
            return this;
        }

        public TaskWrapper<TTask, TTaskParameter, TTaskResult> Catch(Action<Exception> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (this.catchAction != null)
                throw new InvalidOperationException("既に例外アクションが設定されています。");

            this.catchAction = action;
            return this;
        }

        public TaskWrapper<TTask, TTaskParameter, TTaskResult> Complete(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (this.completeAction != null)
                throw new InvalidOperationException("既に完了アクションが設定されています。");

            this.completeAction = action;
            return this;
        }

        public void StartThread()
        {
            if (this.thread != null)
                throw new InvalidOperationException("既にタスク実行スレッドが開始されています。");

            this.thread = System.Threading.Tasks.Task.Run(() => this.DoWork(this.source.Token));
        }

        public void StartTask(TTaskParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            this.ClearQueue();

            var task = new TTask();
            task.ID = new();
            task.Parameter = parameter;
            this.taskQueue.Enqueue(task);
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

                        Logger.Debug($"タスクを実行します。タスクID: {task.ID}, タスク: {task.GetType().Name}");

                        task.WaitAction = () => token.WaitHandle.WaitOne(1);

                        if (this.callbackAction != null)
                        {
                            task.CallbackAction = r =>
                            {
                                this.context.Post(state =>
                                {
                                    if (state == null)
                                        throw new ArgumentNullException(nameof(state));
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
                                    if (state == null)
                                        throw new ArgumentNullException(nameof(state));
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
                            Logger.Debug($"タスクがキャンセルされました。タスクID: {task.ID}, タスク: {task.GetType().Name}");
                        }
                        finally
                        {
                            Logger.Debug($"タスクが完了しました。タスクID: {task.ID}, タスク: {task.GetType().Name}");
                        }
                    }

                    token.WaitHandle.WaitOne(1);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Debug("タスク実行スレッドをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Debug($"タスク実行スレッドで補足されない例外が発生しました。: {ex.Message}");
            }
            finally
            {
                Logger.Debug("タスク実行スレッドが終了します。");
            }
        }
    }

    public sealed class TaskWrapper<TTask, TTaskParameter>
        : TaskWrapper<TTask, TTaskParameter, TaskEmptyResult>
        where TTask : AbstractAsyncTask<TTaskParameter>, new()
        where TTaskParameter : AbstractTaskParameter
    {
        public TaskWrapper()
            : base()
        {

        }
    }
}
