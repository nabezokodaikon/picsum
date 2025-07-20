using SWF.UIComponent.TabOperation;
using System;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.Common
{
    /// <summary>
    /// コンテンツ基底クラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract class AbstractBrowsePage
        : PagePanel
    {
        private bool _disposed = false;

        public event EventHandler<SelectedFileChangeEventArgs> SelectedFileChanged;
        public event EventHandler<BrowsePageEventArgs> OpenPage;
        public new event EventHandler<MouseEventArgs> MouseClick;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public abstract string SelectedFilePath { get; protected set; }

        protected IPageParameter Parameter { get; private set; }
        protected bool IsLoaded { get; private set; }

        public AbstractBrowsePage(IPageParameter parameter)
        {
            this.Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public abstract override void StopPageDraw();

        public abstract string[] GetSelectedFiles();

        protected override void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        protected virtual void OnSelectedFileChanged(SelectedFileChangeEventArgs e)
        {
            this.SelectedFileChanged?.Invoke(this, e);
        }

        protected virtual void OnOpenPage(BrowsePageEventArgs e)
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
