using NLog;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Windows.Forms;

namespace SWF.Core.Job
{
    public sealed partial class UIThreadAccessor
        : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static readonly UIThreadAccessor Instance = new();

        private bool disposed = false;
        private readonly Task thread;
        private readonly CancellationTokenSource source = new();
        private readonly ConcurrentQueue<UIThreadAccessItem> queue = new();

        private UIThreadAccessor()
        {
            this.thread = Task.Run(() => this.DoWork(this.source.Token));
        }

        ~UIThreadAccessor()
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
                this.source.Cancel();
                this.thread.Wait();

                this.source.Dispose();
                this.thread.Dispose();
            }

            this.disposed = true;
        }

        public void Post(AbstractAsyncJob job, Action callbackAction)
        {
            ArgumentNullException.ThrowIfNull(job, nameof(job));
            ArgumentNullException.ThrowIfNull(callbackAction, nameof(callbackAction));

            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(UIThreadAccessor));
            }

            //this.queue.Enqueue(new UIThreadAccessItem(job, callbackAction));
            //return;

            try
            {
                if (job.Sender.InvokeRequired
                    && job.Sender.IsHandleCreated
                    && !job.Sender.IsDisposed)
                {
                    job.Sender.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (job.Sender.IsHandleCreated
                            && !job.Sender.IsDisposed)
                        {
                            callbackAction();
                        }
                    }));
                }
            }
            catch (ObjectDisposedException)
            {
                Logger.Debug("同期コンテキストは破棄されています。");
            }
            catch (InvalidOperationException)
            {
                Logger.Debug("UIスレッドにアクセスできませんでした。");
            }
            catch (ThreadInterruptedException)
            {
                Logger.Debug("UIスレッドが中断されています。");
            }
            catch (InvalidAsynchronousStateException)
            {
                Logger.Debug("UIスレッドへのアクセスに失敗しました。");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "UIスレッドへのアクセスに失敗しました。");
            }
        }

        private void DoWork(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (this.queue.TryDequeue(out var item))
                {
                    if (item.Job.Sender.InvokeRequired
                        && item.Job.Sender.IsHandleCreated
                        && !item.Job.Sender.IsDisposed)
                    {
                        try
                        {
                            // TODO: 画像ビューアページでスライダーを動かしているとおかしな挙動になる。
                            item.Job.Sender.Invoke(new MethodInvoker(() =>
                            {
                                if (item.Job.Sender.IsHandleCreated
                                    && !item.Job.Sender.IsDisposed)
                                {
                                    item.CallbackAction();
                                }
                            }));
                        }
                        catch (ObjectDisposedException)
                        {
                            Logger.Debug("同期コンテキストは破棄されています。");
                        }
                        catch (InvalidOperationException)
                        {
                            Logger.Debug("UIスレッドにアクセスできませんでした。");
                        }
                        catch (ThreadInterruptedException)
                        {
                            Logger.Debug("UIスレッドが中断されています。");
                        }
                        catch (InvalidAsynchronousStateException)
                        {
                            Logger.Debug("UIスレッドへのアクセスに失敗しました。");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "UIスレッドへのアクセスに失敗しました。");
                        }
                    }
                }

                token.WaitHandle.WaitOne(1);
            }
        }
    }
}
