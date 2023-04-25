using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents
{
    /// <summary>
    /// コンテンツ基底クラス
    /// </summary>
    public abstract class BrowserContents : ContentsPanel
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        public event EventHandler<SelectedFileChangeEventArgs> SelectedFileChanged;
        public event EventHandler<BrowserContentsEventArgs> OpenContents;
        public new event EventHandler<MouseEventArgs> MouseClick;

        #endregion

        #region インスタンス変数

        private IContainer _processContainer = null;

        #endregion

        #region パブリックプロパティ

        public abstract string SelectedFilePath { get; protected set; }

        #endregion

        #region 継承プロパティ

        protected IContainer ProcessContainer
        {
            get
            {
                if (_processContainer == null)
                {
                    _processContainer = new Container();
                }

                return _processContainer;
            }
        }

        #endregion

        #region プライベートプロパティ

        protected IContentsParameter Parameter { get; private set; }

        #endregion

        #region コンストラクタ

        public BrowserContents(IContentsParameter parameter)
        {
            initializeComponent();
            this.Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_processContainer != null)
                {
                    _processContainer.Dispose();
                    _processContainer = null;
                }
            }

            base.Dispose(disposing);
        }

        protected virtual void OnSelectedFileChanged(SelectedFileChangeEventArgs e)
        {
            if (SelectedFileChanged != null)
            {
                SelectedFileChanged(this, e);
            }
        }

        protected virtual void OnOpenContents(BrowserContentsEventArgs e)
        {
            if (OpenContents != null)
            {
                OpenContents(this, e);
            }
        }

        protected new virtual void OnMouseClick(MouseEventArgs e)
        {
            if (this.MouseClick == null) return;
            this.MouseClick(this, e);
        }

        protected virtual void OnBackgroundMouseClick(MouseEventArgs e)
        {
            throw new NotImplementedException("継承先のメソッドを使用してください。");
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {

        }

        #endregion
    }
}
