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
        private static readonly Logger LOGGER = Log.GetLogger();
        private static readonly string TASK_NAME = typeof(TwoWayJobQueue).Name;

        private bool _disposed = false;
        private readonly SynchronizationContext _context;
        private readonly Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly BlockingCollection<AbstractAsyncJob> _blockingJobCollection = [];
        private readonly Dictionary<Type, AbstractAsyncJob> _currentJobDictionary = [];

        public TwoWayJobQueue(SynchronizationContext? context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            this._context = context;
            this._task = Task.Run(
                this.DoWork,
                this._cancellationTokenSource.Token);
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
                LOGGER.Trace($"{TASK_NAME} に終了リクエストを送ります。");
                foreach (var job in this._currentJobDictionary.Values)
                {
                    job.BeginCancel();
                }
                this._currentJobDictionary.Clear();
                this._cancellationTokenSource.Cancel();

                LOGGER.Trace($"{TASK_NAME} の終了を待機します。");
                Task.WaitAll(this._task);

                LOGGER.Trace($"{TASK_NAME} が終了しました。");

                this._cancellationTokenSource.Dispose();
                this._task.Dispose();
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

            if (this._currentJobDictionary.TryGetValue(
                type, out var currentJob))
            {
                currentJob.BeginCancel();
                this._currentJobDictionary.Remove(type);
            }

            var job = new TJob()
            {
                Sender = sender,
                Parameter = parameter,
            };

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
                                LOGGER.Debug($"{jobName} がキャンセルされました。");
                            }
                            catch (Exception ex)
                            {
                                LOGGER.Error(ex, $"{jobName} がUIスレッドで補足されない例外が発生しました。");
                                ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                            }
                        }
                    }, null);
                }
            };

            this._currentJobDictionary.Add(type, job);
            this._blockingJobCollection.Add(job);
        }

        public void Enqueue<TJob, TJobResult>(
            ISender sender, Action<TJobResult> callback)
            where TJob : AbstractTwoWayJob<TJobResult>, new()
            where TJobResult : class, IJobResult
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            var type = typeof(TJob);

            if (this._currentJobDictionary.TryGetValue(
                type, out var currentJob))
            {
                currentJob.BeginCancel();
                this._currentJobDictionary.Remove(type);
            }

            var job = new TJob()
            {
                Sender = sender,
            };

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
                                LOGGER.Debug($"{jobName} がキャンセルされました。");
                            }
                            catch (Exception ex)
                            {
                                LOGGER.Error(ex, $"{jobName} がUIスレッドで補足されない例外が発生しました。");
                                ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                            }
                        }
                    }, null);
                }
            };

            this._currentJobDictionary.Add(type, job);
            this._blockingJobCollection.Add(job);
        }

        private async Task DoWork()
        {
            LOGGER.Trace($"{TASK_NAME} が開始されました。");

            var token = this._cancellationTokenSource.Token;

            try
            {
                foreach (var job in this._blockingJobCollection.GetConsumingEnumerable(token))
                {
                    token.ThrowIfCancellationRequested();

                    if (!job.IsCancel)
                    {
                        await job.ExecuteWrapper(token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LOGGER.Trace($"{TASK_NAME} がキャンセルされました。");
            }
            catch (Exception ex)
            {
                LOGGER.Error(ex, $"{TASK_NAME} で補足されない例外が発生しました。");
            }
            finally
            {
                LOGGER.Trace($"{TASK_NAME} が終了します。");
            }
        }
    }
}
