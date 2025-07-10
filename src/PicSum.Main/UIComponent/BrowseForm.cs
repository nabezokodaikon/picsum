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
using System.ComponentModel;
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

        private BrowseMainPanel _browseMainPanel = null;
        private bool _isKeyDown = false;

        private BrowseMainPanel BrowseMainPanel
        {
            get
            {
                if (this._browseMainPanel == null)
                {
                    this.CreateBrowseMainPanel();
                }

                return this._browseMainPanel;
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

            this.ResumeLayout(false);
        }

        public void AddPageEventHandler(BrowsePage page)
        {
            ArgumentNullException.ThrowIfNull(page, nameof(page));

            this.BrowseMainPanel.AddPageEventHandler(page);
        }

        public void AddTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            this.BrowseMainPanel.AddTab(tab);
        }

        public void AddTab(IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this.BrowseMainPanel.AddTab(param);
        }

        public void AddFavoriteDirectoryListTab()
        {
            this.BrowseMainPanel.AddFavoriteDirectoryListTab();
        }

        public void AddImageViewPageTab(ImageViewPageParameter parameter)
        {
            this.BrowseMainPanel.AddImageViewPageTab(parameter);
        }

        public void Reload()
        {
            this.BrowseMainPanel.Reload();
        }

        public void RemoveTabOrWindow()
        {
            if (this.BrowseMainPanel.TabCount > 1)
            {
                this.BrowseMainPanel.RemoveActiveTab();
            }
            else
            {
                this.Close();
            }
        }

        protected override bool CanDragOperation()
        {
            return !this.BrowseMainPanel.IsBeginTabDragOperation;
        }

        //protected override void OnHandleCreated(EventArgs e)
        //{
        //    ConsoleUtil.Write(true, $"BrowseForm.OnHandleCreated");
        //    base.OnHandleCreated(e);
        //}

        protected override void OnShown(EventArgs e)
        {
            ConsoleUtil.Write(true, $"BrowseForm.OnShown Start");

            base.OnShown(e);

            if (BrowseForm.isStartUp)
            {
                this.CreateBrowseMainPanel();
                BrowseForm.isStartUp = false;
            }

            ConsoleUtil.Write(true, $"BrowseForm.OnShown End");
            BootTimeMeasurement.Stop();
        }

        protected override void OnClosing(CancelEventArgs e)
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
                            this._browseMainPanel.MovePreviewPage();
                            this._isKeyDown = true;
                            break;
                        }
                    case Keys.Right:
                        {
                            this._browseMainPanel.MoveNextPage();
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

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            this._isKeyDown = false;
            base.OnKeyUp(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (this._browseMainPanel != null)
            {
                var scale = WindowUtil.GetCurrentWindowScale(this);
                this._browseMainPanel.RedrawPage(scale);
            }
        }

        private void CreateBrowseMainPanel()
        {
            if (this._browseMainPanel != null)
            {
                throw new InvalidOperationException("メインコントロールは既に存在しています。");
            }

            ConsoleUtil.Write(true, $"BrowseForm.CreateBrowseMainPanel Start");

            this._browseMainPanel = new BrowseMainPanel();

            this._browseMainPanel.SuspendLayout();
            this.SuspendLayout();

            this.Controls.Add(this._browseMainPanel);

            var scale = WindowUtil.GetCurrentWindowScale(this);
            var rect = this.CreateBrowseMainPanelBounds(scale);
            this._browseMainPanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            this._browseMainPanel.SetControlsBounds(scale);
            this._browseMainPanel.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this._browseMainPanel.Close += new(this.BrowseMainPanel_Close);
            this._browseMainPanel.BackgroundMouseDoubleLeftClick += new(this.BrowseMainPanel_BackgroundMouseDoubleLeftClick);
            this._browseMainPanel.NewWindowPageOpen += new(this.BrowseMainPanel_NewWindowPageOpen);
            this._browseMainPanel.TabDropouted += new(this.BrowseMainPanel_TabDropouted);

            this.AttachResizeEvents(this);

            if (BrowseForm.isStartUp)
            {
                if (CommandLineArgs.IsNone() || CommandLineArgs.IsCleanup())
                {
                    this._browseMainPanel.AddFavoriteDirectoryListTab();
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
                            BrowseMainPanel.GetImageFilesAction(new ImageFileGetByDirectoryParameter(imageFilePath)),
                            imageFilePath,
                            sortInfo,
                            FileUtil.GetFileName(directoryPath),
                            Instance<IFileIconCacher>.Value.SmallDirectoryIcon,
                            true);

                        this._browseMainPanel.AddImageViewPageTab(parameter);
                    }
                    else
                    {
                        this._browseMainPanel.AddFavoriteDirectoryListTab();
                    }
                }

                LOGGER.Debug("初回表示されました。");
            }

            this.SetControlRegion();

            this._browseMainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

            ConsoleUtil.Write(true, $"BrowseForm.CreateBrowseMainPanel End");
        }

        private void OnTabDropouted(TabDropoutedEventArgs e)
        {
            this.TabDropouted?.Invoke(this, e);
        }

        private void OnNewWindowPageOpen(BrowsePageOpenEventArgs e)
        {
            this.NewWindowPageOpen?.Invoke(this, e);
        }

        private Rectangle CreateBrowseMainPanelBounds(float scale)
        {
            var x = 0;
            var y = (int)(PADDING_TOP * scale);
            var w = this.Width;
            var h = this.Height - y;
            return new Rectangle(x, y, w, h);
        }

        private void Form_ScaleChanged(object sender, ScaleChangedEventArgs e)
        {
            if (this._browseMainPanel == null)
            {
                return;
            }

            this.SuspendLayout();

            var rect = this.CreateBrowseMainPanelBounds(e.Scale);
            this._browseMainPanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            this._browseMainPanel.SetControlsBounds(e.Scale);

            this.ResumeLayout(false);
        }

        private void BrowseMainPanel_Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BrowseMainPanel_BackgroundMouseDoubleLeftClick(object sender, EventArgs e)
        {
            base.MouseLeftDoubleClickProcess();
        }

        private void BrowseMainPanel_NewWindowPageOpen(object sender, BrowsePageOpenEventArgs e)
        {
            this.OnNewWindowPageOpen(e);
        }

        private void BrowseMainPanel_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            this.OnTabDropouted(e);
        }
    }
}
