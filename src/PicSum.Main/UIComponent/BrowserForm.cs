using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PicSum.Core.Task.AsyncTask;
using PicSum.Main.Conf;
using PicSum.Main.Properties;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.UIComponent.Contents;
using SWF.UIComponent.Form;
using SWF.UIComponent.TabOperation;

namespace PicSum.Main.UIComponent
{
    public class BrowserForm : GrassForm
    {
        #region 定数・列挙

        #endregion

        #region クラスメンバ

        private static TwoWayProcess<StartupAsyncFacade, StartupPrameter, DefaultEntity> _startupProcess = null;

        private static TwoWayProcess<StartupAsyncFacade, StartupPrameter, DefaultEntity> startupProcess
        {
            get
            {
                return _startupProcess;
            }
            set
            {
                _startupProcess = value;
            }
        }

        #endregion

        #region イベント

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowserContentsOpenEventArgs> NewWindowContentsOpen;

        #endregion

        #region インスタンス変数

        private IContainer _components = null;
        private BrowserMainPanel _browserMainPanel = null;
        private bool isKeyDown = false;

        #endregion

        #region パブリックプロパティ

        private BrowserMainPanel browserMainPanel
        {
            get
            {
                if (_browserMainPanel == null)
                {
                    createBrowserMainPanel();
                }

                return _browserMainPanel;
            }
        }

        #endregion

        #region プライベートプロパティ

        #endregion

        #region コンストラクタ

        public BrowserForm()
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void AddContentsEventHandler(BrowserContents contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            browserMainPanel.AddContentsEventHandler(contents);
        }

        public void AddTab(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            browserMainPanel.AddTab(tab);
        }

        public void AddTab(IContentsParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            browserMainPanel.AddTab(param);
        }

        public void AddFavoriteDirectoryListTab()
        {
            browserMainPanel.AddFavoriteDirectoryListTab();
        }

        public void RemoveTabOrWindow()
        {
            if (browserMainPanel.TabCount > 1)
            {
                browserMainPanel.RemoveActiveTab();
            }
            else
            {
                this.Close();
            }

        }

        public void OpenContentsByCommandLineArgs(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            browserMainPanel.OpenContentsByCommandLineArgs(args);
        }

        #endregion

        #region 継承メソッド

        protected override void OnHandleCreated(EventArgs e)
        {
            if (startupProcess == null)
            {
                if (_components == null)
                {
                    _components = new Container();
                }

                startupProcess = TaskManager.CreateTwoWayProcess<StartupAsyncFacade, StartupPrameter, DefaultEntity>(_components);
                startupProcess.Callback += new AsyncTaskCallbackEventHandler<DefaultEntity>(startupProcess_Callback);

                var dbDir = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "db");
                if (!Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                StartupPrameter param = new StartupPrameter();
                param.FileInfoDBFilePath = Path.Combine(dbDir, @"fileinfo.sqlite");
                param.ThumbnailDBFilePath = Path.Combine(dbDir, @"thumbnail.sqlite");

                startupProcess.Execute(this, param);
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
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnDwmCompositionChanged(EventArgs e)
        {
            this.SuspendLayout();

            setProperty();
            setBrowserMainPanelProperty(_browserMainPanel);

            this.ResetGrass();
            this.SetGrass();

            this.ResumeLayout();
        }

        protected virtual void OnTabDropouted(TabDropoutedEventArgs e)
        {
            if (TabDropouted != null)
            {
                TabDropouted(this, e);
            }
        }

        protected virtual void OnNewWindowContentsOpen(BrowserContentsOpenEventArgs e)
        {
            if (NewWindowContentsOpen != null)
            {
                NewWindowContentsOpen(this, e);
            }
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

        private void initializeComponent()
        {
            this.SuspendLayout();

            this.Text = "PicSum";
            this.Icon = new Icon("AppIcon.ico");
            this.AutoScaleMode = AutoScaleMode.None;
            this.StartPosition = FormStartPosition.Manual;
            this.MinimumSize = new Size(320, 240);
            this.TopOffset = 41;
            this.KeyPreview = true;

            setProperty();

            this.Location = BrowserConfig.WindowLocaion;
            this.Size = BrowserConfig.WindowSize;
            this.WindowState = BrowserConfig.WindowState;
            this.SetGrass();

            this.ResumeLayout();
        }

        private void createBrowserMainPanel()
        {
            if (_browserMainPanel != null)
            {
                throw new Exception("メインコントロールは既に存在しています。");
            }

            waitInit();

            BrowserMainPanel browserMainPanel = new BrowserMainPanel();

            setBrowserMainPanelProperty(browserMainPanel);

            browserMainPanel.Close += new EventHandler(browserMainPanel_Close);
            browserMainPanel.BackgroundMouseDoubleLeftClick += new EventHandler(browserMainPanel_BackgroundMouseDoubleLeftClick);
            browserMainPanel.NewWindowContentsOpen += new EventHandler<BrowserContentsOpenEventArgs>(browserMainPanel_NewWindowContentsOpen);
            browserMainPanel.TabDropouted += new EventHandler<TabDropoutedEventArgs>(browserMainPanel_TabDropouted);

            this.SuspendLayout();
            this.Controls.Add(browserMainPanel);
            this.SetControlRegion();
            this.ResumeLayout();

            _browserMainPanel = browserMainPanel;
        }

        private void setProperty()
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

        private void setBrowserMainPanelProperty(BrowserMainPanel browserMainPanel)
        {
            if (this.IsGrassEnabled)
            {
                int x = this.Padding.Left;
                int y = this.Padding.Top;
                int w = this.Width - this.Padding.Left - this.Padding.Right;
                int h = this.Height - this.Padding.Top - this.Padding.Bottom;
                browserMainPanel.SetBounds(x, y, w, h, BoundsSpecified.All);
            }
            else
            {
                int x = this.Padding.Left;
                int y = this.Padding.Top;
                int w = this.ClientRectangle.Width - this.Padding.Left - this.Padding.Right;
                int h = this.ClientRectangle.Height - this.Padding.Top - this.Padding.Bottom;
                browserMainPanel.SetBounds(x, y, w, h, BoundsSpecified.All);
            }

            browserMainPanel.Anchor = ((AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right));
        }

        private void waitInit()
        {
            // ウィンドウの初期表示が終了するまで待機します。
            while (base.IsInit)
            {
                Application.DoEvents();
            }
        }

        #endregion

        #region プロセスコールバックイベント

        private void startupProcess_Callback(object sender, DefaultEntity e)
        {
            createBrowserMainPanel();
        }

        #endregion

        #region ブラウザメインパネルイベント

        private void browserMainPanel_Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void browserMainPanel_BackgroundMouseDoubleLeftClick(object sender, EventArgs e)
        {
            base.MouseLeftDoubleClickProcess();
        }

        private void browserMainPanel_NewWindowContentsOpen(object sender, BrowserContentsOpenEventArgs e)
        {
            OnNewWindowContentsOpen(e);
        }

        private void browserMainPanel_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            OnTabDropouted(e);
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BrowserForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(166, 47);
            this.Name = "BrowserForm";
            this.ResumeLayout(false);

        }
    }
}
