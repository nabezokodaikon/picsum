using NLog;
using SWF.Core.Base;
using System.Runtime.Versioning;
using System.Threading.Channels;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class TwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : class, IJobParameter
        where TJobResult : class, IJobResult
    {
        private static readonly Logger LOGGER = Log.GetLogger();
        private static readonly string TASK_NAME = $"{typeof(TJob).Name} Task";

        private bool _disposed = false;
        private bool _isShuttingDown = false;

        private readonly Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Channel<AbstractAsyncJob> _jobsChannel
            = Channel.CreateBounded<AbstractAsyncJob>(new BoundedChannelOptions(1)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = true
            });
        private readonly List<AbstractAsyncJob> _currentJobList = [];
        private readonly SynchronizationContext _context;

        public TwoWayJob(SynchronizationContext? context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            LOGGER.Trace($"{TASK_NAME} を開始します。");

            this._context = context;
            this._task = Task.Run(
                this.DoWork,
                this._cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._isShuttingDown = true;

            LOGGER.Trace($"{TASK_NAME} に終了リクエストを送ります。");
            this.BeginCancel();
            this._jobsChannel.Writer.Complete();
            this._cancellationTokenSource.Cancel();

            try
            {
                LOGGER.Trace($"{TASK_NAME} の終了を待機します。");
                this._task.GetAwaiter().GetResult();
                LOGGER.Trace($"{TASK_NAME} が終了しました。");
            }
            catch (OperationCanceledException)
            {
                LOGGER.Trace($"{TASK_NAME} はキャンセルにより終了しました。");
            }

            this._cancellationTokenSource.Dispose();
            this._disposed = true;
            GC.SuppressFinalize(this);
        }

        public void BeginCancel()
        {
            this._currentJobList.ForEach(_ => _.BeginCancel());
            this._currentJobList.Clear();
        }

        public void StartJob(ISender sender, TJobParameter? parameter, Action<TJobResult>? callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            if (this._isShuttingDown || this._disposed)
            {
                return;
            }

            this.BeginCancel();

            var job = new TJob
            {
                Sender = sender,
                Parameter = parameter
            };

            if (callback != null)
            {
                job.CallbackAction = _ =>
                {
                    this._context.Post(state =>
                    {
                        if (job.CanUIThreadAccess() && state is TJobResult result)
                        {
                            try
                            {
                                callback(result);
                            }
                            catch (Exception ex)
                            {
                                LOGGER.Error(ex, $"{job} がUIスレッドで補足されない例外が発生しました。");
                            }
                        }
                    }, _);
                };
            }

            this._currentJobList.Add(job);
#pragma warning disable CA2012
            this._jobsChannel.Writer.WriteAsync(job).GetAwaiter().GetResult();
#pragma warning restore CA2012
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
                await foreach (var job in this._jobsChannel.Reader.ReadAllAsync(token))
                {
                    token.ThrowIfCancellationRequested();

                    if (!job.IsJobCancel)
                    {
                        await job.ExecuteWrapper(token).WithConfig();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LOGGER.Trace($"{TASK_NAME} がキャンセルされました。");
                throw;
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
        where TJobResult : class, IJobResult
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
