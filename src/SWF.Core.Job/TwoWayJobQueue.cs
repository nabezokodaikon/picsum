using NLog;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class TwoWayJobQueue
        : IDisposable
    {
        private static readonly string TASK_NAME = typeof(TwoWayJobQueue).Name;

        private bool _disposed = false;
        private readonly SynchronizationContext _context;
        private readonly Task _task;
        private readonly ConcurrentQueue<AbstractAsyncJob> _queue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Dictionary<Type, AbstractAsyncJob> _currentJobsDictionary = [];

        public TwoWayJobQueue(SynchronizationContext? context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            this._context = context;
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

                foreach (var job in this._queue.ToArray())
                {
                    job.BeginCancel();
                }

                foreach (var job in this._currentJobsDictionary.Values)
                {
                    job.BeginCancel();
                }

                logger.Trace($"{TASK_NAME} の終了を待機します。");
                Task.WaitAll(this._task);

                this._cancellationTokenSource.Dispose();
                this._task.Dispose();

                logger.Trace($"{TASK_NAME} が終了しました。");
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
                                await job.ExecuteWrapper(token);
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
