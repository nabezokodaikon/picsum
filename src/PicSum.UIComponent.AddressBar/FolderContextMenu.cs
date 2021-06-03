using System;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    /// <summary>
    /// フォルダコンテキストメニューストリップ
    /// </summary>
    internal class FolderContextMenu : ContextMenuStrip
    {
        #region イベント

        public event EventHandler ActiveTabOpen;
        public event EventHandler NewTabOpen;
        public event EventHandler OtherWindowOpen;
        public event EventHandler NewWindowOpen;

        #endregion

        #region インスタンス変数

        private bool _isOpen = false;
        private ToolStripMenuItem _activeTabOpenMenuItem = new ToolStripMenuItem("開く");
        private ToolStripMenuItem _newTabOpenMenuItem = new ToolStripMenuItem("新しいタブで開く");
        private ToolStripMenuItem _otherWindowOpenMenuItem = new ToolStripMenuItem("別のウィンドウで開く");
        private ToolStripMenuItem _newWindowOpenMenuItem = new ToolStripMenuItem("新しいウィンドウで開く");

        #endregion

        #region プロパティ

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
        }

        #endregion

        #region コンストラクタ

        public FolderContextMenu()
        {
            initializeComponent();
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.ShowImageMargin = false;

            this.Items.AddRange(new ToolStripItem[] { _activeTabOpenMenuItem, 
                                                      _newTabOpenMenuItem, 
                                                      _otherWindowOpenMenuItem, 
                                                      _newWindowOpenMenuItem});

            _activeTabOpenMenuItem.MouseUp += new MouseEventHandler(_activeTabOpenMenuItem_MouseUp);
            _newTabOpenMenuItem.MouseUp += new MouseEventHandler(_newTabOpenMenuItem_MouseUp);
            _otherWindowOpenMenuItem.MouseUp += new MouseEventHandler(_otherWindowOpenMenuItem_MouseUp);
            _newWindowOpenMenuItem.MouseUp += new MouseEventHandler(_newWindowOpenMenuItem_MouseUp);
        }

        private void _activeTabOpenMenuItem_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnActiveTabOpen(new EventArgs());
            }
        }

        private void _newTabOpenMenuItem_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnNewTabOpen(new EventArgs());
            }
        }

        private void _otherWindowOpenMenuItem_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnOtherWindowOpen(new EventArgs());
            }
        }

        private void _newWindowOpenMenuItem_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnNewWindowOpen(new EventArgs());
            }
        }

        #endregion

        #region イベント発生メソッド

        protected virtual void OnActiveTabOpen(EventArgs e)
        {
            if (ActiveTabOpen != null)
            {
                ActiveTabOpen(this, e);
            }
        }

        protected virtual void OnNewTabOpen(EventArgs e)
        {
            if (NewTabOpen != null)
            {
                NewTabOpen(this, e);
            }
        }

        protected virtual void OnOtherWindowOpen(EventArgs e)
        {
            if (OtherWindowOpen != null)
            {
                OtherWindowOpen(this, e);
            }
        }

        protected virtual void OnNewWindowOpen(EventArgs e)
        {
            if (NewWindowOpen != null)
            {
                NewWindowOpen(this, e);
            }
        }

        #endregion

        #region 継承メソッド

        protected override void OnOpening(System.ComponentModel.CancelEventArgs e)
        {
            _isOpen = true;

            base.OnOpening(e);
        }

        protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
        {
            _isOpen = false;

            base.OnClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _activeTabOpenMenuItem.Dispose();
                _newTabOpenMenuItem.Dispose();
                _otherWindowOpenMenuItem.Dispose();
                _newWindowOpenMenuItem.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
