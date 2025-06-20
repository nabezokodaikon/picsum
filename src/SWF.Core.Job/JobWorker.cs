using NLog;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using System.Diagnostics;
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

        private bool _disposed = false;

        private readonly string _taskName;
        private readonly Task _task;
        private readonly SynchronizationContext _context;
        private TJob? _currentJob = null;

        private TJob? CurrentJob
        {
            get
            {
                return Interlocked.CompareExchange(ref this._currentJob, null, null);
            }
            set
            {
                Interlocked.Exchange(ref this._currentJob, value);
            }
        }

        public TwoWayJob(SynchronizationContext? context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            this._context = context;
            this._task = Task.Run(this.DoWork);
            this._taskName = $"{typeof(TJob).Name} {TaskID.GetNew()}";
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

                logger.Debug($"{this._taskName} ジョブ実行タスクに終了リクエストを送ります。");
                this.IsAbort = true;
                this.BeginCancel();

                logger.Debug($"{this._taskName} ジョブ実行タスクの終了を待機します。");
                Task.WaitAll(this._task);

                logger.Debug($"{this._taskName} ジョブ実行タスクが終了しました。");
            }

            this._disposed = true;
        }

        public void BeginCancel()
        {
            var job = this.CurrentJob;
            job?.BeginCancel();
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
                                try
                                {
                                    callback.Invoke(result);
                                }
                                catch (Exception ex)
                                {
                                    Log.GetLogger().Error(ex, $"{this._taskName} {job.ID} がUIタスク上で補足されない例外が発生しました。");
                                    ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                                }
                            }
                        }, null);
                    }
                };
            }

            this.CurrentJob = job;
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
            using (ScopeContext.PushProperty(Log.NLOG_PROPERTY, this._taskName))
            {
                var logger = Log.GetLogger();

                logger.Debug("ジョブ実行タスクが開始されました。");

                TJob? previewJob = null;

                try
                {
                    while (true)
                    {
                        if (this.IsAbort)
                        {
                            logger.Debug("ジョブ実行タスクに終了リクエストがありました。");
                            return;
                        }

                        var job = this.CurrentJob;
                        if (job == null || job == previewJob)
                        {
                            await Task.Delay(1);
                            continue;
                        }

                        previewJob = job;

                        logger.Debug($"{job.ID} を実行します。");
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            await job.ExecuteWrapper();
                        }
                        catch (JobCancelException)
                        {
                            logger.Debug($"{job.ID} がキャンセルされました。");
                        }
                        catch (JobException ex)
                        {
                            logger.Error($"{job.ID} {ex}");
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, $"{job.ID} で補足されない例外が発生しました。");
                        }
                        finally
                        {
                            sw.Stop();
                            logger.Debug($"{job.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
                        }

                        await Task.Delay(1);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"ジョブ実行タスクで補足されない例外が発生しました。");
                }
                finally
                {
                    logger.Debug("ジョブ実行タスクが終了します。");
                }
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
