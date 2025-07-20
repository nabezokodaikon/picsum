using NLog;
using PicSum.Job.Parameters;
using PicSum.Main.Conf;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Core;
using SWF.UIComponent.Form;
using SWF.UIComponent.TabOperation;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{
    public partial class BrowseForm
        : GrassForm, ISender
    {
        private static readonly Logger LOGGER = Log.GetLogger();

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

        public BrowseForm()
            : base()
        {
            this.SuspendLayout();

            this.Icon = ResourceFiles.AppIcon.Value;
            this.Text = "PicSum";
            this.StartPosition = FormStartPosition.Manual;
            this.KeyPreview = true;
            this.Size = BrowseConfig.INSTANCE.WindowSize;
            this.WindowState = BrowseConfig.INSTANCE.WindowState;
            this.ScaleChanged += this.Form_ScaleChanged;

            if (BrowseForm.isStartUp)
            {
                this.Location = BrowseConfig.INSTANCE.WindowLocaion;
            }
            else
            {
                this.Location = new Point(
                    BrowseConfig.INSTANCE.WindowLocaion.X + 16,
                    BrowseConfig.INSTANCE.WindowLocaion.Y + 16);
            }

            this.Shown += this.BrowseForm_Shown;
            this.FormClosing += this.BrowseForm_FormClosing;
            this.KeyDown += this.BrowseForm_KeyDown;
            this.KeyUp += this.BrowseForm_KeyUp;
            this.Activated += this.BrowseForm_Activated;

            this.ResumeLayout(false);
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

        protected override bool CanDragOperation()
        {
            return !this.BrowsePanel.IsBeginTabDragOperation;
        }

        private void BrowseForm_Shown(object sender, EventArgs e)
        {
            using (TimeMeasuring.Run(true, "BrowseForm.BrowseForm_Shown"))
            {
                if (BrowseForm.isStartUp)
                {
                    this.CreateBrowsePanel();
                    BrowseForm.isStartUp = false;
                }

                BootTimeMeasurement.Stop();
            }
        }

        private void BrowseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                BrowseConfig.INSTANCE.WindowState = this.WindowState;
                BrowseConfig.INSTANCE.WindowLocaion = this.Location;
                BrowseConfig.INSTANCE.WindowSize = this.Size;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                BrowseConfig.INSTANCE.WindowState = this.WindowState;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }

        private void BrowseForm_KeyDown(object sender, KeyEventArgs e)
        {
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

        private void BrowseForm_KeyUp(object sender, KeyEventArgs e)
        {
            this._isKeyDown = false;
        }

        private void BrowseForm_Activated(object sender, EventArgs e)
        {
            if (this._browsePanel != null)
            {
                var scale = WindowUtil.GetCurrentWindowScale(this);
                this._browsePanel.RedrawPage(scale);
            }
        }

        private void CreateBrowsePanel()
        {
            using (TimeMeasuring.Run(true, "BrowseForm.CreateBrowsePanel"))
            {
                if (this._browsePanel != null)
                {
                    throw new InvalidOperationException("メインコントロールは既に存在しています。");
                }

                this._browsePanel = new BrowsePanel();

                this._browsePanel.SuspendLayout();
                this.SuspendLayout();

                this.Controls.Add(this._browsePanel);

                var scale = WindowUtil.GetCurrentWindowScale(this);
                var rect = this.CreateBrowsePanelBounds(scale);
                this._browsePanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
                this._browsePanel.SetControlsBounds(scale);
                this._browsePanel.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Bottom
                    | AnchorStyles.Left
                    | AnchorStyles.Right;

                this._browsePanel.Close += new(this.BrowsePanel_Close);
                this._browsePanel.BackgroundMouseDoubleLeftClick += new(this.BrowsePanel_BackgroundMouseDoubleLeftClick);
                this._browsePanel.NewWindowPageOpen += new(this.BrowsePanel_NewWindowPageOpen);
                this._browsePanel.TabDropouted += new(this.BrowsePanel_TabDropouted);

                this.AttachResizeEvents(this);

                if (BrowseForm.isStartUp)
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

                            var sortInfo = new SortInfo();
                            sortInfo.SetSortType(SortTypeID.FilePath, true);

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

                    LOGGER.Debug("初回表示されました。");
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

        private void Form_ScaleChanged(object sender, ScaleChangedEventArgs e)
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
