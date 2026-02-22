using PicSum.Job.Parameters;
using PicSum.Main.Conf;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Form;
using SWF.UIComponent.TabOperation;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{
    internal sealed partial class BrowseWindow
        : GrassForm, ISender
    {
        private const float PADDING_TOP = 8f;

        private static bool isStartUp = true;

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowsePageOpenEventArgs> NewWindowPageOpen;

        private BrowsePanel _browsePanel = null;
        private bool _isKeyDown = false;

        private BrowsePanel BrowsePanel
        {
            get
            {
                if (this._browsePanel == null)
                {
                    this.CreateBrowsePanel();
                }

                return this._browsePanel;
            }
        }

        public BrowseWindow()
            : base()
        {
            this.Icon = ResourceFiles.AppIcon.Value;
            this.Text = "PicSum";
            this.StartPosition = FormStartPosition.Manual;
            this.KeyPreview = true;

            this.ScaleChanged += this.BrowseWindow_ScaleChanged;
            this.HandleCreated += this.BrowseWindow_HandleCreated;
            this.Shown += this.BrowseWindow_Shown;
            this.FormClosed += this.BrowseWindow_FormClosed;
            this.KeyDown += this.BrowseWindow_KeyDown;
            this.KeyUp += this.BrowseWindow_KeyUp;
            this.Activated += this.BrowseWindow_Activated;
        }

        public void AddPageEventHandler(AbstractBrowsePage page)
        {
            ArgumentNullException.ThrowIfNull(page, nameof(page));

            this.BrowsePanel.AddPageEventHandler(page);
        }

        public void AddTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            this.BrowsePanel.AddTab(tab);
        }

        public void AddTab(IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this.BrowsePanel.AddTab(param);
        }

        public void AddFavoriteDirectoryListTab()
        {
            this.BrowsePanel.AddFavoriteDirectoryListTab();
        }

        public void AddImageViewPageTab(ImageViewPageParameter parameter)
        {
            this.BrowsePanel.AddImageViewPageTab(parameter);
        }

        public void Reload()
        {
            this.BrowsePanel.Reload();
        }

        public void RemoveTabOrWindow()
        {
            if (this.BrowsePanel.TabCount > 1)
            {
                this.BrowsePanel.RemoveActiveTab();
            }
            else
            {
                this.Close();
            }
        }

        protected override bool IsBeginTabDragOperation()
        {
            return this.BrowsePanel.IsBeginTabDragOperation;
        }

        private void BrowseWindow_HandleCreated(object sender, EventArgs e)
        {
            using (Measuring.Time(true, "BrowseForm.BrowseWindow_HandleCreated"))
            {
                if (BrowseWindow.isStartUp)
                {
                    this.SuspendLayout();

                    var workingArea = Screen.GetWorkingArea(this);

                    // ---- サイズの補正 ----
                    var newWidth = Math.Min(WindowConfig.INSTANCE.WindowSize.Width, workingArea.Width);
                    var newHeight = Math.Min(WindowConfig.INSTANCE.WindowSize.Height, workingArea.Height);

                    // ---- 位置の補正 ----
                    var newX = WindowConfig.INSTANCE.WindowLocaion.X;
                    var newY = WindowConfig.INSTANCE.WindowLocaion.Y;

                    // 右端はみ出し
                    if (newX + newWidth > workingArea.Right)
                    {
                        newX = workingArea.Right - newWidth;
                    }

                    // 下端はみ出し
                    if (newY + newHeight > workingArea.Bottom)
                    {
                        newY = workingArea.Bottom - newHeight;
                    }

                    // 左端はみ出し
                    if (newX < workingArea.Left)
                    {
                        newX = workingArea.Left;
                    }

                    // 上端はみ出し
                    if (newY < workingArea.Top)
                    {
                        newY = workingArea.Top;
                    }

                    this.Location = new Point(newX, newY);
                    this.Size = new Size(newWidth, newHeight);
                    this.WindowState = WindowConfig.INSTANCE.WindowState;

                    this.CreateBrowsePanel();

                    this.ResumeLayout(false);
                }
            }
        }

        private void BrowseWindow_Shown(object sender, EventArgs e)
        {
            using (Measuring.Time(true, "BrowseForm.BrowseWindow_Shown"))
            {
                if (BrowseWindow.isStartUp)
                {
                    BrowseWindow.isStartUp = false;
                    BootTimeMeasurement.Stop();
                }
            }
        }

        private void BrowseWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                WindowConfig.INSTANCE.WindowState = this.WindowState;
                WindowConfig.INSTANCE.WindowLocaion = this.Location;
                WindowConfig.INSTANCE.WindowSize = this.Size;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                WindowConfig.INSTANCE.WindowState = this.WindowState;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._browsePanel.Dispose();
            }

            this._browsePanel = null;

            base.Dispose(disposing);
        }

        private void BrowseWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.IsBeginTabDragOperation())
            {
                return;
            }

            if (this._isKeyDown)
            {
                return;
            }

            if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        {
                            this._browsePanel.MovePreviewPage();
                            this._isKeyDown = true;
                            break;
                        }
                    case Keys.Right:
                        {
                            this._browsePanel.MoveNextPage();
                            this._isKeyDown = true;
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
                            this._isKeyDown = true;
                            break;
                        }
                    case Keys.T:
                        {
                            this.AddFavoriteDirectoryListTab();
                            this._isKeyDown = true;
                            break;
                        }
                    case Keys.R:
                        {
                            this.Reload();
                            this._isKeyDown = true;
                            break;
                        }
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                this.Reload();
                this._isKeyDown = true;
            }
        }

        private void BrowseWindow_KeyUp(object sender, KeyEventArgs e)
        {
            this._isKeyDown = false;
        }

        private void BrowseWindow_Activated(object sender, EventArgs e)
        {
            if (this._browsePanel != null)
            {
                var scale = WindowUtil.GetCurrentWindowScale(this);
                this._browsePanel.RedrawPage(scale);
            }
        }

        private void CreateBrowsePanel()
        {
            using (Measuring.Time(true, "BrowseForm.CreateBrowsePanel"))
            {
                if (this._browsePanel != null)
                {
                    throw new InvalidOperationException("メインコントロールは既に存在しています。");
                }

                this._browsePanel = new BrowsePanel();

                this._browsePanel.SuspendLayout();
                this.SuspendLayout();

                this._browsePanel.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Bottom
                    | AnchorStyles.Left
                    | AnchorStyles.Right;

                var scale = WindowUtil.GetCurrentWindowScale(this);
                var rect = this.CreateBrowsePanelBounds(scale);
                this._browsePanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
                this._browsePanel.SetControlsBounds(scale);
                this.Controls.Add(this._browsePanel);

                this._browsePanel.Close += new(this.BrowsePanel_Close);
                this._browsePanel.BackgroundMouseDoubleLeftClick += new(this.BrowsePanel_BackgroundMouseDoubleLeftClick);
                this._browsePanel.NewWindowPageOpen += new(this.BrowsePanel_NewWindowPageOpen);
                this._browsePanel.TabDropouted += new(this.BrowsePanel_TabDropouted);

                this.AttachResizeEvents(this);

                if (BrowseWindow.isStartUp)
                {
                    if (CommandLineArgs.IsNone() || CommandLineArgs.IsCleanup())
                    {
                        this._browsePanel.AddFavoriteDirectoryListTab();
                    }
                    else
                    {
                        var imageFilePath = CommandLineArgs.GetImageFilePathCommandLineArgs();
                        if (!string.IsNullOrEmpty(imageFilePath))
                        {
                            var directoryPath = FileUtil.GetParentDirectoryPath(imageFilePath);

                            var sortInfo = new SortParameter();
                            sortInfo.SetSortMode(FileSortMode.FilePath, true);

                            var parameter = new ImageViewPageParameter(
                                DirectoryFileListPageParameter.PAGE_SOURCES,
                                directoryPath,
                                BrowsePanel.GetImageFilesAction(new ImageFileGetByDirectoryParameter(imageFilePath)),
                                imageFilePath,
                                sortInfo,
                                FileUtil.GetFileName(directoryPath),
                                Instance<IFileIconCacher>.Value.SmallDirectoryIcon,
                                true);

                            this._browsePanel.AddImageViewPageTab(parameter);
                        }
                        else
                        {
                            this._browsePanel.AddFavoriteDirectoryListTab();
                        }
                    }
                }

                this.SetControlRegion();

                this._browsePanel.ResumeLayout(false);
                this.ResumeLayout(false);
            }
        }

        private void OnTabDropouted(TabDropoutedEventArgs e)
        {
            this.TabDropouted?.Invoke(this, e);
        }

        private void OnNewWindowPageOpen(BrowsePageOpenEventArgs e)
        {
            this.NewWindowPageOpen?.Invoke(this, e);
        }

        private Rectangle CreateBrowsePanelBounds(float scale)
        {
            var x = 0;
            var y = (int)(PADDING_TOP * scale);
            var w = this.Width;
            var h = this.Height - y;
            return new Rectangle(x, y, w, h);
        }

        private void BrowseWindow_ScaleChanged(object sender, ScaleChangedEventArgs e)
        {
            if (this._browsePanel == null)
            {
                return;
            }

            this.SuspendLayout();

            var rect = this.CreateBrowsePanelBounds(e.Scale);
            this._browsePanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            this._browsePanel.SetControlsBounds(e.Scale);

            this.ResumeLayout(false);
        }

        private void BrowsePanel_Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BrowsePanel_BackgroundMouseDoubleLeftClick(object sender, EventArgs e)
        {
            base.MouseLeftDoubleClickProcess();
        }

        private void BrowsePanel_NewWindowPageOpen(object sender, BrowsePageOpenEventArgs e)
        {
            this.OnNewWindowPageOpen(e);
        }

        private void BrowsePanel_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            this.OnTabDropouted(e);
        }
    }
}
