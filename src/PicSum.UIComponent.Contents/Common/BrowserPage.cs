using SWF.UIComponent.TabOperation;
using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.Common
{
    /// <summary>
    /// コンテンツ基底クラス
    /// </summary>
    [SupportedOSPlatform("windows")]
    public abstract class BrowserPage
        : PagePanel
    {
        #region イベント・デリゲート

        public event EventHandler<SelectedFileChangeEventArgs> SelectedFileChanged;
        public event EventHandler<BrowserPageEventArgs> OpenPage;
        public new event EventHandler<MouseEventArgs> MouseClick;

        #endregion

        #region パブリックプロパティ

        public abstract string SelectedFilePath { get; protected set; }

        #endregion

        #region プライベートプロパティ

        protected IPageParameter Parameter { get; private set; }

        #endregion

        #region コンストラクタ

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

        #endregion

        #region パブリックメソッド

        public abstract override void RedrawPage();

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

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

        #endregion
    }
}
