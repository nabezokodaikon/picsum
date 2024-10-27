using NLog;
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
        private Control? sender = null;

        private UIThreadAccessor()
        {

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

            }

            this.disposed = true;
        }

        public void SetSynchronizationContext(Control sender)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            this.sender = sender;
        }

        public void Post(Control sender, Action callbackAction)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callbackAction, nameof(callbackAction));

            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(UIThreadAccessor));
            }

            if (this.sender == null)
            {
                throw new NullReferenceException("同期コンテキストが設定されていません。");
            }

            try
            {
                if (this.sender.InvokeRequired && sender.InvokeRequired
                    && this.sender.IsHandleCreated && sender.IsHandleCreated
                    && !this.sender.IsDisposed && !sender.IsDisposed)
                {
                    sender.BeginInvoke((MethodInvoker)delegate
                    {
                        if (this.sender.IsHandleCreated && sender.IsHandleCreated
                            && !this.sender.IsDisposed && !sender.IsDisposed)
                        {
                            callbackAction();
                        }
                    });
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
    }
}
