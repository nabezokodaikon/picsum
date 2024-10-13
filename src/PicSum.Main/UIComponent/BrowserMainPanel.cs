using SWF.Core.Job;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.InfoPanel;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.UIComponent.TabOperation;
using SWF.UIComponent.WideDropDown;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{
    [SupportedOSPlatform("windows")]
    public sealed partial class BrowserMainPanel : UserControl
    {

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowserPageOpenEventArgs> NewWindowPageOpen;
        public event EventHandler Close;
        public event EventHandler BackgroundMouseDoubleLeftClick;

        private Size previrewSize = Size.Empty;
        private Timer redrawTimer = null;
        private TwoWayJob<TagsGetJob, ListResult<string>> getTagListJob = null;
        private TwoWayJob<ImageFileGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFileGetByDirectoryResult> getFilesJob = null;

        public int TabCount
        {
            get
            {
                return this.tabSwitch.TabCount;
            }
        }

        private TwoWayJob<TagsGetJob, ListResult<string>> GetTagListJob
        {
            get
            {
                if (this.getTagListJob == null)
                {
                    this.getTagListJob = new();
                    this.getTagListJob
                        .Callback(this.GetTagListJob_Callback);
                }

                return this.getTagListJob;
            }
        }

        public BrowserMainPanel()
        {
            this.InitializeComponent();

            if (!this.DesignMode)
            {
                this.pageContainer.SetBounds(
                    0,
                    64,
                    this.Width,
                    402);

                this.infoPanel.SetBounds(
                    this.pageContainer.Width - ApplicationConstants.INFOPANEL_WIDTH,
                    this.pageContainer.Location.Y,
                    ApplicationConstants.INFOPANEL_WIDTH,
                    402);

                this.pageContainer.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Bottom
                    | AnchorStyles.Left
                    | AnchorStyles.Right;

                this.infoPanel.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Bottom
                    | AnchorStyles.Right;

                this.Controls.AddRange(
                    [
                        this.pageContainer,
                        this.infoPanel,
                    ]);

                this.infoPanel.BringToFront();
                this.pageContainer.BringToFront();

                this.redrawTimer = new Timer
                {
                    Enabled = true,
                    Interval = 500,
                };
                this.redrawTimer.Tick += this.RedrawTimer_Tick;
            }
        }

        public void AddPageEventHandler(BrowserPage page)
        {
            ArgumentNullException.ThrowIfNull(page, nameof(page));

            page.SelectedFileChanged += new(this.Page_SelectedFileChanged);
            page.OpenPage += new(this.Page_OpenPage);
            page.MouseClick += new(this.Page_MouseClick);
        }

        public void AddTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            this.AddPageEventHandler(tab.GetPage<BrowserPage>());
            this.tabSwitch.AddTab(tab);
        }

        public void AddTab(IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this.AddPageEventHandler(this.tabSwitch.AddTab<BrowserPage>(param));
        }

        public void AddFavoriteDirectoryListTab()
        {
            this.OpenPage(new FavoriteDirectoryListPageParameter(), PageOpenType.AddTab);
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

            this.AddPageEventHandler(this.tabSwitch.CloneCurrentPage<BrowserPage>());
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

            this.AddPageEventHandler(this.tabSwitch.SetPreviewHistory<BrowserPage>());
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

            this.AddPageEventHandler(this.tabSwitch.SetNextPageHistory<BrowserPage>());
            this.SetPageHistoryButtonEnabled();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.getTagListJob != null)
                {
                    this.getTagListJob.Dispose();
                    this.getTagListJob = null;
                }

                if (this.getFilesJob != null)
                {
                    this.getFilesJob.Dispose();
                    this.getFilesJob = null;
                }

                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.previrewSize = this.Size;
            this.redrawTimer.Start();
            this.addressBar.SetAddress(FileUtil.ROOT_DIRECTORY_PATH);
            base.OnLoad(e);
        }

        private void RedrawPage()
        {
            if (this.tabSwitch.ActiveTab != null)
            {
                var page = this.tabSwitch.ActiveTab.GetPage<BrowserPage>();
                page.RedrawPage();
            }
        }

        private void RedrawTimer_Tick(object sender, EventArgs e)
        {
            if (this.Size != this.previrewSize)
            {
                this.previrewSize = this.Size;
                this.RedrawPage();
            }
        }

        private TwoWayJob<ImageFileGetByDirectoryJob, ImageFileGetByDirectoryParameter, ImageFileGetByDirectoryResult> CreateNewGetFilesJob()
        {
            if (this.getFilesJob != null)
            {
                this.getFilesJob.Dispose();
                this.getFilesJob = null;
            }

            this.getFilesJob = new();
            return this.getFilesJob;
        }

        private void RemovePageEventHandler(BrowserPage page)
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
            if (openType == PageOpenType.OverlapTab)
            {
                this.AddPageEventHandler(this.tabSwitch.OverwriteTab<BrowserPage>(param));
                this.SetPageHistoryButtonEnabled();
            }
            else if (openType == PageOpenType.AddHome)
            {
                this.AddPageEventHandler(this.tabSwitch.AddTab<BrowserPage>(param));
            }
            else if (openType == PageOpenType.AddTab)
            {
                if (this.tabSwitch.ActiveTabIndex < 0)
                {
                    this.AddPageEventHandler(this.tabSwitch.InsertTab<BrowserPage>(0, param));
                }
                else
                {
                    this.AddPageEventHandler(this.tabSwitch.InsertTab<BrowserPage>(this.tabSwitch.ActiveTabIndex + 1, param));
                }
            }
            else if (openType == PageOpenType.NewWindow)
            {
                this.OnNewWindowPageOpen(new BrowserPageOpenEventArgs(param));
            }
            else
            {
                throw new Exception("ファイル実行種別が不正です。");
            }
        }

        private void InsertPage(IPageParameter param, int tabIndex)
        {
            this.AddPageEventHandler(this.tabSwitch.InsertTab<BrowserPage>(tabIndex, param));
        }

        private void OverlapPage(DragEntity dragData)
        {
            if (FileUtil.IsDirectory(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを上書きします。
                this.OpenPage(new DirectoryFileListPageParameter(dragData.CurrentFilePath), PageOpenType.OverlapTab);
            }
            else if (FileUtil.IsFile(dragData.CurrentFilePath) &&
                FileUtil.IsImageFile(dragData.CurrentFilePath))
            {
                // ビューアコンテンツを上書きします。
                var parameter = new ImageViewerPageParameter(
                    dragData.PageSources,
                    dragData.SourcesKey,
                    dragData.GetImageFilesAction,
                    dragData.CurrentFilePath,
                    dragData.SortInfo,
                    dragData.PageTitle,
                    dragData.PageIcon);
                this.OpenPage(parameter, PageOpenType.OverlapTab);
            }
        }

        private void InsertPage(DragEntity dragData, int tabIndex)
        {
            if (FileUtil.IsDirectory(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを挿入します。
                this.InsertPage(new DirectoryFileListPageParameter(dragData.CurrentFilePath), tabIndex);
            }
            else if (FileUtil.IsFile(dragData.CurrentFilePath) &&
                FileUtil.IsImageFile(dragData.CurrentFilePath))
            {
                // ビューアコンテンツを挿入します。
                var parameter = new ImageViewerPageParameter(
                    dragData.PageSources,
                    dragData.SourcesKey,
                    dragData.GetImageFilesAction,
                    dragData.CurrentFilePath,
                    dragData.SortInfo,
                    dragData.PageTitle,
                    dragData.PageIcon);
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

                var dirPath = FileUtil.IsDirectory(filePath) ?
                    filePath : FileUtil.GetParentDirectoryPath(filePath);

                var sortInfo = new SortInfo();
                sortInfo.SetSortType(SortTypeID.FilePath, true);

                var dragData = new DragEntity(
                    DirectoryFileListPageParameter.PAGE_SOURCES,
                    dirPath,
                    filePath,
                    sortInfo,
                    this.GetImageFilesAction(new ImageFileGetByDirectoryParameter(dirPath)),
                    FileUtil.GetFileName(dirPath),
                    FileIconCash.SmallDirectoryIcon);
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

        private void OnNewWindowPageOpen(BrowserPageOpenEventArgs e)
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

        private Func<ImageViewerPageParameter, Action> GetImageFilesAction(
            ImageFileGetByDirectoryParameter subParamter)
        {
            return (parameter) =>
            {
                return () =>
                {
                    var job = this.CreateNewGetFilesJob();
                    job
                    .Callback(e =>
                    {
                        var title = FileUtil.IsDirectory(subParamter.FilePath) ?
                        FileUtil.GetFileName(subParamter.FilePath) :
                        FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(subParamter.FilePath));

                        var eventArgs = new GetImageFilesEventArgs(
                            e.FilePathList, e.SelectedFilePath, title, FileIconCash.SmallDirectoryIcon);
                        parameter.OnGetImageFiles(eventArgs);
                    });

                    job.StartJob(subParamter);
                };
            };
        }

        private void GetTagListJob_Callback(ListResult<string> e)
        {
            this.tagDropToolButton.SetItems(e);

            if (!string.IsNullOrEmpty(this.tagDropToolButton.SelectedItem))
            {
                this.tagDropToolButton.SelectItem(this.tagDropToolButton.SelectedItem);
            }
        }

        // TODO: tabSwitch_ActiveTabChanged の直後に呼び出される場合がある。
        private void Page_SelectedFileChanged(object sender, SelectedFileChangeEventArgs e)
        {
            if (e.FilePathList.Count > 0)
            {
                this.addressBar.SetAddress(e.FilePathList[0]);
            }

            this.infoPanel.SetFileInfo(e.FilePathList);
            this.tabSwitch.InvalidateHeader();
        }

        private void Page_OpenPage(object sender, BrowserPageEventArgs e)
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
                var page = this.tabSwitch.ActiveTab.GetPage<BrowserPage>();
                var selectedFilePath = page.SelectedFilePath;
                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    this.addressBar.SetAddress(selectedFilePath);
                    this.infoPanel.SetFileInfo(selectedFilePath);
                    page.RedrawPage();
                }
            }
        }

        private void TabSwitch_TabCloseButtonClick(object sender, TabEventArgs e)
        {
            this.tabSwitch.RemoveTab(e.Tab);

            e.Tab.Close();

            if (!this.tabSwitch.HasTab)
            {
                this.redrawTimer.Stop();
                this.redrawTimer.Dispose();
                this.OnClose(EventArgs.Empty);
            }
        }

        private void TabSwitch_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            this.RemovePageEventHandler(e.Tab.GetPage<BrowserPage>());

            if (e.ToOtherOwner)
            {
                var browser = e.Tab.Owner.GetForm<BrowserForm>();
                browser.AddPageEventHandler(e.Tab.GetPage<BrowserPage>());
            }
            else
            {
                this.OnTabDropouted(e);
            }

            if (!this.tabSwitch.HasTab)
            {
                this.redrawTimer.Stop();
                this.redrawTimer.Dispose();
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
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
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
            if (this.pageContainer.Width == this.Width)
            {
                this.pageContainer.Width = this.Width - ApplicationConstants.INFOPANEL_WIDTH;
            }
            else
            {
                this.pageContainer.Width = this.Width;
            }

            var activeTab = this.tabSwitch.ActiveTab;
            if (activeTab != null)
            {
                var page = activeTab.GetPage<BrowserPage>();
                page.RedrawPage();
            }
        }

        private void InfoPanel_SelectedTag(object sender, SelectedTagEventArgs e)
        {
            this.OpenPage(new TagFileListPageParameter(e.Tag), e.OpenType);
        }

        private void PageContainer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
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
            this.GetTagListJob.StartJob();
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
