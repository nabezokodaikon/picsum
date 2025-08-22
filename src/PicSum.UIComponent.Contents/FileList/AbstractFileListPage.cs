using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.ContextMenu;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.Core.StringAccessor;
using SWF.UIComponent.FlowList;
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

    public abstract partial class AbstractFileListPage
        : AbstractBrowsePage, ISender
    {
        private static readonly Rectangle TOOL_BAR_DEFAULT_BOUNDS = new(0, 0, 767, 29);
        private static readonly Rectangle FLOW_LIST_DEFAULT_BOUNDS = new(0, 29, 767, 0);

        private bool _disposed = false;
        private float _scale = 0f;
        private Dictionary<string, FileEntity> _masterFileDictionary = null;
        private string[] _filterFilePathList = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string SelectedFilePath { get; protected set; } = FileUtil.ROOT_DIRECTORY_PATH;

        protected SortParameter SortInfo { get; private set; } = new();

        protected bool IsAscending
        {
            get
            {
                return this.SortInfo.IsAscending(this.SortInfo.ActiveSortMode);
            }
        }

        protected FileSortMode SortMode
        {
            get
            {
                return this.SortInfo.ActiveSortMode;
            }
        }

        protected bool IsMoveControlVisible
        {
            get
            {
                return this.toolBar.MovePreviewButtonVisible;
            }
            set
            {
                this.toolBar.MovePreviewButtonVisible = value;
                this.toolBar.MoveNextButtonVisible = value;
            }
        }

        protected int ScrollValue
        {
            get
            {
                return this.flowList.ScrollValue;
            }
        }

        protected Size FlowListSize
        {
            get
            {
                return this.flowList.Size;
            }
        }

        protected Size ItemSize
        {
            get
            {
                return new(
                    this.flowList.ItemWidth,
                    this.flowList.ItemHeight);
            }
        }

        private bool IsShowDirectory
        {
            get
            {
                return this.toolBar.DirectoryMenuItemChecked;
            }
            set
            {
                this.toolBar.DirectoryMenuItemChecked = value;
            }
        }

        private bool IsShowImageFile
        {
            get
            {
                return this.toolBar.ImageFileMenuItemChecked;
            }
            set
            {
                this.toolBar.ImageFileMenuItemChecked = value;
            }
        }

        private bool IsShowOtherFile
        {
            get
            {
                return this.toolBar.OtherFileMenuItemChecked;
            }
            set
            {
                this.toolBar.OtherFileMenuItemChecked = value;
            }
        }

        private bool IsShowFileName
        {
            get
            {
                return this.toolBar.FileNameMenuItemChecked;
            }
            set
            {
                this.toolBar.FileNameMenuItemChecked = value;
            }
        }

        private int ThumbnailSize
        {
            get
            {
                return this.toolBar.ThumbnailSizeSliderValue;
            }
            set
            {
                this.toolBar.ThumbnailSizeSliderValue = value;
            }
        }

        protected AbstractFileListPage(AbstractPageParameter param)
            : base(param)
        {
            using (TimeMeasuring.Run(true, "AbstractFileListPage.New"))
            {
                this.InitializeComponent();

                if (param.SortInfo != null)
                {
                    this.SortInfo = param.SortInfo;
                }

                this.IsShowFileName = FileListPageConfig.INSTANCE.IsShowFileName;
                this.IsShowDirectory = FileListPageConfig.INSTANCE.IsShowDirectory;
                this.IsShowImageFile = FileListPageConfig.INSTANCE.IsShowImageFile;
                this.IsShowOtherFile = FileListPageConfig.INSTANCE.IsShowOtherFile;
                this.ThumbnailSize = FileListPageConfig.INSTANCE.ThumbnailSize;
            }
        }

        public override string[] GetSelectedFiles()
        {
            var filePathList = new List<string>();
            var selectedIndexs = this.flowList.GetSelectedIndexs();
            if (selectedIndexs.Length > 0)
            {
                foreach (var index in selectedIndexs)
                {
                    filePathList.Add(this._filterFilePathList[index]);
                }
            }

            return [.. filePathList];
        }

        public override void RedrawPage(float scale)
        {
            if (this._scale != scale)
            {
                this._scale = scale;
                var baseHeigth = this.Height - 8;

                this.toolBar.SuspendLayout();
                this.SuspendLayout();

                this.toolBar.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left
                    | AnchorStyles.Right;

                this.flowList.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Left
                    | AnchorStyles.Right
                    | AnchorStyles.Bottom;

                this.toolBar.SetBounds(
                    0,
                    0,
                    this.Width,
                    (int)(TOOL_BAR_DEFAULT_BOUNDS.Height * this._scale));

                this.flowList.SetBounds(
                    0,
                    this.toolBar.Bottom,
                    this.Width,
                    baseHeigth - this.toolBar.Bottom);

                this.toolBar.SetControlsBounds(scale);

                this.SetFlowListItemSize();

                this.toolBar.ResumeLayout(false);
                this.ResumeLayout(false);
            }

            this.flowList.SetDrawParameter(true);
            this.flowList.Focus();
        }

        public override void StopPageDraw()
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this._masterFileDictionary != null)
                {
                    foreach (var item in this._masterFileDictionary)
                    {
                        item.Value.ThumbnailImage?.Dispose();
                    }
                }

                this.fileContextMenu.Close();
            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        protected abstract void OnRemoveFile(string[] filePathList);

        protected abstract void OnMovePreviewButtonClick(EventArgs e);

        protected abstract void OnMoveNextButtonClick(EventArgs e);

        protected void SetFiles(
            FileShallowInfoEntity[] srcFiles,
            string selectedFilePath,
            ScrollParameter scrollInfo,
            FileSortMode sortMode,
            bool isAscending)
        {
            ArgumentNullException.ThrowIfNull(srcFiles, nameof(srcFiles));
            ArgumentNullException.ThrowIfNull(selectedFilePath, nameof(selectedFilePath));

            this._masterFileDictionary = [];
            foreach (var srcFile in srcFiles)
            {
                var destFile = new FileEntity
                {
                    FilePath = srcFile.FilePath,
                    FileName = srcFile.FileName,
                    CreateDate = srcFile.CreateDate,
                    UpdateDate = srcFile.UpdateDate,
                    TakenDate = srcFile.TakenDate,
                    RgistrationDate = srcFile.RgistrationDate,
                    SmallIcon = srcFile.SmallIcon,
                    ExtraLargeIcon = srcFile.ExtraLargeIcon,
                    JumboIcon = srcFile.JumboIcon,
                    IsFile = srcFile.IsFile,
                    IsImageFile = srcFile.IsImageFile,
                };

                if (srcFile.ThumbnailImage != null)
                {
                    destFile.ThumbnailImage = srcFile.ThumbnailImage;
                    destFile.ThumbnailWidth = srcFile.ThumbnailWidth;
                    destFile.ThumbnailHeight = srcFile.ThumbnailHeight;
                    destFile.SourceImageWidth = srcFile.SourceWidth;
                    destFile.SourceImageHeight = srcFile.SourceHeight;
                }

                this._masterFileDictionary.Add(destFile.FilePath, destFile);
            }

            var scale = WindowUtil.GetCurrentWindowScale(this);
            this.RedrawPage(scale);

            this.SelectedFilePath = selectedFilePath;
            this.SortInfo.SetSortMode(sortMode, isAscending);

            this.SetSort();
            this.SetFilter(scrollInfo);

            this.flowList.Focus();

            var imageFiles = this._masterFileDictionary
                .AsEnumerable()
                .Where(static item => ImageUtil.IsImageFile(item.Value.FilePath))
                .Select(static item => item.Value.FilePath)
                .ToArray();

            if (imageFiles.Length > 0)
            {
                var param = new TakenDatesGetParameter()
                {
                    FilePathList = imageFiles,
                };

                Instance<JobCaller>.Value.TakenDatesGetJob.Value.StartJob(this, param, result =>
                {
                    if (this._disposed)
                    {
                        return;
                    }

                    if (result == TakenDateResult.COMPLETED)
                    {
                        this.toolBar.TakenDateSortButtonEnabled = true;
                    }
                    else if (this._masterFileDictionary.TryGetValue(result.FilePath, out var item))
                    {
                        item.TakenDate = result.TakenDate;
                    }
                });
            }
        }

        protected void SetFile(
            FileShallowInfoEntity[] srcFiles,
            string selectedFilePath,
            ScrollParameter scrollInfo)
        {
            ArgumentNullException.ThrowIfNull(srcFiles, nameof(srcFiles));
            ArgumentNullException.ThrowIfNull(selectedFilePath, nameof(selectedFilePath));

            this.SetFiles(
                srcFiles,
                selectedFilePath,
                scrollInfo,
                FileSortMode.Default,
                false);
        }

        protected void SetContextMenuFiles(string[] filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            if (filePathList.Length == 0)
            {
                throw new ArgumentException("0件のファイルリストはセットできません。", nameof(filePathList));
            }

            this.fileContextMenu.SetFile(filePathList);
        }

        protected void SetContextMenuFiles(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.SetContextMenuFiles([filePath]);
        }

        protected void RemoveFile(string[] filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            if (filePathList.Length == 0)
            {
                throw new ArgumentException("削除するファイルパスが0件です。", nameof(filePathList));
            }

            var indexList = new List<int>();
            foreach (var filePath in filePathList)
            {
                this._masterFileDictionary.Remove(filePath);
                indexList.Add(Array.IndexOf(this._filterFilePathList, filePath));
            }

            if (indexList.Count > 0)
            {
                var maximumIndex = indexList.Max();
                if (maximumIndex + 1 < this._filterFilePathList.Length)
                {
                    this.SelectedFilePath = this._filterFilePathList[maximumIndex + 1];
                }
                else
                {
                    var minimumIndex = indexList.Min();
                    if (minimumIndex - 1 > -1)
                    {
                        this.SelectedFilePath = this._filterFilePathList[minimumIndex - 1];
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

        protected abstract Action<ISender> GetImageFilesGetAction(ImageViewPageParameter parameter);

        protected virtual void OnSelectedItemChange()
        {
            var filePathList = this.GetSelectedFiles();
            if (filePathList.Length > 0)
            {
                this.SelectedFilePath = filePathList.First();
            }

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(filePathList));
        }

        private void SetSort()
        {
            this.toolBar.ClearSortImage();

            var sortButton = this.toolBar.GetSortToolStripButton(this.SortInfo.ActiveSortMode);
            if (sortButton != null)
            {
                var isAscending = this.SortInfo.IsAscending(this.SortInfo.ActiveSortMode);
                var arrow = this.SortInfo.GetSortDirectionArrow(isAscending);
                sortButton.Text = $"{arrow}  {sortButton.Text}";
            }
        }

        private void SetFilter()
        {
            this.SetFilter(ScrollParameter.EMPTY);
        }

        private void SetFilter(ScrollParameter scrollInfo)
        {
            this.flowList.Focus();

            FileListPageConfig.INSTANCE.IsShowDirectory = this.IsShowDirectory;
            FileListPageConfig.INSTANCE.IsShowImageFile = this.IsShowImageFile;
            FileListPageConfig.INSTANCE.IsShowOtherFile = this.IsShowOtherFile;

            if (this._masterFileDictionary == null)
            {
                return;
            }

            var filterList = new List<FileEntity>();
            foreach (var file in this._masterFileDictionary.Values)
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

            var isAscending = this.SortInfo.IsAscending(this.SortInfo.ActiveSortMode);
            switch (this.SortInfo.ActiveSortMode)
            {
                case FileSortMode.FileName:
                    filterList.Sort((x, y) =>
                    {
                        if (isAscending)
                        {
                            return NaturalStringComparer.WINDOWS.Compare(x.FileName, y.FileName);
                        }
                        else
                        {
                            return NaturalStringComparer.WINDOWS.Compare(y.FileName, x.FileName);
                        }
                    });
                    break;
                case FileSortMode.FilePath:
                    filterList.Sort((x, y) =>
                    {
                        if (isAscending)
                        {
                            return NaturalStringComparer.WINDOWS.Compare(x.FilePath, y.FilePath);
                        }
                        else
                        {
                            return NaturalStringComparer.WINDOWS.Compare(y.FilePath, x.FilePath);
                        }
                    });
                    break;
                case FileSortMode.CreateDate:
                    filterList.Sort((x, y) =>
                    {
                        var xDate = x.CreateDate.GetValueOrDefault(DateTime.MinValue);
                        var yDate = y.CreateDate.GetValueOrDefault(DateTime.MinValue);
                        if (isAscending)
                        {
                            if (xDate == yDate)
                            {
                                return NaturalStringComparer.WINDOWS.Compare(x.FilePath, y.FilePath);
                            }
                            else
                            {
                                return xDate.CompareTo(yDate);
                            }
                        }
                        else
                        {
                            if (xDate == yDate)
                            {
                                return NaturalStringComparer.WINDOWS.Compare(x.FilePath, y.FilePath);
                            }
                            else
                            {
                                return -xDate.CompareTo(yDate);
                            }
                        }
                    });
                    break;
                case FileSortMode.UpdateDate:
                    filterList.Sort((x, y) =>
                    {
                        var xDate = x.UpdateDate.GetValueOrDefault(DateTime.MinValue);
                        var yDate = y.UpdateDate.GetValueOrDefault(DateTime.MinValue);
                        if (isAscending)
                        {
                            if (xDate == yDate)
                            {
                                return NaturalStringComparer.WINDOWS.Compare(x.FilePath, y.FilePath);
                            }
                            else
                            {
                                return xDate.CompareTo(yDate);
                            }
                        }
                        else
                        {
                            if (xDate == yDate)
                            {
                                return NaturalStringComparer.WINDOWS.Compare(x.FilePath, y.FilePath);
                            }
                            else
                            {
                                return -xDate.CompareTo(yDate);
                            }
                        }
                    });
                    break;
                case FileSortMode.TakenDate:
                    if (isAscending)
                    {
                        var a = filterList
                            .AsEnumerable()
                            .Where(static item => item.TakenDate != null && !item.TakenDate.Value.IsEmpty())
                            .OrderBy(static item => item.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderBy(static item => item.TakenDate)
                            .ToArray();
                        var b = filterList
                            .AsEnumerable()
                            .Where(static item => item.TakenDate == null || item.TakenDate.Value.IsEmpty())
                            .OrderBy(static item => item.FilePath, NaturalStringComparer.WINDOWS)
                            .ToArray();
                        filterList = [.. a.Concat(b)];
                    }
                    else
                    {
                        var a = filterList
                            .AsEnumerable()
                            .Where(static item => item.TakenDate != null && !item.TakenDate.Value.IsEmpty())
                            .OrderBy(static item => item.FilePath, NaturalStringComparer.WINDOWS)
                            .OrderByDescending(static item => item.TakenDate)
                            .ToArray();
                        var b = filterList
                            .AsEnumerable()
                            .Where(static item => item.TakenDate == null || item.TakenDate.Value.IsEmpty())
                            .OrderBy(static item => item.FilePath, NaturalStringComparer.WINDOWS)
                            .ToArray();
                        filterList = [.. a.Concat(b)];
                    }
                    break;
                case FileSortMode.AddDate:
                    filterList.Sort((x, y) =>
                    {
                        var xDate = x.RgistrationDate.GetValueOrDefault(DateTimeExtensions.EMPTY);
                        var yDate = y.RgistrationDate.GetValueOrDefault(DateTimeExtensions.EMPTY);
                        if (isAscending)
                        {
                            if (xDate == yDate)
                            {
                                return NaturalStringComparer.WINDOWS.Compare(x.FilePath, y.FilePath);
                            }
                            else
                            {
                                return xDate.CompareTo(yDate);
                            }
                        }
                        else
                        {
                            if (xDate == yDate)
                            {
                                return NaturalStringComparer.WINDOWS.Compare(x.FilePath, y.FilePath);
                            }
                            else
                            {
                                return -xDate.CompareTo(yDate);
                            }
                        }
                    });
                    break;
                default:
                    break;
            }

            this.flowList.BeginUpdate();

            try
            {
                this._filterFilePathList = [.. filterList.ConvertAll(f => f.FilePath)];
                this.flowList.ItemCount = filterList.Count;
                var selectedFile = filterList
                    .FirstOrDefault(f => StringUtil.CompareFilePath(f.FilePath, this.SelectedFilePath));
                if (selectedFile != null)
                {
                    this.flowList.SelectItem(
                        filterList.IndexOf(selectedFile),
                        scrollInfo);
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
            FileListPageConfig.INSTANCE.IsShowFileName = this.IsShowFileName;
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

        private int GetItemTextHeight()
        {
            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            {
                return this.GetItemTextHeight(g);
            }
        }

        private int GetItemTextHeight(Graphics g)
        {
            return (int)(g.MeasureString("A", Fonts.GetRegularFont(Fonts.Size.Small, this._scale)).Height * 2);
        }

        private void SetFlowListItemSize()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            var thumbnailSize = (int)(this.ThumbnailSize * scale);

            if (this.IsShowFileName)
            {
                this.flowList.SetItemSize(thumbnailSize, thumbnailSize + this.GetItemTextHeight());
            }
            else
            {
                this.flowList.SetItemSize(thumbnailSize, thumbnailSize);
            }
        }

        private void DrawItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            var selectedItemPen = FlowList.GetSelectedItemPen(this);

            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(this.flowList.SelectedItemBrush, e.ItemRectangle);
                e.Graphics.DrawRectangle(selectedItemPen, e.ItemRectangle);
            }

            if (e.IsFocus)
            {
                e.Graphics.DrawRectangle(selectedItemPen, e.ItemRectangle);
            }

            if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(this.flowList.MousePointItemBrush, e.ItemRectangle);
            }

            var filePath = this._filterFilePathList[e.ItemIndex];
            var item = this._masterFileDictionary[filePath];

            var font = Fonts.GetRegularFont(Fonts.Size.Small, this._scale);
            var itemTextHeight = this.GetItemTextHeight(e.Graphics);

            if (item.ThumbnailImage == null)
            {
                ThumbnailUtil.DrawIcon(this, e.Graphics, item.JumboIcon, this.GetIconRectangle(e, itemTextHeight));
                e.Graphics.DrawString(item.FileName, font, this.flowList.ItemTextBrush, this.GetTextRectangle(e, itemTextHeight), this.flowList.ItemTextFormat);
            }
            else
            {
                var thumbRect = this.GetThumbnailRectangle(e, itemTextHeight);
                if (item.IsFile)
                {
                    ThumbnailUtil.DrawFileThumbnail(
                        this, e.Graphics, item.ThumbnailImage, thumbRect, new SizeF(item.SourceImageWidth, item.SourceImageHeight));
                }
                else
                {
                    ThumbnailUtil.DrawDirectoryThumbnail(
                        this, e.Graphics, item.ThumbnailImage, thumbRect, new SizeF(item.SourceImageWidth, item.SourceImageHeight), item.JumboIcon);
                }

                if (this.IsShowFileName)
                {
                    e.Graphics.DrawString(item.FileName, font, this.flowList.ItemTextBrush, this.GetTextRectangle(e, itemTextHeight), this.flowList.ItemTextFormat);
                }
            }
        }

        private RectangleF GetIconRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e, int itemTextHeight)
        {
            return new RectangleF(e.ItemRectangle.X,
                                  e.ItemRectangle.Y,
                                  e.ItemRectangle.Width,
                                  e.ItemRectangle.Height - itemTextHeight);
        }

        private RectangleF GetThumbnailRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e, int itemTextHeight)
        {
            if (this.IsShowFileName)
            {
                return new RectangleF(e.ItemRectangle.X,
                                     e.ItemRectangle.Y,
                                     e.ItemRectangle.Width,
                                     e.ItemRectangle.Height - itemTextHeight);
            }
            else
            {
                return e.ItemRectangle;
            }
        }

        private RectangleF GetTextRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e, int itemTextHeight)
        {
            return new RectangleF(e.ItemRectangle.X,
                                  e.ItemRectangle.Bottom - itemTextHeight,
                                  e.ItemRectangle.Width,
                                  itemTextHeight);
        }

        private void GetThumbnailsJob_Callback(ThumbnailImageResult e)
        {
            if (this._masterFileDictionary == null
                || !this._masterFileDictionary.TryGetValue(e.FilePath, out var file))
            {
                return;
            }

            if (file.ThumbnailImage != null)
            {
                if (e.ThumbnailWidth != file.ThumbnailWidth ||
                    e.ThumbnailHeight != file.ThumbnailHeight ||
                    e.FileUpdateDate > file.UpdateDate)
                {
                    file.ThumbnailImage.Dispose();
                    file.ThumbnailImage = null;
                    file.ThumbnailImage = e.ThumbnailImage;
                    file.ThumbnailWidth = e.ThumbnailWidth;
                    file.ThumbnailHeight = e.ThumbnailHeight;
                    file.SourceImageWidth = e.SourceWidth;
                    file.SourceImageHeight = e.SourceHeight;
                    file.UpdateDate = e.FileUpdateDate;
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
                file.UpdateDate = e.FileUpdateDate;
            }

            if (this._filterFilePathList != null)
            {
                var index = Array.IndexOf(this._filterFilePathList, file.FilePath);
                if (index > -1)
                {
                    this.flowList.InvalidateFromItemIndex(index);
                }
            }
        }

        private void AbstractFileListPage_Loaded(object sender, EventArgs e)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            this.RedrawPage(scale);
        }

        private void ToolBar_DirectoryMenuItemClick(object sender, EventArgs e)
        {
            this.IsShowDirectory = !this.IsShowDirectory;
            this.SetFilter();
        }

        private void ToolBar_ImageFileMenuItemClick(object sender, EventArgs e)
        {
            this.IsShowImageFile = !this.IsShowImageFile;
            this.SetFilter();
        }

        private void ToolBar_OtherFileMenuItemClick(object sender, EventArgs e)
        {
            this.IsShowOtherFile = !this.IsShowOtherFile;
            this.SetFilter();
        }

        private void ToolBar_FileNameMenuItemClick(object sender, EventArgs e)
        {
            this.IsShowFileName = !this.IsShowFileName;
            this.ChangeFileNameVisible();
        }

        private void ToolBar_NameSortButtonClick(object sender, EventArgs e)
        {
            this.SortInfo.ChangeSortDirection(FileSortMode.FileName);
            this.SortInfo.ActiveSortMode = FileSortMode.FileName;
            this.SetSort();
            this.SetFilter();
        }

        private void ToolBar_PathSortButtonClick(object sender, EventArgs e)
        {
            this.SortInfo.ChangeSortDirection(FileSortMode.FilePath);
            this.SortInfo.ActiveSortMode = FileSortMode.FilePath;
            this.SetSort();
            this.SetFilter();
        }

        private void ToolBar_CreateDateSortButtonClick(object sender, EventArgs e)
        {
            this.SortInfo.ChangeSortDirection(FileSortMode.CreateDate);
            this.SortInfo.ActiveSortMode = FileSortMode.CreateDate;
            this.SetSort();
            this.SetFilter();
        }

        private void ToolBar_UpdateDateSortButtonClick(object sender, EventArgs e)
        {
            this.SortInfo.ChangeSortDirection(FileSortMode.UpdateDate);
            this.SortInfo.ActiveSortMode = FileSortMode.UpdateDate;
            this.SetSort();
            this.SetFilter();
            this.Set();
        }

        protected virtual void Set()
        {

        }

        private void ToolBar_TakenDateSortButtonClick(object sender, EventArgs e)
        {
            this.SortInfo.ChangeSortDirection(FileSortMode.TakenDate);
            this.SortInfo.ActiveSortMode = FileSortMode.TakenDate;
            this.SetSort();
            this.SetFilter();
        }

        private void ToolBar_AddDateSortButtonClick(object sender, EventArgs e)
        {
            this.SortInfo.ChangeSortDirection(FileSortMode.AddDate);
            this.SortInfo.ActiveSortMode = FileSortMode.AddDate;
            this.SetSort();
            this.SetFilter();
        }

        private void ToolBar_ThumbnailSizeSliderBeginValueChange(object sender, EventArgs e)
        {
            this.SetFlowListItemSize();
        }

        private void ToolBar_ThumbnailSizeSliderValueChanged(object sender, EventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            this.SetFlowListItemSize();
        }

        private void ToolBar_ThumbnailSizeSliderValueChanging(object sender, EventArgs e)
        {
            FileListPageConfig.INSTANCE.ThumbnailSize = this.ThumbnailSize;
            this.SetFlowListItemSize();
            this.flowList.Invalidate();
        }

        private void ToolBar_MovePreviewButtonClick(object sender, EventArgs e)
        {
            this.OnMovePreviewButtonClick(e);
        }

        private void ToolBar_MoveNextButtonClick(object sender, EventArgs e)
        {
            this.OnMoveNextButtonClick(e);
        }

        private void FlowList_MouseClick(object sender, MouseEventArgs e)
        {
            this.flowList.Focus();
            base.OnMouseClick(e);
        }

        private void FlowList_Drawitem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this._filterFilePathList == null)
            {
                return;
            }

            this.DrawItem(e);
        }

        private void FlowList_DrawItemChanged(object sender, DrawItemChangedEventArgs e)
        {
            if (this._filterFilePathList == null)
            {
                return;
            }

            if (this._filterFilePathList.Length > 0 &&
                e.DrawFirstItemIndex > -1 &&
                e.DrawLastItemIndex > -1 &&
                e.DrawLastItemIndex < this._filterFilePathList.Length)
            {
                var thumbSize = Math.Min(
                    this.flowList.ItemWidth,
                    this.flowList.ItemHeight);

                var fileList = this._filterFilePathList.Where((file, index) =>
                {
                    if (index < e.DrawFirstItemIndex || index > e.DrawLastItemIndex)
                    {
                        return false;
                    }

                    var info = this._masterFileDictionary[file];
                    if (info.ThumbnailImage == null)
                    {
                        return true;
                    }
                    else if (info.ThumbnailWidth < thumbSize
                            || info.ThumbnailHeight < thumbSize)
                    {
                        return true;
                    }

                    return false;
                }).ToArray();

                if (fileList.Length < 1)
                {
                    return;
                }

                var param = new ThumbnailsGetParameter
                {
                    FilePathList = fileList,
                    FirstIndex = 0,
                    LastIndex = fileList.Length - 1,
                    ThumbnailWidth = ThumbnailUtil.THUMBNAIL_MAXIMUM_SIZE,
                    ThumbnailHeight = ThumbnailUtil.THUMBNAIL_MAXIMUM_SIZE,
                    IsExecuteCallback = true,
                };

                Instance<JobCaller>.Value.ThumbnailsGetJob.Value
                    .StartJob(this, param, _ =>
                    {
                        if (this._disposed)
                        {
                            return;
                        }

                        this.GetThumbnailsJob_Callback(_);
                    });
            }
        }

        private void FlowList_SelectedItemChange(object sender, EventArgs e)
        {
            this.OnSelectedItemChange();
        }

        private void FlowList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle)
            {
                return;
            }

            var filePath = this._filterFilePathList[this.flowList.GetSelectedIndexs()[0]];
            var file = this._masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewPageParameter(
                    this.Parameter.PageSources,
                    this.Parameter.SourcesKey,
                    this.GetImageFilesGetAction,
                    filePath,
                    this.SortInfo,
                    this.Title,
                    this.Icon,
                    this.Parameter.VisibleBookmarkMenuItem);
                this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.AddTab, param));
            }
            else if (!file.IsFile)
            {
                var param = new DirectoryFileListPageParameter(file.FilePath);
                this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.AddTab, param));
            }
        }

        private void FlowList_ItemMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var filePath = this._filterFilePathList[this.flowList.GetSelectedIndexs()[0]];
            var file = this._masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewPageParameter(
                    this.Parameter.PageSources,
                    this.Parameter.SourcesKey,
                    this.GetImageFilesGetAction,
                    filePath,
                    this.SortInfo,
                    this.Title,
                    this.Icon,
                    this.Parameter.VisibleBookmarkMenuItem);
                this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.OverlapTab, param));
            }
            else if (file.IsFile)
            {
                FileUtil.OpenFile(filePath);
            }
            else if (!file.IsFile)
            {
                var param = new DirectoryFileListPageParameter(file.FilePath);
                this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.OverlapTab, param));
            }
        }

        private void FlowList_ItemExecute(object sender, EventArgs e)
        {
            var selectedIndexs = this.flowList.GetSelectedIndexs();
            if (selectedIndexs.Length > 1)
            {
                return;
            }

            var filePath = this._filterFilePathList[selectedIndexs[0]];
            var file = this._masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewPageParameter(
                    this.Parameter.PageSources,
                    this.Parameter.SourcesKey,
                    this.GetImageFilesGetAction,
                    filePath,
                    this.SortInfo,
                    this.Title,
                    this.Icon,
                    this.Parameter.VisibleBookmarkMenuItem);
                this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.OverlapTab, param));
            }
            else if (file.IsFile)
            {
                FileUtil.OpenFile(filePath);
            }
            else if (!file.IsFile)
            {
                var param = new DirectoryFileListPageParameter(file.FilePath);
                this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.OverlapTab, param));
            }
        }

        private void FlowList_ItemDelete(object sender, EventArgs e)
        {
            var filePathList = new List<string>();
            foreach (var index in this.flowList.GetSelectedIndexs())
            {
                filePathList.Add(this._filterFilePathList[index]);
            }

            this.OnRemoveFile([.. filePathList]);
        }

        private void FlowList_DragStart(object sender, EventArgs e)
        {
            var selectedIndexList = this.flowList.GetSelectedIndexs();
            if (selectedIndexList.Length < 1)
            {
                return;
            }

            var currentFilePath = this._filterFilePathList[selectedIndexList.First()];
            var currentFileInfo = this._masterFileDictionary[currentFilePath];
            if (currentFileInfo.IsFile && currentFileInfo.IsImageFile)
            {
                // 選択項目が画像ファイルの場合。
                var selectedFiles = this.GetSelectedFiles();
                var selectedImageFiles = selectedFiles
                    .Where(static _ => ImageUtil.IsImageFile(_))
                    .ToArray();

                var dragData = new DragParameter(
                    this,
                    this.Parameter.PageSources,
                    this.Parameter.SourcesKey,
                    currentFilePath,
                    this.SortInfo,
                    this.GetImageFilesGetAction,
                    this.Title,
                    this.Icon,
                    this.Parameter.VisibleBookmarkMenuItem);

                var dataObject = new DataObject();
                dataObject.SetData(DataFormats.FileDrop, selectedImageFiles);
                dataObject.SetData(typeof(DragParameter), dragData);
                this.DoDragDrop(dataObject, DragDropEffects.Copy);
            }
            else if (!currentFileInfo.IsFile)
            {
                // 選択項目がフォルダの場合。
                var dragData = new DragParameter(
                    this,
                    this.Parameter.PageSources,
                    this.Parameter.SourcesKey,
                    currentFilePath,
                    this.SortInfo,
                    this.GetImageFilesGetAction,
                    this.Title,
                    this.Icon,
                    this.Parameter.VisibleBookmarkMenuItem);
                this.DoDragDrop(dragData, DragDropEffects.Copy);
            }
        }

        protected virtual void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            var filePathList = this.GetSelectedFiles();
            if (filePathList.Length > 0)
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
            var param = new ImageViewPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this.GetImageFilesGetAction,
                e.FilePath,
                this.SortInfo,
                this.Title,
                this.Icon,
                this.Parameter.VisibleBookmarkMenuItem);
            this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.OverlapTab, param));
        }

        private void FileContextMenu_FileNewTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this.GetImageFilesGetAction,
                e.FilePath,
                this.SortInfo,
                this.Title,
                this.Icon,
                this.Parameter.VisibleBookmarkMenuItem);
            this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.AddTab, param));
        }

        private void FileContextMenu_FileNewWindowOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this.GetImageFilesGetAction,
                e.FilePath,
                this.SortInfo,
                this.Title,
                this.Icon,
                this.Parameter.VisibleBookmarkMenuItem);
            this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.NewWindow, param));
        }

        private void FileContextMenu_FileOpen(object sender, ExecuteFileEventArgs e)
        {
            FileUtil.OpenFile(e.FilePath);
        }

        private void FileContextMenu_SelectApplication(object sender, ExecuteFileEventArgs e)
        {
            FileUtil.SelectApplication(e.FilePath);
        }

        private void FileContextMenu_SaveDirectoryOpen(object sender, ExecuteFileEventArgs e)
        {
            FileUtil.OpenExplorerSelect(e.FilePath);
        }

        private void FileContextMenu_DirectoryActiveTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new DirectoryFileListPageParameter(e.FilePath);
            this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.OverlapTab, param));
        }

        private void FileContextMenu_DirectoryNewTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new DirectoryFileListPageParameter(e.FilePath);
            this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.AddTab, param));
        }

        private void FileContextMenu_DirectoryNewWindowOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new DirectoryFileListPageParameter(e.FilePath);
            this.OnOpenPage(new BrowsePageEventArgs(PageOpenMode.NewWindow, param));
        }

        private void FileContextMenu_ExplorerOpen(object sender, ExecuteFileEventArgs e)
        {
            FileUtil.OpenExplorer(e.FilePath);
        }

        private void FileContextMenu_PathCopy(object sender, ExecuteFileListEventArgs e)
        {
            var sb = new StringBuilder();

            foreach (var filePath in e.FilePathList)
            {
                sb.AppendLine(filePath);
            }

            Clipboard.SetText(sb.ToString().TrimEnd('\n', '\r'));
        }

        private void FileContextMenu_NameCopy(object sender, ExecuteFileListEventArgs e)
        {
            var sb = new StringBuilder();

            foreach (var filePath in e.FilePathList)
            {
                sb.AppendLine(FileUtil.GetFileName(filePath));
            }

            Clipboard.SetText(sb.ToString().TrimEnd('\n', '\r'));
        }

        private void FileContextMenu_RemoveFromList(object sender, ExecuteFileListEventArgs e)
        {
            this.OnRemoveFile(e.FilePathList);
        }

        private void FileContextMenu_Bookmark(object sender, ExecuteFileEventArgs e)
        {
            var paramter = new ValueParameter<string>(e.FilePath);

            Instance<JobCaller>.Value.EnqueueBookmarkUpdateJob(this, paramter);
        }
    }
}
