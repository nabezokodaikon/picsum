using NLog;
using SWF.Core.Base;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class JobQueue
        : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string THREAD_NAME = "JobQueue";

        private bool _disposed = false;
        private readonly Task _task;
        private readonly CancellationTokenSource source;
        private readonly ConcurrentQueue<AbstractAsyncJob> _jobQueue = new();

        public JobQueue()
        {
            this.source = new();
            this._task = Task.Run(
                () => this.DoWork(this.source.Token).GetAwaiter().GetResult());
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {

                if (this._task != null)
                {
                    foreach (var job in this._jobQueue.ToArray())
                    {
                        job.BeginCancel();
                    }

                    Logger.Debug("ジョブキュー実行スレッドにキャンセルリクエストを送ります。");
                    this.source?.Cancel();

                    Logger.Debug("ジョブキュー実行スレッドの終了を待機します。");
                    this._task?.GetAwaiter().GetResult();

                    Logger.Debug("ジョブキュー実行スレッドが終了しました。");
                    this._task?.Dispose();
                    this.source?.Dispose();
                }
            }

            this._disposed = true;
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

            this._jobQueue.Enqueue(job);
        }

        public void Enqueue<TJob>(ISender sender)
            where TJob : AbstractTwoWayJob<EmptyParameter, EmptyResult>, new()
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var job = new TJob()
            {
                Sender = sender,
            };

            this._jobQueue.Enqueue(job);
        }

        private async Task DoWork(CancellationToken token)
        {
            using (ScopeContext.PushProperty(AppConstants.NLOG_PROPERTY, THREAD_NAME))
            {
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

                        if (this._jobQueue.TryPeek(out var currentJob))
                        {
                            var jobName = currentJob.GetType().Name;

                            Logger.Debug($"{jobName} {currentJob.ID} を実行します。");
                            var sw = Stopwatch.StartNew();
                            try
                            {
                                currentJob.ExecuteWrapper().GetAwaiter().GetResult();
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
                                if (this._jobQueue.TryDequeue(out var dequeueJob))
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
                            await Task.Delay(1, token);
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
}
