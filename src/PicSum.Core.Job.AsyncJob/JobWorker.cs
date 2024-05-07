using NLog;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace PicSum.Core.Job.AsyncJob
{
    public class TwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;
        private readonly ThreadID threadID;
        private readonly Type jobType;
        private readonly string jobInfo;
        private readonly SynchronizationContext context;
        private readonly CancellationTokenSource source = new();
        private readonly ConcurrentQueue<TJob> jobQueue = new();
        private Task? thread;
        private Action<TJobResult>? callbackAction;
        private Action<JobException>? catchAction;
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

        public TwoWayJob()
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
            this.jobType = typeof(TJob);
            this.jobInfo = $"{this.jobType.Name} {this.threadID}";
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
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.ClearQueue();

                    if (this.thread != null)
                    {
                        this.source.Cancel();
                        this.thread.Wait();
                        Logger.Debug($"{this.jobInfo} ジョブ実行スレッドが終了しました。");
                    }

                    this.source.Dispose();
                }

                this.thread = null;

                this.disposed = true;
            }
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Callback(Action<TJobResult> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.callbackAction != null)
            {
                throw new InvalidOperationException($"{this.jobInfo} 既にコールバックアクションが設定されています。");
            }

            this.callbackAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Catch(Action<JobException> action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.catchAction != null)
            {
                throw new InvalidOperationException($"{this.jobInfo} 既に例外アクションが設定されています。");
            }

            this.catchAction = action;
            return this;
        }

        public TwoWayJob<TJob, TJobParameter, TJobResult> Complete(Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));

            if (this.completeAction != null)
            {
                throw new InvalidOperationException($"{this.jobInfo} 既に完了アクションが設定されています。");
            }

            this.completeAction = action;
            return this;
        }

        public void StartThread()
        {
            if (this.thread != null)
            {
                throw new InvalidOperationException($"{this.jobInfo} 既にジョブ実行スレッドが開始されています。");
            }

            this.thread = System.Threading.Tasks.Task.Run(() => this.DoWork(this.source.Token));
        }

        public void StartJob(TJobParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.ClearQueue();
            var job = new TJob
            {
                Parameter = parameter
            };
            this.jobQueue.Enqueue(job);
        }

        public void StartJob()
        {
            this.ClearQueue();

            var job = new TJob();
            this.jobQueue.Enqueue(job);
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
            while (this.jobQueue.TryDequeue(out var t))
            {
                t.BeginCancel();
            }
        }

        private void DoWork(CancellationToken token)
        {
            Thread.CurrentThread.Name = this.jobInfo;

            Logger.Debug("ジョブ実行スレッドが開始されました。");

            TJob? previewJob = null;

            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                        token.ThrowIfCancellationRequested();
                    }

                    if (this.jobQueue.TryPeek(out var job))
                    {
                        if (previewJob == job)
                        {
                            token.WaitHandle.WaitOne(1);
                            continue;
                        }

                        previewJob = job;

                        if (job.ID == null)
                            throw new NullReferenceException(nameof(job.ID));

                        Logger.Debug($"{job.ID} を実行します。");

                        if (this.callbackAction != null)
                        {
                            job.CallbackAction = r =>
                            {
                                this.context.Post(state =>
                                {
                                    ArgumentNullException.ThrowIfNull(state, nameof(state));
                                    var result = (TJobResult)state;
                                    this.callbackAction(result);
                                    this.WaitUIThread = false;

                                    if (this.WaitUIThread)
                                    {
                                        Logger.Debug($"{job.ID} UIスレッドの待機を解除します。");
                                        this.WaitUIThread = false;
                                    }
                                }, r);
                            };
                        }

                        if (this.catchAction != null)
                        {
                            job.CatchAction = e =>
                            {
                                this.context.Post(state =>
                                {
                                    ArgumentNullException.ThrowIfNull(state, nameof(state));
                                    var ex = (JobException)state;
                                    this.catchAction(ex);

                                    if (this.WaitUIThread)
                                    {
                                        Logger.Debug($"{job.ID} UIスレッドの待機を解除します。");
                                        this.WaitUIThread = false;
                                    }
                                }, e);
                            };
                        }

                        if (this.completeAction != null)
                        {
                            job.CompleteAction = () =>
                            {
                                this.context.Post(state =>
                                {
                                    this.completeAction();

                                    if (this.WaitUIThread)
                                    {
                                        Logger.Debug($"{job.ID} UIスレッドの待機を解除します。");
                                        this.WaitUIThread = false;
                                    }
                                }, null);
                            };
                        }

                        try
                        {
                            job.ExecuteWrapper();

                            if (this.WaitUIThread)
                            {
                                Logger.Debug($"{job.ID} が終了するまで待機します。");
                                while (this.WaitUIThread)
                                {
                                    token.WaitHandle.WaitOne(1);
                                }

                                return;
                            }
                        }
                        catch (JobCancelException)
                        {
                            Logger.Debug($"{job.ID} がキャンセルされました。");
                        }
                        finally
                        {
                            Logger.Debug($"{job.ID} が終了しました。");
                        }
                    }

                    token.WaitHandle.WaitOne(1);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Debug($"ジョブ実行スレッドをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "ジョブ実行スレッドで補足されない例外が発生しました。");
            }
            finally
            {
                Logger.Debug($"ジョブ実行スレッドが終了します。");
            }
        }
    }

    public sealed class TwoWayJob<TJob, TJobResult>
        : TwoWayJob<TJob, EmptyParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {
        public TwoWayJob()
            : base()
        {

        }
    }

    public sealed class OneWayJob<TJob>
        : TwoWayJob<TJob, EmptyParameter, EmptyResult>
        where TJob : AbstractOneWayJob, new()
    {
        public OneWayJob()
            : base()
        {

        }
    }

    public sealed class OneWayJob<TJob, TJobParameter>
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
