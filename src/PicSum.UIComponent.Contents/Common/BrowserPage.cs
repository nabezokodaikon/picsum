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
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.SubInitializeComponent();
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
            if (this.SelectedFileChanged != null)
            {
                this.SelectedFileChanged(this, e);
            }
        }

        protected virtual void OnOpenPage(BrowserPageEventArgs e)
        {
            if (this.OpenPage != null)
            {
                this.OpenPage(this, e);
            }
        }

        protected new virtual void OnMouseClick(MouseEventArgs e)
        {
            if (this.MouseClick != null)
            {
                this.MouseClick(this, e);
            }
        }

        protected abstract void OnBackgroundMouseClick(MouseEventArgs e);

        #endregion

        #region プライベートメソッド

        private void SubInitializeComponent()
        {

        }

        #endregion
    }
}
