using NLog;
using SWF.Core.Base;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    public interface ITwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        public ITwoWayJob<TJob, TJobParameter, TJobResult> Reset();
        public ITwoWayJob<TJob, TJobParameter, TJobResult> Callback(Action<TJobResult> action);
        public ITwoWayJob<TJob, TJobParameter, TJobResult> Cancel(Action action);
        public ITwoWayJob<TJob, TJobParameter, TJobResult> Catch(Action<JobException> action);
        public ITwoWayJob<TJob, TJobParameter, TJobResult> Complete(Action action);
        public void StartJob(ISender sender, TJobParameter parameter);
        public void StartJob(ISender sender);
        public ITwoWayJob<TJob, TJobParameter, TJobResult> BeginCancel();
        public ITwoWayJob<TJob, TJobParameter, TJobResult> WaitJobComplete();
    }

    public interface ITwoWayJob<TJob, TJobResult>
        : ITwoWayJob<TJob, EmptyParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {

    }

    public interface IOneWayJob<TJob>
        : ITwoWayJob<TJob, EmptyParameter, EmptyResult>
        where TJob : AbstractOneWayJob, new()
    {

    }

    public interface IOneWayJob<TJob, TJobParameter>
        : ITwoWayJob<TJob, TJobParameter, EmptyResult>
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : IJobParameter
    {

    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class TwoWayThread<TJob, TJobParameter, TJobResult>
        : IDisposable, ITwoWayJob<TJob, TJobParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;

        private readonly string threadName;
        private readonly IThreadWrapper thread;
        private readonly SynchronizationContext context;
        private readonly ConcurrentQueue<TJob> jobQueue = new();
        private readonly CancellationTokenSource source = new();

        private Action<TJobResult>? callbackAction;
        private Action? cancelAction;
        private Action<JobException>? catchAction;
        private Action? completeAction;

        public TwoWayThread(SynchronizationContext? context, IThreadWrapper thread)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(thread, nameof(thread));

            this.context = context;
            this.thread = thread;
            this.threadName = $"{typeof(TJob).Name} {ThreadID.GetNew()}";
        }

        ~TwoWayThread()
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
                this.BeginCancel();

                Logger.Debug("ジョブ実行スレッドにキャンセルリクエストを送ります。");
                this.source.Cancel();

                Logger.Debug("ジョブ実行スレッドの終了を待機します。");
                this.thread.Wait();
                this.thread.Dispose();
                this.source.Dispose();

                Logger.Debug($"{this.threadName}: ジョブ実行スレッドが終了しました。");
            }

            this.disposed = true;
        }

        public ITwoWayJob<TJob, TJobParameter, TJobResult> Callback(Action<TJobResult> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.callbackAction != null)
            {
                throw new InvalidOperationException("コールバックアクションが初期化されていません。");
            }

            this.callbackAction = action;
            return this;
        }

        public ITwoWayJob<TJob, TJobParameter, TJobResult> Reset()
        {
            this.callbackAction = null;
            this.cancelAction = null;
            this.catchAction = null;
            this.completeAction = null;

            return this;
        }

        public ITwoWayJob<TJob, TJobParameter, TJobResult> Cancel(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.cancelAction != null)
            {
                throw new InvalidOperationException("キャンセルアクションが初期化されていません。");
            }

            this.cancelAction = action;
            return this;
        }

        public ITwoWayJob<TJob, TJobParameter, TJobResult> Catch(Action<JobException> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.catchAction != null)
            {
                throw new InvalidOperationException("例外アクションが初期化されていません。");
            }

            this.catchAction = action;
            return this;
        }

        public ITwoWayJob<TJob, TJobParameter, TJobResult> Complete(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.completeAction != null)
            {
                throw new InvalidOperationException("完了アクションが初期化されていません。");
            }

            this.completeAction = action;
            return this;
        }

        public ITwoWayJob<TJob, TJobParameter, TJobResult> BeginCancel()
        {
            foreach (var job in this.jobQueue.ToArray())
            {
                job.BeginCancel();
            }

            return this;
        }

        public ITwoWayJob<TJob, TJobParameter, TJobResult> WaitJobComplete()
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

            return this;
        }

        public void StartJob(ISender sender, TJobParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.thread.Start(() => this.DoWork(this.source.Token));

            var job = this.CreateJob();
            job.Sender = sender;
            job.Parameter = parameter;
            this.jobQueue.Enqueue(job);
        }

        public void StartJob(ISender sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.thread.Start(() => this.DoWork(this.source.Token));

            var job = this.CreateJob();
            job.Sender = sender;
            this.jobQueue.Enqueue(job);
        }

        protected TJob CreateJob()
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
                                try
                                {
                                    innerAction.Invoke(result);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex, $"{job.ID} がUIスレッド上で補足されない例外が発生しました。");
                                    ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                                }
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
                                try
                                {
                                    innerAction.Invoke();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex, $"{job.ID} がUIスレッド上で補足されない例外が発生しました。");
                                    ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                                }
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
                                try
                                {
                                    innerAction.Invoke(exception);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex, $"{job.ID} がUIスレッド上で補足されない例外が発生しました。");
                                    ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                                }
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
                                try
                                {
                                    innerAction.Invoke();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex, $"{job.ID} がUIスレッド上で補足されない例外が発生しました。");
                                    ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                                }
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

            Logger.Debug("ジョブ実行スレッドが開始されました。");

            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
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
                                throw new InvalidOperationException("他のスレッドでキューの操作が行われました。");
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
                Logger.Debug("ジョブ実行スレッドをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"ジョブ実行スレッドで補足されない例外が発生しました。");
            }
            finally
            {
                Logger.Debug("ジョブ実行スレッドが終了します。");
            }
        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class TwoWayThread<TJob, TJobResult>
        : TwoWayThread<TJob, EmptyParameter, TJobResult>, ITwoWayJob<TJob, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {
        public TwoWayThread(SynchronizationContext? context, IThreadWrapper thread)
            : base(context, thread)
        {

        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayThread<TJob, TJobParameter>
        : TwoWayThread<TJob, TJobParameter, EmptyResult>, IOneWayJob<TJob, TJobParameter>
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : IJobParameter
    {
        public OneWayThread(SynchronizationContext? context, IThreadWrapper thread)
            : base(context, thread)
        {

        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayThread<TJob>
        : TwoWayThread<TJob, EmptyParameter, EmptyResult>, IOneWayJob<TJob>
        where TJob : AbstractOneWayJob, new()
    {
        public OneWayThread(SynchronizationContext? context, IThreadWrapper thread)
            : base(context, thread)
        {

        }
    }
}
