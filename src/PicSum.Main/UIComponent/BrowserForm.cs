using PicSum.Core.Task.AsyncTask;
using PicSum.Main.Conf;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.UIComponent.Contents.Common;
using SWF.UIComponent.Form;
using SWF.UIComponent.TabOperation;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{
    [SupportedOSPlatform("windows")]
    public sealed class BrowserForm : GrassForm
    {
        #region クラスメンバ

        private static TwoWayProcess<StartupAsyncFacade, StartupPrameter, DefaultEntity> startupProcess = null;

        #endregion

        #region イベント

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowserContentsOpenEventArgs> NewWindowContentsOpen;

        #endregion

        #region インスタンス変数

        private IContainer components = null;
        private BrowserMainPanel browserMainPanel = null;
        private bool isKeyDown = false;

        #endregion

        #region プライベートプロパティ

        private BrowserMainPanel BrowserMainPanel
        {
            get
            {
                if (this.browserMainPanel == null)
                {
                    this.CreateBrowserMainPanel();
                }

                return this.browserMainPanel;
            }
        }

        #endregion

        #region コンストラクタ

        public BrowserForm()
        {
            this.InitializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void AddContentsEventHandler(BrowserContents contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            this.BrowserMainPanel.AddContentsEventHandler(contents);
        }

        public void AddTab(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            this.BrowserMainPanel.AddTab(tab);
        }

        public void AddTab(IContentsParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            this.BrowserMainPanel.AddTab(param);
        }

        public void AddFavoriteDirectoryListTab()
        {
            this.BrowserMainPanel.AddFavoriteDirectoryListTab();
        }

        public void RemoveTabOrWindow()
        {
            if (this.BrowserMainPanel.TabCount > 1)
            {
                this.BrowserMainPanel.RemoveActiveTab();
            }
            else
            {
                this.Close();
            }

        }

        #endregion

        #region 継承メソッド

        protected override void OnResizeEnd(EventArgs e)
        {
            this.browserMainPanel.ResizeEnd();
            base.OnResizeEnd(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (BrowserForm.startupProcess == null)
            {
                if (this.components == null)
                {
                    this.components = new Container();
                }

                BrowserForm.startupProcess = TaskManager.CreateTwoWayProcess<StartupAsyncFacade, StartupPrameter, DefaultEntity>(this.components);
                BrowserForm.startupProcess.Callback += new AsyncTaskCallbackEventHandler<DefaultEntity>(this.StartupProcess_Callback);

                var dbDir = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "db");
                if (!Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                var param = new StartupPrameter();
                param.FileInfoDBFilePath = Path.Combine(dbDir, @"fileinfo.sqlite");
                param.ThumbnailDBFilePath = Path.Combine(dbDir, @"thumbnail.sqlite");

                BrowserForm.startupProcess.Execute(this, param);
            }

            base.OnHandleCreated(e);
        }

        protected override void OnShown(EventArgs e)
        {
            this.Activate();

            base.OnShown(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                BrowserConfig.WindowState = this.WindowState;
                BrowserConfig.WindowLocaion = this.Location;
                BrowserConfig.WindowSize = this.Size;
            }

            base.OnClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnDwmCompositionChanged(EventArgs e)
        {
            this.SuspendLayout();

            this.SetProperty();
            this.SetBrowserMainPanelProperty(this.browserMainPanel);

            this.ResetGrass();
            this.SetGrass();

            this.ResumeLayout();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.isKeyDown)
            {
                return;
            }

            if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        {
                            this.browserMainPanel.MovePreviewContents();
                            this.isKeyDown = true;
                            break;
                        }
                    case Keys.Right:
                        {
                            this.browserMainPanel.MoveNextContents();
                            this.isKeyDown = true;
                            break;
                        }
                }
            }
            else if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        {
                            this.RemoveTabOrWindow();
                            this.Focus();
                            this.isKeyDown = true;
                            break;
                        }
                    case Keys.T:
                        {
                            this.AddFavoriteDirectoryListTab();
                            this.Focus();
                            this.isKeyDown = true;
                            break;
                        }
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            this.isKeyDown = false;
            base.OnKeyUp(e);
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "PicSum";
            this.Icon = new Icon("AppIcon.ico");
            this.AutoScaleMode = AutoScaleMode.None;
            this.StartPosition = FormStartPosition.Manual;
            this.MinimumSize = new Size(320, 240);
            this.TopOffset = 41;
            this.KeyPreview = true;

            this.SetProperty();

            this.Location = BrowserConfig.WindowLocaion;
            this.Size = BrowserConfig.WindowSize;
            this.WindowState = BrowserConfig.WindowState;
            this.SetGrass();

            this.ResumeLayout();
        }

        private void CreateBrowserMainPanel()
        {
            if (this.browserMainPanel != null)
            {
                throw new Exception("メインコントロールは既に存在しています。");
            }

            this.WaitInit();

            var browserMainPanel = new BrowserMainPanel();

            this.SetBrowserMainPanelProperty(browserMainPanel);

            browserMainPanel.Close += new EventHandler(this.BrowserMainPanel_Close);
            browserMainPanel.BackgroundMouseDoubleLeftClick += new EventHandler(this.BrowserMainPanel_BackgroundMouseDoubleLeftClick);
            browserMainPanel.NewWindowContentsOpen += new EventHandler<BrowserContentsOpenEventArgs>(this.BrowserMainPanel_NewWindowContentsOpen);
            browserMainPanel.TabDropouted += new EventHandler<TabDropoutedEventArgs>(this.BrowserMainPanel_TabDropouted);

            this.SuspendLayout();
            this.Controls.Add(browserMainPanel);
            this.SetControlRegion();
            this.ResumeLayout();

            this.browserMainPanel = browserMainPanel;
        }

        private void SetProperty()
        {
            if (this.IsGrassEnabled)
            {
                this.Padding = new Padding(8, 12, 8, 8);
            }
            else
            {
                this.Padding = new Padding(0, 0, 0, 0);
            }
        }

        private void SetBrowserMainPanelProperty(BrowserMainPanel browserMainPanel)
        {
            if (this.IsGrassEnabled)
            {
                var x = this.Padding.Left;
                var y = this.Padding.Top;
                var w = this.Width - this.Padding.Left - this.Padding.Right;
                var h = this.Height - this.Padding.Top - this.Padding.Bottom;
                browserMainPanel.SetBounds(x, y, w, h, BoundsSpecified.All);
            }
            else
            {
                var x = this.Padding.Left;
                var y = this.Padding.Top;
                var w = this.ClientRectangle.Width - this.Padding.Left - this.Padding.Right;
                var h = this.ClientRectangle.Height - this.Padding.Top - this.Padding.Bottom;
                browserMainPanel.SetBounds(x, y, w, h, BoundsSpecified.All);
            }

            browserMainPanel.Anchor = ((AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right));
        }

        private void WaitInit()
        {
            // ウィンドウの初期表示が終了するまで待機します。
            while (base.IsInit)
            {
                Application.DoEvents();
            }
        }

        private void OnTabDropouted(TabDropoutedEventArgs e)
        {
            if (this.TabDropouted != null)
            {
                this.TabDropouted(this, e);
            }
        }

        private void OnNewWindowContentsOpen(BrowserContentsOpenEventArgs e)
        {
            if (this.NewWindowContentsOpen != null)
            {
                this.NewWindowContentsOpen(this, e);
            }
        }

        #endregion

        #region プロセスコールバックイベント

        private void StartupProcess_Callback(object sender, DefaultEntity e)
        {
            this.CreateBrowserMainPanel();
        }

        #endregion

        #region ブラウザメインパネルイベント

        private void BrowserMainPanel_Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BrowserMainPanel_BackgroundMouseDoubleLeftClick(object sender, EventArgs e)
        {
            base.MouseLeftDoubleClickProcess();
        }

        private void BrowserMainPanel_NewWindowContentsOpen(object sender, BrowserContentsOpenEventArgs e)
        {
            this.OnNewWindowContentsOpen(e);
        }

        private void BrowserMainPanel_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            this.OnTabDropouted(e);
        }

        #endregion
    }
}
