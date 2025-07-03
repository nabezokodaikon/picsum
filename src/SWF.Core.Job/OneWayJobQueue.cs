using NLog;
using SWF.Core.ConsoleAccessor;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayJobQueue
        : IDisposable
    {
        private static readonly string TASK_NAME = typeof(OneWayJobQueue).Name;

        private bool _disposed = false;
        private readonly Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ConcurrentQueue<AbstractAsyncJob> _queue = new();

        public OneWayJobQueue()
        {
            this._task = Task.Factory.StartNew(
                this.DoWork,
                this._cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
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
                var logger = Log.GetLogger();

                logger.Trace($"{TASK_NAME} に終了リクエストを送ります。");
                this._cancellationTokenSource.Cancel();

                logger.Trace($"{TASK_NAME} の終了を待機します。");
                Task.WaitAll(this._task);

                this._cancellationTokenSource.Dispose();
                this._task.Dispose();

                logger.Trace($"{TASK_NAME} が終了しました。");
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

            this._queue.Enqueue(job);
        }

        public void Enqueue<TJob>(ISender sender)
            where TJob : AbstractTwoWayJob<EmptyParameter, EmptyResult>, new()
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var job = new TJob()
            {
                Sender = sender,
            };

            this._queue.Enqueue(job);
        }

        private async Task DoWork()
        {
            using (ScopeContext.PushProperty(Log.NLOG_PROPERTY, TASK_NAME))
            {
                var logger = Log.GetLogger();

                logger.Trace("開始されました。");

                var token = this._cancellationTokenSource.Token;

                try
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        if (this._queue.TryPeek(out var job))
                        {
                            try
                            {
                                await job.ExecuteWrapper();
                            }
                            finally
                            {
                                if (this._queue.TryDequeue(out var dequeueJob))
                                {
                                    if (job != dequeueJob)
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
                            }
                        }
                        else
                        {
                            await Task.Delay(100, token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    logger.Trace("キャンセルされました。");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "補足されない例外が発生しました。");
                }
                finally
                {
                    logger.Trace("終了します。");
                }
            }
        }
    }
}
