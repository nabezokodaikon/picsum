using NLog;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace SWF.Core.Job
{
    public sealed partial class UIThreadAccessor
        : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static UIThreadAccessor Instance = new UIThreadAccessor();

        public static void DisposeStaticResources()
        {
            Instance.Dispose();
        }

        private bool disposed = false;
        private readonly ConcurrentQueue<CallbackActionItem> callbackQueue;
        private readonly CancellationTokenSource source;
        private readonly Task thread;

        private UIThreadAccessor()
        {
            this.callbackQueue = new();
            this.source = new();
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

        private void DoWork(CancellationToken token)
        {
            Thread.CurrentThread.Name = "UIThreadAccessor";

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (this.callbackQueue.TryDequeue(out var item))
                {
                    if (!item.Sender.IsHandleCreated)
                    {
                        return;
                    }

                    if (item.Sender.IsDisposed)
                    {
                        return;
                    }

                    if (item.Sender.InvokeRequired)
                    {
                        try
                        {
                            item.Sender.Invoke((MethodInvoker)delegate
                            {
                                item.CallbackAction();
                            });
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
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "UIスレッドへのアクセスに失敗しました。");
                        }
                    }
                    else
                    {
                        item.CallbackAction();
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        public void Post(Control sender, Action callbackAction)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callbackAction, nameof(callbackAction));

            if (this.disposed)
            {
                throw new ObjectDisposedException("同期コンテキストは破棄されています。");
            }

            this.callbackQueue.Enqueue(new CallbackActionItem(sender, callbackAction));
        }
    }
}
