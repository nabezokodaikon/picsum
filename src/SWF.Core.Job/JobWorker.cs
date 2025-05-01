using NLog;
using SWF.Core.Base;
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
    public partial class TwoWayThread<TJob, TJobParameter, TJobResult>
        : IDisposable, ITwoWayJob<TJob, TJobParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : class, IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool _disposed = false;

        private readonly string _threadName;
        private readonly IThreadWrapper _thread;
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

        public TwoWayThread(SynchronizationContext? context, IThreadWrapper thread)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(thread, nameof(thread));

            this._context = context;
            this._thread = thread;
            this._threadName = $"{typeof(TJob).Name} {ThreadID.GetNew()}";
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

                Logger.Debug("ジョブ実行スレッドにキャンセルリクエストを送ります。");
                this._source.Cancel();

                Logger.Debug("ジョブ実行スレッドの終了を待機します。");
                this._thread.Wait();

                Logger.Debug($"{this._threadName}: ジョブ実行スレッドが終了しました。");
                this._thread.Dispose();
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

            if (!this._thread.IsRunning())
            {
                this._thread.Start(() => this.DoWork(this._source.Token));
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
                                    Logger.Error(ex, $"{job.ID} がUIスレッド上で補足されない例外が発生しました。");
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

        private void DoWork(CancellationToken token)
        {
            Thread.CurrentThread.Name = this._threadName;

            Logger.Debug("ジョブ実行スレッドが開始されました。");

            TJob? previewJob = null;

            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                        token.ThrowIfCancellationRequested();
                    }

                    var job = this.CurrentJob;
                    if (job == null || job == previewJob)
                    {
                        token.WaitHandle.WaitOne(1);
                        continue;
                    }

                    previewJob = job;

                    Logger.Debug($"{job.ID} を実行します。");
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        job.ExecuteWrapper();
                    }
                    catch (JobCancelException)
                    {
                        Logger.Debug($"{job.ID} がキャンセルされました。");
                    }
                    catch (JobException ex)
                    {
                        Logger.Error($"{job.ID} {ex}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"{job.ID} で補足されない例外が発生しました。");
                    }
                    finally
                    {
                        sw.Stop();
                        Logger.Debug($"{job.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
                    }

                    token.WaitHandle.WaitOne(1);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Debug("ジョブ実行スレッドをキャンセルします。");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"ジョブ実行スレッドで補足されない例外が発生しました。");
            }
            finally
            {
                Logger.Debug("ジョブ実行スレッドが終了します。");
            }
        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class TwoWayThread<TJob, TJobResult>
        : TwoWayThread<TJob, EmptyParameter, TJobResult>, ITwoWayJob<TJob, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {
        public TwoWayThread(SynchronizationContext? context, IThreadWrapper thread)
            : base(context, thread)
        {

        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayThread<TJob, TJobParameter>
        : TwoWayThread<TJob, TJobParameter, EmptyResult>, IOneWayJob<TJob, TJobParameter>
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : class, IJobParameter
    {
        public OneWayThread(SynchronizationContext? context, IThreadWrapper thread)
            : base(context, thread)
        {

        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class OneWayThread<TJob>
        : TwoWayThread<TJob, EmptyParameter, EmptyResult>, IOneWayJob<TJob>
        where TJob : AbstractOneWayJob, new()
    {
        public OneWayThread(SynchronizationContext? context, IThreadWrapper thread)
            : base(context, thread)
        {

        }
    }
}
