using NLog;
using SWF.Core.ConsoleAccessor;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class JobQueue
        : IDisposable
    {
        private const string TASK_NAME = "JobQueue";

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

        public JobQueue()
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

                if (this._task != null)
                {
                    foreach (var job in this._queue.ToArray())
                    {
                        job.BeginCancel();
                    }

                    Log.Writer.Debug("ジョブキュー実行タスクに終了リクエストを送ります。");
                    this.IsAbort = true;

                    Log.Writer.Debug("ジョブキュー実行タスクの終了を待機します。");
                    if (this._task != null)
                    {
                        Task.WaitAll(this._task);
                    }

                    Log.Writer.Debug("ジョブキュー実行タスクが終了しました。");
                    this._task?.Dispose();
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
                Log.Writer.Debug("ジョブキュー実行タスクが開始されました。");

                try
                {
                    while (true)
                    {
                        if (this.IsAbort)
                        {
                            Log.Writer.Debug("ジョブキュー実行タスクに終了リクエストがありました。");
                            return;
                        }

                        if (this._queue.TryPeek(out var currentJob))
                        {
                            var jobName = currentJob.GetType().Name;

                            Log.Writer.Debug($"{jobName} {currentJob.ID} を実行します。");
                            var sw = Stopwatch.StartNew();
                            try
                            {
                                await currentJob.ExecuteWrapper();
                            }
                            catch (JobCancelException)
                            {
                                Log.Writer.Debug($"{jobName} {currentJob.ID} がキャンセルされました。");
                            }
                            catch (JobException ex)
                            {
                                Log.Writer.Error($"{jobName} {currentJob.ID} {ex}");
                            }
                            catch (Exception ex)
                            {
                                Log.Writer.Error(ex, $"{jobName} {currentJob.ID} で補足されない例外が発生しました。");
                            }
                            finally
                            {
                                if (this._queue.TryDequeue(out var dequeueJob))
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
                                Log.Writer.Debug($"{jobName} {currentJob.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
                            }
                        }
                        else
                        {
                            await Task.Delay(1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Writer.Error(ex, $"ジョブキュー実行タスクで補足されない例外が発生しました。");
                }
                finally
                {
                    Log.Writer.Debug("ジョブキュー実行タスクが終了します。");
                }
            }
        }
    }
}
