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
using SWF.UIComponent.Base;
using SWF.UIComponent.TabOperation;
using SWF.UIComponent.WideDropDown;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{

    internal sealed partial class BrowsePanel
        : BaseControl, ISender
    {
        private const int INFOPANEL_WIDTH = 240;
        private const int TOOLPANEL2_VERTICAL_DEFAULT_TOP_MARGIN = 28;

        private static readonly Rectangle TAB_SWITCH_DEFAULT_BOUNDS = new(0, 0, 746, 29);
        private static readonly Rectangle TOOL_PANEL_DEFAULT_BOUNDS = new(0, 29, 746, 34);
        private static readonly Rectangle TOOL_PANEL2_DEFAULT_BOUNDS = new(0, 63, 38, 403);
        private static readonly Rectangle INFO_PANEL_DEFAULT_BOUNDS = new(0, 0, INFOPANEL_WIDTH, 100);
        private static readonly Rectangle PREVIEW_BUTTON_DEFAULT_BOUNDS = new(3, 3, 28, 28);
        private static readonly Rectangle HOME_BUTTON_DEFAULT_BOUNDS = new(3, 0, 28, 28);

        public static Func<ImageViewPageParameter, Action<ISender>> GetImageFilesAction(
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

                    var dirPrameter = new ValueParameter<string>(dir);
                    Instance<JobCaller>.Value.EnqueueDirectoryViewCounterIncrementJob(sender, dirPrameter);
                    Instance<JobCaller>.Value.EnqueueDirectoryViewHistoryAddJob(sender, dirPrameter);

                    Instance<JobCaller>.Value.EnqueueImageFilesGetByDirectoryJob(sender, subParamter, e =>
                        {
                            var title = FileUtil.IsExistsDirectory(subParamter.FilePath) ?
                            FileUtil.GetFileName(subParamter.FilePath) :
                            FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(subParamter.FilePath));

                            var eventArgs = new GetImageFilesEventArgs(
                                [.. e.FilePathList.OrderBy(static _ => _, NaturalStringComparer.WINDOWS)],
                                e.SelectedFilePath, title,
                                Instance<IFileIconCacher>.Value.SmallDirectoryIcon);
                            parameter.OnGetImageFiles(eventArgs);
                        });
                };
            };
        }

        private static bool HasControl(object target, Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child == target)
                {
                    return true;
                }

                return HasControl(target, child);
            }

            return false;
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
                return this._tabSwitch.TabCount;
            }
        }

        public bool IsBeginTabDragOperation
        {
            get
            {
                return this._tabSwitch.IsBeginTabDragOperation;
            }
        }

        public BrowsePanel()
        {
            this._pageContainer = new PageContainer();
            this._infoPanel = new InfoPanel();
            this._tabSwitch = new TabSwitch();
            this._toolPanel = new Control();
            this._reloadToolButton = new BaseIconButton();
            this._nextPageHistoryButton = new BaseIconButton();
            this._previewPageHistoryButton = new BaseIconButton();
            this._showInfoToolButton = new BaseIconButton();
            this._addressBar = new AddressBar();
            this._searchBookmarkToolButton = new BaseIconButton();
            this._historyToolButton = new BaseIconButton();
            this._tagDropToolButton = new WideDropToolButton();
            this._homeToolButton = new BaseIconButton();
            this._searchRatingToolButton = new BaseIconButton();
            this._toolPanel2 = new ToolPanel();

            this._toolPanel.SuspendLayout();
            this._toolPanel2.SuspendLayout();
            this.SuspendLayout();

            this.InitializeComponent();

            this._previewPageHistoryButton.Image = ResourceFiles.GoBackIcon.Value;
            this._nextPageHistoryButton.Image = ResourceFiles.GoNextIcon.Value;
            this._reloadToolButton.Image = ResourceFiles.ReloadIcon.Value;
            this._showInfoToolButton.Image = ResourceFiles.InfoIcon.Value;

            this._homeToolButton.Image = ResourceFiles.HomeIcon.Value;
            this._tagDropToolButton.Image = ResourceFiles.TagIcon.Value;
            this._tagDropToolButton.Icon = SkiaUtil.ToSKImage(ResourceFiles.TagIcon.Value);
            this._searchRatingToolButton.Image = ResourceFiles.ActiveRatingIcon.Value;
            this._searchBookmarkToolButton.Image = ResourceFiles.BookmarkIcon.Value;
            this._historyToolButton.Image = ResourceFiles.HistoryIcon.Value;

            this._infoPanel.Visible = false;

            this.Controls.AddRange(
                [
                    this._tabSwitch,
                        this._toolPanel,
                        this._toolPanel2,
                        this._infoPanel,
                        this._pageContainer,
                    ]);

            this._infoPanel.BringToFront();
            this._pageContainer.BringToFront();

            this._toolPanel.ResumeLayout(false);
            this._toolPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public void SetControlsBounds(float scale)
        {
            using (Measuring.Time(true, "BrowsePanel.SetControlsBounds"))
            {
                this._toolPanel.SuspendLayout();
                this._toolPanel2.SuspendLayout();
                this._infoPanel.SuspendLayout();
                this.SuspendLayout();

                var baseWidth = this.Width - 16;

                this._tabSwitch.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left
                    | AnchorStyles.Right;
                this._tabSwitch.SetBounds(
                    TAB_SWITCH_DEFAULT_BOUNDS.X,
                    TAB_SWITCH_DEFAULT_BOUNDS.Y,
                    baseWidth,
                    (int)(TAB_SWITCH_DEFAULT_BOUNDS.Height * scale));

                this._toolPanel.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left
                    | AnchorStyles.Right;
                this._toolPanel.SetBounds(
                    0,
                    (int)(TOOL_PANEL_DEFAULT_BOUNDS.Y * scale),
                    baseWidth,
                    (int)(TOOL_PANEL_DEFAULT_BOUNDS.Height * scale));

                this._toolPanel2.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left
                    | AnchorStyles.Bottom;
                this._toolPanel2.SetBounds(
                    0,
                    this._toolPanel.Bottom,
                    (int)(TOOL_PANEL2_DEFAULT_BOUNDS.Width * scale),
                    this.Height - this._toolPanel.Bottom);
                this._toolPanel2.VerticalTopMargin = (int)(TOOLPANEL2_VERTICAL_DEFAULT_TOP_MARGIN * scale);

                this._infoPanel.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Right
                    | AnchorStyles.Bottom;
                this._infoPanel.SetBounds(
                    (int)(baseWidth - INFO_PANEL_DEFAULT_BOUNDS.Width * scale),
                    this._toolPanel.Bottom,
                    (int)(INFO_PANEL_DEFAULT_BOUNDS.Width * scale),
                    this.Height - this._toolPanel.Bottom);

                this._pageContainer.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left
                    | AnchorStyles.Right
                    | AnchorStyles.Bottom;
                if (this._infoPanel.Visible)
                {
                    this._pageContainer.SetBounds(
                        this._toolPanel2.Right,
                        this._toolPanel.Bottom,
                        baseWidth - this._toolPanel2.Right - this._infoPanel.Width,
                        this.Height - this._toolPanel.Bottom);
                }
                else
                {
                    this._pageContainer.SetBounds(
                        this._toolPanel2.Right,
                        this._toolPanel.Bottom,
                        baseWidth - this._toolPanel2.Right,
                        this.Height - this._toolPanel.Bottom);
                }

                this._previewPageHistoryButton.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left;
                this._previewPageHistoryButton.SetBounds(
                    (int)(PREVIEW_BUTTON_DEFAULT_BOUNDS.X * scale),
                    (int)(PREVIEW_BUTTON_DEFAULT_BOUNDS.Y * scale),
                    (int)(PREVIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                    (int)(PREVIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

                this._nextPageHistoryButton.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left;
                this._nextPageHistoryButton.SetBounds(
                    this._previewPageHistoryButton.Left * 2 + this._previewPageHistoryButton.Width,
                    this._previewPageHistoryButton.Top,
                    this._previewPageHistoryButton.Width,
                    this._previewPageHistoryButton.Height);

                this._reloadToolButton.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left;
                this._reloadToolButton.SetBounds(
                    this._previewPageHistoryButton.Left * 3 + this._previewPageHistoryButton.Width * 2,
                    this._previewPageHistoryButton.Top,
                    this._previewPageHistoryButton.Width,
                    this._previewPageHistoryButton.Height);

                this._addressBar.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left
                    | AnchorStyles.Right;
                this._addressBar.SetBounds(
                    this._previewPageHistoryButton.Left * 4 + this._previewPageHistoryButton.Width * 3,
                    this._previewPageHistoryButton.Top,
                    this._toolPanel.Width - this._previewPageHistoryButton.Left * 6 - this._previewPageHistoryButton.Width * 4,
                    this._previewPageHistoryButton.Height);

                this._showInfoToolButton.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Right;
                this._showInfoToolButton.SetBounds(
                    this._addressBar.Right + this._previewPageHistoryButton.Left,
                    this._previewPageHistoryButton.Top,
                    this._previewPageHistoryButton.Width,
                    this._previewPageHistoryButton.Height);

                this._homeToolButton.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left;
                this._homeToolButton.SetBounds(
                    (int)(HOME_BUTTON_DEFAULT_BOUNDS.X * scale),
                    (int)(HOME_BUTTON_DEFAULT_BOUNDS.Y * scale),
                    (int)(HOME_BUTTON_DEFAULT_BOUNDS.Width * scale),
                    (int)(HOME_BUTTON_DEFAULT_BOUNDS.Height * scale));

                this._tagDropToolButton.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left;
                this._tagDropToolButton.SetBounds(
                    this._homeToolButton.Left,
                    this._homeToolButton.Top * 2 + this._homeToolButton.Height,
                    this._homeToolButton.Width,
                    this._homeToolButton.Height);

                this._searchRatingToolButton.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left;
                this._searchRatingToolButton.SetBounds(
                    this._homeToolButton.Left,
                    this._homeToolButton.Top * 3 + this._homeToolButton.Height * 2,
                    this._homeToolButton.Width,
                    this._homeToolButton.Height);

                this._searchBookmarkToolButton.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left;
                this._searchBookmarkToolButton.SetBounds(
                    this._homeToolButton.Left,
                    this._homeToolButton.Top * 4 + this._homeToolButton.Height * 3,
                    this._homeToolButton.Width,
                    this._homeToolButton.Height);

                this._historyToolButton.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left;
                this._historyToolButton.SetBounds(
                    this._homeToolButton.Left,
                    this._homeToolButton.Top * 4 + this._homeToolButton.Height * 4,
                    this._homeToolButton.Width,
                    this._homeToolButton.Height);

                this._infoPanel.SetControlsBounds(scale);

                if (this._tabSwitch.ActiveTab != null)
                {
                    var page = this._tabSwitch.ActiveTab.GetPage<AbstractBrowsePage>();
                    page.RedrawPage(scale);
                }

                this._infoPanel.ResumeLayout(false);
                this._toolPanel.ResumeLayout(false);
                this._toolPanel2.ResumeLayout(false);
                this.ResumeLayout(false);
            }
        }

        private void TabSwitch_BeginSetPage(object sender, EventArgs e)
        {
            this.BeginSetPage.Invoke(this, e);
        }

        private void TabSwitch_EndSetPage(object sender, EventArgs e)
        {
            this.EndSetPage.Invoke(this, e);
        }

        public void AddPageEventHandler(AbstractBrowsePage page)
        {
            ArgumentNullException.ThrowIfNull(page, nameof(page));

            page.SelectedFileChanged += new(this.Page_SelectedFileChanged);
            page.OpenPage += new(this.Page_OpenPage);
            page.MouseClick += new(this.Page_MouseClick);
        }

        public void AddTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            this.AddPageEventHandler(tab.GetPage<AbstractBrowsePage>());
            this._tabSwitch.AddTab(tab);
        }

        public void AddTab(IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this.AddPageEventHandler(this._tabSwitch.AddTab<AbstractBrowsePage>(param));
        }

        public void AddFavoriteDirectoryListTab()
        {
            using (Measuring.Time(true, "BrowsePanel.AddFavoriteDirectoryListTab"))
            {
                this.OpenPage(new FavoriteDirectoryListPageParameter(), PageOpenMode.AddTab);
            }
        }

        public void AddImageViewPageTab(ImageViewPageParameter parameter)
        {
            this.OpenPage(parameter, PageOpenMode.AddTab);
        }

        public void Reload()
        {
            if (this._tabSwitch.ActiveTab == null)
            {
                return;
            }

            if (!this._tabSwitch.ActiveTab.HasPage)
            {
                return;
            }

            this.AddPageEventHandler(this._tabSwitch.CloneCurrentPage<AbstractBrowsePage>());
        }

        public void RemoveActiveTab()
        {
            this._tabSwitch.RemoveActiveTab();
        }

        public void MovePreviewPage()
        {
            if (!this._previewPageHistoryButton.Enabled)
            {
                return;
            }

            if (this._tabSwitch.ActiveTab == null)
            {
                throw new InvalidOperationException("アクティブなタブが存在しません。");
            }

            this.AddPageEventHandler(this._tabSwitch.SetPreviewHistory<AbstractBrowsePage>());
            this.SetPageHistoryButtonEnabled();
        }

        public void MoveNextPage()
        {
            if (!this._nextPageHistoryButton.Enabled)
            {
                return;
            }

            if (this._tabSwitch.ActiveTab == null)
            {
                throw new InvalidOperationException("アクティブなタブが存在しません。");
            }

            this.AddPageEventHandler(this._tabSwitch.SetNextPageHistory<AbstractBrowsePage>());
            this.SetPageHistoryButtonEnabled();
        }

        public void RedrawPage(float scale)
        {
            if (this._tabSwitch.ActiveTab != null)
            {
                using (Measuring.Time(true, "BrowsePanel.RedrawPage"))
                {
                    var page = this._tabSwitch.ActiveTab.GetPage<AbstractBrowsePage>();
                    page.RedrawPage(scale);
                }
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
                this._pageContainer.Dispose();
                this._infoPanel.Dispose();
                this._tabSwitch.Dispose();
                this._toolPanel.Dispose();
                this._toolPanel2.Dispose();
                this._showInfoToolButton.Dispose();
                this._addressBar.Dispose();
                this._nextPageHistoryButton.Dispose();
                this._previewPageHistoryButton.Dispose();
                this._homeToolButton.Dispose();
                this._searchRatingToolButton.Dispose();
                this._tagDropToolButton.Dispose();
                this._reloadToolButton.Dispose();
                this._searchBookmarkToolButton.Dispose();

                this._tagDropToolButton.Icon.Dispose();
            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        private void BrowsePanel_Loaded(object sender, EventArgs e)
        {
            this._addressBar.SetAddress(FileUtil.ROOT_DIRECTORY_PATH);
        }

        private void RemovePageEventHandler(AbstractBrowsePage page)
        {
            page.SelectedFileChanged -= new(this.Page_SelectedFileChanged);
            page.OpenPage -= new(this.Page_OpenPage);
            page.MouseClick -= new(this.Page_MouseClick);
        }

        private void SetPageHistoryButtonEnabled()
        {
            if (this._tabSwitch.ActiveTab != null)
            {
                this._previewPageHistoryButton.Enabled = this._tabSwitch.ActiveTab.HasPreviewPage;
                this._nextPageHistoryButton.Enabled = this._tabSwitch.ActiveTab.HasNextPage;
            }
            else
            {
                this._previewPageHistoryButton.Enabled = false;
                this._nextPageHistoryButton.Enabled = false;
            }
        }

        private void OpenPage(IPageParameter param, PageOpenMode openMode)
        {
            using (Measuring.Time(true, "BrowsePanel.OpenPage"))
            {
                if (openMode == PageOpenMode.OverlapTab)
                {
                    this.AddPageEventHandler(this._tabSwitch.OverwriteTab<AbstractBrowsePage>(param));
                    this.SetPageHistoryButtonEnabled();
                }
                else if (openMode == PageOpenMode.AddHome)
                {
                    this.AddPageEventHandler(this._tabSwitch.AddTab<AbstractBrowsePage>(param));
                }
                else if (openMode == PageOpenMode.AddTab)
                {
                    if (this._tabSwitch.ActiveTabIndex < 0)
                    {
                        this.AddPageEventHandler(this._tabSwitch.InsertTab<AbstractBrowsePage>(0, param));
                    }
                    else
                    {
                        this.AddPageEventHandler(this._tabSwitch.InsertTab<AbstractBrowsePage>(this._tabSwitch.ActiveTabIndex + 1, param));
                    }
                }
                else if (openMode == PageOpenMode.NewWindow)
                {
                    this.OnNewWindowPageOpen(new BrowsePageOpenEventArgs(param));
                }
                else
                {
                    throw new NotSupportedException("ファイル実行種別が不正です。");
                }
            }
        }

        private void InsertPage(IPageParameter param, int tabIndex)
        {
            this.AddPageEventHandler(this._tabSwitch.InsertTab<AbstractBrowsePage>(tabIndex, param));
        }

        private void OverlapPage(DragParameter dragData)
        {
            if (FileUtil.IsExistsDirectory(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを上書きします。
                this.OpenPage(new DirectoryFileListPageParameter(dragData.CurrentFilePath), PageOpenMode.OverlapTab);
            }
            else if (FileUtil.IsExistsFile(dragData.CurrentFilePath) &&
                ImageUtil.IsImageFile(dragData.CurrentFilePath))
            {
                // ビューアコンテンツを上書きします。
                var parameter = new ImageViewPageParameter(
                    dragData.PageSources,
                    dragData.SourcesKey,
                    dragData.GetImageFilesAction,
                    dragData.CurrentFilePath,
                    dragData.SortInfo,
                    dragData.PageTitle,
                    dragData.PageIcon,
                    dragData.VisibleBookmarkMenuItem);
                this.OpenPage(parameter, PageOpenMode.OverlapTab);
            }
        }

        private void InsertPage(DragParameter dragData, int tabIndex)
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
                var parameter = new ImageViewPageParameter(
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
            if (e.Data.GetDataPresent(typeof(DragParameter)))
            {
                if (e.IsOverlap)
                {
                    this.OverlapPage((DragParameter)e.Data.GetData(typeof(DragParameter)));
                }
                else
                {
                    this.InsertPage((DragParameter)e.Data.GetData(typeof(DragParameter)), e.TabIndex);
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

                var sortInfo = new SortParameter();
                sortInfo.SetSortMode(FileSortMode.FilePath, true);

                var dragData = new DragParameter(
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
            this._tagDropToolButton.SetItems([.. e]);

            if (!string.IsNullOrEmpty(this._tagDropToolButton.SelectedItem))
            {
                this._tagDropToolButton.SelectItem(this._tagDropToolButton.SelectedItem);
            }
        }

        // TODO: tabSwitch_ActiveTabChanged の直後に呼び出される場合がある。
        private void Page_SelectedFileChanged(object sender, SelectedFileChangeEventArgs e)
        {
            if (e.FilePathList.Length > 0)
            {
                this._addressBar.SetAddress(e.FilePathList[0]);
            }

            this._infoPanel.SetFileInfo(e.FilePathList);
            this._tabSwitch.InvalidateHeaderWithAnimation();
        }

        private void Page_OpenPage(object sender, BrowsePageEventArgs e)
        {
            this.OpenPage(e.Parameter, e.OpenMode);
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
            this.OpenPage(new FavoriteDirectoryListPageParameter(), PageOpenMode.AddHome);
        }

        private void TabSwitch_ActiveTabChanged(object sender, EventArgs e)
        {
            this.SetPageHistoryButtonEnabled();

            if (this._tabSwitch.ActiveTab != null)
            {
                foreach (var tab in this._tabSwitch.GetInactiveTabs())
                {
                    var p = tab.GetPage<AbstractBrowsePage>();
                    p.StopPageDraw();
                }

                var page = this._tabSwitch.ActiveTab.GetPage<AbstractBrowsePage>();
                var selectedFilePath = page.SelectedFilePath;
                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    this._addressBar.SetAddress(selectedFilePath);
                    this._infoPanel.SetFileInfo(page.GetSelectedFiles());
                    var scale = WindowUtil.GetCurrentWindowScale(this);
                    page.RedrawPage(scale);
                }
            }
        }

        private void TabSwitch_TabCloseButtonClick(object sender, TabEventArgs e)
        {
            this._tabSwitch.RemoveTab(e.Tab);

            e.Tab.Close();

            if (!this._tabSwitch.HasTab)
            {
                this.OnClose(EventArgs.Empty);
            }
        }

        private void TabSwitch_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            this.RemovePageEventHandler(e.Tab.GetPage<AbstractBrowsePage>());

            if (e.ToOtherOwner)
            {
                var window = e.Tab.Owner.GetWindow<BrowseWindow>();
                window.AddPageEventHandler(e.Tab.GetPage<AbstractBrowsePage>());
            }
            else
            {
                this.OnTabDropouted(e);
            }

            if (!this._tabSwitch.HasTab)
            {
                //this.FindForm().Opacity = 0;
                this.OnClose(EventArgs.Empty);
            }
        }

        private void TabSwitch_BackgroundMouseDoubleLeftClick(object sender, EventArgs e)
        {
            this.OnBackgroundMouseLeftDoubleClick(e);
        }

        private void TabSwitch_TabAreaDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragParameter)))
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
            this.OpenPage(new DirectoryFileListPageParameter(e.DirectoryPath, e.SubDirectoryPath), e.OpenMode);
        }

        private void ShowInfoToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (this._infoPanel.Visible)
            {
                this._infoPanel.Visible = false;
            }
            else
            {
                this._infoPanel.Visible = true;
            }

            var scale = WindowUtil.GetCurrentWindowScale(this);
            this.SetControlsBounds(scale);
        }

        private void InfoPanel_SelectedTag(object sender, SelectedTagEventArgs e)
        {
            this.OpenPage(new TagFileListPageParameter(e.Tag), e.OpenMode);
        }

        private void PageContainer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Shell IDList Array")
                && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }

            if (e.Data.GetDataPresent(typeof(DragParameter)))
            {
                var entity = (DragParameter)e.Data.GetData(typeof(DragParameter));
                if (!HasControl(entity.Sender, this))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void PageContainer_DragDrop(object sender, DragEventArgs e)
        {
            if (this._tabSwitch.ActiveTabIndex < 0)
            {
                this.DragDrop(new TabAreaDragEventArgs(true, 0, e));
            }
            else
            {
                this.DragDrop(new TabAreaDragEventArgs(true, this._tabSwitch.ActiveTabIndex, e));
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
                this.OpenPage(new FavoriteDirectoryListPageParameter(), PageOpenMode.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenPage(new FavoriteDirectoryListPageParameter(), PageOpenMode.AddTab);
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
                this.OpenPage(new TagFileListPageParameter(e.Item), PageOpenMode.OverlapTab);
            }
            else
            {
                this.OpenPage(new TagFileListPageParameter(e.Item), PageOpenMode.AddTab);
            }
        }

        private void SearchRatingToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenPage(new RatingFileListPageParameter(1), PageOpenMode.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenPage(new RatingFileListPageParameter(1), PageOpenMode.AddTab);
            }
        }

        private void SearchBookmarkToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenPage(new BookmarkFileListPageParameter(), PageOpenMode.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenPage(new BookmarkFileListPageParameter(), PageOpenMode.AddTab);
            }
        }

        private void HistoryToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenPage(new HistoryFileListPageParameter(), PageOpenMode.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenPage(new HistoryFileListPageParameter(), PageOpenMode.AddTab);
            }
        }
    }
}
