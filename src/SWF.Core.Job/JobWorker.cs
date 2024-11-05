using NLog;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SWF.Core.Job
{
    public partial class TwoWayTask<TJob, TJobParameter, TJobResult>
        : IDisposable, ITwoWayJob<TJob, TJobParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;
        private readonly string threadName;
        private readonly SynchronizationContext context;
        private readonly ConcurrentQueue<TJob> jobQueue = new();
        private Task? thread = null;
        private CancellationTokenSource? source = new();
        private Action<TJobResult>? callbackAction;
        private Action? cancelAction;
        private Action<JobException>? catchAction;
        private Action? completeAction;

        public TwoWayTask(SynchronizationContext? context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            this.context = context;
            this.threadName = $"{typeof(TJob).Name} {ThreadID.GetNew()}";
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
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {

                if (this.thread != null)
                {
                    this.BeginCancel();

                    Logger.Debug("ジョブ実行タスクにキャンセルリクエストを送ります。");
                    this.source?.Cancel();

                    Logger.Debug("タスクの終了を待機します。");
                    this.thread.Wait();

                    Logger.Debug($"{this.threadName}: ジョブ実行タスクが終了しました。");
                }

                this.source?.Dispose();
                this.thread?.Dispose();
            }

            this.disposed = true;
        }

        public TwoWayTask<TJob, TJobParameter, TJobResult> Reset()
        {
            this.callbackAction = null;
            this.cancelAction = null;
            this.catchAction = null;
            this.completeAction = null;

            return this;
        }

        public TwoWayTask<TJob, TJobParameter, TJobResult> Callback(Action<TJobResult> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.callbackAction != null)
            {
                throw new InvalidOperationException("コールバックアクションが初期化されていません。");
            }

            this.callbackAction = action;
            return this;
        }

        public TwoWayTask<TJob, TJobParameter, TJobResult> Cancel(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.cancelAction != null)
            {
                throw new InvalidOperationException("キャンセルアクションが初期化されていません。");
            }

            this.cancelAction = action;
            return this;
        }

        public TwoWayTask<TJob, TJobParameter, TJobResult> Catch(Action<JobException> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.catchAction != null)
            {
                throw new InvalidOperationException("例外アクションが初期化されていません。");
            }

            this.catchAction = action;
            return this;
        }

        public TwoWayTask<TJob, TJobParameter, TJobResult> Complete(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.completeAction != null)
            {
                throw new InvalidOperationException("完了アクションが初期化されていません。");
            }

            this.completeAction = action;
            return this;
        }

        public void StartJob(ISender sender, TJobParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            var job = this.CreateJob();
            job.Sender = sender;
            job.Parameter = parameter;

            this.source ??= new();
            this.thread ??= Task.Run(() => this.DoWork(this.source.Token));
            this.jobQueue.Enqueue(job);
        }

        public void StartJob(ISender sender)
        {
            var job = this.CreateJob();
            job.Sender = sender;

            this.source ??= new();
            this.thread ??= Task.Run(() => this.DoWork(this.source.Token));
            this.jobQueue.Enqueue(job);
        }

        public TwoWayTask<TJob, TJobParameter, TJobResult> BeginCancel()
        {
            foreach (var job in this.jobQueue.ToArray())
            {
                job.BeginCancel();
            }

            return this;
        }

        public void WaitJobComplete()
        {
            Logger.Debug("ジョブキューの完了を待ちます。");

            foreach (var job in this.jobQueue.ToArray())
            {
                while (!job.IsCompleted)
                {
                    Thread.Sleep(1);
                }
            }

            Logger.Debug("ジョブキューが完了しました。");
        }

        private TJob CreateJob()
        {
            var job = new TJob();

            if (this.callbackAction != null)
            {
                var action = this.callbackAction;
                job.CallbackAction = result =>
                {
                    if (job.CanUIThreadAccess())
                    {
                        this.context.Post(state =>
                        {
#pragma warning disable CS8600
                            var objects = (object[])state;
#pragma warning restore CS8600
#pragma warning disable CS8602
                            var innerJob = (TJob)objects[0];
#pragma warning restore CS8602
                            var innerAction = (Action<TJobResult>)objects[1];
                            if (innerJob.CanUIThreadAccess())
                            {
                                innerAction.Invoke(result);
                            }
                        }, new object[] { job, action });
                    }
                };
            }

            if (this.cancelAction != null)
            {
                var action = this.cancelAction;
                job.CancelAction = () =>
                {
                    if (job.CanUIThreadAccess())
                    {
                        this.context.Post(state =>
                        {
#pragma warning disable CS8600
                            var objects = (object[])state;
#pragma warning restore CS8600
#pragma warning disable CS8602
                            var innerJob = (TJob)objects[0];
#pragma warning restore CS8602
                            var innerAction = (Action)objects[1];
                            if (innerJob.CanUIThreadAccess())
                            {
                                innerAction.Invoke();
                            }
                        }, new object[] { job, action });
                    }
                };
            }

            if (this.catchAction != null)
            {
                var action = this.catchAction;
                job.CatchAction = exception =>
                {
                    if (job.CanUIThreadAccess())
                    {
                        this.context.Post(state =>
                        {
#pragma warning disable CS8600
                            var objects = (object[])state;
#pragma warning restore CS8600
#pragma warning disable CS8602
                            var innerJob = (TJob)objects[0];
#pragma warning restore CS8602
                            var innerAction = (Action<JobException>)objects[1];
                            if (innerJob.CanUIThreadAccess())
                            {
                                innerAction.Invoke(exception);
                            }
                        }, new object[] { job, action });
                    }
                };
            }

            if (this.completeAction != null)
            {
                var action = this.completeAction;
                job.CompleteAction = () =>
                {
                    if (job.CanUIThreadAccess())
                    {
                        this.context.Post(state =>
                        {
#pragma warning disable CS8600
                            var objects = (object[])state;
#pragma warning restore CS8600
#pragma warning disable CS8602
                            var innerJob = (TJob)objects[0];
#pragma warning restore CS8602
                            var innerAction = (Action)objects[1];
                            if (innerJob.CanUIThreadAccess())
                            {
                                innerAction.Invoke();
                            }
                        }, new object[] { job, action });
                    }
                };
            }

            return job;
        }

        private void DoWork(CancellationToken token)
        {
            Thread.CurrentThread.Name = this.threadName;

            Logger.Debug("ジョブ実行タスクが開始されました。");

            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブ実行タスクにキャンセルリクエストがありました。");
                        token.ThrowIfCancellationRequested();
                    }

                    if (this.jobQueue.TryPeek(out var currentJob))
                    {
                        Logger.Debug($"{currentJob.ID} を実行します。");
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            currentJob.ExecuteWrapper();
                        }
                        catch (JobCancelException)
                        {
                            currentJob.CancelAction?.Invoke();
                            Logger.Debug($"{currentJob.ID} がキャンセルされました。");
                        }
                        catch (JobException ex)
                        {
                            Logger.Error($"{currentJob.ID} {ex}");
                            currentJob.CatchAction?.Invoke(ex);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, $"{currentJob.ID} で補足されない例外が発生しました。");
                            throw;
                        }
                        finally
                        {
                            currentJob.CompleteAction?.Invoke();
                            currentJob.IsCompleted = true;

                            if (this.jobQueue.TryDequeue(out var dequeueJob))
                            {
                                if (currentJob != dequeueJob)
                                {
#pragma warning disable CA2219
                                    throw new InvalidOperationException("キューからPeekしたジョブとDequeueしたジョブが一致しません。");
#pragma warning restore CA2219
                                }
                            }
                            else
                            {
#pragma warning disable CA2219
                                throw new InvalidOperationException("他のタスクでキューの操作が行われました。");
#pragma warning restore CA2219
                            }

                            sw.Stop();
                            Logger.Debug($"{currentJob.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
                        }
                    }
                    else
                    {
                        token.WaitHandle.WaitOne(1);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Debug("ジョブ実行タスクをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "ジョブ実行タスクで補足されない例外が発生しました。");
            }
            finally
            {
                Logger.Debug("ジョブ実行タスクが終了します。");
            }
        }
    }

    public sealed partial class TwoWayTask<TJob, TJobResult>
        : TwoWayTask<TJob, EmptyParameter, TJobResult>, ITwoWayJob<TJob, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {
        public TwoWayTask(SynchronizationContext? context)
            : base(context)
        {

        }
    }

    public sealed partial class OneWayTask<TJob>
        : TwoWayTask<TJob, EmptyParameter, EmptyResult>, IOneWayJob<TJob>
        where TJob : AbstractOneWayJob, new()
    {
        public OneWayTask(SynchronizationContext? context)
            : base(context)
        {

        }
    }

    public sealed partial class OneWayTask<TJob, TJobParameter>
        : TwoWayTask<TJob, TJobParameter, EmptyResult>, IOneWayJob<TJob, TJobParameter>
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : IJobParameter
    {
        public OneWayTask(SynchronizationContext? context)
            : base(context)
        {

        }
    }
}
