using NLog;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SWF.Core.Job
{
    public sealed partial class JobQueue
        : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;
        private readonly string threadName = "JobQueue";
        private readonly Task task;
        private readonly CancellationTokenSource source;
        private readonly ConcurrentQueue<AbstractAsyncJob> jobQueue = new();

        public JobQueue()
        {
            this.source = new();
            this.task = Task.Run(() => this.DoWork(this.source.Token));
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
                    foreach (var job in this.jobQueue.ToArray())
                    {
                        job.BeginCancel();
                    }

                    Logger.Debug("ジョブキュー実行スレッドにキャンセルリクエストを送ります。");
                    this.source?.Cancel();

                    Logger.Debug("ジョブキュー実行スレッドの終了を待機します。");
                    this.task?.Wait();

                    Logger.Debug("ジョブキュー実行スレッドが終了しました。");
                    this.task?.Dispose();
                    this.source?.Dispose();
                }
            }

            this.disposed = true;
        }

        public void Enqueue<TJob, TJobParameter>(ISender sender, TJobParameter parameter)
            where TJob : AbstractTwoWayJob<TJobParameter, EmptyResult>, new()
            where TJobParameter : class, IJobParameter
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            var job = new TJob()
            {
                Sender = sender,
                Parameter = parameter,
            };

            this.jobQueue.Enqueue(job);
        }

        public void Enqueue<TJob>(ISender sender)
            where TJob : AbstractTwoWayJob<EmptyParameter, EmptyResult>, new()
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var job = new TJob()
            {
                Sender = sender,
            };

            this.jobQueue.Enqueue(job);
        }

        private void DoWork(CancellationToken token)
        {
            Thread.CurrentThread.Name = this.threadName;

            Logger.Debug("ジョブキュー実行スレッドが開始されました。");

            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブキュー実行スレッドにキャンセルリクエストがありました。");
                        token.ThrowIfCancellationRequested();
                    }

                    if (this.jobQueue.TryPeek(out var currentJob))
                    {
                        var jobName = currentJob.GetType().Name;

                        Logger.Debug($"{jobName} {currentJob.ID} を実行します。");
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            currentJob.ExecuteWrapper();
                        }
                        catch (JobCancelException)
                        {
                            Logger.Debug($"{jobName} {currentJob.ID} がキャンセルされました。");
                        }
                        catch (JobException ex)
                        {
                            Logger.Error($"{jobName} {currentJob.ID} {ex}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, $"{jobName} {currentJob.ID} で補足されない例外が発生しました。");
                        }
                        finally
                        {
                            if (this.jobQueue.TryDequeue(out var dequeueJob))
                            {
                                if (currentJob != dequeueJob)
                                {
#pragma warning disable CA2219
                                    throw new InvalidOperationException("ジョブキューからPeekしたジョブとDequeueしたジョブが一致しません。");
#pragma warning restore CA2219
                                }
                            }
                            else
                            {
#pragma warning disable CA2219
                                throw new InvalidOperationException("他のスレッドでジョブキューの操作が行われました。");
#pragma warning restore CA2219
                            }

                            sw.Stop();
                            Logger.Debug($"{jobName} {currentJob.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
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
                Logger.Debug("ジョブキュー実行スレッドをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"ジョブキュー実行スレッドで補足されない例外が発生しました。");
            }
            finally
            {
                Logger.Debug("ジョブキュー実行スレッドが終了します。");
            }
        }
    }
}
