using NLog;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Paramters;
using PicSum.Task.Results;
using PicSum.Task.Tasks;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.InfoPanel;
using SWF.Common;
using SWF.UIComponent.TabOperation;
using SWF.UIComponent.WideDropDown;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{
    [SupportedOSPlatform("windows")]
    public sealed partial class BrowserMainPanel : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region 定数・列挙

        #endregion

        #region イベント

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowserContentsOpenEventArgs> NewWindowContentsOpen;
        public event EventHandler Close;
        public event EventHandler BackgroundMouseDoubleLeftClick;

        #endregion

        #region インスタンス変数

        private Size previrewSize = Size.Empty;
        private Timer redrawTimer = null;
        private TwoWayTask<GetTagListTask, ListResult<string>> getTagListTask = null;
        private TwoWayTask<GetImageFileByDirectoryTask, GetImageFileByDirectoryParameter, GetImageFileByDirectoryResult> getFilesTask = null;

        #endregion

        #region パブリックプロパティ

        public int TabCount
        {
            get
            {
                return this.tabSwitch.TabCount;
            }
        }

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private TwoWayTask<GetTagListTask, ListResult<string>> GetTagListTask
        {
            get
            {
                if (this.getTagListTask == null)
                {
                    this.getTagListTask = new();
                    this.getTagListTask
                        .Callback(this.GetTagListTask_Callback)
                        .StartThread();
                }

                return this.getTagListTask;
            }
        }

        #endregion

        #region コンストラクタ

        public BrowserMainPanel()
        {
            this.InitializeComponent();

            if (!this.DesignMode)
            {
                this.SubInitializeComponent();
            }
        }

        #endregion

        #region パブリックメソッド

        public void AddContentsEventHandler(BrowserContents contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            contents.SelectedFileChanged += new EventHandler<SelectedFileChangeEventArgs>(this.Contents_SelectedFileChanged);
            contents.OpenContents += new EventHandler<BrowserContentsEventArgs>(this.Contents_OpenContents);
            contents.MouseClick += new EventHandler<MouseEventArgs>(this.Contents_MouseClick);
        }

        public void AddTab(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException(nameof(tab));
            }

            this.AddContentsEventHandler(tab.GetContents<BrowserContents>());

            this.tabSwitch.AddTab(tab);
        }

        public void AddTab(IContentsParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            this.AddContentsEventHandler(this.tabSwitch.AddTab<BrowserContents>(param));
        }

        public void AddFavoriteDirectoryListTab()
        {
            this.OpenContents(new FavoriteDirectoryListContentsParameter(), ContentsOpenType.AddTab);
        }

        public void RemoveActiveTab()
        {
            this.tabSwitch.RemoveActiveTab();
        }

        public void MovePreviewContents()
        {
            if (!this.previewContentsHistoryButton.Enabled)
            {
                return;
            }

            if (this.tabSwitch.ActiveTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            this.AddContentsEventHandler(this.tabSwitch.SetPreviewContentsHistory<BrowserContents>());
            this.SetContentsHistoryButtonEnabled();
        }

        public void MoveNextContents()
        {
            if (!this.nextContentsHistoryButton.Enabled)
            {
                return;
            }

            if (this.tabSwitch.ActiveTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            this.AddContentsEventHandler(this.tabSwitch.SetNextContentsHistory<BrowserContents>());
            this.SetContentsHistoryButtonEnabled();
        }

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (this.getTagListTask != null)
                {
                    this.getTagListTask.Dispose();
                    this.getTagListTask = null;
                }

                if (this.getFilesTask != null)
                {
                    this.getFilesTask.Dispose();
                    this.getFilesTask = null;
                }

                components.Dispose();
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

        #endregion

        #region プライベートメソッド

        private void SubInitializeComponent()
        {
            if (this.components == null)
            {
                this.components = new Container();
            }

            this.contentsContainer.SetBounds(
                0,
                64,
                this.Width - ApplicationConst.INFOPANEL_WIDTH,
                402);

            this.infoPanel.SetBounds(
                this.contentsContainer.Width,
                this.contentsContainer.Location.Y,
                ApplicationConst.INFOPANEL_WIDTH,
                402);

            this.contentsContainer.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.infoPanel.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Right;

            this.Controls.AddRange(new Control[]
            {
                this.contentsContainer,
                this.infoPanel,
            });

            this.infoPanel.BringToFront();
            this.contentsContainer.BringToFront();

            this.redrawTimer = new Timer();
            this.redrawTimer.Enabled = true;
            this.redrawTimer.Interval = 100;
            this.redrawTimer.Tick += this.RedrawTimer_Tick;
        }

        private void RedrawContents()
        {
            if (this.tabSwitch.ActiveTab != null)
            {
                var contents = this.tabSwitch.ActiveTab.GetContents<BrowserContents>();
                contents.RedrawContents();
            }
        }

        private void RedrawTimer_Tick(object sender, EventArgs e)
        {
            if (this.Size != this.previrewSize)
            {
                this.previrewSize = this.Size;
                this.RedrawContents();
            }
        }

        private TwoWayTask<GetImageFileByDirectoryTask, GetImageFileByDirectoryParameter, GetImageFileByDirectoryResult> CreateNewGetFilesTask()
        {
            if (this.getFilesTask != null)
            {
                this.getFilesTask.Dispose();
                this.getFilesTask = null;
            }

            this.getFilesTask = new();
            return this.getFilesTask;
        }

        private void RemoveContentsEventHandler(BrowserContents contents)
        {
            contents.SelectedFileChanged -= new EventHandler<SelectedFileChangeEventArgs>(this.Contents_SelectedFileChanged);
            contents.OpenContents -= new EventHandler<BrowserContentsEventArgs>(this.Contents_OpenContents);
            contents.MouseClick -= new EventHandler<MouseEventArgs>(this.Contents_MouseClick);
        }

        private void SetContentsHistoryButtonEnabled()
        {
            if (this.tabSwitch.ActiveTab != null)
            {
                this.previewContentsHistoryButton.Enabled = this.tabSwitch.ActiveTab.HasPreviewContents;
                this.nextContentsHistoryButton.Enabled = this.tabSwitch.ActiveTab.HasNextContents;
            }
            else
            {
                this.previewContentsHistoryButton.Enabled = false;
                this.nextContentsHistoryButton.Enabled = false;
            }
        }

        private void OpenContents(IContentsParameter param, ContentsOpenType openType)
        {
            if (openType == ContentsOpenType.OverlapTab)
            {
                this.AddContentsEventHandler(this.tabSwitch.OverwriteTab<BrowserContents>(param));
                this.SetContentsHistoryButtonEnabled();
            }
            else if (openType == ContentsOpenType.AddTab)
            {
                this.AddContentsEventHandler(this.tabSwitch.AddTab<BrowserContents>(param));
            }
            else if (openType == ContentsOpenType.NewWindow)
            {
                this.OnNewWindowContentsOpen(new BrowserContentsOpenEventArgs(param));
            }
            else
            {
                throw new Exception("ファイル実行種別が不正です。");
            }
        }

        private void InsertContents(IContentsParameter param, int tabIndex)
        {
            this.AddContentsEventHandler(this.tabSwitch.InsertTab<BrowserContents>(tabIndex, param));
        }

        private void OverlapContents(DragEntity dragData, int tabIndex)
        {
            if (FileUtil.IsDirectory(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを上書きします。
                this.OpenContents(new DirectoryFileListContentsParameter(dragData.CurrentFilePath), ContentsOpenType.OverlapTab);
            }
            else if (FileUtil.IsFile(dragData.CurrentFilePath) &&
                FileUtil.IsImageFile(dragData.CurrentFilePath))
            {
                // ビューアコンテンツを上書きします。
                var parameter = new ImageViewerContentsParameter(
                    dragData.ContentsSources,
                    dragData.SourcesKey,
                    dragData.GetImageFilesAction,
                    dragData.CurrentFilePath,
                    dragData.ContentsTitle,
                    dragData.ContentsIcon);
                this.OpenContents(parameter, ContentsOpenType.OverlapTab);
            }
        }

        private void InsertContents(DragEntity dragData, int tabIndex)
        {
            if (FileUtil.IsDirectory(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを挿入します。
                this.InsertContents(new DirectoryFileListContentsParameter(dragData.CurrentFilePath), tabIndex);
            }
            else if (FileUtil.IsFile(dragData.CurrentFilePath) &&
                FileUtil.IsImageFile(dragData.CurrentFilePath))
            {
                // ビューアコンテンツを挿入します。
                var parameter = new ImageViewerContentsParameter(
                    dragData.ContentsSources,
                    dragData.SourcesKey,
                    dragData.GetImageFilesAction,
                    dragData.CurrentFilePath,
                    dragData.ContentsTitle,
                    dragData.ContentsIcon);
                this.InsertContents(parameter, tabIndex);
            }
        }

        private new void DragDrop(TabAreaDragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragEntity)))
            {
                if (e.IsOverlap)
                {
                    this.OverlapContents((DragEntity)e.Data.GetData(typeof(DragEntity)), e.TabIndex);
                }
                else
                {
                    this.InsertContents((DragEntity)e.Data.GetData(typeof(DragEntity)), e.TabIndex);
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
                var dragData = new DragEntity(
                    DirectoryFileListContentsParameter.CONTENTS_SOURCES,
                    dirPath,
                    filePath,
                    this.GetImageFilesAction(new GetImageFileByDirectoryParameter(dirPath)),
                    FileUtil.GetFileName(dirPath),
                    FileIconCash.SmallDirectoryIcon);
                if (e.IsOverlap)
                {
                    this.OverlapContents(dragData, e.TabIndex);
                }
                else
                {
                    this.InsertContents(dragData, e.TabIndex);
                }
            }
        }

        private void OnTabDropouted(TabDropoutedEventArgs e)
        {
            if (this.TabDropouted != null)
            {
                this.TabDropouted(this, e);
            }
        }

        private void OnNewWindowContentsOpen(BrowserContentsOpenEventArgs e)
        {
            if (this.NewWindowContentsOpen != null)
            {
                this.NewWindowContentsOpen(this, e);
            }
        }

        private void OnClose(EventArgs e)
        {
            if (this.Close != null)
            {
                this.Close(this, e);
            }
        }

        private void OnBackgroundMouseLeftDoubleClick(EventArgs e)
        {
            if (this.BackgroundMouseDoubleLeftClick != null)
            {
                this.BackgroundMouseDoubleLeftClick(this, e);
            }
        }

        #endregion

        #region プロセスイベント

        private Func<ImageViewerContentsParameter, Action> GetImageFilesAction(
            GetImageFileByDirectoryParameter subParamter)
        {
            return (parameter) =>
            {
                return () =>
                {
                    var task = this.CreateNewGetFilesTask();
                    task
                    .Callback(e =>
                    {
                        var title = FileUtil.IsDirectory(subParamter.FilePath) ?
                        FileUtil.GetFileName(subParamter.FilePath) :
                        FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(subParamter.FilePath));

                        var eventArgs = new GetImageFilesEventArgs(
                            e.FilePathList, e.SelectedFilePath, title, FileIconCash.SmallDirectoryIcon);
                        parameter.OnGetImageFiles(eventArgs);
                    })
                    .Catch(e => ExceptionUtil.ShowErrorDialog(e.InnerException))
                    .StartThread();

                    task.StartTask(subParamter);
                };
            };
        }

        private void GetTagListTask_Callback(ListResult<string> e)
        {
            this.tagDropToolButton.SetItems(e);

            if (!string.IsNullOrEmpty(this.tagDropToolButton.SelectedItem))
            {
                this.tagDropToolButton.SelectItem(this.tagDropToolButton.SelectedItem);
            }
        }

        #endregion

        #region コンテンツイベント

        // TODO: tabSwitch_ActiveTabChanged の直後に呼び出される場合がある。
        private void Contents_SelectedFileChanged(object sender, SelectedFileChangeEventArgs e)
        {
            if (e.FilePathList.Count > 0)
            {
                this.addressBar.SetAddress(e.FilePathList[0]);
            }

            this.infoPanel.SetFileInfo(e.FilePathList);
            this.tabSwitch.InvalidateHeader();

            Logger.Debug("コンテンツ内の選択ファイルが変更されました。");
        }

        private void Contents_OpenContents(object sender, BrowserContentsEventArgs e)
        {
            this.OpenContents(e.Parameter, e.OpenType);
        }

        private void Contents_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.XButton1:
                    this.MovePreviewContents();
                    break;
                case MouseButtons.XButton2:
                    this.MoveNextContents();
                    break;
                default: break;
            }
        }

        #endregion

        #region タブ切替コントロールイベント

        private void TabSwitch_AddTabButtonMouseClick(object sender, MouseEventArgs e)
        {
            this.OpenContents(new FavoriteDirectoryListContentsParameter(), ContentsOpenType.AddTab);
        }

        private void TabSwitch_ActiveTabChanged(object sender, EventArgs e)
        {
            this.SetContentsHistoryButtonEnabled();

            if (this.tabSwitch.ActiveTab != null)
            {
                var contents = this.tabSwitch.ActiveTab.GetContents<BrowserContents>();
                var selectedFilePath = contents.SelectedFilePath;
                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    this.addressBar.SetAddress(selectedFilePath);
                    this.infoPanel.SetFileInfo(selectedFilePath);
                    contents.RedrawContents();
                }

                Logger.Debug("アクティブなタブが変更されました。");
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
                this.OnClose(new EventArgs());
            }
        }

        private void TabSwitch_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            this.RemoveContentsEventHandler(e.Tab.GetContents<BrowserContents>());

            if (e.ToOtherOwner)
            {
                var browser = e.Tab.Owner.GetForm<BrowserForm>();
                browser.AddContentsEventHandler(e.Tab.GetContents<BrowserContents>());
            }
            else
            {
                this.OnTabDropouted(e);
            }

            if (!this.tabSwitch.HasTab)
            {
                this.redrawTimer.Stop();
                this.redrawTimer.Dispose();
                this.OnClose(new EventArgs());
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

        #endregion

        #region コンテンツ履歴ボタンイベント

        private void PreviewContentsHistoryButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.MovePreviewContents();
        }

        private void NextContentsHistoryButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.MoveNextContents();
        }

        #endregion

        #region アドレスバーイベント

        private void AddressBar_SelectedDirectory(object sender, PicSum.UIComponent.AddressBar.SelectedDirectoryEventArgs e)
        {
            this.OpenContents(new DirectoryFileListContentsParameter(e.DirectoryPath, e.SubDirectoryPath), e.OpenType);
        }

        #endregion

        #region 情報表示ボタンイベント

        private void ShowInfoToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.contentsContainer.Width == this.Width)
            {
                this.contentsContainer.Width = this.Width - ApplicationConst.INFOPANEL_WIDTH;
            }
            else
            {
                this.contentsContainer.Width = this.Width;
            }

            var activeTab = this.tabSwitch.ActiveTab;
            if (activeTab != null)
            {
                var contents = activeTab.GetContents<BrowserContents>();
                contents.RedrawContents();
            }
        }

        #endregion

        #region 情報パネルイベント

        private void InfoPanel_SelectedTag(object sender, SelectedTagEventArgs e)
        {
            this.OpenContents(new TagFileListContentsParameter(e.Tag), e.OpenType);
        }

        #endregion

        #region コンテンツコンテナイベント

        private void ContentsContainer_DragEnter(object sender, DragEventArgs e)
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

        private void ContentsContainer_DragDrop(object sender, DragEventArgs e)
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

        #endregion

        #region ツールボタンイベント

        private void ReloadToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (this.tabSwitch.ActiveTab == null)
            {
                return;
            }

            if (!this.tabSwitch.ActiveTab.HasContents)
            {
                return;
            }

            this.AddContentsEventHandler(this.tabSwitch.CloneCurrentContents<BrowserContents>());
        }

        private void HomeToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenContents(new FavoriteDirectoryListContentsParameter(), ContentsOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenContents(new FavoriteDirectoryListContentsParameter(), ContentsOpenType.AddTab);
            }
        }

        private void TagDropToolButton_DropDownOpening(object sender, DropDownOpeningEventArgs e)
        {
            this.GetTagListTask.StartTask();
        }

        private void TagDropToolButton_ItemMouseClick(object sender, ItemMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenContents(new TagFileListContentsParameter(e.Item), ContentsOpenType.OverlapTab);
            }
            else
            {
                this.OpenContents(new TagFileListContentsParameter(e.Item), ContentsOpenType.AddTab);
            }
        }

        private void SearchRatingToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenContents(new RatingFileListContentsParameter(1), ContentsOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenContents(new RatingFileListContentsParameter(1), ContentsOpenType.AddTab);
            }
        }

        private void SearchBookmarkToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenContents(new BookmarkFileListContentsParameter(), ContentsOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OpenContents(new BookmarkFileListContentsParameter(), ContentsOpenType.AddTab);
            }
        }

        #endregion
    }
}
