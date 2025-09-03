using NLog;
using SWF.Core.Base;
using System.Threading.Channels;

namespace SWF.Core.Job
{

    public partial class TwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : class, IJobParameter
        where TJobResult : class, IJobResult
    {
        private static readonly Logger LOGGER = NLogManager.GetLogger();
        private static readonly string TASK_NAME = $"{typeof(TJob).Name} Task";

        private bool _disposed = false;
        private bool _isShuttingDown = false;

        private readonly Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Channel<AbstractJob> _jobsChannel
            = Channel.CreateBounded<AbstractJob>(new BoundedChannelOptions(1)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = true
            });
        private readonly List<AbstractJob> _currentJobList = [];

        public TwoWayJob()
        {
            LOGGER.Trace($"{TASK_NAME} を開始します。");

            this._task = Task.Factory.StartNew(
                this.DoWork,
                this._cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
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
            this._currentJobList.ForEach(static _ => _.BeginCancel());
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
                var context = AppConstants.GetUIThreadContext();
                job.CallbackAction = _ =>
                {
                    context.Post(state =>
                    {
                        if (job.CanUIThreadAccess() && state is TJobResult result)
                        {
                            callback(result);
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

#pragma warning disable CA1031
        private async ValueTask DoWork()
        {
            LOGGER.Trace($"{TASK_NAME} が開始されました。");

            var token = this._cancellationTokenSource.Token;

            try
            {
                await foreach (var job in this._jobsChannel.Reader.ReadAllAsync(token).WithConfig())
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
#pragma warning restore CA1031
    }


    public sealed partial class TwoWayJob<TJob, TJobResult>()
        : TwoWayJob<TJob, EmptyParameter, TJobResult>()
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : class, IJobResult
    {
    }


    public sealed partial class OneWayJob<TJob, TJobParameter>()
        : TwoWayJob<TJob, TJobParameter, EmptyResult>()
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : class, IJobParameter
    {
    }


    public sealed partial class OneWayJob<TJob>()
        : TwoWayJob<TJob, EmptyParameter, EmptyResult>()
        where TJob : AbstractOneWayJob, new()
    {
    }
}
