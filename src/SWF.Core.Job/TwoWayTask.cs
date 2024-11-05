using NLog;
using System.Diagnostics;

namespace SWF.Core.Job
{
    public partial class TwoWayTask<TJob, TJobParameter, TJobResult>
        : AbstractBackgroudProcess<TJob, TJobParameter, TJobResult>, IDisposable, ITwoWayJob<TJob, TJobParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;
        private Task? task = null;
        private CancellationTokenSource? source = null;

        public TwoWayTask(SynchronizationContext? context)
            : base(context)
        {

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

                if (this.task != null)
                {
                    this.BeginCancel();

                    Logger.Debug("ジョブ実行タスクにキャンセルリクエストを送ります。");
                    this.source?.Cancel();

                    Logger.Debug("タスクの終了を待機します。");
                    this.task.Wait();

                    Logger.Debug($"{this.ThreadName}: ジョブ実行タスクが終了しました。");
                }

                this.source?.Dispose();
                this.task?.Dispose();
            }

            this.disposed = true;
        }

        public override void StartJob(ISender sender, TJobParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            var job = this.CreateJob();
            job.Sender = sender;
            job.Parameter = parameter;

            this.source ??= new();
            this.task ??= Task.Run(() => this.DoWork(this.source.Token));
            this.JobQueue.Enqueue(job);
        }

        public override void StartJob(ISender sender)
        {
            var job = this.CreateJob();
            job.Sender = sender;

            this.source ??= new();
            this.task ??= Task.Run(() => this.DoWork(this.source.Token));
            this.JobQueue.Enqueue(job);
        }

        private void DoWork(CancellationToken token)
        {
            Thread.CurrentThread.Name = this.ThreadName;

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
