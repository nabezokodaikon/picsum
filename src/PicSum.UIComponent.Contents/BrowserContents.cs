using SWF.UIComponent.TabOperation;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents
{
    /// <summary>
    /// コンテンツ基底クラス
    /// </summary>
    public abstract class BrowserContents 
        : ContentsPanel
    {
        #region イベント・デリゲート

        public event EventHandler<SelectedFileChangeEventArgs> SelectedFileChanged;
        public event EventHandler<BrowserContentsEventArgs> OpenContents;
        public new event EventHandler<MouseEventArgs> MouseClick;

        #endregion

        #region インスタンス変数

        private IContainer processContainer = null;

        #endregion

        #region パブリックプロパティ

        public abstract string SelectedFilePath { get; protected set; }

        #endregion

        #region 継承プロパティ

        protected IContainer ProcessContainer
        {
            get
            {
                if (this.processContainer == null)
                {
                    this.processContainer = new Container();
                }

                return this.processContainer;
            }
        }

        #endregion

        #region プライベートプロパティ

        protected IContentsParameter Parameter { get; private set; }

        #endregion

        #region コンストラクタ

        public BrowserContents(IContentsParameter parameter)
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint, true);

            this.SubInitializeComponent();
            this.Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.processContainer != null)
                {
                    this.processContainer.Dispose();
                    this.processContainer = null;
                }
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

        protected virtual void OnOpenContents(BrowserContentsEventArgs e)
        {
            if (this.OpenContents != null)
            {
                this.OpenContents(this, e);
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
