using NLog;
using SWF.Core.ConsoleAccessor;
using System.Collections.Concurrent;
using System.Diagnostics;
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
        private long _isAbort = 0;
        private readonly ConcurrentQueue<AbstractAsyncJob> _queue = new();

        private bool IsAbort
        {
            get
            {
                return Interlocked.Read(ref this._isAbort) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this._isAbort, Convert.ToInt64(value));
            }
        }

        public OneWayJobQueue()
        {
            this._task = Task.Run(this.DoWork);
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

                logger.Debug($"{TASK_NAME} 実行タスクに終了リクエストを送ります。");
                this.IsAbort = true;

                foreach (var job in this._queue.ToArray())
                {
                    job.BeginCancel();
                }

                logger.Debug($"{TASK_NAME} 実行タスクの終了を待機します。");
                Task.WaitAll(this._task);

                logger.Debug($"{TASK_NAME} 実行タスクが終了しました。");
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

                logger.Debug($"{TASK_NAME} 実行タスクが開始されました。");

                try
                {
                    while (true)
                    {
                        if (this.IsAbort)
                        {
                            logger.Debug($"{TASK_NAME} 実行タスクに終了リクエストがありました。");
                            return;
                        }

                        if (this._queue.TryPeek(out var job))
                        {
                            var jobName = $"{job.GetType().Name} {job.ID}";

                            logger.Debug($"{jobName} を実行します。");
                            var sw = Stopwatch.StartNew();
                            try
                            {
                                await job.ExecuteWrapper();
                            }
                            catch (JobCancelException)
                            {
                                logger.Debug($"{jobName} がキャンセルされました。");
                            }
                            catch (JobException ex)
                            {
                                logger.Error($"{jobName} {ex}");
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex, $"{jobName} で補足されない例外が発生しました。");
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

                                sw.Stop();
                                logger.Debug($"{jobName} が終了しました。{sw.ElapsedMilliseconds} ms");
                            }
                        }
                        else
                        {
                            await Task.Delay(100);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{TASK_NAME} 実行タスクで補足されない例外が発生しました。");
                }
                finally
                {
                    logger.Debug($"{TASK_NAME} 実行タスクが終了します。");
                }
            }
        }
    }
}
