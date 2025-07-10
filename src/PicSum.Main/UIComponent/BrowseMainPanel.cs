using PicSum.Job.Common;
using PicSum.Job.Parameters;
using PicSum.UIComponent.AddressBar;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.InfoPanel;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.Core.StringAccessor;
using SWF.UIComponent.Core;
using SWF.UIComponent.TabOperation;
using SWF.UIComponent.WideDropDown;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class BrowseMainPanel
        : UserControl, ISender
    {
        private const int INFOPANEL_WIDTH = 240;

        private static readonly Rectangle TAB_SWITCH_DEFAULT_BOUNDS = new(0, 0, 746, 29);
        private static readonly Rectangle TOOL_PANEL_DEFAULT_BOUNDS = new(0, 29, 746, 34);
        private static readonly Rectangle TOOL_PANEL2_DEFAULT_BOUNDS = new(0, 63, 38, 403);
        private static readonly int TOOLPANEL2_VERTICAL_DEFAULT_TOP_MARGIN = 28;
        private static readonly Rectangle INFO_PANEL_DEFAULT_BOUNDS = new(0, 0, INFOPANEL_WIDTH, 100);
        private static readonly Rectangle PREVIEW_BUTTON_DEFAULT_BOUNDS = new(3, 3, 32, 28);
        private static readonly Rectangle HOME_BUTTON_DEFAULT_BOUNDS = new(3, 5, 32, 28);

        public static Func<ImageViewerPageParameter, Action<ISender>> GetImageFilesAction(
            ImageFileGetByDirectoryParameter subParamter)
        {
            return (parameter) =>
            {
                return sender =>
                {
                    var dir = FileUtil.IsExistsDirectory(subParamter.FilePath) switch
                    {
                        true => subParamter.FilePath,
                        false => FileUtil.GetParentDirectoryPath(subParamter.FilePath),
                    };

                    Instance<JobCaller>.Value.EnqueueDirectoryViewHistoryAddJob(sender, new ValueParameter<string>(dir));

                    Instance<JobCaller>.Value.EnqueueImageFilesGetByDirectoryJob(sender, subParamter, e =>
                        {
                            var title = FileUtil.IsExistsDirectory(subParamter.FilePath) ?
                            FileUtil.GetFileName(subParamter.FilePath) :
                            FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(subParamter.FilePath));

                            var eventArgs = new GetImageFilesEventArgs(
                                [.. e.FilePathList.OrderBy(_ => _, NaturalStringComparer.WINDOWS)],
                                e.SelectedFilePath, title,
                                Instance<IFileIconCacher>.Value.SmallDirectoryIcon);
                            parameter.OnGetImageFiles(eventArgs);
                        });
                };
            };
        }

        private bool _disposed = false;

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowsePageOpenEventArgs> NewWindowPageOpen;
        public event EventHandler Close;
        public event EventHandler BackgroundMouseDoubleLeftClick;
        public event EventHandler BeginSetPage;
        public event EventHandler EndSetPage;

        public int TabCount
        {
            get
            {
                return this.tabSwitch.TabCount;
            }
        }

        public bool IsBeginTabDragOperation
        {
            get
            {
                return this.tabSwitch.IsBeginTabDragOperation;
            }
        }

        public BrowseMainPanel()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            this.UpdateStyles();

            this.pageContainer = new PageContainer();
            this.infoPanel = new InfoPanel();
            this.tabSwitch = new TabSwitch();
            this.toolPanel = new Panel();
            this.reloadToolButton = new ToolIconButton();
            this.nextPageHistoryButton = new ToolIconButton();
            this.previewPageHistoryButton = new ToolIconButton();
            this.showInfoToolButton = new ToolIconButton();
            this.addressBar = new AddressBar();
            this.searchBookmarkToolButton = new ToolIconButton();
            this.tagDropToolButton = new WideDropToolButton();
            this.homeToolButton = new ToolIconButton();
            this.searchRatingToolButton = new ToolIconButton();
            this.toolPanel2 = new ToolPanel();

            this.toolPanel.SuspendLayout();
            this.toolPanel2.SuspendLayout();
            this.SuspendLayout();

            this.InitializeComponent();

            this.previewPageHistoryButton.Image = ResourceFiles.GoBackIcon.Value;
            this.nextPageHistoryButton.Image = ResourceFiles.GoNextIcon.Value;
            this.reloadToolButton.Image = ResourceFiles.ReloadIcon.Value;
            this.showInfoToolButton.Image = ResourceFiles.InfoIcon.Value;

            this.homeToolButton.Image = ResourceFiles.HomeIcon.Value;
            this.tagDropToolButton.Image = ResourceFiles.TagIcon.Value;
            this.tagDropToolButton.Icon = ResourceFiles.TagIcon.Value;
            this.searchRatingToolButton.Image = ResourceFiles.ActiveRatingIcon.Value;
            this.searchBookmarkToolButton.Image = ResourceFiles.BookmarkIcon.Value;

            this.infoPanel.Visible = false;

            this.Controls.AddRange(
                [
                    this.tabSwitch,
                        this.toolPanel,
                        this.toolPanel2,
                        this.infoPanel,
                        this.pageContainer,
                    ]);

            this.infoPanel.BringToFront();
            this.pageContainer.BringToFront();

            this.toolPanel.ResumeLayout(false);
            this.toolPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public void SetControlsBounds(float scale)
        {
            ConsoleUtil.Write(true, $"BrowseMainPanel.SetControlsBounds Start");

            this.toolPanel.SuspendLayout();
            this.toolPanel2.SuspendLayout();
            this.infoPanel.SuspendLayout();
            this.SuspendLayout();

            var baseWidth = this.Width - 16;

            this.tabSwitch.SetBounds(
                TAB_SWITCH_DEFAULT_BOUNDS.X,
                TAB_SWITCH_DEFAULT_BOUNDS.Y,
                baseWidth,
                (int)(TAB_SWITCH_DEFAULT_BOUNDS.Height * scale));
            this.tabSwitch.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.toolPanel.SetBounds(
                0,
                (int)(TOOL_PANEL_DEFAULT_BOUNDS.Y * scale),
                baseWidth,
                (int)(TOOL_PANEL_DEFAULT_BOUNDS.Height * scale));
            this.toolPanel.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.toolPanel2.SetBounds(
                0,
                this.toolPanel.Bottom,
                (int)(TOOL_PANEL2_DEFAULT_BOUNDS.Width * scale),
                this.Height - this.toolPanel.Bottom);
            this.toolPanel2.VerticalTopMargin = (int)(TOOLPANEL2_VERTICAL_DEFAULT_TOP_MARGIN * scale);
            this.toolPanel2.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Bottom;

            this.infoPanel.SetBounds(
                (int)(baseWidth - INFO_PANEL_DEFAULT_BOUNDS.Width * scale),
                this.toolPanel.Bottom,
                (int)(INFO_PANEL_DEFAULT_BOUNDS.Width * scale),
                this.Height - this.toolPanel.Bottom);
            this.infoPanel.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Right
                | AnchorStyles.Bottom;

            if (this.infoPanel.Visible)
            {
                this.pageContainer.SetBounds(
                    this.toolPanel2.Right,
                    this.toolPanel.Bottom,
                    baseWidth - this.toolPanel2.Right - this.infoPanel.Width,
                    this.Height - this.toolPanel.Bottom);
            }
            else
            {
                this.pageContainer.SetBounds(
                    this.toolPanel2.Right,
                    this.toolPanel.Bottom,
                    baseWidth - this.toolPanel2.Right,
                    this.Height - this.toolPanel.Bottom);
            }
            this.pageContainer.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right
                | AnchorStyles.Bottom;

            this.previewPageHistoryButton.SetBounds(
                (int)(PREVIEW_BUTTON_DEFAULT_BOUNDS.X * scale),
                (int)(PREVIEW_BUTTON_DEFAULT_BOUNDS.Y * scale),
                (int)(PREVIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(PREVIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));
            this.previewPageHistoryButton.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left;

            this.nextPageHistoryButton.SetBounds(
                this.previewPageHistoryButton.Left * 2 + this.previewPageHistoryButton.Width,
                this.previewPageHistoryButton.Top,
                this.previewPageHistoryButton.Width,
                this.previewPageHistoryButton.Height);
            this.nextPageHistoryButton.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left;

            this.reloadToolButton.SetBounds(
                this.previewPageHistoryButton.Left * 3 + this.previewPageHistoryButton.Width * 2,
                this.previewPageHistoryButton.Top,
                this.previewPageHistoryButton.Width,
                this.previewPageHistoryButton.Height);
            this.reloadToolButton.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left;

            this.addressBar.SetBounds(
                this.previewPageHistoryButton.Left * 4 + this.previewPageHistoryButton.Width * 3,
                this.previewPageHistoryButton.Top,
                this.toolPanel.Width - this.previewPageHistoryButton.Left * 6 - this.previewPageHistoryButton.Width * 4,
                this.previewPageHistoryButton.Height);
            this.addressBar.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.showInfoToolButton.SetBounds(
                this.addressBar.Right + this.previewPageHistoryButton.Left,
                this.previewPageHistoryButton.Top,
                this.previewPageHistoryButton.Width,
                this.previewPageHistoryButton.Height);
            this.showInfoToolButton.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Right;

            this.homeToolButton.SetBounds(
                (int)(HOME_BUTTON_DEFAULT_BOUNDS.X * scale),
                (int)(HOME_BUTTON_DEFAULT_BOUNDS.Y * scale),
                (int)(HOME_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(HOME_BUTTON_DEFAULT_BOUNDS.Height * scale));
            this.homeToolButton.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left;

            this.tagDropToolButton.SetBounds(
                this.homeToolButton.Left,
                this.homeToolButton.Top * 2 + this.homeToolButton.Height,
                this.homeToolButton.Width,
                this.homeToolButton.Height);
            this.tagDropToolButton.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left;

            this.searchRatingToolButton.SetBounds(
                this.homeToolButton.Left,
                this.homeToolButton.Top * 3 + this.homeToolButton.Height * 2,
                this.homeToolButton.Width,
                this.homeToolButton.Height);
            this.searchRatingToolButton.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left;

            this.searchBookmarkToolButton.SetBounds(
                this.homeToolButton.Left,
                this.homeToolButton.Top * 4 + this.homeToolButton.Height * 3,
                this.homeToolButton.Width,
                this.homeToolButton.Height);
            this.searchBookmarkToolButton.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left;

            this.infoPanel.SetControlsBounds(scale);

            if (this.tabSwitch.ActiveTab != null)
            {
                var page = this.tabSwitch.ActiveTab.GetPage<BrowsePage>();
                page.RedrawPage(scale);
            }

            this.infoPanel.ResumeLayout(false);
            this.toolPanel.ResumeLayout(false);
            this.toolPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

            ConsoleUtil.Write(true, $"BrowseMainPanel.SetControlsBounds End");
        }

        private void TabSwitch_BeginSetPage(object sender, EventArgs e)
        {
            this.BeginSetPage.Invoke(this, e);
        }

        private void TabSwitch_EndSetPage(object sender, EventArgs e)
        {
            this.EndSetPage.Invoke(this, e);
        }

        public void AddPageEventHandler(BrowsePage page)
        {
            ArgumentNullException.ThrowIfNull(page, nameof(page));

            page.SelectedFileChanged += new(this.Page_SelectedFileChanged);
            page.OpenPage += new(this.Page_OpenPage);
            page.MouseClick += new(this.Page_MouseClick);
        }

        public void AddTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            this.AddPageEventHandler(tab.GetPage<BrowsePage>());
            this.tabSwitch.AddTab(tab);
        }

        public void AddTab(IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this.AddPageEventHandler(this.tabSwitch.AddTab<BrowsePage>(param));
        }

        public void AddFavoriteDirectoryListTab()
        {
            ConsoleUtil.Write(true, $"BrowseMainPanel.AddFavoriteDirectoryListTab Start");
            this.OpenPage(new FavoriteDirectoryListPageParameter(), PageOpenType.AddTab);
            ConsoleUtil.Write(true, $"BrowseMainPanel.AddFavoriteDirectoryListTab End");
        }

        public void AddImageViewerPageTab(ImageViewerPageParameter parameter)
        {
            this.OpenPage(parameter, PageOpenType.AddTab);
        }

        public void Reload()
        {
            if (this.tabSwitch.ActiveTab == null)
            {
                return;
            }

            if (!this.tabSwitch.ActiveTab.HasPage)
            {
                return;
            }

            this.AddPageEventHandler(this.tabSwitch.CloneCurrentPage<BrowsePage>());
        }

        public void RemoveActiveTab()
        {
            this.tabSwitch.RemoveActiveTab();
        }

        public void MovePreviewPage()
        {
            if (!this.previewPageHistoryButton.Enabled)
            {
                return;
            }

            if (this.tabSwitch.ActiveTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            this.AddPageEventHandler(this.tabSwitch.SetPreviewHistory<BrowsePage>());
            this.SetPageHistoryButtonEnabled();
        }

        public void MoveNextPage()
        {
            if (!this.nextPageHistoryButton.Enabled)
            {
                return;
            }

            if (this.tabSwitch.ActiveTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            this.AddPageEventHandler(this.tabSwitch.SetNextPageHistory<BrowsePage>());
            this.SetPageHistoryButtonEnabled();
        }

        public void RedrawPage(float scale)
        {
            if (this.tabSwitch.ActiveTab != null)
            {
                var page = this.tabSwitch.ActiveTab.GetPage<BrowsePage>();
                ConsoleUtil.Write(true, $"BrowseMainPanel.RedrawPage Start");
                page.RedrawPage(scale);
                ConsoleUtil.Write(true, $"BrowseMainPanel.RedrawPage End");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.addressBar.SetAddress(FileUtil.ROOT_DIRECTORY_PATH);

            base.OnLoad(e);
        }

        private void RemovePageEventHandler(BrowsePage page)
        {
            page.SelectedFileChanged -= new(this.Page_SelectedFileChanged);
            page.OpenPage -= new(this.Page_OpenPage);
            page.MouseClick -= new(this.Page_MouseClick);
        }

        private void SetPageHistoryButtonEnabled()
        {
            if (this.tabSwitch.ActiveTab != null)
            {
                this.previewPageHistoryButton.Enabled = this.tabSwitch.ActiveTab.HasPreviewPage;
                this.nextPageHistoryButton.Enabled = this.tabSwitch.ActiveTab.HasNextPage;
            }
            else
            {
                this.previewPageHistoryButton.Enabled = false;
                this.nextPageHistoryButton.Enabled = false;
            }
        }

        private void OpenPage(IPageParameter param, PageOpenType openType)
        {
            ConsoleUtil.Write(true, $"BrowseMainPanel.OpenPage Start");

            if (openType == PageOpenType.OverlapTab)
            {
                this.AddPageEventHandler(this.tabSwitch.OverwriteTab<BrowsePage>(param));
                this.SetPageHistoryButtonEnabled();
            }
            else if (openType == PageOpenType.AddHome)
            {
                this.AddPageEventHandler(this.tabSwitch.AddTab<BrowsePage>(param));
            }
            else if (openType == PageOpenType.AddTab)
            {
                if (this.tabSwitch.ActiveTabIndex < 0)
                {
                    this.AddPageEventHandler(this.tabSwitch.InsertTab<BrowsePage>(0, param));
                }
                else
                {
                    this.AddPageEventHandler(this.tabSwitch.InsertTab<BrowsePage>(this.tabSwitch.ActiveTabIndex + 1, param));
                }
            }
            else if (openType == PageOpenType.NewWindow)
            {
                this.OnNewWindowPageOpen(new BrowsePageOpenEventArgs(param));
            }
            else
            {
                throw new Exception("ファイル実行種別が不正です。");
            }

            ConsoleUtil.Write(true, $"BrowseMainPanel.OpenPage End");
        }

        private void InsertPage(IPageParameter param, int tabIndex)
        {
            this.AddPageEventHandler(this.tabSwitch.InsertTab<BrowsePage>(tabIndex, param));
        }

        private void OverlapPage(DragEntity dragData)
        {
            if (FileUtil.IsExistsDirectory(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを上書きします。
                this.OpenPage(new DirectoryFileListPageParameter(dragData.CurrentFilePath), PageOpenType.OverlapTab);
            }
            else if (FileUtil.IsExistsFile(dragData.CurrentFilePath) &&
                ImageUtil.IsImageFile(dragData.CurrentFilePath))
            {
                // ビューアコンテンツを上書きします。
                var parameter = new ImageViewerPageParameter(
                    dragData.PageSources,
                    dragData.SourcesKey,
                    dragData.GetImageFilesAction,
                    dragData.CurrentFilePath,
                    dragData.SortInfo,
                    dragData.PageTitle,
                    dragData.PageIcon,
                    dragData.VisibleBookmarkMenuItem);
                this.OpenPage(parameter, PageOpenType.OverlapTab);
            }
        }

        private void InsertPage(DragEntity dragData, int tabIndex)
        {
            if (FileUtil.IsExistsDirectory(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを挿入します。
                this.InsertPage(new DirectoryFileListPageParameter(dragData.CurrentFilePath), tabIndex);
            }
            else if (FileUtil.IsExistsFile(dragData.CurrentFilePath) &&
                ImageUtil.IsImageFile(dragData.CurrentFilePath))
            {
                // ビューアコンテンツを挿入します。
                var parameter = new ImageViewerPageParameter(
                    dragData.PageSources,
                    dragData.SourcesKey,
                    dragData.GetImageFilesAction,
                    dragData.CurrentFilePath,
                    dragData.SortInfo,
                    dragData.PageTitle,
                    dragData.PageIcon,
                    dragData.VisibleBookmarkMenuItem);
                this.InsertPage(parameter, tabIndex);
            }
        }

        private new void DragDrop(TabAreaDragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragEntity)))
            {
                if (e.IsOverlap)
                {
                    this.OverlapPage((DragEntity)e.Data.GetData(typeof(DragEntity)));
                }
                else
                {
                    this.InsertPage((DragEntity)e.Data.GetData(typeof(DragEntity)), e.TabIndex);
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                if (filePaths.Length < 1)
                {
                    return;
                }

                var filePath = filePaths.First();

                var dirPath = FileUtil.IsExistsDirectory(filePath) ?
                    filePath : FileUtil.GetParentDirectoryPath(filePath);

                var sortInfo = new SortInfo();
                sortInfo.SetSortType(SortTypeID.FilePath, true);

                var dragData = new DragEntity(
                    this,
                    DirectoryFileListPageParameter.PAGE_SOURCES,
                    dirPath,
                    filePath,
                    sortInfo,
                    GetImageFilesAction(new ImageFileGetByDirectoryParameter(dirPath)),
                    FileUtil.GetFileName(dirPath),
                    Instance<IFileIconCacher>.Value.SmallDirectoryIcon,
                    true);
                if (e.IsOverlap)
                {
                    this.OverlapPage(dragData);
                }
                else
                {
                    this.InsertPage(dragData, e.TabIndex);
                }
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

        private void OnClose(EventArgs e)
        {
            this.Close?.Invoke(this, e);
        }

        private void OnBackgroundMouseLeftDoubleClick(EventArgs e)
        {
            this.BackgroundMouseDoubleLeftClick?.Invoke(this, e);
        }

        private void GetTagListJob_Callback(ListResult<string> e)
        {
            this.tagDropToolButton.SetItems([.. e]);

            if (!string.IsNullOrEmpty(this.tagDropToolButton.SelectedItem))
            {
                this.tagDropToolButton.SelectItem(this.tagDropToolButton.SelectedItem);
            }
        }

        // TODO: tabSwitch_ActiveTabChanged の直後に呼び出される場合がある。
        private void Page_SelectedFileChanged(object sender, SelectedFileChangeEventArgs e)
        {
            if (e.FilePathList.Length > 0)
            {
                this.addressBar.SetAddress(e.FilePathList[0]);
            }

            this.infoPanel.SetFileInfo(e.FilePathList);
            this.tabSwitch.InvalidateHeader();
        }

        private void Page_OpenPage(object sender, BrowsePageEventArgs e)
        {
            this.OpenPage(e.Parameter, e.OpenType);
        }

        private void Page_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.XButton1:
                    this.MovePreviewPage();
                    break;
                case MouseButtons.XButton2:
                    this.MoveNextPage();
                    break;
                default: break;
            }
        }

        private void TabSwitch_AddTabButtonMouseClick(object sender, MouseEventArgs e)
        {
            this.OpenPage(new FavoriteDirectoryListPageParameter(), PageOpenType.AddHome);
        }

        private void TabSwitch_ActiveTabChanged(object sender, EventArgs e)
        {
            this.SetPageHistoryButtonEnabled();

            if (this.tabSwitch.ActiveTab != null)
            {
                foreach (var tab in this.tabSwitch.GetInactiveTabs())
                {
                    var p = tab.GetPage<BrowsePage>();
                    p.StopPageDraw();
                }

                var page = this.tabSwitch.ActiveTab.GetPage<BrowsePage>();
                var selectedFilePath = page.SelectedFilePath;
                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    this.addressBar.SetAddress(selectedFilePath);
                    this.infoPanel.SetFileInfo(page.GetSelectedFiles());
                    var scale = WindowUtil.GetCurrentWindowScale(this);
                    page.RedrawPage(scale);
                }
            }
        }

        private void TabSwitch_TabCloseButtonClick(object sender, TabEventArgs e)
        {
            this.tabSwitch.RemoveTab(e.Tab);

            e.Tab.Close();

            if (!this.tabSwitch.HasTab)
            {
                this.OnClose(EventArgs.Empty);
            }
        }

        private void TabSwitch_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            this.RemovePageEventHandler(e.Tab.GetPage<BrowsePage>());

            if (e.ToOtherOwner)
            {
                var browse = e.Tab.Owner.GetForm<BrowseForm>();
                browse.AddPageEventHandler(e.Tab.GetPage<BrowsePage>());
            }
            else
            {
                this.OnTabDropouted(e);
            }

            if (!this.tabSwitch.HasTab)
            {
                this.OnClose(EventArgs.Empty);
            }
        }

        private void TabSwitch_BackgroundMouseDoubleLeftClick(object sender, EventArgs e)
        {
            this.OnBackgroundMouseLeftDoubleClick(e);
        }

        private void TabSwitch_TabAreaDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragEntity)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent("Shell IDList Array")
                && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void TabSwitch_TabAreaDragDrop(object sender, TabAreaDragEventArgs e)
        {
            this.DragDrop(e);
        }

        private void PreviewPageHistoryButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.MovePreviewPage();
        }

        private void NextPageHistoryButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.MoveNextPage();
        }

        private void AddressBar_SelectedDirectory(object sender, PicSum.UIComponent.AddressBar.SelectedDirectoryEventArgs e)
        {
            this.OpenPage(new DirectoryFileListPageParameter(e.DirectoryPath, e.SubDirectoryPath), e.OpenType);
        }

        private void ShowInfoToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.infoPanel.Visible)
            {
                this.infoPanel.Visible = false;
            }
            else
            {
                this.infoPanel.Visible = true;
            }

            var scale = WindowUtil.GetCurrentWindowScale(this);
            this.SetControlsBounds(scale);
        }

        private void InfoPanel_SelectedTag(object sender, SelectedTagEventArgs e)
        {
            this.OpenPage(new TagFileListPageParameter(e.Tag), e.OpenType);
        }

        private void PageContainer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Shell IDList Array")
                && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }

            if (e.Data.GetDataPresent(typeof(DragEntity)))
            {
                var entity = (DragEntity)e.Data.GetData(typeof(DragEntity));
                if (entity.Sender != this.ActiveControl)
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void PageContainer_DragDrop(object sender, DragEventArgs e)
        {
            if (this.tabSwitch.ActiveTabIndex < 0)
            {
                this.DragDrop(new TabAreaDragEventArgs(true, 0, e));
            }
            else
            {
                this.DragDrop(new TabAreaDragEventArgs(true, this.tabSwitch.ActiveTabIndex, e));
            }
        }

        private void ReloadToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.Reload();
        }

        private void HomeToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenPage(new FavoriteDirectoryListPageParameter(), PageOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenPage(new FavoriteDirectoryListPageParameter(), PageOpenType.AddTab);
            }
        }

        private void TagDropToolButton_DropDownOpening(object sender, DropDownOpeningEventArgs e)
        {
            Instance<JobCaller>.Value.EnqueueTagsGetJob(this, _ =>
                {
                    if (this._disposed)
                    {
                        return;
                    }

                    this.GetTagListJob_Callback(_);
                });
        }

        private void TagDropToolButton_ItemMouseClick(object sender, ItemMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenPage(new TagFileListPageParameter(e.Item), PageOpenType.OverlapTab);
            }
            else
            {
                this.OpenPage(new TagFileListPageParameter(e.Item), PageOpenType.AddTab);
            }
        }

        private void SearchRatingToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenPage(new RatingFileListPageParameter(1), PageOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenPage(new RatingFileListPageParameter(1), PageOpenType.AddTab);
            }
        }

        private void SearchBookmarkToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenPage(new BookmarkFileListPageParameter(), PageOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenPage(new BookmarkFileListPageParameter(), PageOpenType.AddTab);
            }
        }
    }
}
