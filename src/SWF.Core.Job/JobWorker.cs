using NLog;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SWF.Core.Job
{
    public partial class TwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;
        private readonly string threadName;
        private readonly CancellationTokenSource source = new();
        private readonly Task thread;
        private readonly ConcurrentQueue<TJob> jobQueue = new();
        private Action<TJobResult>? callbackAction;
        private Action? cancelAction;
        private Action<JobException>? catchAction;
        private Action? completeAction;

        public TwoWayJob()
        {
            this.threadName = $"{typeof(TJob).Name} {ThreadID.GetNew()}";
            this.thread = Task.Run(() => this.DoWork(this.source.Token));
        }

        ~TwoWayJob()
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

                Logger.Debug("UIスレッドを待機します。");
                this.thread.Wait();

                Logger.Debug($"{this.threadName}: ジョブ実行スレッドが終了しました。");

                this.source.Dispose();
                this.thread.Dispose();
            }

            this.disposed = true;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Initialize()
        {
            this.callbackAction = null;
            this.cancelAction = null;
            this.catchAction = null;
            this.completeAction = null;

            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Callback(Action<TJobResult> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.callbackAction != null)
            {
                throw new InvalidOperationException("コールバックアクションが初期化されていません。");
            }

            this.callbackAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Cancel(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.cancelAction != null)
            {
                throw new InvalidOperationException("キャンセルアクションが初期化されていません。");
            }

            this.cancelAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Catch(Action<JobException> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.catchAction != null)
            {
                throw new InvalidOperationException("例外アクションが初期化されていません。");
            }

            this.catchAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Complete(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.completeAction != null)
            {
                throw new InvalidOperationException("完了アクションが初期化されていません。");
            }

            this.completeAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> StartJob(ISender sender, TJobParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            var job = this.CreateJob();
            job.Sender = sender;
            job.Parameter = parameter;

            this.jobQueue.Enqueue(job);

            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> StartJob(ISender sender)
        {
            var job = this.CreateJob();
            job.Sender = sender;

            this.jobQueue.Enqueue(job);

            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> BeginCancel()
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
                var innerAction = this.callbackAction;
                job.CallbackAction = _ =>
                {
                    UIThreadAccessor.Instance.Post(job, () =>
                    {
                        innerAction.Invoke(_);
                    });
                };
            }

            if (this.cancelAction != null)
            {
                var innerAction = this.cancelAction;
                job.CancelAction = () =>
                {
                    UIThreadAccessor.Instance.Post(job, () =>
                    {
                        innerAction.Invoke();
                    });
                };
            }

            if (this.catchAction != null)
            {
                var innerAction = this.catchAction;
                job.CatchAction = _ =>
                {
                    UIThreadAccessor.Instance.Post(job, () =>
                    {
                        innerAction.Invoke(_);
                    });
                };
            }

            if (this.completeAction != null)
            {
                var innerAction = this.completeAction;
                job.CompleteAction = () =>
                {
                    UIThreadAccessor.Instance.Post(job, () =>
                    {
                        innerAction.Invoke();
                    });
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
                    if (!this.jobQueue.TryPeek(out var currentJob))
                    {
                        if (token.IsCancellationRequested)
                        {
                            Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                            token.ThrowIfCancellationRequested();
                        }

                        token.WaitHandle.WaitOne(1);
                        continue;
                    }

                    if (currentJob.ID == null)
                    {
                        throw new NullReferenceException($"{this.threadName}: ジョブIDがNullです。");
                    }

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
                        Logger.Error(ex, $"{currentJob.ID} で予期しない例外が発生しました。");
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
                                throw new InvalidOperationException("キューからPeekしたジョブとDequeueしたジョブが一致しません。");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("他のスレッドでキューの操作が行われました。");
                        }

                        sw.Stop();
                        Logger.Debug($"{currentJob.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
                    }

                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                        token.ThrowIfCancellationRequested();
                    }

                    token.WaitHandle.WaitOne(1);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Debug("ジョブ実行スレッドをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "ジョブ実行スレッドで補足されない例外が発生しました。");
            }
            finally
            {
                Logger.Debug("ジョブ実行スレッドが終了します。");
            }
        }
    }

    public sealed partial class TwoWayJob<TJob, TJobResult>
        : TwoWayJob<TJob, EmptyParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {
        public TwoWayJob()
            : base()
        {

        }
    }

    public sealed partial class OneWayJob<TJob>
        : TwoWayJob<TJob, EmptyParameter, EmptyResult>
        where TJob : AbstractOneWayJob, new()
    {
        public OneWayJob()
            : base()
        {

        }
    }

    public sealed partial class OneWayJob<TJob, TJobParameter>
        : TwoWayJob<TJob, TJobParameter, EmptyResult>
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : IJobParameter
    {
        public OneWayJob()
            : base()
        {

        }
    }
}
