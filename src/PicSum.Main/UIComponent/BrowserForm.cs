using PicSum.Core.Base.Exception;
using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Main.Conf;
using PicSum.UIComponent.Contents.Common;
using SWF.Core.Base;
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
        public event EventHandler<BrowserPageOpenEventArgs> NewWindowPageOpen;

        #endregion

        #region インスタンス変数

        private BrowserMainPanel browserMainPanel = null;
        private bool isKeyDown = false;
        private TwoWayJob<StartupJob, StartupPrameter, EmptyResult> startupJob = null;
        private OneWayJob<FileInfoDBCleanupJob> fileInfoDBCleanupJob = null;
        private OneWayJob<ThumbnailDBCleanupJob, ValueParameter<string>> thumbnailDBCleanupJob = null;

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

        public void AddPageEventHandler(BrowserPage page)
        {
            ArgumentNullException.ThrowIfNull(page, nameof(page));

            this.BrowserMainPanel.AddPageEventHandler(page);
        }

        public void AddTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            this.BrowserMainPanel.AddTab(tab);
        }

        public void AddTab(IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this.BrowserMainPanel.AddTab(param);
        }

        public void AddFavoriteDirectoryListTab()
        {
            this.BrowserMainPanel.AddFavoriteDirectoryListTab();
        }

        public void Reload()
        {
            this.BrowserMainPanel.Reload();
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

                var dbDir = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "db");
                if (!Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                var startupParameter = new StartupPrameter
                {
                    FileInfoDBFilePath = Path.Combine(dbDir, @"fileinfo.sqlite"),
                    ThumbnailDBFilePath = Path.Combine(dbDir, @"thumbnail.sqlite")
                };

                this.fileInfoDBCleanupJob = new();
                this.fileInfoDBCleanupJob
                    .Catch(ex =>
                    {
                        ExceptionUtil.ShowErrorDialog("ファイル情報データベースのクリーンアップ処理が失敗しました。", ex);
                    })
                    .Complete(() =>
                    {
                        this.CreateBrowserMainPanel();
                        BrowserForm.isStartUp = false;
                    });

                this.thumbnailDBCleanupJob = new();
                this.thumbnailDBCleanupJob
                    .Catch(ex =>
                    {
                        ExceptionUtil.ShowErrorDialog("サムネイルデータベースのクリーンアップ処理が失敗しました。", ex);
                    })
                    .Complete(() =>
                    {
                        this.startupJob.StartThread();
                        this.startupJob.StartJob(startupParameter);
                    });

                this.startupJob = new();
                this.startupJob
                    .Catch(ex =>
                    {
                        ExceptionUtil.ShowErrorDialog("起動に失敗しました。", ex);
                    })
                    .Complete(() =>
                    {
                        if (IsCleanup())
                        {
                            this.fileInfoDBCleanupJob.StartThread();
                            this.fileInfoDBCleanupJob.StartJob();
                        }
                        else
                        {
                            this.CreateBrowserMainPanel();
                            BrowserForm.isStartUp = false;
                        }
                    });

                if (BrowserForm.IsCleanup())
                {
                    this.thumbnailDBCleanupJob.StartThread();
                    this.thumbnailDBCleanupJob.StartJob(
                        new ValueParameter<string>(dbDir));
                }
                else
                {
                    this.startupJob.StartThread();
                    this.startupJob.StartJob(startupParameter);
                }
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
            else if (this.WindowState == FormWindowState.Maximized)
            {
                BrowserConfig.WindowState = this.WindowState;
            }

            base.OnClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.startupJob != null)
                {
                    this.startupJob.Dispose();
                    this.startupJob = null;
                }

                if (this.fileInfoDBCleanupJob != null)
                {
                    this.fileInfoDBCleanupJob.Dispose();
                    this.fileInfoDBCleanupJob = null;
                }

                if (this.thumbnailDBCleanupJob != null)
                {
                    this.thumbnailDBCleanupJob.Dispose();
                    this.thumbnailDBCleanupJob = null;
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
                            this.browserMainPanel.MovePreviewPage();
                            this.isKeyDown = true;
                            break;
                        }
                    case Keys.Right:
                        {
                            this.browserMainPanel.MoveNextPage();
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
                    case Keys.R:
                        {
                            this.Reload();
                            this.isKeyDown = true;
                            break;
                        }
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                this.Reload();
                this.isKeyDown = true;
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

            browserMainPanel.Close += new(this.BrowserMainPanel_Close);
            browserMainPanel.BackgroundMouseDoubleLeftClick += new(this.BrowserMainPanel_BackgroundMouseDoubleLeftClick);
            browserMainPanel.NewWindowPageOpen += new(this.BrowserMainPanel_NewWindowPageOpen);
            browserMainPanel.TabDropouted += new(this.BrowserMainPanel_TabDropouted);

            this.SuspendLayout();
            this.Controls.Add(browserMainPanel);

            if (BrowserForm.isStartUp && BrowserForm.IsHome())
            {
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

        private void OnNewWindowPageOpen(BrowserPageOpenEventArgs e)
        {
            this.NewWindowPageOpen?.Invoke(this, e);
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

        private void BrowserMainPanel_NewWindowPageOpen(object sender, BrowserPageOpenEventArgs e)
        {
            this.OnNewWindowPageOpen(e);
        }

        private void BrowserMainPanel_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            this.OnTabDropouted(e);
        }

        #endregion
    }
}
