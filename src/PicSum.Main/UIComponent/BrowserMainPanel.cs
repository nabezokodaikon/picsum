using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using NLog;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Main.Properties;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using PicSum.UIComponent.Contents;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.InfoPanel;
using SWF.Common;
using SWF.UIComponent.TabOperation;
using SWF.UIComponent.WideDropDown;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PicSum.Main.UIComponent
{
    public partial class BrowserMainPanel : UserControl
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

        private TwoWayProcess<GetTagListAsyncFacade, ListEntity<string>> _getTagListProcess = null;

        #endregion

        #region パブリックプロパティ

        public int TabCount
        {
            get
            {
                return tabSwitch.TabCount;
            }
        }

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private bool isShowFileInfo
        {
            get
            {
                return !splitContainer.Panel2Collapsed;
            }
            set
            {
                splitContainer.Panel2Collapsed = !value;
            }
        }

        private TwoWayProcess<GetTagListAsyncFacade, ListEntity<string>> getTagListProcess
        {
            get
            {
                if (this._getTagListProcess == null)
                {
                    this._getTagListProcess = TaskManager.CreateTwoWayProcess<GetTagListAsyncFacade, ListEntity<string>>(this.components);
                    this._getTagListProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<string>>(getTagListProcess_Callback);
                }

                return this._getTagListProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public BrowserMainPanel()
        {
            InitializeComponent();

            if (!this.DesignMode)
            {
                initializeComponent();
            }
        }

        #endregion

        #region パブリックメソッド

        public void AddContentsEventHandler(BrowserContents contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            addContentsEventHandler(contents);
        }

        public void AddTab(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            addContentsEventHandler(tab.GetContents<BrowserContents>());

            tabSwitch.AddTab(tab);
        }

        public void AddTab(IContentsParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            addContentsEventHandler(tabSwitch.AddTab<BrowserContents>(param));
        }

        public void AddFavoriteDirectoryListTab()
        {
            openContents(new FavoriteDirectoryListContentsParameter(), ContentsOpenType.AddTab);
        }

        public void RemoveActiveTab()
        {
            tabSwitch.RemoveActiveTab();
        }

        public void MovePreviewContents()
        {
            if (!this.previewContentsHistoryButton.Enabled)
            {
                return;
            }

            if (tabSwitch.ActiveTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            addContentsEventHandler(tabSwitch.SetPreviewContentsHistory<BrowserContents>());
            setContentsHistoryButtonEnabled();
        }

        public void MoveNextContents()
        {
            if (!this.nextContentsHistoryButton.Enabled)
            {
                return;
            }

            if (tabSwitch.ActiveTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            addContentsEventHandler(tabSwitch.SetNextContentsHistory<BrowserContents>());
            setContentsHistoryButtonEnabled();
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            this.addressBar.SetAddress(FileUtil.ROOT_DIRECTORY_PATH);
            base.OnLoad(e);
        }

        protected virtual void OnTabDropouted(TabDropoutedEventArgs e)
        {
            if (TabDropouted != null)
            {
                TabDropouted(this, e);
            }
        }

        protected virtual void OnNewWindowContentsOpen(BrowserContentsOpenEventArgs e)
        {
            if (NewWindowContentsOpen != null)
            {
                NewWindowContentsOpen(this, e);
            }
        }

        protected virtual void OnClose(EventArgs e)
        {
            if (Close != null)
            {
                Close(this, e);
            }
        }

        protected virtual void OnBackgroundMouseLeftDoubleClick(EventArgs e)
        {
            if (BackgroundMouseDoubleLeftClick != null)
            {
                BackgroundMouseDoubleLeftClick(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            if (components == null)
            {
                components = new Container();
            }

            splitContainer.Panel2MinSize = ApplicationConst.INFOPANEL_WIDTH;
            splitContainer.SplitterDistance = splitContainer.Width - splitContainer.Panel2MinSize - splitContainer.SplitterWidth;
        }

        private void addContentsEventHandler(BrowserContents contents)
        {
            contents.SelectedFileChanged += new EventHandler<SelectedFileChangeEventArgs>(contents_SelectedFileChanged);
            contents.OpenContents += new EventHandler<BrowserContentsEventArgs>(contents_OpenContents);
            contents.MouseClick += new EventHandler<MouseEventArgs>(contents_MouseClick);
        }

        private void removeContentsEventHandler(BrowserContents contents)
        {
            contents.SelectedFileChanged -= new EventHandler<SelectedFileChangeEventArgs>(contents_SelectedFileChanged);
            contents.OpenContents -= new EventHandler<BrowserContentsEventArgs>(contents_OpenContents);
            contents.MouseClick -= new EventHandler<MouseEventArgs>(contents_MouseClick);
        }

        private void setContentsHistoryButtonEnabled()
        {
            if (tabSwitch.ActiveTab != null)
            {
                previewContentsHistoryButton.Enabled = tabSwitch.ActiveTab.HasPreviewContents;
                nextContentsHistoryButton.Enabled = tabSwitch.ActiveTab.HasNextContents;
            }
            else
            {
                previewContentsHistoryButton.Enabled = false;
                nextContentsHistoryButton.Enabled = false;
            }
        }

        private void openContents(IContentsParameter param, ContentsOpenType openType)
        {
            if (openType == ContentsOpenType.OverlapTab)
            {
                addContentsEventHandler(tabSwitch.OverwriteTab<BrowserContents>(param));
                setContentsHistoryButtonEnabled();
            }
            else if (openType == ContentsOpenType.AddTab)
            {
                addContentsEventHandler(tabSwitch.AddTab<BrowserContents>(param));
            }
            else if (openType == ContentsOpenType.NewWindow)
            {
                OnNewWindowContentsOpen(new BrowserContentsOpenEventArgs(param));
            }
            else
            {
                throw new Exception("ファイル実行種別が不正です。");
            }
        }

        private void insertContents(IContentsParameter param, int tabIndex)
        {
            addContentsEventHandler(tabSwitch.InsertTab<BrowserContents>(tabIndex, param));
        }

        private void overlapContents(DragEntity dragData, int tabIndex)
        {
            if (FileUtil.IsDirectory(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを上書きします。
                this.openContents(new DirectoryFileListContentsParameter(dragData.CurrentFilePath), ContentsOpenType.OverlapTab);
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
                this.openContents(parameter, ContentsOpenType.OverlapTab);
            }
        }

        private void insertContents(DragEntity dragData, int tabIndex)
        {
            if (FileUtil.IsDirectory(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを挿入します。
                this.insertContents(new DirectoryFileListContentsParameter(dragData.CurrentFilePath), tabIndex);
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
                this.insertContents(parameter, tabIndex);
            }
        }

        private void dragDrop(TabAreaDragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragEntity)))
            {
                if (e.IsOverlap)
                {
                    overlapContents((DragEntity)e.Data.GetData(typeof(DragEntity)), e.TabIndex);
                }
                else
                {
                    insertContents((DragEntity)e.Data.GetData(typeof(DragEntity)), e.TabIndex);
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
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
                    overlapContents(dragData, e.TabIndex);
                }
                else
                {
                    insertContents(dragData, e.TabIndex);
                }
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
                    var proces = TaskManager.CreateTwoWayProcess<GetImageFileByDirectoryAsyncFacade, GetImageFileByDirectoryParameter, GetImageFileByDirectoryResult>(this.components);
                    proces.Callback += ((sender, e) =>
                    {
                        if (e.DirectoryNotFoundException != null)
                        {
                            ExceptionUtil.ShowErrorDialog(e.DirectoryNotFoundException);
                            return;
                        }

                        var title = FileUtil.IsDirectory(subParamter.FilePath) ?
                        FileUtil.GetFileName(subParamter.FilePath) :
                        FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(subParamter.FilePath));

                        var eventArgs = new GetImageFilesEventArgs(
                            e.FilePathList, e.SelectedFilePath, title, FileIconCash.SmallDirectoryIcon);
                        parameter.OnGetImageFiles(eventArgs);
                    });

                    proces.Execute(this, subParamter);
                };
            };
        }

        private void getTagListProcess_Callback(object sender, ListEntity<string> e)
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
        private void contents_SelectedFileChanged(object sender, SelectedFileChangeEventArgs e)
        {
            if (e.FilePathList.Count > 0)
            {
                addressBar.SetAddress(e.FilePathList[0]);
            }

            infoPanel.SetFileInfo(e.FilePathList);
            tabSwitch.InvalidateHeader();

#if DEBUG
            Logger.Debug("コンテンツ内の選択ファイルが変更されました。");
#endif
        }

        private void contents_OpenContents(object sender, BrowserContentsEventArgs e)
        {
            openContents(e.Parameter, e.OpenType);
        }

        private void contents_MouseClick(object sender, MouseEventArgs e)
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

        private void tabSwitch_AddTabButtonMouseClick(object sender, MouseEventArgs e)
        {
            openContents(new FavoriteDirectoryListContentsParameter(), ContentsOpenType.AddTab);
        }

        private void tabSwitch_ActiveTabChanged(object sender, EventArgs e)
        {
            setContentsHistoryButtonEnabled();

            if (tabSwitch.ActiveTab != null)
            {
                var selectedFilePath = tabSwitch.ActiveTab.GetContents<BrowserContents>().SelectedFilePath;
                if (!string.IsNullOrEmpty(selectedFilePath)) 
                {
                    addressBar.SetAddress(selectedFilePath);
                    infoPanel.SetFileInfo(selectedFilePath);
                }

#if DEBUG
                Logger.Debug("アクティブなタブが変更されました。");
#endif
            }
        }

        private void tabSwitch_TabCloseButtonClick(object sender, SWF.UIComponent.TabOperation.TabEventArgs e)
        {
            tabSwitch.RemoveTab(e.Tab);

            e.Tab.Close();

            if (!tabSwitch.HasTab)
            {
                OnClose(new EventArgs());
            }
        }

        private void tabSwitch_TabDropouted(object sender, SWF.UIComponent.TabOperation.TabDropoutedEventArgs e)
        {
            removeContentsEventHandler(e.Tab.GetContents<BrowserContents>());

            if (e.ToOtherOwner)
            {
                BrowserForm browser = e.Tab.Owner.GetForm<BrowserForm>();
                browser.AddContentsEventHandler(e.Tab.GetContents<BrowserContents>());
            }
            else
            {
                OnTabDropouted(e);
            }

            if (!tabSwitch.HasTab)
            {
                OnClose(new EventArgs());
            }
        }

        private void tabSwitch_BackgroundMouseDoubleLeftClick(object sender, EventArgs e)
        {
            OnBackgroundMouseLeftDoubleClick(e);
        }

        private void tabSwitch_TabAreaDragOver(object sender, DragEventArgs e)
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

        private void tabSwitch_TabAreaDragDrop(object sender, TabAreaDragEventArgs e)
        {
            dragDrop(e);
        }

        #endregion

        #region コンテンツ履歴ボタンイベント

        private void previewContentsHistoryButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.MovePreviewContents();
        }

        private void nextContentsHistoryButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.MoveNextContents();
        }

        #endregion

        #region アドレスバーイベント

        private void addressBar_SelectedDirectory(object sender, PicSum.UIComponent.AddressBar.SelectedDirectoryEventArgs e)
        {
            openContents(new DirectoryFileListContentsParameter(e.DirectoryPath, e.SubDirectoryPath), e.OpenType);
        }

        #endregion

        #region 情報表示ボタンイベント

        private void showInfoToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            isShowFileInfo = !isShowFileInfo;
        }

        #endregion

        #region 情報パネルイベント

        private void infoPanel_SelectedTag(object sender, SelectedTagEventArgs e)
        {
            this.openContents(new TagFileListContentsParameter(e.Tag), e.OpenType);
        }

        #endregion

        #region コンテンツコンテナイベント

        private void contentsContainer_DragEnter(object sender, DragEventArgs e)
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

        private void contentsContainer_DragDrop(object sender, DragEventArgs e)
        {
            if (tabSwitch.ActiveTabIndex < 0)
            {
                dragDrop(new TabAreaDragEventArgs(true, 0, e));
            }
            else
            {
                dragDrop(new TabAreaDragEventArgs(true, tabSwitch.ActiveTabIndex, e));
            }
        }

        #endregion

        #region ツールボタンイベント

        private void reloadToolButton_MouseClick(object sender, MouseEventArgs e)
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

            this.addContentsEventHandler(this.tabSwitch.CloneCurrentContents<BrowserContents>());
        }

        private void homeToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                openContents(new FavoriteDirectoryListContentsParameter(), ContentsOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                openContents(new FavoriteDirectoryListContentsParameter(), ContentsOpenType.AddTab);
            }
        }

        private void tagDropToolButton_DropDownOpening(object sender, DropDownOpeningEventArgs e)
        {
            this.getTagListProcess.Execute(this);
        }

        private void tagDropToolButton_ItemMouseClick(object sender, ItemMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.openContents(new TagFileListContentsParameter(e.Item), ContentsOpenType.OverlapTab);
            }
            else
            {
                this.openContents(new TagFileListContentsParameter(e.Item), ContentsOpenType.AddTab);
            }
        }

        private void searchRatingToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                openContents(new RatingFileListContentsParameter(1), ContentsOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                openContents(new RatingFileListContentsParameter(1), ContentsOpenType.AddTab);
            }
        }

        private void searchBookmarkToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                openContents(new BookmarkFileListContentsParameter(), ContentsOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                openContents(new BookmarkFileListContentsParameter(), ContentsOpenType.AddTab);
            }
        }

        #endregion
    }
}
