using NLog;
using SWF.Core.Base;
using System.Threading.Channels;

namespace SWF.Core.Job
{
    public sealed partial class OneWayJobQueue
        : IDisposable
    {
        private static readonly Logger LOGGER = NLogManager.GetLogger();
        private static readonly string TASK_NAME = $"{typeof(OneWayJobQueue).Name} Task";

        private bool _disposed = false;
        private bool _isShuttingDown = false;
        private readonly Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Channel<AbstractJob> _jobsChannel
            = Channel.CreateUnbounded<AbstractJob>(new UnboundedChannelOptions()
            {
                AllowSynchronousContinuations = false,
                SingleReader = true,
                SingleWriter = true
            });

        public OneWayJobQueue()
        {
            AppConstants.ThrowIfNotUIThread();

            LOGGER.Trace($"{TASK_NAME} を開始します。");

            this._task = Task.Factory.StartNew(
                this.DoWork,
                this._cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).Unwrap();
        }

        public void Dispose()
        {
            AppConstants.ThrowIfNotUIThread();

            if (this._disposed)
            {
                return;
            }

            this._isShuttingDown = true;

            LOGGER.Trace($"{TASK_NAME} に終了リクエストを送ります。");
            this._jobsChannel.Writer.Complete();
            this._cancellationTokenSource.Cancel();

            try
            {
                LOGGER.Trace($"{TASK_NAME} の終了を待機します。");
                this._task.GetAwaiter().GetResult();
                LOGGER.Trace($"{TASK_NAME} が終了しました。");
            }
            catch (Exception ex) when (
                ex is TaskCanceledException ||
                ex is OperationCanceledException ||
                ex is ChannelClosedException)
            {
                LOGGER.Trace($"{TASK_NAME} はキャンセルにより終了しました。");
            }

            this._cancellationTokenSource.Dispose();
            this._disposed = true;
            GC.SuppressFinalize(this);
        }

        public void Enqueue<TJob, TJobParameter>(ISender sender, TJobParameter parameter)
            where TJob : AbstractTwoWayJob<TJobParameter, EmptyResult>, new()
            where TJobParameter : class, IJobParameter
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            AppConstants.ThrowIfNotUIThread();

            if (this._isShuttingDown || this._disposed)
            {
                return;
            }

            var job = new TJob()
            {
                Sender = sender,
                Parameter = parameter,
            };

            try
            {
                this._jobsChannel.Writer.TryWrite(job);
            }
            catch (Exception ex) when (
                ex is TaskCanceledException ||
                ex is OperationCanceledException ||
                ex is ChannelClosedException)
            { }
        }

        public void Enqueue<TJob>(ISender sender)
            where TJob : AbstractTwoWayJob<EmptyParameter, EmptyResult>, new()
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            AppConstants.ThrowIfNotUIThread();

            if (this._isShuttingDown || this._disposed)
            {
                return;
            }

            var job = new TJob()
            {
                Sender = sender,
            };

            try
            {
                this._jobsChannel.Writer.TryWrite(job);
            }
            catch (Exception ex) when (
                ex is TaskCanceledException ||
                ex is OperationCanceledException ||
                ex is ChannelClosedException)
            { }
        }

#pragma warning disable CA1031
        private async Task DoWork()
        {
            LOGGER.Trace($"{TASK_NAME} が開始されました。");

            var token = this._cancellationTokenSource.Token;

            try
            {
                await foreach (var job in this._jobsChannel.Reader.ReadAllAsync(token).False())
                {
                    token.ThrowIfCancellationRequested();

                    if (!job.IsJobCancel)
                    {
                        await job.ExecuteWrapper(token).False();
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
}
