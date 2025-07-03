using NLog;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class TwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : class, IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger LOGGER = Log.GetLogger();
        private static readonly string TASK_NAME = typeof(TJob).Name;

        private bool _disposed = false;

        private readonly Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly BlockingCollection<AbstractAsyncJob> _blockingJobCollection = [];
        private readonly List<AbstractAsyncJob> _currentJobList = [];
        private readonly SynchronizationContext _context;

        public TwoWayJob(SynchronizationContext? context)
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
                this.BeginCancel();
                this._cancellationTokenSource.Cancel();

                LOGGER.Trace($"{TASK_NAME} の終了を待機します。");
                Task.WaitAll(this._task);

                LOGGER.Trace($"{TASK_NAME} が終了しました。");

                this._cancellationTokenSource.Dispose();
                this._task.Dispose();
            }

            this._disposed = true;
        }

        public void BeginCancel()
        {
            this._currentJobList.ForEach(_ => _.BeginCancel());
            this._currentJobList.Clear();
        }

        public void StartJob(ISender sender, TJobParameter? parameter, Action<TJobResult>? callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.BeginCancel();

            var job = new TJob
            {
                Sender = sender,
                Parameter = parameter
            };

            if (callback != null)
            {
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
                                    callback.Invoke(result);
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
            }

            this._currentJobList.Add(job);
            this._blockingJobCollection.Add(job);
        }

        public void StartJob(ISender sender, Action<TJobResult> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            this.StartJob(sender, null, callback);
        }

        public void StartJob(ISender sender, TJobParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this.StartJob(sender, parameter, null);
        }

        public void StartJob(ISender sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.StartJob(sender, null, null);
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

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class TwoWayJob<TJob, TJobResult>(SynchronizationContext? context)
        : TwoWayJob<TJob, EmptyParameter, TJobResult>(context)
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayJob<TJob, TJobParameter>(SynchronizationContext? context)
        : TwoWayJob<TJob, TJobParameter, EmptyResult>(context)
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : class, IJobParameter
    {
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayJob<TJob>(SynchronizationContext? context)
        : TwoWayJob<TJob, EmptyParameter, EmptyResult>(context)
        where TJob : AbstractOneWayJob, new()
    {
    }
}
