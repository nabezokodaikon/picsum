using NLog;
using PicSum.Core.Base.Exception;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Main.Conf;
using PicSum.Task.Paramters;
using PicSum.Task.Tasks;
using PicSum.UIComponent.Contents.Common;
using SWF.Common;
using SWF.UIComponent.Form;
using SWF.UIComponent.TabOperation;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{
    [SupportedOSPlatform("windows")]
    public sealed class BrowserForm
        : GrassForm
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool isStartUp = true;

        private static bool IsCleanup()
        {
            return Environment.GetCommandLineArgs().Contains("--cleanup");
        }

        private static bool IsHome()
        {
            return Environment.GetCommandLineArgs().Contains("--home");
        }

        #region イベント

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowserContentsOpenEventArgs> NewWindowContentsOpen;

        #endregion

        #region インスタンス変数

        private BrowserMainPanel browserMainPanel = null;
        private bool isKeyDown = false;
        private TwoWayTask<StartupTask, StartupPrameter, EmptyResult> startupTask = null;
        private OneWayTask<DBCleanupTask> dbCleanupTask = null;

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
            ArgumentNullException.ThrowIfNull(nameof(contents));

            this.BrowserMainPanel.AddContentsEventHandler(contents);
        }

        public void AddTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(nameof(tab));

            this.BrowserMainPanel.AddTab(tab);
        }

        public void AddTab(IContentsParameter param)
        {
            ArgumentNullException.ThrowIfNull(nameof(param));

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

        protected override void OnHandleCreated(EventArgs e)
        {
            if (BrowserForm.isStartUp)
            {
                this.Location = BrowserConfig.WindowLocaion;

                this.startupTask = new();
                this.startupTask
                    .Catch(ex =>
                        ExceptionUtil.ShowErrorDialog("起動処理が失敗しました。", ex))
                    .Complete(() =>
                    {
                        if (BrowserForm.IsCleanup())
                        {
                            Logger.Debug("DBのクリーンアップ処理を実行します。");
                            this.dbCleanupTask = new();
                            this.dbCleanupTask
                                .Catch(ex =>
                                    ExceptionUtil.ShowErrorDialog("DBクリーンアップ処理が失敗しました。", ex))
                                .Complete(() =>
                                {
                                    this.CreateBrowserMainPanel();
                                    BrowserForm.isStartUp = false;
                                })
                                    .StartThread();
                            this.dbCleanupTask.StartTask();
                        }
                        else
                        {
                            this.CreateBrowserMainPanel();
                            BrowserForm.isStartUp = false;
                        }
                    })
                    .StartThread();

                var dbDir = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "db");
                if (!Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                var param = new StartupPrameter
                {
                    FileInfoDBFilePath = Path.Combine(dbDir, @"fileinfo.sqlite"),
                    ThumbnailDBFilePath = Path.Combine(dbDir, @"thumbnail.sqlite")
                };

                this.startupTask.StartTask(param);
            }
            else
            {
                this.Location = new Point(
                    BrowserConfig.WindowLocaion.X + 16,
                    BrowserConfig.WindowLocaion.Y + 16);
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
            if (disposing)
            {
                if (this.startupTask != null)
                {
                    this.startupTask.Dispose();
                    this.startupTask = null;
                }

                if (this.dbCleanupTask != null)
                {
                    this.dbCleanupTask.Dispose();
                    this.dbCleanupTask = null;
                }
            }

            base.Dispose(disposing);
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
            this.KeyPreview = true;
            this.Padding = new Padding(8, 12, 8, 8);

            this.Size = BrowserConfig.WindowSize;
            this.WindowState = BrowserConfig.WindowState;

            this.SetGrass();

            this.ResumeLayout(false);
        }

        private void CreateBrowserMainPanel()
        {
            if (this.browserMainPanel != null)
            {
                throw new PicSumException("メインコントロールは既に存在しています。");
            }

            var browserMainPanel = new BrowserMainPanel();

            var x = this.Padding.Left;
            var y = this.Padding.Top;
            var w = this.Width - this.Padding.Left - this.Padding.Right;
            var h = this.Height - this.Padding.Top - this.Padding.Bottom;
            browserMainPanel.SetBounds(x, y, w, h, BoundsSpecified.All);
            browserMainPanel.Dock = DockStyle.Fill;

            browserMainPanel.Close += new EventHandler(this.BrowserMainPanel_Close);
            browserMainPanel.BackgroundMouseDoubleLeftClick += new EventHandler(this.BrowserMainPanel_BackgroundMouseDoubleLeftClick);
            browserMainPanel.NewWindowContentsOpen += new EventHandler<BrowserContentsOpenEventArgs>(this.BrowserMainPanel_NewWindowContentsOpen);
            browserMainPanel.TabDropouted += new EventHandler<TabDropoutedEventArgs>(this.BrowserMainPanel_TabDropouted);

            this.SuspendLayout();
            this.Controls.Add(browserMainPanel);

            if (BrowserForm.isStartUp && BrowserForm.IsHome())
            {
                Logger.Debug("起動時にホームタブを追加します。");
                browserMainPanel.AddFavoriteDirectoryListTab();
            }

            this.SetControlRegion();
            this.ResumeLayout();

            this.browserMainPanel = browserMainPanel;
        }

        private void OnTabDropouted(TabDropoutedEventArgs e)
        {
            this.TabDropouted?.Invoke(this, e);
        }

        private void OnNewWindowContentsOpen(BrowserContentsOpenEventArgs e)
        {
            this.NewWindowContentsOpen?.Invoke(this, e);
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
