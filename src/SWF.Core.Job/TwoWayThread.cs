using NLog;
using System.Diagnostics;

namespace SWF.Core.Job
{
    public partial class TwoWayThread<TJob, TJobParameter, TJobResult>
        : AbstractBackgroudProcess<TJob, TJobParameter, TJobResult>, IDisposable, ITwoWayJob<TJob, TJobParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;
        private Thread? thread = null;
        private long isThreadCancel = 0;

        private bool IsThreadCancel
        {
            get
            {
                return Interlocked.Read(ref this.isThreadCancel) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this.isThreadCancel, Convert.ToInt64(value));
            }
        }

        public TwoWayThread(SynchronizationContext? context)
            : base(context)
        {

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

                if (this.thread != null)
                {
                    this.BeginCancel();

                    Logger.Debug("ジョブ実行スレッドにキャンセルリクエストを送ります。");
                    this.IsThreadCancel = true;

                    Logger.Debug("スレッドの終了を待機します。");
                    this.thread.Join();

                    Logger.Debug($"{this.BackgroudProcessName}: ジョブ実行スレッドが終了しました。");
                }
            }

            this.disposed = true;
        }

        public override void StartJob(ISender sender, TJobParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            var job = this.CreateJob();
            job.Sender = sender;
            job.Parameter = parameter;

            if (this.thread == null)
            {
                this.thread = new(this.DoWork);
                this.thread.Priority = ThreadPriority.Highest;
                this.thread.Start();
            }

            this.JobQueue.Enqueue(job);
        }

        public override void StartJob(ISender sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var job = this.CreateJob();
            job.Sender = sender;

            if (this.thread == null)
            {
                this.thread = new Thread(() => this.DoWork());
                this.thread.Start();
            }

            this.JobQueue.Enqueue(job);
        }

        private void DoWork()
        {
            Thread.CurrentThread.Name = this.BackgroudProcessName;

            Logger.Debug("ジョブ実行スレッドが開始されました。");

            try
            {
                while (true)
                {
                    if (this.IsThreadCancel)
                    {
                        Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                        return;
                    }

                    if (this.JobQueue.TryPeek(out var currentJob))
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

                            if (this.JobQueue.TryDequeue(out var dequeueJob))
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
                        Thread.Sleep(1);
                    }
                }
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

    public sealed partial class TwoWayThread<TJob, TJobResult>
        : TwoWayThread<TJob, EmptyParameter, TJobResult>, ITwoWayJob<TJob, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {
        public TwoWayThread(SynchronizationContext? context)
            : base(context)
        {

        }
    }

    public sealed partial class OneWayThread<TJob, TJobParameter>
        : TwoWayThread<TJob, TJobParameter, EmptyResult>, IOneWayJob<TJob, TJobParameter>
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : IJobParameter
    {
        public OneWayThread(SynchronizationContext? context)
            : base(context)
        {

        }
    }

    public sealed partial class OneWayThread<TJob>
        : TwoWayThread<TJob, EmptyParameter, EmptyResult>, IOneWayJob<TJob>
        where TJob : AbstractOneWayJob, new()
    {
        public OneWayThread(SynchronizationContext? context)
            : base(context)
        {

        }
    }
}
