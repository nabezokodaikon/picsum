using NLog;
using SWF.Core.Base;
using System.Runtime.Versioning;
using System.Threading.Channels;

namespace SWF.Core.Job
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class TwoWayJobQueue
        : IAsyncDisposable
    {
        private static readonly Logger LOGGER = Log.GetLogger();
        private static readonly string TASK_NAME = $"{typeof(TwoWayJobQueue).Name} Task";

        private bool _disposed = false;
        private bool _isShuttingDown = false;
        private readonly SynchronizationContext _context;
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
        private readonly Dictionary<Type, AbstractAsyncJob> _currentJobDictionary = [];

        public TwoWayJobQueue(SynchronizationContext? context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            LOGGER.Trace($"{TASK_NAME} を開始します。");

            this._context = context;
            this._task = Task.Run(
                this.DoWork,
                this._cancellationTokenSource.Token);
        }

        public async ValueTask DisposeAsync()
        {
            if (this._disposed)
            {
                return;
            }

            this._isShuttingDown = true;

            LOGGER.Trace($"{TASK_NAME} に終了リクエストを送ります。");
            foreach (var job in this._currentJobDictionary.Values)
            {
                job.BeginCancel();
            }
            this._currentJobDictionary.Clear();
            this._jobsChannel.Writer.Complete();
            this._cancellationTokenSource.Cancel();

            try
            {
                LOGGER.Trace($"{TASK_NAME} の終了を待機します。");
                await this._task;
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

        public void Enqueue<TJob, TJobParameter, TJobResult>(
            ISender sender, TJobParameter parameter, Action<TJobResult> callback)
            where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
            where TJobParameter : class, IJobParameter
            where TJobResult : class, IJobResult
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            if (this._isShuttingDown || this._disposed)
            {
                return;
            }

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

            this._currentJobDictionary.Add(type, job);
#pragma warning disable CA2012 // ValueTask を正しく使用する必要があります
            this._jobsChannel.Writer.WriteAsync(job).GetAwaiter().GetResult();
#pragma warning restore CA2012 // ValueTask を正しく使用する必要があります
        }

        public void Enqueue<TJob, TJobResult>(
            ISender sender, Action<TJobResult> callback)
            where TJob : AbstractTwoWayJob<TJobResult>, new()
            where TJobResult : class, IJobResult
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            if (this._isShuttingDown || this._disposed)
            {
                return;
            }

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

            this._currentJobDictionary.Add(type, job);
#pragma warning disable CA2012 // ValueTask を正しく使用する必要があります
            this._jobsChannel.Writer.WriteAsync(job).GetAwaiter().GetResult();
#pragma warning restore CA2012 // ValueTask を正しく使用する必要があります
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
                        await job.ExecuteWrapper(token);
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
}
