using NLog;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class JobQueue
        : IDisposable
    {
        private static readonly string TASK_NAME = typeof(JobQueue).Name;

        private bool _disposed = false;
        private readonly SynchronizationContext _context;
        private readonly Task _task;
        private readonly ConcurrentQueue<AbstractAsyncJob> _queue = new();
        private readonly Dictionary<Type, AbstractAsyncJob> _currentJobsDictionary = [];
        private long _isAbort = 0;

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

        public JobQueue(SynchronizationContext? context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            this._context = context;
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

                foreach (var job in this._currentJobsDictionary.Values)
                {
                    job.BeginCancel();
                }

                logger.Debug($"{TASK_NAME} 実行タスクの終了を待機します。");
                Task.WaitAll(this._task);

                logger.Debug($"{TASK_NAME} 実行タスクが終了しました。");
            }

            this._disposed = true;
        }

        public void Enqueue<TJob, TJobParameter, TJobResult>(
            ISender sender, TJobParameter parameter, Action<TJobResult> callback)
            where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
            where TJobParameter : class, IJobParameter
            where TJobResult : class, IJobResult
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            var type = typeof(TJob);

            if (this._currentJobsDictionary.TryGetValue(
                type, out var currentJob))
            {
                currentJob.BeginCancel();
                this._currentJobsDictionary.Remove(type);
            }

            var job = new TJob()
            {
                Sender = sender,
                Parameter = parameter,
            };

            this._currentJobsDictionary.Add(type, job);

            job.CallbackAction = result =>
            {
                if (job.CanUIThreadAccess())
                {
                    this._context.Post(_ =>
                    {
                        if (job.CanUIThreadAccess())
                        {
                            var jobName = $"{job.GetType().Name} {job.ID}";

                            try
                            {
                                job.CheckCancel();
                                callback.Invoke(result);
                            }
                            catch (JobCancelException)
                            {
                                Log.GetLogger().Debug($"{jobName} がキャンセルされました。");
                            }
                            catch (Exception ex)
                            {
                                Log.GetLogger().Error(ex, $"{jobName} がUIスレッドで補足されない例外が発生しました。");
                                ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                            }
                        }
                    }, null);
                }
            };

            this._queue.Enqueue(job);
        }

        public void Enqueue<TJob, TJobResult>(
            ISender sender, Action<TJobResult> callback)
            where TJob : AbstractTwoWayJob<TJobResult>, new()
            where TJobResult : class, IJobResult
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            var type = typeof(TJob);

            if (this._currentJobsDictionary.TryGetValue(
                type, out var currentJob))
            {
                currentJob.BeginCancel();
                this._currentJobsDictionary.Remove(type);
            }

            var job = new TJob()
            {
                Sender = sender,
            };

            this._currentJobsDictionary.Add(type, job);

            job.CallbackAction = result =>
            {
                if (job.CanUIThreadAccess())
                {
                    this._context.Post(_ =>
                    {
                        if (job.CanUIThreadAccess())
                        {
                            try
                            {
                                callback.Invoke(result);
                            }
                            catch (Exception ex)
                            {
                                var jobName = $"{job.GetType().Name} {job.ID}";
                                Log.GetLogger().Error(ex, $"{jobName} がUIスレッドで補足されない例外が発生しました。");
                                ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                            }
                        }
                    }, null);
                }
            };

            this._queue.Enqueue(job);
        }

        public void Enqueue<TJob, TJobParameter>(ISender sender, TJobParameter parameter)
            where TJob : AbstractTwoWayJob<TJobParameter, EmptyResult>, new()
            where TJobParameter : class, IJobParameter
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            var type = typeof(TJob);

            this._currentJobsDictionary.Remove(type);

            var job = new TJob()
            {
                Sender = sender,
                Parameter = parameter,
            };

            this._currentJobsDictionary.Add(type, job);

            this._queue.Enqueue(job);
        }

        public void Enqueue<TJob>(ISender sender)
            where TJob : AbstractTwoWayJob<EmptyParameter, EmptyResult>, new()
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var type = typeof(TJob);

            this._currentJobsDictionary.Remove(type);

            var job = new TJob()
            {
                Sender = sender,
            };

            this._currentJobsDictionary.Add(type, job);

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
                finally
                {
                    logger.Debug($"{TASK_NAME} 実行タスクが終了します。");
                }
            }
        }
    }
}
