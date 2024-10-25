using NLog;
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
        private TJob? currentJob = null;
        private Action<TJobResult>? callbackAction;
        private Action? cancelAction;
        private Action<JobException>? catchAction;
        private Action? completeAction;

        private TJob? CurrentJob
        {
            get
            {
                return Interlocked.CompareExchange(ref this.currentJob, null, null);
            }
            set
            {
                Interlocked.Exchange(ref this.currentJob, value);
            }
        }

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
            }

            this.disposed = true;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Callback(Action<TJobResult> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.callbackAction != null)
            {
                throw new InvalidOperationException($"{this.threadName}: 既にコールバックアクションが設定されています。");
            }

            this.callbackAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Cancel(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.cancelAction != null)
            {
                throw new InvalidOperationException($"{this.threadName}: 既にキャンセルアクションが設定されています。");
            }

            this.cancelAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Catch(Action<JobException> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.catchAction != null)
            {
                throw new InvalidOperationException($"{this.threadName}: 既に例外アクションが設定されています。");
            }

            this.catchAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Complete(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.completeAction != null)
            {
                throw new InvalidOperationException($"{this.threadName}: 既に完了アクションが設定されています。");
            }

            this.completeAction = action;
            return this;
        }

        public void StartJob(TJobParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.BeginCancel();

            this.CurrentJob = new TJob
            {
                Parameter = parameter
            };
        }

        public void StartJob()
        {
            this.BeginCancel();

            this.CurrentJob = new TJob();
        }

        public void BeginCancel()
        {
            var job = this.CurrentJob;
            job?.BeginCancel();
        }

        public void WaitJobComplete()
        {
            var job = this.CurrentJob;
            if (job == null)
            {
                return;
            }

            Logger.Debug("ジョブの完了を待ちます。");
            while (!job.IsCompleted)
            {
                Thread.Sleep(1);
            }
            Logger.Debug("ジョブが完了しました。");
        }

        private void DoWork(CancellationToken token)
        {
            Thread.CurrentThread.Name = this.threadName;

            Logger.Debug("ジョブ実行スレッドが開始されました。");

            TJob? previewJob = null;

            try
            {
                while (true)
                {
                    var job = this.CurrentJob;
                    if (job == null)
                    {
                        if (token.IsCancellationRequested)
                        {
                            Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                            token.ThrowIfCancellationRequested();
                        }

                        Thread.Sleep(1);
                        continue;
                    }

                    if (job.ID == null)
                    {
                        throw new NullReferenceException($"{this.threadName}: ジョブIDがNullです。");
                    }

                    if (previewJob == job)
                    {
                        if (token.IsCancellationRequested)
                        {
                            Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                            token.ThrowIfCancellationRequested();
                        }

                        Thread.Sleep(1);
                        continue;
                    }

                    previewJob = job;

                    if (this.callbackAction != null)
                    {
                        job.CallbackAction = r =>
                        {
                            SynchronizationContextWrapper.Instance.Post(() =>
                            {
                                this.callbackAction(r);
                            });
                        };
                    }

                    if (this.cancelAction != null)
                    {
                        job.CancelAction = () =>
                        {
                            SynchronizationContextWrapper.Instance.Post(() =>
                            {
                                this.cancelAction();
                            });
                        };
                    }

                    if (this.catchAction != null)
                    {
                        job.CatchAction = e =>
                        {
                            SynchronizationContextWrapper.Instance.Post(() =>
                            {
                                this.catchAction(e);
                            });
                        };
                    }

                    if (this.completeAction != null)
                    {
                        job.CompleteAction = () =>
                        {
                            SynchronizationContextWrapper.Instance.Post(() =>
                            {
                                this.completeAction();
                            });
                        };
                    }

                    Logger.Debug($"{job.ID} を実行します。");
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        job.ExecuteWrapper();
                    }
                    catch (JobCancelException)
                    {
                        job.CancelAction?.Invoke();
                        Logger.Debug($"{job.ID} がキャンセルされました。");
                    }
                    catch (JobException ex)
                    {
                        Logger.Error($"{job.ID} {ex}");
                        job.CatchAction?.Invoke(ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"{job.ID} で予期しない例外が発生しました。");
                        throw;
                    }
                    finally
                    {
                        job.CompleteAction?.Invoke();
                        job.IsCompleted = true;
                        sw.Stop();
                        Logger.Debug($"{job.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
                    }

                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                        token.ThrowIfCancellationRequested();
                    }

                    Thread.Sleep(1);
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
