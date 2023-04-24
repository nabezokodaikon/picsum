using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.ContentsParameter;
using SWF.Common;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents.FileListContents
{
    /// <summary>
    /// ファイルリストコンテンツ基底クラス
    /// </summary>
    internal partial class FileListContentsBase : BrowserContents
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private Dictionary<string, FileEntity> _masterFileDictionary = null;
        private List<string> _filterFilePathList = null;
        private string _selectedFilePath = string.Empty;
        private readonly SolidBrush _selectedTextBrush = new SolidBrush(Color.White);
        private readonly SortInfo _sortInfo = new SortInfo();
        private TwoWayProcess<GetThumbnailsAsyncFacade, GetThumbnailParameter, ThumbnailImageEntity> _getThumbnailsProcess = null;
        private OneWayProcess<AddKeepAsyncFacade, ListEntity<KeepFileEntity>> _addKeepProcess = null;
        private OneWayProcess<ExportFileAsyncFacade, ExportFileParameter> _exportFileProcess = null;

        #endregion

        #region パブリックプロパティ

        public override string SelectedFilePath
        {
            get
            {
                return _selectedFilePath;
            }
        }

        #endregion

        #region 継承プロパティ

        protected bool IsAscending
        {
            get
            {
                return _sortInfo.IsAscending(_sortInfo.ActiveSortType);
            }
        }

        protected SortTypeID SortTypeID
        {
            get
            {
                return _sortInfo.ActiveSortType;
            }
        }

        public bool IsFileActiveTabOpenMenuItemVisible
        {
            get
            {
                return fileContextMenu.IsFileActiveTabOpenMenuItemVisible;
            }
            set
            {
                fileContextMenu.IsFileActiveTabOpenMenuItemVisible = value;
            }
        }

        public bool IsDirectoryActiveTabOpenMenuItemVisible
        {
            get
            {
                return fileContextMenu.IsDirectoryActiveTabOpenMenuItemVisible;
            }
            set
            {
                fileContextMenu.IsDirectoryActiveTabOpenMenuItemVisible = value;
            }
        }

        protected bool IsAddKeepMenuItemVisible
        {
            get
            {
                return fileContextMenu.IsAddKeepMenuItemVisible;
            }
            set
            {
                fileContextMenu.IsAddKeepMenuItemVisible = value;
            }
        }

        protected bool IsRemoveFromListMenuItemVisible
        {
            get
            {
                return fileContextMenu.IsRemoveFromListMenuItemVisible;
            }
            set
            {
                fileContextMenu.IsRemoveFromListMenuItemVisible = value;
            }
        }

        public bool IsMoveControlVisible
        {
            get
            {
                return movePreviewToolStripButton.Visible;
            }
            set
            {
                movePreviewToolStripButton.Visible = value;
                moveNextToolStripButton.Visible = value;
            }
        }

        #endregion

        #region プライベートプロパティ

        private bool isShowDirectory
        {
            get
            {
                return showDirectoryToolStripMenuItem.Checked;
            }
            set
            {
                showDirectoryToolStripMenuItem.Checked = value;
            }
        }

        private bool isShowImageFile
        {
            get
            {
                return showImageFileToolStripMenuItem.Checked;
            }
            set
            {
                showImageFileToolStripMenuItem.Checked = value;
            }
        }

        private bool isShowOtherFile
        {
            get
            {
                return showOtherFileToolStripMenuItem.Checked;
            }
            set
            {
                showOtherFileToolStripMenuItem.Checked = value;
            }
        }

        private bool isShowFileName
        {
            get
            {
                return showFileNameToolStripMenuItem.Checked;
            }
            set
            {
                showFileNameToolStripMenuItem.Checked = value;
            }
        }

        private int thumbnailSize
        {
            get
            {
                return thumbnailSizeToolStripSlider.Value;
            }
            set
            {
                thumbnailSizeToolStripSlider.Value = value;
            }
        }

        private TwoWayProcess<GetThumbnailsAsyncFacade, GetThumbnailParameter, ThumbnailImageEntity> getThumbnailsProcess
        {
            get
            {
                if (_getThumbnailsProcess == null)
                {
                    _getThumbnailsProcess = TaskManager.CreateTwoWayProcess<GetThumbnailsAsyncFacade, GetThumbnailParameter, ThumbnailImageEntity>(ProcessContainer);
                    _getThumbnailsProcess.Callback += new AsyncTaskCallbackEventHandler<ThumbnailImageEntity>(getThumbnailsProcess_Callback);

                }

                return _getThumbnailsProcess;
            }
        }

        private OneWayProcess<AddKeepAsyncFacade, ListEntity<KeepFileEntity>> addKeepProcess
        {
            get
            {
                if (_addKeepProcess == null)
                {
                    _addKeepProcess = TaskManager.CreateOneWayProcess<AddKeepAsyncFacade, ListEntity<KeepFileEntity>>(ProcessContainer);
                }

                return _addKeepProcess;
            }
        }

        private OneWayProcess<ExportFileAsyncFacade, ExportFileParameter> exportFileProcess
        {
            get
            {
                if (_exportFileProcess == null)
                {
                    _exportFileProcess = TaskManager.CreateOneWayProcess<ExportFileAsyncFacade, ExportFileParameter>(ProcessContainer);
                }

                return _exportFileProcess;
            }
        }

        private int itemTextHeight
        {
            get
            {
                return base.FontHeight * 2;
            }
        }

        #endregion

        #region コンストラクタ

        public FileListContentsBase(IContentsParameter param)
            : base(param)
        {
            InitializeComponent();
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            getThumbnailsProcess.Cancel();
            base.OnInvalidated(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            flowList.MouseWheelProcess(e);
            base.OnMouseWheel(e);
        }

        protected virtual void OnRemoveFile(IList<string> filePathList)
        {
            throw new NotImplementedException("継承先から呼び出して下さい。");
        }

        protected virtual void OnMovePreviewButtonClick(EventArgs e)
        {
            throw new NotImplementedException("継承先から呼び出して下さい。");
        }

        protected virtual void OnMoveNextButtonClick(EventArgs e)
        {
            throw new NotImplementedException("継承先から呼び出して下さい。");
        }

        protected void SetFiles(IList<FileShallowInfoEntity> srcFiles, string selectedFilePath, SortTypeID sortTypeID, bool isAscending)
        {
            if (srcFiles == null)
            {
                throw new ArgumentNullException("srcFiles");
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException("selectedFilePath");
            }

            _masterFileDictionary = new Dictionary<string, FileEntity>();
            foreach (FileShallowInfoEntity srcFile in srcFiles)
            {
                FileEntity destFile = new FileEntity();
                destFile.FilePath = srcFile.FilePath;
                destFile.FileName = srcFile.FileName;
                destFile.UpdateDate = srcFile.UpdateDate;
                destFile.CreateDate = srcFile.CreateDate;
                destFile.RgistrationDate = srcFile.RgistrationDate;
                destFile.Icon = srcFile.LargeIcon;
                destFile.IsFile = srcFile.IsFile;
                destFile.IsImageFile = srcFile.IsImageFile;
                _masterFileDictionary.Add(destFile.FilePath, destFile);
            }

            _selectedFilePath = selectedFilePath;
            _sortInfo.SetSortType(sortTypeID, isAscending);

            setSort();
            setFilter();

            this.Focus();
        }

        protected void SetFile(IList<FileShallowInfoEntity> srcFiles, string selectedFilePath)
        {
            if (srcFiles == null)
            {
                throw new ArgumentNullException("srcFiles");
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException("selectedFilePath");
            }

            SetFiles(srcFiles, selectedFilePath, SortTypeID.Default, false);
        }

        protected IList<string> GetSelectedFiles()
        {
            List<string> filePathList = new List<string>();
            IList<int> selectedIndexs = flowList.GetSelectedIndexs();
            if (selectedIndexs.Count > 0)
            {
                foreach (int index in selectedIndexs)
                {
                    filePathList.Add(_filterFilePathList[index]);
                }
            }

            return filePathList;
        }

        protected void SetContextMenuFiles(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("0件のファイルリストはセットできません。", "filePathList");
            }

            fileContextMenu.SetFile(filePathList);
        }

        protected void SetContextMenuFiles(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            SetContextMenuFiles(new List<string>() { filePath });
        }

        protected void RemoveFile(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("削除するファイルパスが0件です。", "filePath");
            }

            List<int> _indexList = new List<int>();
            foreach (string filePath in filePathList)
            {
                _masterFileDictionary.Remove(filePath);
                _indexList.Add(_filterFilePathList.IndexOf(filePath));
            }

            if (_indexList.Count > 0)
            {
                int maximumIndex = _indexList.Max();
                if (maximumIndex + 1 < _filterFilePathList.Count)
                {
                    _selectedFilePath = _filterFilePathList[maximumIndex + 1];
                }
                else
                {
                    int minimumIndex = _indexList.Min();
                    if (minimumIndex - 1 > -1)
                    {
                        _selectedFilePath = _filterFilePathList[minimumIndex - 1];
                    }
                    else
                    {
                        _selectedFilePath = string.Empty;
                    }
                }
            }
            else
            {
                _selectedFilePath = string.Empty;
            }

            setFilter();
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Font = new Font("Courier", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
            isShowFileName = FileListContentsConfig.IsShowFileName;
            isShowDirectory = FileListContentsConfig.IsShowDirectory;
            isShowImageFile = FileListContentsConfig.IsShowImageFile;
            isShowOtherFile = FileListContentsConfig.IsShowOtherFile;
            thumbnailSize = FileListContentsConfig.ThumbnailSize;
            setFlowListItemSize();
        }

        private ToolStripButton getSortToolStripButton(SortTypeID sortType)
        {
            switch (sortType)
            {
                case SortTypeID.FileName:
                    return sortFileNameToolStripButton;
                case SortTypeID.FilePath:
                    return sortFilePathToolStripButton;
                case SortTypeID.UpdateDate:
                    return sortFileUpdateDateToolStripButton;
                case SortTypeID.CreateDate:
                    return sortFileCreateDateToolStripButton;
                case SortTypeID.RgistrationDate:
                    return sortFileRgistrationDateToolStripButton;
                default:
                    return null;
            }
        }

        private void setSort()
        {
            sortFileNameToolStripButton.Image = null;
            sortFilePathToolStripButton.Image = null;
            sortFileUpdateDateToolStripButton.Image = null;
            sortFileCreateDateToolStripButton.Image = null;
            sortFileRgistrationDateToolStripButton.Image = null;

            ToolStripButton sortButton = getSortToolStripButton(_sortInfo.ActiveSortType);
            if (sortButton != null)
            {
                bool isAscending = _sortInfo.IsAscending(_sortInfo.ActiveSortType);
                sortButton.Image = _sortInfo.GetSortDirectionImage(isAscending);
            }
        }

        private void setFilter()
        {
            FileListContentsConfig.IsShowDirectory = isShowDirectory;
            FileListContentsConfig.IsShowImageFile = isShowImageFile;
            FileListContentsConfig.IsShowOtherFile = isShowOtherFile;

            if (_masterFileDictionary == null)
            {
                return;
            }

            List<FileEntity> filterList = new List<FileEntity>();
            foreach (FileEntity file in _masterFileDictionary.Values)
            {
                if (file.IsFile)
                {
                    if (file.IsImageFile)
                    {
                        if (isShowImageFile)
                        {
                            filterList.Add(file);
                        }
                    }
                    else
                    {
                        if (isShowOtherFile)
                        {
                            filterList.Add(file);
                        }
                    }
                }
                else
                {
                    if (isShowDirectory)
                    {
                        filterList.Add(file);
                    }
                }
            }

            bool isAscending = _sortInfo.IsAscending(_sortInfo.ActiveSortType);
            switch (_sortInfo.ActiveSortType)
            {
                case SortTypeID.FileName:
                    filterList.Sort((x, y) =>
                    {
                        if (isAscending)
                        {
                            return x.FileName.CompareTo(y.FileName);
                        }
                        else
                        {
                            return -x.FileName.CompareTo(y.FileName);
                        }
                    });
                    break;
                case SortTypeID.FilePath:
                    filterList.Sort((x, y) =>
                    {
                        if (isAscending)
                        {
                            return x.FilePath.CompareTo(y.FilePath);
                        }
                        else
                        {
                            return -x.FilePath.CompareTo(y.FilePath);
                        }
                    });
                    break;
                case SortTypeID.UpdateDate:
                    filterList.Sort((x, y) =>
                    {
                        if (isAscending)
                        {
                            return x.UpdateDate.GetValueOrDefault(DateTime.MinValue).CompareTo(y.UpdateDate.GetValueOrDefault(DateTime.MinValue));
                        }
                        else
                        {
                            return -x.UpdateDate.GetValueOrDefault(DateTime.MinValue).CompareTo(y.UpdateDate.GetValueOrDefault(DateTime.MinValue));
                        }
                    });
                    break;
                case SortTypeID.CreateDate:
                    filterList.Sort((x, y) =>
                    {
                        if (isAscending)
                        {
                            return x.CreateDate.GetValueOrDefault(DateTime.MinValue).CompareTo(y.CreateDate.GetValueOrDefault(DateTime.MinValue));
                        }
                        else
                        {
                            return -x.CreateDate.GetValueOrDefault(DateTime.MinValue).CompareTo(y.CreateDate.GetValueOrDefault(DateTime.MinValue));
                        }
                    });
                    break;
                case SortTypeID.RgistrationDate:
                    filterList.Sort((x, y) =>
                    {
                        if (isAscending)
                        {
                            return x.RgistrationDate.GetValueOrDefault(DateTime.MinValue).CompareTo(y.RgistrationDate.GetValueOrDefault(DateTime.MinValue));
                        }
                        else
                        {
                            return -x.RgistrationDate.GetValueOrDefault(DateTime.MinValue).CompareTo(y.RgistrationDate.GetValueOrDefault(DateTime.MinValue));
                        }
                    });
                    break;
                default:
                    break;
            }

            flowList.BeginUpdate();

            try
            {
                _filterFilePathList = new List<string>(filterList.ConvertAll(f => f.FilePath));
                flowList.ItemCount = filterList.Count;
                FileEntity selectedFile = filterList.FirstOrDefault(f => f.FilePath.Equals(_selectedFilePath, StringComparison.Ordinal));
                if (selectedFile != null)
                {
                    flowList.SelectItem(filterList.IndexOf(selectedFile));
                }
                else if (flowList.ItemCount > 0)
                {
                    flowList.SelectItem(0);
                }
            }
            finally
            {
                flowList.EndUpdate();
            }
        }

        private void changeFileNameVisible()
        {
            FileListContentsConfig.IsShowFileName = isShowFileName;
            flowList.BeginUpdate();
            try
            {
                setFlowListItemSize();
            }
            finally
            {
                flowList.EndUpdate();
            }
        }

        private void setFlowListItemSize()
        {
            if (isShowFileName)
            {
                flowList.SetItemSize(thumbnailSize, thumbnailSize + itemTextHeight);
            }
            else
            {
                flowList.SetItemSize(thumbnailSize, thumbnailSize);
            }
        }

        private IList<string> getImageFiles()
        {
            var imageFiles = from file in _filterFilePathList
                             where _masterFileDictionary.ContainsKey(file) && _masterFileDictionary[file].IsImageFile
                             select file;
            return imageFiles.ToArray();
        }

        private void addKeep(IList<KeepFileEntity> filePathList)
        {
            ListEntity<KeepFileEntity> param = new ListEntity<KeepFileEntity>(filePathList);
            addKeepProcess.Execute(this, param);
        }

        private void drawItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(flowList.SelectedItemBrush, e.ItemRectangle);
            }

            if (e.IsFocus)
            {
                e.Graphics.FillRectangle(flowList.FocusItemBrush, e.ItemRectangle);
            }

            if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(flowList.MousePointItemBrush, e.ItemRectangle);
            }

            string filePath = _filterFilePathList[e.ItemIndex];
            FileEntity item = _masterFileDictionary[filePath];

            if (item.ThumbnailImage == null)
            {
                ThumbnailUtil.DrawIcon(e.Graphics, item.Icon, getIconRectangle(e));
                e.Graphics.DrawString(item.FileName, this.Font, getTextBrush(e), getTextRectangle(e), flowList.ItemTextFormat);
            }
            else
            {
                if (item.IsFile)
                {
                    Rectangle thumbRect = getThumbnailRectangle(e);
                    if (item.ThumbnailWidth == thumbRect.Width && item.ThumbnailHeight == thumbRect.Height)
                    {
                        ThumbnailUtil.DrawFileThumbnail(e.Graphics, item.ThumbnailImage, thumbRect);
                    }
                    else
                    {
                        ThumbnailUtil.AdjustDrawFileThumbnail(e.Graphics, item.ThumbnailImage, thumbRect);
                    }
                }
                else
                {
                    Rectangle thumbRect = getThumbnailRectangle(e);
                    if (item.ThumbnailWidth == thumbRect.Width && item.ThumbnailHeight == thumbRect.Height)
                    {
                        ThumbnailUtil.DrawDirectoryThumbnail(e.Graphics, item.ThumbnailImage, thumbRect, item.Icon);
                    }
                    else
                    {
                        ThumbnailUtil.AdjustDrawDirectoryThumbnail(e.Graphics, item.ThumbnailImage, thumbRect, item.Icon);
                    }
                }

                if (isShowFileName)
                {
                    e.Graphics.DrawString(item.FileName, this.Font, getTextBrush(e), getTextRectangle(e), flowList.ItemTextFormat);
                }
            }
        }

        private Rectangle getIconRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            return new Rectangle(e.ItemRectangle.X,
                                 e.ItemRectangle.Y,
                                 e.ItemRectangle.Width,
                                 e.ItemRectangle.Height - itemTextHeight);
        }

        private Rectangle getThumbnailRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (isShowFileName)
            {
                return new Rectangle(e.ItemRectangle.X,
                                     e.ItemRectangle.Y,
                                     e.ItemRectangle.Width,
                                     e.ItemRectangle.Height - itemTextHeight);
            }
            else
            {
                return e.ItemRectangle;
            }
        }

        private Rectangle getTextRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            return new Rectangle(e.ItemRectangle.X,
                                 e.ItemRectangle.Bottom - itemTextHeight,
                                 e.ItemRectangle.Width,
                                 itemTextHeight);
        }

        private SolidBrush getTextBrush(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsSelected)
            {
                return _selectedTextBrush;
            }
            else
            {
                return flowList.ItemTextBrush;
            }
        }

        #endregion

        #region プロセスイベント

        private void getThumbnailsProcess_Callback(object sender, ThumbnailImageEntity e)
        {
            if (_masterFileDictionary == null || !_masterFileDictionary.ContainsKey(e.FilePath))
            {
                return;
            }

            FileEntity file = _masterFileDictionary[e.FilePath];

            if (file.ThumbnailImage != null)
            {
                if (e.ThumbnailWidth != file.ThumbnailWidth ||
                    e.ThumbnailHeight != file.ThumbnailHeight ||
                    e.FileUpdatedate > file.UpdateDate)
                {
                    file.ThumbnailImage.Dispose();
                    file.ThumbnailImage = null;
                    file.ThumbnailImage = e.ThumbnailImage;
                    file.ThumbnailWidth = e.ThumbnailWidth;
                    file.ThumbnailHeight = e.ThumbnailHeight;
                    file.SourceImageWidth = e.SourceWidth;
                    file.SourceImageHeight = e.SourceHeight;
                    file.UpdateDate = e.FileUpdatedate;
                }
                else
                {
                    return;
                }
            }
            else
            {
                file.ThumbnailImage = e.ThumbnailImage;
                file.ThumbnailWidth = e.ThumbnailWidth;
                file.ThumbnailHeight = e.ThumbnailHeight;
                file.SourceImageWidth = e.SourceWidth;
                file.SourceImageHeight = e.SourceHeight;
                file.UpdateDate = e.FileUpdatedate;
            }

            if (_filterFilePathList != null)
            {
                int index = _filterFilePathList.IndexOf(file.FilePath);
                if (index > -1)
                {
                    flowList.InvalidateFromItemIndex(index);
                }
            }
        }

        #endregion

        #region ツールバーイベント

        private void showDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isShowDirectory = !isShowDirectory;
            setFilter();
        }

        private void showImageFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isShowImageFile = !isShowImageFile;
            setFilter();
        }

        private void showOtherFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isShowOtherFile = !isShowOtherFile;
            setFilter();
        }

        private void showFileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isShowFileName = !isShowFileName;
            changeFileNameVisible();
        }

        private void sortFileNameToolStripButton_Click(object sender, EventArgs e)
        {
            _sortInfo.ChangeSortDirection(SortTypeID.FileName);
            _sortInfo.ActiveSortType = SortTypeID.FileName;
            setSort();
            setFilter();
        }

        private void sortFilePathToolStripButton_Click(object sender, EventArgs e)
        {
            _sortInfo.ChangeSortDirection(SortTypeID.FilePath);
            _sortInfo.ActiveSortType = SortTypeID.FilePath;
            setSort();
            setFilter();
        }

        private void sortFileUpdateDateToolStripButton_Click(object sender, EventArgs e)
        {
            _sortInfo.ChangeSortDirection(SortTypeID.UpdateDate);
            _sortInfo.ActiveSortType = SortTypeID.UpdateDate;
            setSort();
            setFilter();
        }

        private void sortFileCreateDateToolStripButton_Click(object sender, EventArgs e)
        {
            _sortInfo.ChangeSortDirection(SortTypeID.CreateDate);
            _sortInfo.ActiveSortType = SortTypeID.CreateDate;
            setSort();
            setFilter();
        }

        private void sortFilerRgistrationDateToolStripButton_Click(object sender, EventArgs e)
        {
            _sortInfo.ChangeSortDirection(SortTypeID.RgistrationDate);
            _sortInfo.ActiveSortType = SortTypeID.RgistrationDate;
            setSort();
            setFilter();
        }

        private void thumbnailSizeToolStripSlider_BeginValueChange(object sender, EventArgs e)
        {
            setFlowListItemSize();
        }

        private void thumbnailSizeToolStripSlider_ValueChanged(object sender, EventArgs e)
        {
            setFlowListItemSize();
        }

        private void thumbnailSizeToolStripSlider_ValueChanging(object sender, EventArgs e)
        {
            FileListContentsConfig.ThumbnailSize = thumbnailSize;
            setFlowListItemSize();
            flowList.Refresh();
        }

        private void movePreviewToolStripButton_Click(object sender, EventArgs e)
        {
            OnMovePreviewButtonClick(e);
        }

        private void moveNextToolStripButton_Click(object sender, EventArgs e)
        {
            OnMoveNextButtonClick(e);
        }

        #endregion

        #region フローリストイベント

        private void flowList_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
        }

        private void flowList_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        private void flowList_Drawitem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (_filterFilePathList == null)
            {
                return;
            }

            drawItem(e);
        }

        private void flowList_DrawItemChanged(object sender, SWF.UIComponent.FlowList.DrawItemChangedEventArgs e)
        {
            if (_filterFilePathList == null)
            {
                return;
            }

            if (_filterFilePathList.Count > 0 &&
                e.DrawFirstItemIndex > -1 &&
                e.DrawLastItemIndex > -1 &&
                e.DrawLastItemIndex < _filterFilePathList.Count)
            {
                GetThumbnailParameter param = new GetThumbnailParameter();
                param.FilePathList = _filterFilePathList;
                param.FirstIndex = e.DrawFirstItemIndex;
                param.LastIndex = e.DrawLastItemIndex;
                param.ThumbnailWidth = flowList.ItemWidth - flowList.ItemSpace * 2;
                if (isShowFileName)
                {
                    param.ThumbnailHeight = flowList.ItemHeight - flowList.ItemSpace * 2 - itemTextHeight;
                }
                else
                {
                    param.ThumbnailHeight = flowList.ItemHeight - flowList.ItemSpace * 2;
                }
                getThumbnailsProcess.Cancel();
                getThumbnailsProcess.Execute(this, param);
            }
        }

        private void flowList_SelectedItemChange(object sender, EventArgs e)
        {
            IList<string> filePathList = GetSelectedFiles();
            if (filePathList.Count > 0)
            {
                _selectedFilePath = filePathList.First();
            }

            OnSelectedFileChanged(new SelectedFileChangeEventArgs(filePathList));
        }

        private void flowList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle)
            {
                return;
            }

            string filePath = _filterFilePathList[flowList.GetSelectedIndexs()[0]];
            FileEntity file = _masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewerContentsParameter(
                    this.Parameter.ContentsSources,
                    this.Parameter.SourcesKey,
                    getImageFiles(),
                    file.FilePath,
                    this.Title,
                    this.Icon);
                OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.AddTab, param));
            }
            else if (!file.IsFile)
            {
                DirectoryFileListContentsParameter param = new DirectoryFileListContentsParameter(file.FilePath);
                OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.AddTab, param));
            }
        }

        private void flowList_ItemMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            string filePath = _filterFilePathList[flowList.GetSelectedIndexs()[0]];
            FileEntity file = _masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewerContentsParameter(
                    this.Parameter.ContentsSources,
                    this.Parameter.SourcesKey,
                    getImageFiles(),
                    file.FilePath,
                    this.Title,
                    this.Icon);
                OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
            }
            else if (!file.IsFile)
            {
                DirectoryFileListContentsParameter param = new DirectoryFileListContentsParameter(file.FilePath);
                OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
            }
            else
            {
                FileUtil.OpenFile(file.FilePath);
            }
        }

        protected virtual void flowLilst_BackgroundMouseClick(object sender, MouseEventArgs e)
        {
            this.OnBackgroundMouseClick(e);
        }

        private void flowList_ItemExecute(object sender, EventArgs e)
        {
            string filePath = _filterFilePathList[flowList.GetSelectedIndexs()[0]];
            FileEntity file = _masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewerContentsParameter(
                    this.Parameter.ContentsSources,
                    this.Parameter.SourcesKey,
                    getImageFiles(),
                    file.FilePath,
                    this.Title,
                    this.Icon);
                OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
            }
            else if (!file.IsFile)
            {
                DirectoryFileListContentsParameter param = new DirectoryFileListContentsParameter(file.FilePath);
                OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
            }
            else
            {
                FileUtil.OpenFile(file.FilePath);
            }
        }

        private void flowList_ItemDelete(object sender, EventArgs e)
        {
            List<string> filePathList = new List<string>();
            foreach (int index in flowList.GetSelectedIndexs())
            {
                filePathList.Add(_filterFilePathList[index]);
            }

            OnRemoveFile(filePathList);
        }

        private void flowList_DragStart(object sender, EventArgs e)
        {
            IList<int> selectedIndexList = flowList.GetSelectedIndexs();
            if (selectedIndexList.Count < 1)
            {
                return;
            }

            string currentFilePath = _filterFilePathList[selectedIndexList.First()];
            FileEntity currentFileInfo = _masterFileDictionary[currentFilePath];
            if (currentFileInfo.IsFile && currentFileInfo.IsImageFile)
            {
                // 選択項目が画像ファイルの場合。
                List<string> filePathList = new List<string>();
                foreach (string filePath in _filterFilePathList)
                {
                    FileEntity fileInfo = _masterFileDictionary[filePath];
                    if (fileInfo.IsFile && fileInfo.IsImageFile)
                    {
                        filePathList.Add(filePath);
                    }
                }

                if (filePathList.Count > 0)
                {
                    var dragData = new DragEntity(
                        this.Parameter.ContentsSources,
                        this.Parameter.SourcesKey,
                        currentFilePath,
                        filePathList,
                        this.Title,
                        this.Icon);
                    this.DoDragDrop(dragData, DragDropEffects.All);
                }
            }
            else if (!currentFileInfo.IsFile)
            {
                // 選択項目がフォルダの場合。
                var dragData = new DragEntity(
                    this.Parameter.ContentsSources,
                    this.Parameter.SourcesKey,
                    currentFilePath,
                    this.Title,
                    this.Icon);
                this.DoDragDrop(dragData, DragDropEffects.All);
            }
        }

        #endregion

        #region コンテキストメニューイベント

        protected virtual void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            IList<string> filePathList = GetSelectedFiles();
            if (filePathList.Count > 0)
            {
                fileContextMenu.SetFile(filePathList);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void fileContextMenu_FileActiveTabOpen(object sender, PicSum.UIComponent.Common.ExecuteFileEventArgs e)
        {
            var param = new ImageViewerContentsParameter(
                this.Parameter.ContentsSources,
                this.Parameter.SourcesKey,
                getImageFiles(),
                e.FilePath,
                this.Title,
                this.Icon);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        private void fileContextMenu_FileNewTabOpen(object sender, PicSum.UIComponent.Common.ExecuteFileEventArgs e)
        {
            var param = new ImageViewerContentsParameter(
                this.Parameter.ContentsSources,
                this.Parameter.SourcesKey,
                getImageFiles(),
                e.FilePath,
                this.Title,
                this.Icon);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.AddTab, param));
        }

        private void fileContextMenu_FileNewWindowOpen(object sender, PicSum.UIComponent.Common.ExecuteFileEventArgs e)
        {
            var param = new ImageViewerContentsParameter(
                this.Parameter.ContentsSources,
                this.Parameter.SourcesKey,
                getImageFiles(),
                e.FilePath,
                this.Title,
                this.Icon);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.NewWindow, param));
        }

        private void fileContextMenu_FileOpen(object sender, PicSum.UIComponent.Common.ExecuteFileEventArgs e)
        {
            FileUtil.OpenFile(e.FilePath);
        }

        private void fileContextMenu_SaveDirectoryOpen(object sender, PicSum.UIComponent.Common.ExecuteFileEventArgs e)
        {
            FileUtil.OpenExplorerSelect(e.FilePath);
        }

        private void fileContextMenu_DirectoryActiveTabOpen(object sender, PicSum.UIComponent.Common.ExecuteFileEventArgs e)
        {
            DirectoryFileListContentsParameter param = new DirectoryFileListContentsParameter(e.FilePath);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        private void fileContextMenu_DirectoryNewTabOpen(object sender, PicSum.UIComponent.Common.ExecuteFileEventArgs e)
        {
            DirectoryFileListContentsParameter param = new DirectoryFileListContentsParameter(e.FilePath);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.AddTab, param));
        }

        private void fileContextMenu_DirectoryNewWindowOpen(object sender, PicSum.UIComponent.Common.ExecuteFileEventArgs e)
        {
            DirectoryFileListContentsParameter param = new DirectoryFileListContentsParameter(e.FilePath);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.NewWindow, param));
        }

        private void fileContextMenu_ExplorerOpen(object sender, PicSum.UIComponent.Common.ExecuteFileEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FilePath))
            {
                FileUtil.OpenMyComputer();
            }
            else
            {
                FileUtil.OpenExplorer(e.FilePath);
            }
        }

        private void fileContextMenu_Export(object sender, PicSum.UIComponent.Common.ExecuteFileListEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (FileUtil.IsExists(CommonConfig.ExportDirectoryPath))
                {
                    fbd.SelectedPath = CommonConfig.ExportDirectoryPath;
                }

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ExportFileParameter param = new ExportFileParameter();
                    param.ExportDirectoryPath = fbd.SelectedPath;
                    param.FilePathList = e.FilePathList;
                    exportFileProcess.Execute(this, param);

                    CommonConfig.ExportDirectoryPath = fbd.SelectedPath;
                }
            }
        }

        private void fileContextMenu_PathCopy(object sender, PicSum.UIComponent.Common.ExecuteFileListEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string filePath in e.FilePathList)
            {
                sb.AppendLine(filePath);
            }

            Clipboard.SetText(sb.ToString());
        }

        private void fileContextMenu_NameCopy(object sender, PicSum.UIComponent.Common.ExecuteFileListEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string filePath in e.FilePathList)
            {
                sb.AppendLine(FileUtil.GetFileName(filePath));
            }

            Clipboard.SetText(sb.ToString());
        }

        private void fileContextMenu_AddKeep(object sender, PicSum.UIComponent.Common.ExecuteFileListEventArgs e)
        {
            addKeep(e.FilePathList.Select(filePath => new KeepFileEntity(filePath, DateTime.Now)).ToList());
        }

        private void fileContextMenu_RemoveFromList(object sender, PicSum.UIComponent.Common.ExecuteFileListEventArgs e)
        {
            OnRemoveFile(e.FilePathList);
        }

        #endregion
    }
}
