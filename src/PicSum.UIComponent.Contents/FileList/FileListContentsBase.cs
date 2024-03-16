using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.ContextMenu;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Common;
using SWF.UIComponent.FlowList;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// ファイルリストコンテンツ基底クラス
    /// </summary>
    internal abstract partial class FileListContentsBase
        : BrowserContents
    {
        #region インスタンス変数

        private Dictionary<string, FileEntity> masterFileDictionary = null;
        private List<string> filterFilePathList = null;
        private readonly SortInfo sortInfo = new SortInfo();
        private TwoWayProcess<GetThumbnailsAsyncFacade, GetThumbnailParameter, ThumbnailImageEntity> getThumbnailsProcess = null;
        private OneWayProcess<ExportFileAsyncFacade, ExportFileParameter> exportFileProcess = null;
        private OneWayProcess<AddBookmarkAsyncFacade, SingleValueEntity<string>> addBookmarkProcess = null;

        #endregion

        #region パブリックプロパティ

        public override string SelectedFilePath { get; protected set; } = FileUtil.ROOT_DIRECTORY_PATH;

        #endregion

        #region 継承プロパティ

        protected bool IsAscending
        {
            get
            {
                return this.sortInfo.IsAscending(this.sortInfo.ActiveSortType);
            }
        }

        protected SortTypeID SortTypeID
        {
            get
            {
                return this.sortInfo.ActiveSortType;
            }
        }

        protected bool IsRemoveFromListMenuItemVisible
        {
            get
            {
                return this.fileContextMenu.VisibleRemoveFromListMenuItem;
            }
            set
            {
                this.fileContextMenu.VisibleRemoveFromListMenuItem = value;
            }
        }

        protected bool IsMoveControlVisible
        {
            get
            {
                return this.movePreviewToolStripButton.Visible;
            }
            set
            {
                this.movePreviewToolStripButton.Visible = value;
                this.moveNextToolStripButton.Visible = value;
            }
        }

        protected bool IsBookmarkMenuItem
        {
            get
            {
                return this.fileContextMenu.VisibleBookmarkMenuItem;
            }
            set
            {
                this.fileContextMenu.VisibleBookmarkMenuItem = value;
            }
        }

        protected bool IsDirectoryActiveTabOpenMenuItemVisible
        {
            get
            {
                return this.fileContextMenu.VisibleDirectoryActiveTabOpenMenuItem;
            }
            set
            {
                this.fileContextMenu.VisibleDirectoryActiveTabOpenMenuItem = value;
            }
        }

        #endregion

        #region プライベートプロパティ

        private bool IsShowDirectory
        {
            get
            {
                return this.showDirectoryToolStripMenuItem.Checked;
            }
            set
            {
                this.showDirectoryToolStripMenuItem.Checked = value;
            }
        }

        private bool IsShowImageFile
        {
            get
            {
                return this.showImageFileToolStripMenuItem.Checked;
            }
            set
            {
                this.showImageFileToolStripMenuItem.Checked = value;
            }
        }

        private bool IsShowOtherFile
        {
            get
            {
                return this.showOtherFileToolStripMenuItem.Checked;
            }
            set
            {
                this.showOtherFileToolStripMenuItem.Checked = value;
            }
        }

        private bool IsShowFileName
        {
            get
            {
                return this.showFileNameToolStripMenuItem.Checked;
            }
            set
            {
                this.showFileNameToolStripMenuItem.Checked = value;
            }
        }

        private int ThumbnailSize
        {
            get
            {
                return this.thumbnailSizeToolStripSlider.Value;
            }
            set
            {
                this.thumbnailSizeToolStripSlider.Value = value;
            }
        }

        private TwoWayProcess<GetThumbnailsAsyncFacade, GetThumbnailParameter, ThumbnailImageEntity> GetThumbnailsProcess
        {
            get
            {
                if (this.getThumbnailsProcess == null)
                {
                    this.getThumbnailsProcess = TaskManager.CreateTwoWayProcess<GetThumbnailsAsyncFacade, GetThumbnailParameter, ThumbnailImageEntity>(this.ProcessContainer);
                    this.getThumbnailsProcess.Callback += new AsyncTaskCallbackEventHandler<ThumbnailImageEntity>(this.GetThumbnailsProcess_Callback);

                }

                return this.getThumbnailsProcess;
            }
        }

        private OneWayProcess<ExportFileAsyncFacade, ExportFileParameter> ExportFileProcess
        {
            get
            {
                if (this.exportFileProcess == null)
                {
                    this.exportFileProcess = TaskManager.CreateOneWayProcess<ExportFileAsyncFacade, ExportFileParameter>(this.ProcessContainer);
                }

                return this.exportFileProcess;
            }
        }

        private OneWayProcess<AddBookmarkAsyncFacade, SingleValueEntity<string>> AddBookmarkProcess
        {
            get
            {
                if (this.addBookmarkProcess == null)
                {
                    this.addBookmarkProcess = TaskManager.CreateOneWayProcess<AddBookmarkAsyncFacade, SingleValueEntity<string>>(this.ProcessContainer);
                }

                return this.addBookmarkProcess;
            }
        }

        private int ItemTextHeight
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
            this.InitializeComponent();
            this.SubInitializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public override void RedrawContents()
        {
            this.flowList.Refresh();
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.GetThumbnailsProcess.Cancel();
            base.OnInvalidated(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.flowList.MouseWheelProcess(e);
            base.OnMouseWheel(e);
        }

        protected abstract void OnRemoveFile(IList<string> filePathList);

        protected abstract void OnMovePreviewButtonClick(EventArgs e);

        protected abstract void OnMoveNextButtonClick(EventArgs e);

        protected void SetFiles(IList<FileShallowInfoEntity> srcFiles, string selectedFilePath, SortTypeID sortTypeID, bool isAscending)
        {
            if (srcFiles == null)
            {
                throw new ArgumentNullException(nameof(srcFiles));
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException(nameof(selectedFilePath));
            }

            this.masterFileDictionary = new Dictionary<string, FileEntity>();
            foreach (var srcFile in srcFiles)
            {
                var destFile = new FileEntity();
                destFile.FilePath = srcFile.FilePath;
                destFile.FileName = srcFile.FileName;
                destFile.UpdateDate = srcFile.UpdateDate;
                destFile.CreateDate = srcFile.CreateDate;
                destFile.RgistrationDate = srcFile.RgistrationDate;
                destFile.Icon = srcFile.LargeIcon;
                destFile.IsFile = srcFile.IsFile;
                destFile.IsImageFile = srcFile.IsImageFile;
                this.masterFileDictionary.Add(destFile.FilePath, destFile);
            }

            this.SelectedFilePath = selectedFilePath;
            this.sortInfo.SetSortType(sortTypeID, isAscending);

            this.SetSort();
            this.SetFilter();

            this.flowList.Focus();
        }

        protected void SetFile(IList<FileShallowInfoEntity> srcFiles, string selectedFilePath)
        {
            if (srcFiles == null)
            {
                throw new ArgumentNullException(nameof(srcFiles));
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException(nameof(selectedFilePath));
            }

            this.SetFiles(srcFiles, selectedFilePath, SortTypeID.Default, false);
        }

        protected IList<string> GetSelectedFiles()
        {
            var filePathList = new List<string>();
            var selectedIndexs = this.flowList.GetSelectedIndexs();
            if (selectedIndexs.Count > 0)
            {
                foreach (var index in selectedIndexs)
                {
                    filePathList.Add(this.filterFilePathList[index]);
                }
            }

            return filePathList;
        }

        protected void SetContextMenuFiles(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException(nameof(filePathList));
            }

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("0件のファイルリストはセットできません。", nameof(filePathList));
            }

            this.fileContextMenu.SetFile(filePathList);
        }

        protected void SetContextMenuFiles(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            this.SetContextMenuFiles(new List<string>() { filePath });
        }

        protected void RemoveFile(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException(nameof(filePathList));
            }

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("削除するファイルパスが0件です。", nameof(filePathList));
            }

            var indexList = new List<int>();
            foreach (var filePath in filePathList)
            {
                this.masterFileDictionary.Remove(filePath);
                indexList.Add(this.filterFilePathList.IndexOf(filePath));
            }

            if (indexList.Count > 0)
            {
                var maximumIndex = indexList.Max();
                if (maximumIndex + 1 < this.filterFilePathList.Count)
                {
                    this.SelectedFilePath = this.filterFilePathList[maximumIndex + 1];
                }
                else
                {
                    var minimumIndex = indexList.Min();
                    if (minimumIndex - 1 > -1)
                    {
                        this.SelectedFilePath = this.filterFilePathList[minimumIndex - 1];
                    }
                    else
                    {
                        this.SelectedFilePath = string.Empty;
                    }
                }
            }
            else
            {
                this.SelectedFilePath = string.Empty;
            }

            this.SetFilter();
        }

        protected IEnumerable<FileShallowInfoEntity> GetSortFiles(IEnumerable<FileShallowInfoEntity> files)
        {
            if (files == null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            var isAscending = this.sortInfo.IsAscending(this.sortInfo.ActiveSortType);
            switch (this.sortInfo.ActiveSortType)
            {
                case SortTypeID.FileName:
                    if (isAscending)
                    {
                        return files.OrderBy(file => file.FileName);
                    }
                    else
                    {
                        return files.OrderByDescending(file => file.FileName);
                    }
                case SortTypeID.FilePath:
                    if (isAscending)
                    {
                        return files.OrderBy(file => file.FilePath);
                    }
                    else
                    {
                        return files.OrderByDescending(file => file.FilePath);
                    }
                case SortTypeID.UpdateDate:
                    if (isAscending)
                    {
                        return files.OrderBy(file => file.UpdateDate.GetValueOrDefault(DateTime.MinValue));
                    }
                    else
                    {
                        return files.OrderByDescending(file => file.UpdateDate.GetValueOrDefault(DateTime.MinValue));
                    }
                case SortTypeID.CreateDate:
                    if (isAscending)
                    {
                        return files.OrderBy(file => file.CreateDate.GetValueOrDefault(DateTime.MinValue));
                    }
                    else
                    {
                        return files.OrderByDescending(file => file.CreateDate.GetValueOrDefault(DateTime.MinValue));
                    }
                case SortTypeID.RgistrationDate:
                    if (isAscending)
                    {
                        return files.OrderBy(file => file.RgistrationDate.GetValueOrDefault(DateTime.MinValue));
                    }
                    else
                    {
                        return files.OrderByDescending(file => file.RgistrationDate.GetValueOrDefault(DateTime.MinValue));
                    }
                default:
                    return files;
            }
        }

        protected abstract Action GetImageFilesAction(ImageViewerContentsParameter paramter);

        #endregion

        #region プライベートメソッド

        private void SubInitializeComponent()
        {
            this.Font = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
            this.IsShowFileName = FileListContentsConfig.IsShowFileName;
            this.IsShowDirectory = FileListContentsConfig.IsShowDirectory;
            this.IsShowImageFile = FileListContentsConfig.IsShowImageFile;
            this.IsShowOtherFile = FileListContentsConfig.IsShowOtherFile;
            this.ThumbnailSize = FileListContentsConfig.ThumbnailSize;
            this.SetFlowListItemSize();
        }

        private ToolStripButton GetSortToolStripButton(SortTypeID sortType)
        {
            switch (sortType)
            {
                case SortTypeID.FileName:
                    return this.sortFileNameToolStripButton;
                case SortTypeID.FilePath:
                    return this.sortFilePathToolStripButton;
                case SortTypeID.UpdateDate:
                    return this.sortFileUpdateDateToolStripButton;
                case SortTypeID.CreateDate:
                    return this.sortFileCreateDateToolStripButton;
                case SortTypeID.RgistrationDate:
                    return this.sortFileRgistrationDateToolStripButton;
                default:
                    return null;
            }
        }

        private void SetSort()
        {
            this.sortFileNameToolStripButton.Image = null;
            this.sortFilePathToolStripButton.Image = null;
            this.sortFileUpdateDateToolStripButton.Image = null;
            this.sortFileCreateDateToolStripButton.Image = null;
            this.sortFileRgistrationDateToolStripButton.Image = null;

            var sortButton = this.GetSortToolStripButton(this.sortInfo.ActiveSortType);
            if (sortButton != null)
            {
                var isAscending = this.sortInfo.IsAscending(this.sortInfo.ActiveSortType);
                sortButton.Image = this.sortInfo.GetSortDirectionImage(isAscending);
            }
        }

        private void SetFilter()
        {
            FileListContentsConfig.IsShowDirectory = this.IsShowDirectory;
            FileListContentsConfig.IsShowImageFile = this.IsShowImageFile;
            FileListContentsConfig.IsShowOtherFile = this.IsShowOtherFile;

            if (this.masterFileDictionary == null)
            {
                return;
            }

            var filterList = new List<FileEntity>();
            foreach (var file in this.masterFileDictionary.Values)
            {
                if (file.IsFile)
                {
                    if (file.IsImageFile)
                    {
                        if (this.IsShowImageFile)
                        {
                            filterList.Add(file);
                        }
                    }
                    else
                    {
                        if (this.IsShowOtherFile)
                        {
                            filterList.Add(file);
                        }
                    }
                }
                else
                {
                    if (this.IsShowDirectory)
                    {
                        filterList.Add(file);
                    }
                }
            }

            var isAscending = this.sortInfo.IsAscending(this.sortInfo.ActiveSortType);
            switch (this.sortInfo.ActiveSortType)
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

            this.flowList.BeginUpdate();

            try
            {
                this.filterFilePathList = new List<string>(filterList.ConvertAll(f => f.FilePath));
                this.flowList.ItemCount = filterList.Count;
                var selectedFile = filterList.FirstOrDefault(f => f.FilePath.Equals(this.SelectedFilePath, StringComparison.Ordinal));
                if (selectedFile != null)
                {
                    this.flowList.SelectItem(filterList.IndexOf(selectedFile));
                }
                else if (this.flowList.ItemCount > 0)
                {
                    this.flowList.SelectItem(0);
                }
            }
            finally
            {
                this.flowList.EndUpdate();
            }
        }

        private void ChangeFileNameVisible()
        {
            FileListContentsConfig.IsShowFileName = this.IsShowFileName;
            this.flowList.BeginUpdate();
            try
            {
                this.SetFlowListItemSize();
            }
            finally
            {
                this.flowList.EndUpdate();
            }
        }

        private void SetFlowListItemSize()
        {
            if (this.IsShowFileName)
            {
                this.flowList.SetItemSize(this.ThumbnailSize, this.ThumbnailSize + this.ItemTextHeight);
            }
            else
            {
                this.flowList.SetItemSize(this.ThumbnailSize, this.ThumbnailSize);
            }
        }

        private IList<string> GetImageFiles()
        {
            return this.filterFilePathList
                .Where(item =>
                {
                    if (this.masterFileDictionary.TryGetValue(item, out var file))
                    {
                        return file.IsImageFile;
                    }
                    else
                    {
                        return false;
                    }
                })
                .ToArray();
        }

        private void DrawItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(this.flowList.SelectedItemBrush, e.ItemRectangle);
                e.Graphics.DrawRectangle(this.flowList.SelectedItemPen, e.ItemRectangle);
            }

            if (e.IsFocus)
            {
                e.Graphics.FillRectangle(this.flowList.FocusItemBrush, e.ItemRectangle);
            }

            if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(this.flowList.MousePointItemBrush, e.ItemRectangle);
            }

            var filePath = this.filterFilePathList[e.ItemIndex];
            var item = this.masterFileDictionary[filePath];

            if (item.ThumbnailImage == null)
            {
                ThumbnailUtil.DrawIcon(e.Graphics, item.Icon, this.GetIconRectangle(e));
                e.Graphics.DrawString(item.FileName, this.Font, this.flowList.ItemTextBrush, this.GetTextRectangle(e), this.flowList.ItemTextFormat);
            }
            else
            {
                if (item.IsFile)
                {
                    var thumbRect = this.GetThumbnailRectangle(e);
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
                    var thumbRect = this.GetThumbnailRectangle(e);
                    if (item.ThumbnailWidth == thumbRect.Width && item.ThumbnailHeight == thumbRect.Height)
                    {
                        ThumbnailUtil.DrawDirectoryThumbnail(e.Graphics, item.ThumbnailImage, thumbRect, item.Icon);
                    }
                    else
                    {
                        ThumbnailUtil.AdjustDrawDirectoryThumbnail(e.Graphics, item.ThumbnailImage, thumbRect, item.Icon);
                    }
                }

                if (this.IsShowFileName)
                {
                    e.Graphics.DrawString(item.FileName, this.Font, this.flowList.ItemTextBrush, this.GetTextRectangle(e), this.flowList.ItemTextFormat);
                }
            }
        }

        private Rectangle GetIconRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            return new Rectangle(e.ItemRectangle.X,
                                 e.ItemRectangle.Y,
                                 e.ItemRectangle.Width,
                                 e.ItemRectangle.Height - this.ItemTextHeight);
        }

        private Rectangle GetThumbnailRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.IsShowFileName)
            {
                return new Rectangle(e.ItemRectangle.X,
                                     e.ItemRectangle.Y,
                                     e.ItemRectangle.Width,
                                     e.ItemRectangle.Height - this.ItemTextHeight);
            }
            else
            {
                return e.ItemRectangle;
            }
        }

        private Rectangle GetTextRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            return new Rectangle(e.ItemRectangle.X,
                                 e.ItemRectangle.Bottom - this.ItemTextHeight,
                                 e.ItemRectangle.Width,
                                 this.ItemTextHeight);
        }

        #endregion

        #region プロセスイベント

        private void GetThumbnailsProcess_Callback(object sender, ThumbnailImageEntity e)
        {
            if (this.masterFileDictionary == null || !this.masterFileDictionary.ContainsKey(e.FilePath))
            {
                return;
            }

            var file = this.masterFileDictionary[e.FilePath];

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

            if (this.filterFilePathList != null)
            {
                var index = this.filterFilePathList.IndexOf(file.FilePath);
                if (index > -1)
                {
                    this.flowList.InvalidateFromItemIndex(index);
                }
            }
        }

        #endregion

        #region ツールバーイベント

        private void ShowDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.IsShowDirectory = !this.IsShowDirectory;
            this.SetFilter();
        }

        private void ShowImageFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.IsShowImageFile = !this.IsShowImageFile;
            this.SetFilter();
        }

        private void ShowOtherFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.IsShowOtherFile = !this.IsShowOtherFile;
            this.SetFilter();
        }

        private void ShowFileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.IsShowFileName = !this.IsShowFileName;
            this.ChangeFileNameVisible();
        }

        private void SortFileNameToolStripButton_Click(object sender, EventArgs e)
        {
            this.sortInfo.ChangeSortDirection(SortTypeID.FileName);
            this.sortInfo.ActiveSortType = SortTypeID.FileName;
            this.SetSort();
            this.SetFilter();
        }

        private void SortFilePathToolStripButton_Click(object sender, EventArgs e)
        {
            this.sortInfo.ChangeSortDirection(SortTypeID.FilePath);
            this.sortInfo.ActiveSortType = SortTypeID.FilePath;
            this.SetSort();
            this.SetFilter();
        }

        private void SortFileUpdateDateToolStripButton_Click(object sender, EventArgs e)
        {
            this.sortInfo.ChangeSortDirection(SortTypeID.UpdateDate);
            this.sortInfo.ActiveSortType = SortTypeID.UpdateDate;
            this.SetSort();
            this.SetFilter();
        }

        private void SortFileCreateDateToolStripButton_Click(object sender, EventArgs e)
        {
            this.sortInfo.ChangeSortDirection(SortTypeID.CreateDate);
            this.sortInfo.ActiveSortType = SortTypeID.CreateDate;
            this.SetSort();
            this.SetFilter();
        }

        private void SortFilerRgistrationDateToolStripButton_Click(object sender, EventArgs e)
        {
            this.sortInfo.ChangeSortDirection(SortTypeID.RgistrationDate);
            this.sortInfo.ActiveSortType = SortTypeID.RgistrationDate;
            this.SetSort();
            this.SetFilter();
        }

        private void ThumbnailSizeToolStripSlider_BeginValueChange(object sender, EventArgs e)
        {
            this.SetFlowListItemSize();
        }

        private void ThumbnailSizeToolStripSlider_ValueChanged(object sender, EventArgs e)
        {
            this.SetFlowListItemSize();
        }

        private void ThumbnailSizeToolStripSlider_ValueChanging(object sender, EventArgs e)
        {
            FileListContentsConfig.ThumbnailSize = this.ThumbnailSize;
            this.SetFlowListItemSize();
            this.flowList.Refresh();
        }

        private void MovePreviewToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnMovePreviewButtonClick(e);
        }

        private void MoveNextToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnMoveNextButtonClick(e);
        }

        #endregion

        #region フローリストイベント

        private void FlowList_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
        }

        private void FlowList_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        private void FlowList_Drawitem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.filterFilePathList == null)
            {
                return;
            }

            this.DrawItem(e);
        }

        private void FlowList_DrawItemChanged(object sender, DrawItemChangedEventArgs e)
        {
            if (this.filterFilePathList == null)
            {
                return;
            }

            if (this.filterFilePathList.Count > 0 &&
                e.DrawFirstItemIndex > -1 &&
                e.DrawLastItemIndex > -1 &&
                e.DrawLastItemIndex < this.filterFilePathList.Count)
            {
                var param = new GetThumbnailParameter();
                param.FilePathList = this.filterFilePathList;
                param.FirstIndex = e.DrawFirstItemIndex;
                param.LastIndex = e.DrawLastItemIndex;
                param.ThumbnailWidth = this.flowList.ItemWidth - this.flowList.ItemSpace * 2;
                if (this.IsShowFileName)
                {
                    param.ThumbnailHeight = this.flowList.ItemHeight - this.flowList.ItemSpace * 2 - this.ItemTextHeight;
                }
                else
                {
                    param.ThumbnailHeight = this.flowList.ItemHeight - this.flowList.ItemSpace * 2;
                }

                this.GetThumbnailsProcess.Cancel();
                this.GetThumbnailsProcess.Execute(this, param);
            }
        }

        private void FlowList_SelectedItemChange(object sender, EventArgs e)
        {
            var filePathList = this.GetSelectedFiles();
            if (filePathList.Count > 0)
            {
                this.SelectedFilePath = filePathList.First();
            }

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(filePathList));
        }

        private void FlowList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle)
            {
                return;
            }

            var filePath = this.filterFilePathList[this.flowList.GetSelectedIndexs()[0]];
            var file = this.masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewerContentsParameter(
                    this.Parameter.ContentsSources,
                    this.Parameter.SourcesKey,
                    this.GetImageFilesAction,
                    filePath,
                    this.Title,
                    this.Icon);
                this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.AddTab, param));
            }
            else if (!file.IsFile)
            {
                var param = new DirectoryFileListContentsParameter(file.FilePath);
                this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.AddTab, param));
            }
        }

        private void FlowList_ItemMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var filePath = this.filterFilePathList[this.flowList.GetSelectedIndexs()[0]];
            var file = this.masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewerContentsParameter(
                    this.Parameter.ContentsSources,
                    this.Parameter.SourcesKey,
                    this.GetImageFilesAction,
                    filePath,
                    this.Title,
                    this.Icon);
                this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
            }
            else if (!file.IsFile)
            {
                var param = new DirectoryFileListContentsParameter(file.FilePath);
                this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
            }
            else
            {
                FileUtil.OpenFile(file.FilePath);
            }
        }

        protected virtual void FlowLilst_BackgroundMouseClick(object sender, MouseEventArgs e)
        {
            this.OnBackgroundMouseClick(e);
        }

        private void FlowList_ItemExecute(object sender, EventArgs e)
        {
            var filePath = this.filterFilePathList[this.flowList.GetSelectedIndexs()[0]];
            var file = this.masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewerContentsParameter(
                    this.Parameter.ContentsSources,
                    this.Parameter.SourcesKey,
                    this.GetImageFilesAction,
                    filePath,
                    this.Title,
                    this.Icon);
                this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
            }
            else if (!file.IsFile)
            {
                var param = new DirectoryFileListContentsParameter(file.FilePath);
                this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
            }
            else
            {
                FileUtil.OpenFile(file.FilePath);
            }
        }

        private void FlowList_ItemDelete(object sender, EventArgs e)
        {
            var filePathList = new List<string>();
            foreach (var index in this.flowList.GetSelectedIndexs())
            {
                filePathList.Add(this.filterFilePathList[index]);
            }

            this.OnRemoveFile(filePathList);
        }

        private void FlowList_DragStart(object sender, EventArgs e)
        {
            var selectedIndexList = this.flowList.GetSelectedIndexs();
            if (selectedIndexList.Count < 1)
            {
                return;
            }

            var currentFilePath = this.filterFilePathList[selectedIndexList.First()];
            var currentFileInfo = this.masterFileDictionary[currentFilePath];
            if (currentFileInfo.IsFile && currentFileInfo.IsImageFile)
            {
                // 選択項目が画像ファイルの場合。
                var filePathList = new List<string>();
                foreach (var filePath in this.filterFilePathList)
                {
                    var fileInfo = this.masterFileDictionary[filePath];
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
                        this.GetImageFilesAction,
                        this.Title,
                        this.Icon); ;
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
                    this.GetImageFilesAction,
                    this.Title,
                    this.Icon);
                this.DoDragDrop(dragData, DragDropEffects.All);
            }
        }

        #endregion

        #region コンテキストメニューイベント

        protected virtual void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var filePathList = this.GetSelectedFiles();
            if (filePathList.Count > 0)
            {
                this.fileContextMenu.SetFile(filePathList);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void FileContextMenu_FileActiveTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewerContentsParameter(
                this.Parameter.ContentsSources,
                this.Parameter.SourcesKey,
                this.GetImageFilesAction,
                e.FilePath,
                this.Title,
                this.Icon);
            this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        private void FileContextMenu_FileNewTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewerContentsParameter(
                this.Parameter.ContentsSources,
                this.Parameter.SourcesKey,
                this.GetImageFilesAction,
                e.FilePath,
                this.Title,
                this.Icon);
            this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.AddTab, param));
        }

        private void FileContextMenu_FileNewWindowOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewerContentsParameter(
                this.Parameter.ContentsSources,
                this.Parameter.SourcesKey,
                this.GetImageFilesAction,
                e.FilePath,
                this.Title,
                this.Icon);
            this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.NewWindow, param));
        }

        private void FileContextMenu_FileOpen(object sender, ExecuteFileEventArgs e)
        {
            FileUtil.OpenFile(e.FilePath);
        }

        private void FileContextMenu_SaveDirectoryOpen(object sender, ExecuteFileEventArgs e)
        {
            FileUtil.OpenExplorerSelect(e.FilePath);
        }

        private void FileContextMenu_DirectoryActiveTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new DirectoryFileListContentsParameter(e.FilePath);
            this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        private void FileContextMenu_DirectoryNewTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new DirectoryFileListContentsParameter(e.FilePath);
            this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.AddTab, param));
        }

        private void FileContextMenu_DirectoryNewWindowOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new DirectoryFileListContentsParameter(e.FilePath);
            this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.NewWindow, param));
        }

        private void FileContextMenu_ExplorerOpen(object sender, ExecuteFileEventArgs e)
        {
            FileUtil.OpenExplorer(e.FilePath);
        }

        private void FileContextMenu_Export(object sender, ExecuteFileListEventArgs e)
        {
            using (var ofd = new SaveFileDialog())
            {
                ofd.InitialDirectory = CommonConfig.ExportDirectoryPath;
                ofd.FileName = FileUtil.GetFileName(e.FilePathList.First());
                ofd.CheckFileExists = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var dir = FileUtil.GetParentDirectoryPath(ofd.FileName);
                    var param = new ExportFileParameter();
                    param.ExportDirectoryPath = dir;
                    param.FilePathList = e.FilePathList;
                    this.ExportFileProcess.Execute(this, param);

                    CommonConfig.ExportDirectoryPath = dir;
                }
            }
        }

        private void FileContextMenu_PathCopy(object sender, ExecuteFileListEventArgs e)
        {
            var sb = new StringBuilder();

            foreach (var filePath in e.FilePathList)
            {
                sb.AppendLine(filePath);
            }

            Clipboard.SetText(sb.ToString());
        }

        private void FileContextMenu_NameCopy(object sender, ExecuteFileListEventArgs e)
        {
            var sb = new StringBuilder();

            foreach (var filePath in e.FilePathList)
            {
                sb.AppendLine(FileUtil.GetFileName(filePath));
            }

            Clipboard.SetText(sb.ToString());
        }

        private void FileContextMenu_RemoveFromList(object sender, ExecuteFileListEventArgs e)
        {
            this.OnRemoveFile(e.FilePathList);
        }

        private void FileContextMenu_Bookmark(object sender, ExecuteFileEventArgs e)
        {
            var paramter = new SingleValueEntity<string>()
            {
                Value = e.FilePath,
            };

            this.AddBookmarkProcess.Execute(this, paramter);
        }

        #endregion
    }
}
