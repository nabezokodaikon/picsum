using NLog;
using SWF.Core.Base;
using System.Windows.Forms;

namespace SWF.Core.Job
{
    public sealed partial class UIThreadAccessor
        : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static UIThreadAccessor Instance = new UIThreadAccessor();

        private bool disposed = false;
        private SynchronizationContext? context;

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

        public void SetSynchronizationContext()
        {
            if (Thread.CurrentThread.Name != ApplicationConstants.UI_THREAD_NAME)
            {
                throw new InvalidOperationException("同期コンテキストをUIスレッド以外から設定しようとしました。");
            }

            if (SynchronizationContext.Current == null)
            {
                throw new NullReferenceException("同期コンテキストがNUllです。");
            }

            if (this.context != null)
            {
                throw new InvalidOperationException("同期コンテキストは既に設定されています。");
            }

            this.context = SynchronizationContext.Current;
        }

        public void Post(Control sender, Action callbackAction)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callbackAction, nameof(callbackAction));

            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(UIThreadAccessor));
            }

            if (this.context == null)
            {
                throw new InvalidOperationException("同期コンテキストが設定されていません。");
            }

            this.context.Post(_ =>
            {
                if (!sender.IsHandleCreated)
                {
                    return;
                }

                if (sender.IsDisposed)
                {
                    return;
                }

                callbackAction();
            }, null);
        }
    }
}
