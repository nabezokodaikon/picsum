using SWF.UIComponent.TabOperation;
using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.Common
{
    /// <summary>
    /// コンテンツ基底クラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract class BrowserPage
        : PagePanel
    {
        private bool disposed = false;

        public event EventHandler<SelectedFileChangeEventArgs> SelectedFileChanged;
        public event EventHandler<BrowserPageEventArgs> OpenPage;
        public new event EventHandler<MouseEventArgs> MouseClick;

        public abstract string SelectedFilePath { get; protected set; }

        protected IPageParameter Parameter { get; private set; }

        public BrowserPage(IPageParameter parameter)
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public abstract override void RedrawPage();

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        protected virtual void OnSelectedFileChanged(SelectedFileChangeEventArgs e)
        {
            this.SelectedFileChanged?.Invoke(this, e);
        }

        protected virtual void OnOpenPage(BrowserPageEventArgs e)
        {
            this.OpenPage?.Invoke(this, e);
        }

        protected new virtual void OnMouseClick(MouseEventArgs e)
        {
            this.MouseClick?.Invoke(this, e);
        }

        protected abstract void OnBackgroundMouseClick(MouseEventArgs e);

    }
}
