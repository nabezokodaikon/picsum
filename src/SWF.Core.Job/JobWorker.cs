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
            this.BeginCancel();
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

        public void BeginCancel()
        {
            AppConstants.ThrowIfNotUIThread();

            this._currentJobList.ForEach(static _ => _.BeginCancel());
            this._currentJobList.Clear();
        }

        public void StartJob(
            ISender sender,
            TJobParameter? parameter,
            Action<TJobResult>? callback,
            bool isCheckCancel = true)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            AppConstants.ThrowIfNotUIThread();

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
                var context = AppConstants.GetUIContext();
                job.CallbackAction = result =>
                {
                    context.Post(s =>
                    {
                        var (cb, res, j, check) = (ValueTuple<Action<TJobResult>, TJobResult, TJob, bool>)s!;
                        if (!j.CanUIThreadAccess() || this._isShuttingDown || this._disposed)
                        {
                            return;
                        }

                        if (check && j.IsJobCancel)
                        {
                            return;
                        }

                        cb(res);
                    }, (callback, result, job, isCheckCancel));
                };
            }

            this._currentJobList.Add(job);

            if (!this._jobsChannel.Writer.TryWrite(job))
            {
                ConsoleUtil.Write(true, $"{job} をキューに追加できませんでした。");
                LOGGER.Warn($"{job} をキューに追加できませんでした。");
            }
        }

        public void StartJob(
            ISender sender,
            Action<TJobResult> callback,
            bool isCheckCancel = true)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            AppConstants.ThrowIfNotUIThread();

            this.StartJob(sender, null, callback, isCheckCancel);
        }

        public void StartJob(
            ISender sender,
            TJobParameter parameter,
            bool isCheckCancel = true)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            AppConstants.ThrowIfNotUIThread();

            this.StartJob(sender, parameter, null, isCheckCancel);
        }

        public void StartJob(ISender sender, bool isCheckCancel = true)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            AppConstants.ThrowIfNotUIThread();

            this.StartJob(sender, null, null, isCheckCancel);
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
