using NLog;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace SWF.Core.Job
{
    public sealed partial class SynchronizationContextWrapper
        : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

#pragma warning disable CS8618
        private static SynchronizationContextWrapper innerInstance;
#pragma warning restore CS8618

        public static SynchronizationContextWrapper Instance
        {
            get
            {
                if (innerInstance == null)
                {
                    throw new NullReferenceException("同期コンテキストが設定されていません。");
                }

                return innerInstance;
            }
        }

        public static void SetSynchronizationContext(Control control)
        {
            ArgumentNullException.ThrowIfNull(control, nameof(control));

            if (innerInstance != null)
            {
                throw new InvalidOperationException("同期コンテキストは既に設定されています。");
            }

            innerInstance = new SynchronizationContextWrapper(control);
        }

        public static void DisposeStaticResources()
        {
            innerInstance.Dispose();
        }

        private bool disposed = false;
        private readonly Control control;
        private readonly ConcurrentQueue<Action> callbackQueue;
        private readonly CancellationTokenSource source;
        private readonly Task thread;

        private SynchronizationContextWrapper(Control control)
        {
            this.control = control;
            this.callbackQueue = new();
            this.source = new();
            this.thread = Task.Run(() => this.DoWork(this.source.Token));
        }

        ~SynchronizationContextWrapper()
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
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (this.callbackQueue.TryDequeue(out var callbackAction))
                {
                    if (!this.control.IsHandleCreated)
                    {
                        return;
                    }

                    if (this.control.IsDisposed)
                    {
                        return;
                    }

                    if (this.control.InvokeRequired)
                    {
                        try
                        {
                            this.control.Invoke((MethodInvoker)delegate
                            {
                                callbackAction();
                            });
                        }
                        catch (ObjectDisposedException ex)
                        {
                            Logger.Error(ex, "同期コンテキストは破棄されています。");
                        }
                        catch (InvalidOperationException ex)
                        {
                            Logger.Error(ex, "UIスレッドにアクセスできませんでした。");
                        }
                        catch (ThreadInterruptedException ex)
                        {
                            Logger.Error(ex, "UIスレッドが中断されています。");
                        }
                    }
                    else
                    {
                        callbackAction();
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        public void Post(Action callbackAction)
        {
            ArgumentNullException.ThrowIfNull(callbackAction, nameof(callbackAction));

            if (this.disposed)
            {
                throw new ObjectDisposedException("同期コンテキストは破棄されています。");
            }

            if (this.control == null)
            {
                throw new NullReferenceException("同期コンテキストが設定されていません。");
            }

            this.callbackQueue.Enqueue(callbackAction);
        }
    }
}
