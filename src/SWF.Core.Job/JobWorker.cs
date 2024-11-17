using NLog;
using SWF.Core.Base;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.Job
{
    public interface ITwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
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
        where TJobParameter : IJobParameter
    {

    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class TwoWayThread<TJob, TJobParameter, TJobResult>
        : IDisposable, ITwoWayJob<TJob, TJobParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool disposed = false;

        private readonly string threadName;
        private readonly IThreadWrapper thread;
        private readonly SynchronizationContext context;
        private readonly ConcurrentQueue<TJob> jobQueue = new();
        private readonly CancellationTokenSource source = new();

        public TwoWayThread(SynchronizationContext? context, IThreadWrapper thread)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(thread, nameof(thread));

            this.context = context;
            this.thread = thread;
            this.threadName = $"{typeof(TJob).Name} {ThreadID.GetNew()}";
        }

        ~TwoWayThread()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.BeginCancel();

                Logger.Debug("ジョブ実行スレッドにキャンセルリクエストを送ります。");
                this.source.Cancel();

                Logger.Debug("ジョブ実行スレッドの終了を待機します。");
                this.thread.Wait();

                Logger.Debug($"{this.threadName}: ジョブ実行スレッドが終了しました。");
                this.thread.Dispose();
                this.source.Dispose();
            }

            this.disposed = true;
        }

        public void BeginCancel()
        {
            foreach (var job in this.jobQueue.ToArray())
            {
                job.BeginCancel();
            }
        }

        public void StartJob(ISender sender, TJobParameter? parameter, Action<TJobResult>? callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.BeginCancel();
            this.thread.Start(() => this.DoWork(this.source.Token));

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
                        this.context.Post(_ =>
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

            this.jobQueue.Enqueue(job);
        }

        public void StartJob(ISender sender, Action<TJobResult> callback)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            this.StartJob(sender, default, callback);
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

            this.StartJob(sender, default, null);
        }

        private void DoWork(CancellationToken token)
        {
            Thread.CurrentThread.Name = this.threadName;

            Logger.Debug("ジョブ実行スレッドが開始されました。");

            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Logger.Debug("ジョブ実行スレッドにキャンセルリクエストがありました。");
                        token.ThrowIfCancellationRequested();
                    }

                    if (this.jobQueue.TryPeek(out var currentJob))
                    {
                        Logger.Debug($"{currentJob.ID} を実行します。");
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            currentJob.ExecuteWrapper();
                        }
                        catch (JobCancelException)
                        {
                            Logger.Debug($"{currentJob.ID} がキャンセルされました。");
                        }
                        catch (JobException ex)
                        {
                            Logger.Error($"{currentJob.ID} {ex}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, $"{currentJob.ID} で補足されない例外が発生しました。");
                        }
                        finally
                        {
                            if (this.jobQueue.TryDequeue(out var dequeueJob))
                            {
                                if (currentJob != dequeueJob)
                                {
#pragma warning disable CA2219
                                    throw new InvalidOperationException("キューからPeekしたジョブとDequeueしたジョブが一致しません。");
#pragma warning restore CA2219
                                }
                            }
                            else
                            {
#pragma warning disable CA2219
                                throw new InvalidOperationException("他のスレッドでキューの操作が行われました。");
#pragma warning restore CA2219
                            }

                            sw.Stop();
                            Logger.Debug($"{currentJob.ID} が終了しました。{sw.ElapsedMilliseconds} ms");
                        }
                    }
                    else
                    {
                        token.WaitHandle.WaitOne(1);
                    }
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
        where TJobParameter : IJobParameter
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
