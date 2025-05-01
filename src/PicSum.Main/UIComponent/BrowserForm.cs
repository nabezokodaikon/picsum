using NLog;
using PicSum.Job.Parameters;
using PicSum.Main.Conf;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
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
    public partial class BrowserForm
        : GrassForm, ISender
    {
        private const float PADDING_TOP = 8f;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static bool isStartUp = true;

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowserPageOpenEventArgs> NewWindowPageOpen;

        private BrowserMainPanel _browserMainPanel = null;
        private bool _isKeyDown = false;

        private BrowserMainPanel BrowserMainPanel
        {
            get
            {
                if (this._browserMainPanel == null)
                {
                    this.CreateBrowserMainPanel();
                }

                return this._browserMainPanel;
            }
        }

        public BrowserForm()
            : base()
        {
            this.SuspendLayout();

            this.Icon = ResourceFiles.AppIcon.Value;
            this.Text = "PicSum";
            this.StartPosition = FormStartPosition.Manual;
            this.KeyPreview = true;
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
            }

            this.ResumeLayout(false);
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

        //protected override void OnHandleCreated(EventArgs e)
        //{
        //    ConsoleUtil.Write(true, $"BrowserForm.OnHandleCreated");
        //    base.OnHandleCreated(e);
        //}

        protected override void OnShown(EventArgs e)
        {
            ConsoleUtil.Write(true, $"BrowserForm.OnShown Start");

            base.OnShown(e);

            if (BrowserForm.isStartUp)
            {
                this.CreateBrowserMainPanel();
                BrowserForm.isStartUp = false;

                AppConstants.StopBootTimeMeasurement();
            }

            ConsoleUtil.Write(true, $"BrowserForm.OnShown End");
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
                            this._browserMainPanel.MovePreviewPage();
                            this._isKeyDown = true;
                            break;
                        }
                    case Keys.Right:
                        {
                            this._browserMainPanel.MoveNextPage();
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

            if (this._browserMainPanel != null)
            {
                var scale = WindowUtil.GetCurrentWindowScale(this);
                this._browserMainPanel.RedrawPage(scale);
            }
        }

        private void CreateBrowserMainPanel()
        {
            if (this._browserMainPanel != null)
            {
                throw new SWFException("メインコントロールは既に存在しています。");
            }

            ConsoleUtil.Write(true, $"BrowserForm.CreateBrowserMainPanel Start");

            this._browserMainPanel = new BrowserMainPanel();

            this._browserMainPanel.SuspendLayout();
            this.SuspendLayout();

            this.Controls.Add(this._browserMainPanel);

            var scale = WindowUtil.GetCurrentWindowScale(this);
            var rect = this.CreateBrowserMainPanelBounds(scale);
            this._browserMainPanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            this._browserMainPanel.SetControlsBounds(scale);
            this._browserMainPanel.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this._browserMainPanel.Close += new(this.BrowserMainPanel_Close);
            this._browserMainPanel.BackgroundMouseDoubleLeftClick += new(this.BrowserMainPanel_BackgroundMouseDoubleLeftClick);
            this._browserMainPanel.NewWindowPageOpen += new(this.BrowserMainPanel_NewWindowPageOpen);
            this._browserMainPanel.TabDropouted += new(this.BrowserMainPanel_TabDropouted);

            this.AttachResizeEvents(this);

            if (BrowserForm.isStartUp)
            {
                if (CommandLineArgs.IsNone() || CommandLineArgs.IsCleanup())
                {
                    this._browserMainPanel.AddFavoriteDirectoryListTab();
                }
                else
                {
                    var imageFilePath = CommandLineArgs.GetImageFilePathCommandLineArgs();
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

                        this._browserMainPanel.AddImageViewerPageTab(parameter);
                    }
                    else
                    {
                        this._browserMainPanel.AddFavoriteDirectoryListTab();
                    }
                }

                logger.Debug("初回表示されました。");
            }

            this.SetControlRegion();

            this._browserMainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

            ConsoleUtil.Write(true, $"BrowserForm.CreateBrowserMainPanel End");
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
            var y = (int)(PADDING_TOP * scale);
            var w = this.Width;
            var h = this.Height - y;
            return new Rectangle(x, y, w, h);
        }

        private void Form_ScaleChanged(object sender, ScaleChangedEventArgs e)
        {
            if (this._browserMainPanel == null)
            {
                return;
            }

            this.SuspendLayout();

            var rect = this.CreateBrowserMainPanelBounds(e.Scale);
            this._browserMainPanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            this._browserMainPanel.SetControlsBounds(e.Scale);

            this.ResumeLayout(false);
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
