using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using PicSum.Core.Base.Conf;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Main.Properties;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Contents;
using PicSum.UIComponent.Contents.ContentsParameter;
using PicSum.UIComponent.InfoPanel;
using SWF.Common;
using SWF.UIComponent.TabOperation;

namespace PicSum.Main.UIComponent
{
    public partial class BrowserMainPanel : UserControl
    {
        #region 定数・列挙

        #endregion

        #region イベント

        public event EventHandler<TabDropoutedEventArgs> TabDropouted;
        public event EventHandler<BrowserContentsOpenEventArgs> NewWindowContentsOpen;
        public event EventHandler Close;
        public event EventHandler BackgroundMouseDoubleLeftClick;

        #endregion

        #region インスタンス変数

        private TwoWayProcess<SearchImageFileByFolderAsyncFacade, SearchImageFileParameterEntity, SearchImageFileResultEntity> _searchImageFileProcess = null;
        private readonly Action<bool> reloadButtonEnableAction = null;

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

        private TwoWayProcess<SearchImageFileByFolderAsyncFacade, SearchImageFileParameterEntity, SearchImageFileResultEntity> searchImageFileProcess
        {
            get
            {
                if (_searchImageFileProcess == null)
                {
                    _searchImageFileProcess = TaskManager.CreateTwoWayProcess<SearchImageFileByFolderAsyncFacade, SearchImageFileParameterEntity, SearchImageFileResultEntity>(components);
                    _searchImageFileProcess.Callback += new AsyncTaskCallbackEventHandler<SearchImageFileResultEntity>(searchImageFileProcess_Callback);
                }

                return _searchImageFileProcess;
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

                this.reloadButtonEnableAction = (isImage) =>
                {
                    this.reloadToolButton.Enabled = !isImage;
                };
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

        public void AddFavoriteFolderListTab()
        {
            openContents(new FavoriteFolderListContentsParameter(this.reloadButtonEnableAction), ContentsOpenType.AddTab);
        }

        public void RemoveActiveTab()
        {
            tabSwitch.RemoveActiveTab();
        }

        public void OpenContentsByCommandLineArgs(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (args.Length == 1)
            {
                addressBar.SetAddress();
            }
            else
            {
                List<string> filePathList = new List<string>(args);
                filePathList.RemoveAt(0);
                addContents(filePathList);
            }
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            IList<string> cmdLineList = new List<string>(Environment.GetCommandLineArgs());
            cmdLineList.RemoveAt(0);

            if (cmdLineList.Count > 0)
            {
                addContents(cmdLineList);
            }
            else
            {
                addressBar.SetAddress();
            }

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

            const int INFOPANEL_MINSIZE = 192;
            splitContainer.Panel2MinSize = INFOPANEL_MINSIZE;
            splitContainer.SplitterDistance = splitContainer.Width - splitContainer.Panel2MinSize - splitContainer.SplitterWidth;
        }

        private void addContentsEventHandler(BrowserContents contents)
        {
            contents.SelectedFileChanged += new EventHandler<SelectedFileChangeEventArgs>(contents_SelectedFileChanged);
            contents.OpenContents += new EventHandler<BrowserContentsEventArgs>(contents_OpenContents);
        }

        private void removeContentsEventHandler(BrowserContents contents)
        {
            contents.SelectedFileChanged -= new EventHandler<SelectedFileChangeEventArgs>(contents_SelectedFileChanged);
            contents.OpenContents -= new EventHandler<BrowserContentsEventArgs>(contents_OpenContents);
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

        private void addContents(IList<string> filePathList)
        {
            if (filePathList.Count == 0)
            {
                throw new ArgumentException("ファイルパスリストが0件です。", "filePathList");
            }

            if (filePathList.Count == 1)
            {
                string filePath = filePathList[0];
                if (FileUtil.IsFolder(filePath))
                {
                    // フォルダコンテンツを追加します。
                    openContents(new FolderFileListContentsParameter(filePath, this.reloadButtonEnableAction), ContentsOpenType.AddTab);
                }
                else if (FileUtil.IsFile(filePath))
                {
                    string ex = FileUtil.GetExtension(filePath);
                    if (ImageUtil.ImageFileExtensionList.Contains(ex))
                    {
                        // ビューアコンテンツを追加します。
                        SearchImageFileParameterEntity param = new SearchImageFileParameterEntity();
                        param.FilePath = filePath;
                        param.FileOpenType = ContentsOpenType.AddTab;
                        searchImageFileProcess.Execute(this, param);
                    }
                }
            }
            else
            {
                List<string> imgFiles = new List<string>();
                foreach (string filePath in filePathList)
                {
                    if (FileUtil.IsFolder(filePath))
                    {
                        // フォルダコンテンツを追加します。
                        openContents(new FolderFileListContentsParameter(filePath, this.reloadButtonEnableAction), ContentsOpenType.AddTab);
                    }
                    else if (FileUtil.IsFile(filePath))
                    {
                        string ex = FileUtil.GetExtension(filePath);
                        if (ImageUtil.ImageFileExtensionList.Contains(ex))
                        {
                            imgFiles.Add(filePath);
                        }
                    }
                }

                if (imgFiles.Count > 0)
                {
                    // TODO: コマンドライン引数、関連付けされた場合を考える。
                    // 引数に画像ファイルが存在する場合、ビューアコンテンツを追加します。
                    //openContents(new ImageViewerContentsParameter(imgFiles, imgFiles[0], this.reloadButtonEnableAction), ContentsOpenType.AddTab);
                }
            }
        }

        private void overlapContents(DragEntity dragData, int tabIndex)
        {
            if (dragData.FilePathList.Count > 0)
            {
                // ビューアコンテンツを上書きします。
                this.openContents(new ImageViewerContentsParameter(dragData.FilePathList, dragData.CurrentFilePath, dragData.ContentsTitle, dragData.ContentsIcon, this.reloadButtonEnableAction), ContentsOpenType.OverlapTab);
            }
            else if (FileUtil.IsFolder(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを上書きします。
                openContents(new FolderFileListContentsParameter(dragData.CurrentFilePath, this.reloadButtonEnableAction), ContentsOpenType.OverlapTab);
            }
            else if (FileUtil.IsFile(dragData.CurrentFilePath)
                && ImageUtil.ImageFileExtensionList.Contains(FileUtil.GetExtension(dragData.CurrentFilePath)))
            {
                // ビューアコンテンツを上書きします。
                SearchImageFileParameterEntity param = new SearchImageFileParameterEntity();
                param.FilePath = dragData.CurrentFilePath;
                param.FileOpenType = ContentsOpenType.OverlapTab;
                param.TabIndex = tabIndex;
                searchImageFileProcess.Execute(this, param);
            }
        }

        private void insertContents(DragEntity dragData, int tabIndex)
        {
            if (dragData.FilePathList.Count > 0)
            {
                // ビューアコンテンツを挿入します。
                insertContents(new ImageViewerContentsParameter(dragData.FilePathList, dragData.CurrentFilePath, dragData.ContentsTitle, dragData.ContentsIcon, this.reloadButtonEnableAction), tabIndex);
            }
            else if (FileUtil.IsFolder(dragData.CurrentFilePath))
            {
                // フォルダコンテンツを挿入します。
                insertContents(new FolderFileListContentsParameter(dragData.CurrentFilePath, this.reloadButtonEnableAction), tabIndex);
            }
            else if (FileUtil.IsFile(dragData.CurrentFilePath)
                && ImageUtil.ImageFileExtensionList.Contains(FileUtil.GetExtension(dragData.CurrentFilePath)))
            {
                // ビューアコンテンツを挿入します。
                SearchImageFileParameterEntity param = new SearchImageFileParameterEntity();
                param.FilePath = dragData.CurrentFilePath;
                param.FileOpenType = ContentsOpenType.InsertTab;
                param.TabIndex = tabIndex;
                searchImageFileProcess.Execute(this, param);
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
                var dragData = new DragEntity(filePath, FileUtil.IsFile(filePath) ? FileUtil.GetFileName(FileUtil.GetParentFolderPath(filePath)) : FileUtil.GetFileName(filePath), FileIconCash.SmallFolderIcon);
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

        private void searchImageFileProcess_Callback(object sender, SearchImageFileResultEntity e)
        {
            if (e.DirectoryNotFoundException != null)
            {
                MessageBox.Show(e.DirectoryNotFoundException.Message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dirName = FileUtil.GetFileName(e.DirectoryPath);
            if (e.FileOpenType == ContentsOpenType.OverlapTab)
            {
                if (e.TabIndex > -1 && tabSwitch.TabCount > e.TabIndex)
                {
                    tabSwitch.SetActiveTab(e.TabIndex);
                }

                openContents(new ImageViewerContentsParameter(e.FilePathList, e.SelectedFilePath, dirName, FileIconCash.SmallFolderIcon, this.reloadButtonEnableAction), ContentsOpenType.OverlapTab);
            }
            else if (e.FileOpenType == ContentsOpenType.AddTab)
            {
                openContents(new ImageViewerContentsParameter(e.FilePathList, e.SelectedFilePath, dirName, FileIconCash.SmallFolderIcon, this.reloadButtonEnableAction), ContentsOpenType.AddTab);
            }
            else if (e.FileOpenType == ContentsOpenType.InsertTab)
            {
                insertContents(new ImageViewerContentsParameter(e.FilePathList, e.SelectedFilePath, dirName, FileIconCash.SmallFolderIcon, this.reloadButtonEnableAction), e.TabIndex);
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

            //Console.WriteLine("[{0:HH:mm:ss.fff}] contents_SelectedFileChanged", DateTime.Now);
        }

        private void contents_OpenContents(object sender, BrowserContentsEventArgs e)
        {
            openContents(e.Parameter, e.OpenType);
        }

        #endregion

        #region タブ切替コントロールイベント

        private void tabSwitch_AddTabButtonMouseClick(object sender, MouseEventArgs e)
        {
            openContents(new FavoriteFolderListContentsParameter(this.reloadButtonEnableAction), ContentsOpenType.AddTab);
        }

        private void tabSwitch_ActiveTabChanged(object sender, EventArgs e)
        {
            setContentsHistoryButtonEnabled();

            if (tabSwitch.ActiveTab != null)
            {
                var selectedFilePath = tabSwitch.ActiveTab.GetContents<BrowserContents>().SelectedFilePath;
                addressBar.SetAddress(selectedFilePath);
                infoPanel.SetFileInfo(selectedFilePath);

                //Console.WriteLine("[{0:HH:mm:ss.fff}] tabSwitch_ActiveTabChanged", DateTime.Now);
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
            if (tabSwitch.ActiveTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (e.Button == MouseButtons.Left)
            {
                addContentsEventHandler(tabSwitch.SetPreviewContentsHistory<BrowserContents>());
                setContentsHistoryButtonEnabled();
            }
        }

        private void nextContentsHistoryButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (tabSwitch.ActiveTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (e.Button == MouseButtons.Left)
            {
                addContentsEventHandler(tabSwitch.SetNextContentsHistory<BrowserContents>());
                setContentsHistoryButtonEnabled();
            }
        }

        #endregion

        #region アドレスバーイベント

        private void addressBar_SelectedFolder(object sender, PicSum.UIComponent.AddressBar.SelectedFolderEventArgs e)
        {
            openContents(new FolderFileListContentsParameter(e.FolderPath, e.SubFolderPath, this.reloadButtonEnableAction), e.OpenType);
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
            this.openContents(new TagFileListContentsParameter(e.Tag, this.reloadButtonEnableAction), e.OpenType);
        }

        #endregion

        #region コンテンツコンテナイベント

        private void contentsContainer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragEntity)))
            {
                DragEntity dragData = (DragEntity)e.Data.GetData(typeof(DragEntity));
                if (tabSwitch.ActiveTab != null)
                {
                    if (!tabSwitch.ActiveTab.GetContents<BrowserContents>().Equals(dragData.SourceControl))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                }
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

        private void homeToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                openContents(new FavoriteFolderListContentsParameter(this.reloadButtonEnableAction), ContentsOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                openContents(new FavoriteFolderListContentsParameter(this.reloadButtonEnableAction), ContentsOpenType.AddTab);
            }
        }

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

        private void searchTagToolButton_SelectedTag(object sender, PicSum.UIComponent.SearchTool.SelectedTagEventArgs e)
        {
            openContents(new TagFileListContentsParameter(e.Value, this.reloadButtonEnableAction), e.OpenType);
        }

        private void searchRatingToolButton_SelectedRating(object sender, PicSum.UIComponent.SearchTool.SelectedRatingEventArgs e)
        {
            openContents(new RatingFileListContentsParameter(e.Value, this.reloadButtonEnableAction), e.OpenType);
        }

        private void keepToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                openContents(new KeepFileListContentsParameter(this.reloadButtonEnableAction), ContentsOpenType.OverlapTab);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                openContents(new KeepFileListContentsParameter(this.reloadButtonEnableAction), ContentsOpenType.AddTab);
            }
        }

        #endregion
    }
}
