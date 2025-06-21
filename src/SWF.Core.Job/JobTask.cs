using NLog;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    // TODO: いつか実装する？
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class JobTask
        : IDisposable
    {
        private bool _disposed = false;
        private readonly SynchronizationContext _context;
        private Task? _currentTask = null;
        private AbstractAsyncJob? _currentJob = null;

        public JobTask(SynchronizationContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            this._context = context;
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
                this._currentJob?.BeginCancel();
                this._currentTask?.Wait();
            }

            this._currentJob = null;
            this._currentTask = null;

            this._disposed = true;
        }

        public async Task Run<TJob, TJobParameter, TJobResult>(
            ISender sender, TJobParameter? parameter, Action<TJobResult> callback)
            where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
            where TJobParameter : class, IJobParameter
            where TJobResult : class, IJobResult
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            this._currentJob?.BeginCancel();

            if (this._currentTask != null)
            {
                await this._currentTask.WaitAsync(Timeout.InfiniteTimeSpan);
            }

            var job = new TJob()
            {
                Sender = sender,
                Parameter = parameter,
            };

            var jobName = $"{job.GetType().Name}: {job.ID}";

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
                                Log.GetLogger().Error(ex, $"{jobName} がUIタスク上で補足されない例外が発生しました。");
                                ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                            }
                        }
                    }, null);
                }
            };

            this._currentJob = job;
            this._currentTask = Task.Run(() => this.DoWork(this._currentJob));
        }

        public async Task Run<TJob, TJobResult>(
            ISender sender, Action<TJobResult> callback)
            where TJob : AbstractTwoWayJob<TJobResult>, new()
            where TJobResult : class, IJobResult
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this._currentJob?.BeginCancel();

            if (this._currentTask != null)
            {
                await this._currentTask.WaitAsync(Timeout.InfiniteTimeSpan);
            }

            var job = new TJob()
            {
                Sender = sender,
            };

            var jobName = $"{job.GetType().Name}: {job.ID}";

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
                                Log.GetLogger().Error(ex, $"{jobName} がUIタスク上で補足されない例外が発生しました。");
                                ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", ex);
                            }
                        }
                    }, null);
                }
            };

            this._currentJob = job;
            this._currentTask = Task.Run(() => this.DoWork(this._currentJob));
        }

        private Task DoWork(AbstractAsyncJob job)
        {
            var jobName = $"{job.GetType().Name}: {job.ID}";

            using (ScopeContext.PushProperty(Log.NLOG_PROPERTY, jobName))
            {
                var logger = Log.GetLogger();

                logger.Debug($"ジョブを実行します。");

                try
                {
                    job.ExecuteWrapper();
                }
                catch (JobCancelException)
                {
                    logger.Debug($"ジョブがキャンセルされました。");
                }
                catch (JobException ex)
                {
                    logger.Error($"{ex}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"ジョブで補足されない例外が発生しました。");
                }
                finally
                {
                    logger.Debug($"ジョブが終了します。");
                }

                return Task.CompletedTask;
            }
        }
    }
}
