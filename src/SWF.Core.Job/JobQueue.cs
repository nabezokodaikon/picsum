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
        private readonly SynchronizationContext context;
        private readonly string taskName = "JobQueue";
        private Task? task = null;
        private CancellationTokenSource? source = null;
        private readonly ConcurrentQueue<AbstractAsyncJob> jobQueue = new();

        public JobQueue(SynchronizationContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            this.context = context;
        }

        ~JobQueue()
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
                    foreach (var job in this.jobQueue.ToArray())
                    {
                        job.BeginCancel();
                    }

                    Logger.Debug("ジョブキュー実行タスクにキャンセルリクエストを送ります。");
                    this.source?.Cancel();

                    Logger.Debug("ジョブキュー実行タスクの終了を待機します。");
                    this.task.Wait();

                    Logger.Debug($"{this.taskName}: ジョブキュー実行タスクが終了しました。");
                }
            }

            this.disposed = true;
        }

        public void Enqueue<TJob, TJobParameter>(ISender sender, TJobParameter parameter)
            where TJob : AbstractTwoWayJob<TJobParameter, EmptyResult>, new()
            where TJobParameter : IJobParameter
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.StartTask();

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

            this.StartTask();

            var job = new TJob()
            {
                Sender = sender,
            };

            this.jobQueue.Enqueue(job);
        }

        private void StartTask()
        {
            this.source ??= new();
            this.task ??= Task.Run(() => this.DoWork(this.source.Token));
        }

        private void DoWork(CancellationToken token)
        {
            Logger.Debug("ジョブキュー実行タスクが開始されました。");

            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブキュー実行タスクにキャンセルリクエストがありました。");
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
                            Logger.Debug($"{currentJob.ID} がキャンセルされました。");
                        }
                        catch (JobException ex)
                        {
                            Logger.Error($"{currentJob.ID} {ex}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, $"{currentJob.ID} で補足されない例外が発生しました。");
                        }
                        finally
                        {
                            currentJob.IsCompleted = true;

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
                                throw new InvalidOperationException("他のタスクでジョブキューの操作が行われました。");
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
            catch (OperationCanceledException)
            {
                Logger.Debug("ジョブキュー実行タスクをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"ジョブキュー実行タスクで補足されない例外が発生しました。");
            }
            finally
            {
                Logger.Debug("ジョブキュー実行タスクが終了します。");
            }
        }
    }
}
