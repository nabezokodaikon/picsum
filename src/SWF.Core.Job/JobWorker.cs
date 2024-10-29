using NLog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Forms;

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
        private Control? currentSender = null;
        private Action<TJobResult>? callbackAction;
        private Action? cancelAction;
        private Action<JobException>? catchAction;
        private Action? completeAction;

        private Control? CurrentSender
        {
            get
            {
                return Interlocked.CompareExchange(ref this.currentSender, null, null);
            }
            set
            {
                Interlocked.Exchange(ref this.currentSender, value);
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
                this.thread.Dispose();
            }

            this.disposed = true;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> SetCurrentSender(Control sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.CurrentSender = sender;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Callback(Action<TJobResult> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            this.callbackAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Cancel(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            this.cancelAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Catch(Action<JobException> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            this.catchAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Complete(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            this.completeAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> StartJob(Control sender, TJobParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (!sender.Equals(this.CurrentSender))
            {
                throw new InvalidOperationException("コンテキストが一致しません。");
            }

            this.BeginCancel();

            var job = new TJob
            {
                Sender = sender,
                Parameter = parameter,
            };

            this.jobQueue.Enqueue(job);

            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> StartJob(Control sender)
        {
            if (!sender.Equals(this.CurrentSender))
            {
                throw new InvalidOperationException("コンテキストが一致しません。");
            }

            this.BeginCancel();

            var job = new TJob
            {
                Sender = sender,
            };

            this.jobQueue.Enqueue(job);

            return this;
        }

        public void BeginCancel()
        {
            while (this.jobQueue.TryDequeue(out var job))
            {
                job.BeginCancel();
            }
        }

        public void WaitJobComplete()
        {
            Logger.Debug("ジョブキューの完了を待ちます。");
            while (this.jobQueue.TryPeek(out var job))
            {
                if (job.IsStarted)
                {
                    if (this.jobQueue.TryDequeue(out var dequeueJob))
                    {
                        if (job != dequeueJob)
                        {
                            throw new InvalidOperationException("TryPeekしたジョブとDequeueしたジョブが一致しません。");
                        }

                        while (!job.IsCompleted)
                        {
                            Thread.Sleep(1);
                        }
                    }
                }
            }
            Logger.Debug("ジョブキューが完了しました。");
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
                    if (!jobQueue.TryPeek(out var currentJob))
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

                    if (previewJob == currentJob)
                    {
                        if (token.IsCancellationRequested)
                        {
                            Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                            token.ThrowIfCancellationRequested();
                        }

                        token.WaitHandle.WaitOne(1);
                        continue;
                    }

                    previewJob = currentJob;

                    currentJob.CallbackAction = r =>
                    {
                        UIThreadAccessor.Instance.Post(currentJob, this.CurrentSender, () =>
                        {
                            this.callbackAction?.Invoke(r);
                        });
                    };

                    currentJob.CancelAction = () =>
                    {
                        UIThreadAccessor.Instance.Post(currentJob, this.CurrentSender, () =>
                        {
                            this.cancelAction?.Invoke();
                        });
                    };

                    currentJob.CatchAction = e =>
                    {
                        UIThreadAccessor.Instance.Post(currentJob, this.CurrentSender, () =>
                        {
                            this.catchAction?.Invoke(e);
                        });
                    };

                    currentJob.CompleteAction = () =>
                    {
                        UIThreadAccessor.Instance.Post(currentJob, this.CurrentSender, () =>
                        {
                            this.completeAction?.Invoke();
                        });
                    };

                    Logger.Debug($"{currentJob.ID} を実行します。");
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        currentJob.IsStarted = true;
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
                        sw.Stop();
                        Logger.Debug($"{currentJob.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
                    }

                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                        token.ThrowIfCancellationRequested();
                    }

                    if (!this.jobQueue.TryDequeue(out var completeJob))
                    {
                        if (completeJob != null && currentJob != completeJob)
                        {
                            throw new InvalidOperationException("実行したジョブと完了したジョブが一致しません。");
                        }
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
