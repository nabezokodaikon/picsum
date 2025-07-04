using NLog;
using SWF.Core.ConsoleAccessor;
using System.Runtime.Versioning;
using System.Threading.Channels;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayJobQueue
        : IDisposable
    {
        private static readonly Logger LOGGER = Log.GetLogger();
        private static readonly string TASK_NAME = typeof(OneWayJobQueue).Name;

        private bool _disposed = false;
        private readonly Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Channel<AbstractAsyncJob> _jobsChannel
            = Channel.CreateUnbounded<AbstractAsyncJob>(new UnboundedChannelOptions()
            {
                AllowSynchronousContinuations = false,
                SingleReader = true,
                SingleWriter = true
            });

        public OneWayJobQueue()
        {
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
                this._jobsChannel.Writer.Complete();
                this._cancellationTokenSource.Cancel();

                LOGGER.Trace($"{TASK_NAME} の終了を待機します。");
                Task.WaitAll(this._task);

                LOGGER.Trace($"{TASK_NAME} が終了しました。");

                this._cancellationTokenSource.Dispose();
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

            this._jobsChannel.Writer.WriteAsync(job).GetAwaiter().GetResult();
        }

        public void Enqueue<TJob>(ISender sender)
            where TJob : AbstractTwoWayJob<EmptyParameter, EmptyResult>, new()
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var job = new TJob()
            {
                Sender = sender,
            };

            this._jobsChannel.Writer.WriteAsync(job).GetAwaiter().GetResult();
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
