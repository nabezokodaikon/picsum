using NLog;
using PicSum.Job.Parameters;
using PicSum.Main.Conf;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using SWF.UIComponent.Form;
using SWF.UIComponent.TabOperation;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class BrowserForm
        : GrassForm, ISender
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static bool isStartUp = true;

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowserPageOpenEventArgs> NewWindowPageOpen;

        private BrowserMainPanel browserMainPanel = null;
        private bool isKeyDown = false;

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

        public BrowserForm()
        {
            this.SuspendLayout();

            this.Icon = ResourceFiles.ApplicationIcon.Value;
            this.Text = "PicSum";
            this.StartPosition = FormStartPosition.Manual;
            this.MinimumSize = new Size(320, 240);
            this.KeyPreview = true;
            this.Padding = new Padding(0, 8, 0, 0);

            this.Size = BrowserConfig.Instance.WindowSize;
            this.WindowState = BrowserConfig.Instance.WindowState;
            this.ScaleChanged += this.Form_ScaleChanged;

            if (BrowserForm.isStartUp)
            {
                this.Location = BrowserConfig.Instance.WindowLocaion;
            }
            else
            {
                this.Location = new Point(
                    BrowserConfig.Instance.WindowLocaion.X + 16,
                    BrowserConfig.Instance.WindowLocaion.Y + 16);
                this.SetGrass();
            }

            this.ResumeLayout(false);
            this.PerformLayout();
            this.Invalidate();
            this.Update();
        }

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

        public void AddImageViewerPageTab(ImageViewerPageParameter parameter)
        {
            this.BrowserMainPanel.AddImageViewerPageTab(parameter);
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

        protected override bool CanDragOperation()
        {
            return !this.BrowserMainPanel.IsBeginTabDragOperation;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (!BrowserForm.isStartUp)
            {
                return;
            }

            using (TimeMeasuring.Run(false, "BrowserForm.OnShown"))
            {
                this.SetGrass();
                this.CreateBrowserMainPanel();
                BrowserForm.isStartUp = false;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                BrowserConfig.Instance.WindowState = this.WindowState;
                BrowserConfig.Instance.WindowLocaion = this.Location;
                BrowserConfig.Instance.WindowSize = this.Size;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                BrowserConfig.Instance.WindowState = this.WindowState;
            }

            base.OnClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

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
                            this.isKeyDown = true;
                            break;
                        }
                    case Keys.T:
                        {
                            this.AddFavoriteDirectoryListTab();
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

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (this.browserMainPanel != null)
            {
                this.browserMainPanel.RedrawPage();
            }
        }

        private void CreateBrowserMainPanel()
        {
            if (this.browserMainPanel != null)
            {
                throw new SWFException("メインコントロールは既に存在しています。");
            }

            this.browserMainPanel = new BrowserMainPanel();

            var scale = AppConstants.GetCurrentWindowScale(this.Handle);
            var rect = this.CreateBrowserMainPanelBounds(scale);
            this.browserMainPanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            this.browserMainPanel.SetControlsBounds(scale);
            this.browserMainPanel.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.browserMainPanel.Close += new(this.BrowserMainPanel_Close);
            this.browserMainPanel.BackgroundMouseDoubleLeftClick += new(this.BrowserMainPanel_BackgroundMouseDoubleLeftClick);
            this.browserMainPanel.NewWindowPageOpen += new(this.BrowserMainPanel_NewWindowPageOpen);
            this.browserMainPanel.TabDropouted += new(this.BrowserMainPanel_TabDropouted);

            this.SuspendLayout();
            this.Controls.Add(this.browserMainPanel);
            this.AttachResizeEvents(this);
            if (BrowserForm.isStartUp && CommandLineArgs.IsFilePath())
            {
                var imageFilePath = CommandLineArgs.GetImageFilePatCommandLineArgs();
                if (!string.IsNullOrEmpty(imageFilePath))
                {
                    var directoryPath = FileUtil.GetParentDirectoryPath(imageFilePath);

                    var sortInfo = new SortInfo();
                    sortInfo.SetSortType(SortTypeID.FilePath, true);

                    var parameter = new ImageViewerPageParameter(
                        DirectoryFileListPageParameter.PAGE_SOURCES,
                        directoryPath,
                        BrowserMainPanel.GetImageFilesAction(new ImageFileGetByDirectoryParameter(imageFilePath)),
                        imageFilePath,
                        sortInfo,
                        FileUtil.GetFileName(directoryPath),
                        Instance<IFileIconCacher>.Value.SmallDirectoryIcon,
                        true);

                    this.browserMainPanel.AddImageViewerPageTab(parameter);
                }
            }
            else if (BrowserForm.isStartUp && CommandLineArgs.IsEmpty())
            {

            }
            else if (BrowserForm.isStartUp)
            {
                this.browserMainPanel.AddFavoriteDirectoryListTab();

                LogManager.GetCurrentClassLogger().Debug("初期回表示されました。");
            }

            this.SetControlRegion();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Invalidate();
            this.Update();
        }

        private void OnTabDropouted(TabDropoutedEventArgs e)
        {
            this.TabDropouted?.Invoke(this, e);
        }

        private void OnNewWindowPageOpen(BrowserPageOpenEventArgs e)
        {
            this.NewWindowPageOpen?.Invoke(this, e);
        }

        private Rectangle CreateBrowserMainPanelBounds(float scale)
        {
            var x = 0;
            var y = (int)(this.Padding.Top * scale);
            var w = this.Width;
            var h = this.Height - y;
            return new Rectangle(x, y, w, h);
        }

        private void Form_ScaleChanged(object sender, ScaleChangedEventArgs e)
        {
            if (this.browserMainPanel == null)
            {
                return;
            }

            this.SuspendLayout();

            var rect = this.CreateBrowserMainPanelBounds(e.Scale);
            this.browserMainPanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            this.browserMainPanel.SetControlsBounds(e.Scale);

            this.ResumeLayout(false);
            this.PerformLayout();
            this.Invalidate();
            this.Update();
        }

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
    }
}
