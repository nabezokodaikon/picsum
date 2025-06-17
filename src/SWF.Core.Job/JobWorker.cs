using NLog;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    public interface ITwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : class, IJobParameter
        where TJobResult : IJobResult
    {
        public void StartJob(ISender sender, TJobParameter? parameter, Action<TJobResult>? callback);
        public void StartJob(ISender sender, Action<TJobResult> callback);
        public void StartJob(ISender sender, TJobParameter parameter);
        public void StartJob(ISender sender);
        public void BeginCancel();
    }

    public interface ITwoWayJob<TJob, TJobResult>
        : ITwoWayJob<TJob, EmptyParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {

    }

    public interface IOneWayJob<TJob>
        : ITwoWayJob<TJob, EmptyParameter, EmptyResult>
        where TJob : AbstractOneWayJob, new()
    {

    }

    public interface IOneWayJob<TJob, TJobParameter>
        : ITwoWayJob<TJob, TJobParameter, EmptyResult>
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : class, IJobParameter
    {

    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class TwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable, ITwoWayJob<TJob, TJobParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : class, IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = Log.Writer;

        private bool _disposed = false;

        private readonly string _taskName;
        private readonly JobTask _task;
        private readonly SynchronizationContext _context;
        private readonly CancellationTokenSource _source = new();
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

        public TwoWayJob(SynchronizationContext? context, JobTask task)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(task, nameof(task));

            this._context = context;
            this._task = task;
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
                this.BeginCancel();

                Log.Writer.Debug("ジョブ実行タスクにキャンセルリクエストを送ります。");
                this._source.Cancel();

                Log.Writer.Debug("ジョブ実行タスクの終了を待機します。");
                this._task.Wait();

                Log.Writer.Debug($"{this._taskName}: ジョブ実行タスクが終了しました。");
                this._task.Dispose();
                this._source.Dispose();
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

            if (!this._task.IsRunning())
            {
                this._task.Start(
                    () => this.DoWork(this._source.Token).GetAwaiter().GetResult());
            }

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
                                    Log.Writer.Error(ex, $"{job.ID} がUIタスク上で補足されない例外が発生しました。");
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

        private async Task DoWork(CancellationToken token)
        {
            using (ScopeContext.PushProperty(Log.NLOG_PROPERTY, this._taskName))
            {
                Log.Writer.Debug("ジョブ実行タスクが開始されました。");

                TJob? previewJob = null;

                try
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                        {
                            Log.Writer.Debug("ジョブ実行タスクにキャンセルリクエストがありました。");
                            token.ThrowIfCancellationRequested();
                        }

                        var job = this.CurrentJob;
                        if (job == null || job == previewJob)
                        {
                            await Task.Delay(1, token);
                            continue;
                        }

                        previewJob = job;

                        Log.Writer.Debug($"{job.ID} を実行します。");
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            job.ExecuteWrapper().GetAwaiter().GetResult();
                        }
                        catch (JobCancelException)
                        {
                            Log.Writer.Debug($"{job.ID} がキャンセルされました。");
                        }
                        catch (JobException ex)
                        {
                            Log.Writer.Error($"{job.ID} {ex}");
                        }
                        catch (Exception ex)
                        {
                            Log.Writer.Error(ex, $"{job.ID} で補足されない例外が発生しました。");
                        }
                        finally
                        {
                            sw.Stop();
                            Log.Writer.Debug($"{job.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
                        }

                        await Task.Delay(1, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.Writer.Debug("ジョブ実行タスクをキャンセルします。");
                }
                catch (Exception ex)
                {
                    Log.Writer.Error(ex, $"ジョブ実行タスクで補足されない例外が発生しました。");
                }
                finally
                {
                    Log.Writer.Debug("ジョブ実行タスクが終了します。");
                }
            }
        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class TwoWayJob<TJob, TJobResult>
        : TwoWayJob<TJob, EmptyParameter, TJobResult>, ITwoWayJob<TJob, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {
        public TwoWayJob(SynchronizationContext? context, JobTask task)
            : base(context, task)
        {

        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayJob<TJob, TJobParameter>
        : TwoWayJob<TJob, TJobParameter, EmptyResult>, IOneWayJob<TJob, TJobParameter>
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : class, IJobParameter
    {
        public OneWayJob(SynchronizationContext? context, JobTask task)
            : base(context, task)
        {

        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayJob<TJob>
        : TwoWayJob<TJob, EmptyParameter, EmptyResult>, IOneWayJob<TJob>
        where TJob : AbstractOneWayJob, new()
    {
        public OneWayJob(SynchronizationContext? context, JobTask task)
            : base(context, task)
        {

        }
    }
}
