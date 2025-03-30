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
using SWF.UIComponent.FlowList;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// ファイルリストコンテンツ基底クラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public abstract partial class AbstractFileListPage
        : BrowserPage, ISender
    {
        private static readonly Rectangle TOOL_BAR_DEFAULT_BOUNDS = new(0, 0, 767, 29);
        private static readonly Rectangle FLOW_LIST_DEFAULT_BOUNDS = new(0, 29, 767, 0);

        private bool disposed = false;
        private bool isLoaded = false;
        private float scale = 0f;
        private Dictionary<string, FileEntity> masterFileDictionary = null;
        private string[] filterFilePathList = null;
        private readonly Font defaultFont = new("Yu Gothic UI", 9);
        private readonly Dictionary<float, Font> fontCache = [];

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string SelectedFilePath { get; protected set; } = FileUtil.ROOT_DIRECTORY_PATH;

        protected SortInfo SortInfo { get; private set; } = new();

        protected bool IsAscending
        {
            get
            {
                return this.SortInfo.IsAscending(this.SortInfo.ActiveSortType);
            }
        }

        protected SortTypeID SortTypeID
        {
            get
            {
                return this.SortInfo.ActiveSortType;
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

        private bool IsShowDirectory
        {
            get
            {
                return this.toolBar.FolderMenuItemChecked;
            }
            set
            {
                this.toolBar.FolderMenuItemChecked = value;
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

        public AbstractFileListPage(IPageParameter param)
            : base(param)
        {
            this.InitializeComponent();

            this.IsShowFileName = FileListPageConfig.Instance.IsShowFileName;
            this.IsShowDirectory = FileListPageConfig.Instance.IsShowDirectory;
            this.IsShowImageFile = FileListPageConfig.Instance.IsShowImageFile;
            this.IsShowOtherFile = FileListPageConfig.Instance.IsShowOtherFile;
            this.ThumbnailSize = FileListPageConfig.Instance.ThumbnailSize;
        }

        public override string[] GetSelectedFiles()
        {
            var filePathList = new List<string>();
            var selectedIndexs = this.flowList.GetSelectedIndexs();
            if (selectedIndexs.Length > 0)
            {
                foreach (var index in selectedIndexs)
                {
                    filePathList.Add(this.filterFilePathList[index]);
                }
            }

            return [.. filePathList];
        }

        public override void RedrawPage(float scale)
        {
            if (this.scale == scale)
            {
                return;
            }

            this.scale = scale;
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
                (int)(TOOL_BAR_DEFAULT_BOUNDS.Height * this.scale));

            this.flowList.SetBounds(
                0,
                this.toolBar.Bottom,
                this.Width,
                baseHeigth - this.toolBar.Bottom);

            this.flowList.SetDrawParameter(true);

            this.toolBar.SetControlsBounds(scale);

            this.SetFlowListItemSize();

            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public override void StopPageDraw()
        {
            Instance<JobCaller>.Value.ThumbnailsGetJob.Value.BeginCancel();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.fileContextMenu.Close();
                Instance<JobCaller>.Value.ThumbnailsGetJob.Value.BeginCancel();

                this.components?.Dispose();

                foreach (var font in this.fontCache.Values)
                {
                    font.Dispose();
                }
                this.fontCache.Clear();
                this.defaultFont.Dispose();
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.isLoaded = true;

            var scale = AppConstants.GetCurrentWindowScale(this);
            this.RedrawPage(scale);

            base.OnLoad(e);
        }

        protected abstract void OnRemoveFile(string[] filePathList);

        protected abstract void OnMovePreviewButtonClick(EventArgs e);

        protected abstract void OnMoveNextButtonClick(EventArgs e);

        protected void SetFiles(FileShallowInfoEntity[] srcFiles, string selectedFilePath, SortTypeID sortTypeID, bool isAscending)
        {
            ArgumentNullException.ThrowIfNull(srcFiles, nameof(srcFiles));
            ArgumentNullException.ThrowIfNull(selectedFilePath, nameof(selectedFilePath));

            this.masterFileDictionary = [];
            foreach (var srcFile in srcFiles)
            {
                var destFile = new FileEntity
                {
                    FilePath = srcFile.FilePath,
                    FileName = srcFile.FileName,
                    UpdateDate = srcFile.UpdateDate,
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

                this.masterFileDictionary.Add(destFile.FilePath, destFile);
            }

            var scale = AppConstants.GetCurrentWindowScale(this);
            this.RedrawPage(scale);

            this.SelectedFilePath = selectedFilePath;
            this.SortInfo.SetSortType(sortTypeID, isAscending);

            this.SetSort();
            this.SetFilter();

            this.flowList.Focus();
        }

        protected void SetFile(FileShallowInfoEntity[] srcFiles, string selectedFilePath)
        {
            ArgumentNullException.ThrowIfNull(srcFiles, nameof(srcFiles));
            ArgumentNullException.ThrowIfNull(selectedFilePath, nameof(selectedFilePath));

            this.SetFiles(srcFiles, selectedFilePath, SortTypeID.Default, false);
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
                this.masterFileDictionary.Remove(filePath);
                indexList.Add(Array.IndexOf(this.filterFilePathList, filePath));
            }

            if (indexList.Count > 0)
            {
                var maximumIndex = indexList.Max();
                if (maximumIndex + 1 < this.filterFilePathList.Length)
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

        protected abstract Action<ISender> GetImageFilesGetAction(ImageViewerPageParameter paramter);

        private void SetSort()
        {
            this.toolBar.ClearSortImage();

            var sortButton = this.toolBar.GetSortToolStripButton(this.SortInfo.ActiveSortType);
            if (sortButton != null)
            {
                var isAscending = this.SortInfo.IsAscending(this.SortInfo.ActiveSortType);
                var arrow = this.SortInfo.GetSortDirectionArrow(isAscending);
                sortButton.Text = $"{arrow}  {sortButton.Text}";
            }
        }

        private void SetFilter()
        {
            FileListPageConfig.Instance.IsShowDirectory = this.IsShowDirectory;
            FileListPageConfig.Instance.IsShowImageFile = this.IsShowImageFile;
            FileListPageConfig.Instance.IsShowOtherFile = this.IsShowOtherFile;

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

            var isAscending = this.SortInfo.IsAscending(this.SortInfo.ActiveSortType);
            switch (this.SortInfo.ActiveSortType)
            {
                case SortTypeID.FileName:
                    filterList.Sort((x, y) =>
                    {
                        if (isAscending)
                        {
                            return NaturalStringComparer.Windows.Compare(x.FileName, y.FileName);
                        }
                        else
                        {
                            return NaturalStringComparer.Windows.Compare(y.FileName, x.FileName);
                        }
                    });
                    break;
                case SortTypeID.FilePath:
                    filterList.Sort((x, y) =>
                    {
                        if (isAscending)
                        {
                            return NaturalStringComparer.Windows.Compare(x.FilePath, y.FilePath);
                        }
                        else
                        {
                            return NaturalStringComparer.Windows.Compare(y.FilePath, x.FilePath);
                        }
                    });
                    break;
                case SortTypeID.UpdateDate:
                    filterList.Sort((x, y) =>
                    {
                        var xDate = x.UpdateDate.GetValueOrDefault(DateTime.MinValue);
                        var yDate = y.UpdateDate.GetValueOrDefault(DateTime.MinValue);
                        if (isAscending)
                        {
                            if (xDate == yDate)
                            {
                                return NaturalStringComparer.Windows.Compare(x.FilePath, y.FilePath);
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
                                return NaturalStringComparer.Windows.Compare(y.FilePath, x.FilePath);
                            }
                            else
                            {
                                return -xDate.CompareTo(yDate);
                            }
                        }
                    });
                    break;
                case SortTypeID.RegistrationDate:
                    filterList.Sort((x, y) =>
                    {
                        var xDate = x.RgistrationDate.GetValueOrDefault(DateTime.MinValue);
                        var yDate = y.RgistrationDate.GetValueOrDefault(DateTime.MinValue);
                        if (isAscending)
                        {
                            if (xDate == yDate)
                            {
                                return NaturalStringComparer.Windows.Compare(x.FilePath, y.FilePath);
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
                                return NaturalStringComparer.Windows.Compare(y.FilePath, x.FilePath);
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
                this.filterFilePathList = [.. filterList.ConvertAll(f => f.FilePath)];
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

        private Font GetFont(float scale)
        {
            if (this.fontCache.TryGetValue(scale, out var font))
            {
                return font;
            }

            var newFont = new Font(this.defaultFont.FontFamily, this.defaultFont.Size * scale);
            this.fontCache.Add(scale, newFont);
            return newFont;
        }

        private void ChangeFileNameVisible()
        {
            FileListPageConfig.Instance.IsShowFileName = this.IsShowFileName;
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
            return (int)(g.MeasureString("A", this.GetFont(this.scale)).Height * 2);
        }

        private void SetFlowListItemSize()
        {
            if (!this.isLoaded)
            {
                return;
            }

            if (this.IsShowFileName)
            {
                this.flowList.SetItemSize(this.ThumbnailSize, this.ThumbnailSize + this.GetItemTextHeight());
            }
            else
            {
                this.flowList.SetItemSize(this.ThumbnailSize, this.ThumbnailSize);
            }
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
                e.Graphics.DrawRectangle(this.flowList.SelectedItemPen, e.ItemRectangle);
            }

            if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(this.flowList.MousePointItemBrush, e.ItemRectangle);
            }

            var filePath = this.filterFilePathList[e.ItemIndex];
            var item = this.masterFileDictionary[filePath];

            var scale = AppConstants.GetCurrentWindowScale(this);
            var font = this.GetFont(scale);
            var itemTextHeight = this.GetItemTextHeight(e.Graphics);

            if (item.ThumbnailImage == null)
            {
                ThumbnailUtil.DrawIcon(e.Graphics, item.JumboIcon, this.GetIconRectangle(e, itemTextHeight));
                e.Graphics.DrawString(item.FileName, font, this.flowList.ItemTextBrush, this.GetTextRectangle(e, itemTextHeight), this.flowList.ItemTextFormat);
            }
            else
            {
                var thumbRect = this.GetThumbnailRectangle(e, itemTextHeight);
                if (item.IsFile)
                {
                    ThumbnailUtil.AdjustDrawFileThumbnail(
                        e.Graphics, item.ThumbnailImage, thumbRect, new SizeF(item.SourceImageWidth, item.SourceImageHeight));
                }
                else
                {
                    ThumbnailUtil.AdjustDrawDirectoryThumbnail(
                        e.Graphics, item.ThumbnailImage, thumbRect, new SizeF(item.SourceImageWidth, item.SourceImageHeight), item.JumboIcon);
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
            if (this.masterFileDictionary == null
                || !this.masterFileDictionary.TryGetValue(e.FilePath, out var file))
            {
                return;
            }

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
                var index = Array.IndexOf(this.filterFilePathList, file.FilePath);
                if (index > -1)
                {
                    this.flowList.InvalidateFromItemIndex(index);
                }
            }
        }

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
            this.SortInfo.ChangeSortDirection(SortTypeID.FileName);
            this.SortInfo.ActiveSortType = SortTypeID.FileName;
            this.SetSort();
            this.SetFilter();
        }

        private void SortFilePathToolStripButton_Click(object sender, EventArgs e)
        {
            this.SortInfo.ChangeSortDirection(SortTypeID.FilePath);
            this.SortInfo.ActiveSortType = SortTypeID.FilePath;
            this.SetSort();
            this.SetFilter();
        }

        private void SortFileUpdateDateToolStripButton_Click(object sender, EventArgs e)
        {
            this.SortInfo.ChangeSortDirection(SortTypeID.UpdateDate);
            this.SortInfo.ActiveSortType = SortTypeID.UpdateDate;
            this.SetSort();
            this.SetFilter();
        }

        private void SortFilerRgistrationDateToolStripButton_Click(object sender, EventArgs e)
        {
            this.SortInfo.ChangeSortDirection(SortTypeID.RegistrationDate);
            this.SortInfo.ActiveSortType = SortTypeID.RegistrationDate;
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
            FileListPageConfig.Instance.ThumbnailSize = this.ThumbnailSize;
            this.SetFlowListItemSize();
            this.flowList.Invalidate();
            this.flowList.Update();
        }

        private void MovePreviewToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnMovePreviewButtonClick(e);
        }

        private void MoveNextToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnMoveNextButtonClick(e);
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

            if (this.filterFilePathList.Length > 0 &&
                e.DrawFirstItemIndex > -1 &&
                e.DrawLastItemIndex > -1 &&
                e.DrawLastItemIndex < this.filterFilePathList.Length)
            {
                var thumbnailWidth = this.flowList.ItemWidth - this.flowList.ItemSpace * 2;
                var thumbnailHeight = this.IsShowFileName switch
                {
                    true => this.flowList.ItemHeight - this.flowList.ItemSpace * 2 - this.GetItemTextHeight(),
                    _ => this.flowList.ItemHeight - this.flowList.ItemSpace * 2,
                };

                var fileList = this.filterFilePathList.Where((file, index) =>
                {
                    if (index < e.DrawFirstItemIndex || index > e.DrawLastItemIndex)
                    {
                        return false;
                    }

                    var info = this.masterFileDictionary[file];
                    if (info.ThumbnailImage == null)
                    {
                        return true;
                    }
                    else if (info.ThumbnailWidth < thumbnailWidth
                            || info.ThumbnailHeight < thumbnailHeight)
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
                    ThumbnailWidth = thumbnailWidth,
                    ThumbnailHeight = thumbnailHeight,
                };

                Instance<JobCaller>.Value.ThumbnailsGetJob.Value
                    .StartJob(this, param, _ =>
                    {
                        if (this.disposed)
                        {
                            return;
                        }

                        this.GetThumbnailsJob_Callback(_);
                    });
            }
        }

        private void FlowList_SelectedItemChange(object sender, EventArgs e)
        {
            var filePathList = this.GetSelectedFiles();
            if (filePathList.Length > 0)
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
                var param = new ImageViewerPageParameter(
                    this.Parameter.PageSources,
                    this.Parameter.SourcesKey,
                    this.GetImageFilesGetAction,
                    filePath,
                    this.SortInfo,
                    this.Title,
                    this.Icon,
                    this.Parameter.VisibleBookmarkMenuItem);
                this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.AddTab, param));
            }
            else if (!file.IsFile)
            {
                var param = new DirectoryFileListPageParameter(file.FilePath);
                this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.AddTab, param));
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
                var param = new ImageViewerPageParameter(
                    this.Parameter.PageSources,
                    this.Parameter.SourcesKey,
                    this.GetImageFilesGetAction,
                    filePath,
                    this.SortInfo,
                    this.Title,
                    this.Icon,
                    this.Parameter.VisibleBookmarkMenuItem);
                this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.OverlapTab, param));
            }
            else if (file.IsFile)
            {
                FileUtil.OpenFile(filePath);
            }
            else if (!file.IsFile)
            {
                var param = new DirectoryFileListPageParameter(file.FilePath);
                this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.OverlapTab, param));
            }
        }

        protected virtual void FlowLilst_BackgroundMouseClick(object sender, MouseEventArgs e)
        {
            this.OnBackgroundMouseClick(e);
        }

        private void FlowList_ItemExecute(object sender, EventArgs e)
        {
            var selectedIndexs = this.flowList.GetSelectedIndexs();
            if (selectedIndexs.Length > 1)
            {
                return;
            }

            var filePath = this.filterFilePathList[selectedIndexs[0]];
            var file = this.masterFileDictionary[filePath];
            if (file.IsImageFile)
            {
                var param = new ImageViewerPageParameter(
                    this.Parameter.PageSources,
                    this.Parameter.SourcesKey,
                    this.GetImageFilesGetAction,
                    filePath,
                    this.SortInfo,
                    this.Title,
                    this.Icon,
                    this.Parameter.VisibleBookmarkMenuItem);
                this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.OverlapTab, param));
            }
            else if (file.IsFile)
            {
                FileUtil.OpenFile(filePath);
            }
            else if (!file.IsFile)
            {
                var param = new DirectoryFileListPageParameter(file.FilePath);
                this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.OverlapTab, param));
            }
        }

        private void FlowList_ItemDelete(object sender, EventArgs e)
        {
            var filePathList = new List<string>();
            foreach (var index in this.flowList.GetSelectedIndexs())
            {
                filePathList.Add(this.filterFilePathList[index]);
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

            var currentFilePath = this.filterFilePathList[selectedIndexList.First()];
            var currentFileInfo = this.masterFileDictionary[currentFilePath];
            if (currentFileInfo.IsFile && currentFileInfo.IsImageFile)
            {
                // 選択項目が画像ファイルの場合。
                var selectedFiles = this.GetSelectedFiles();
                var selectedImageFiles = selectedFiles
                    .Where(FileUtil.IsImageFile)
                    .ToArray();

                var dragData = new DragEntity(
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
                dataObject.SetData(typeof(DragEntity), dragData);
                this.DoDragDrop(dataObject, DragDropEffects.Copy);
            }
            else if (!currentFileInfo.IsFile)
            {
                // 選択項目がフォルダの場合。
                var dragData = new DragEntity(
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
            var param = new ImageViewerPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this.GetImageFilesGetAction,
                e.FilePath,
                this.SortInfo,
                this.Title,
                this.Icon,
                this.Parameter.VisibleBookmarkMenuItem);
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.OverlapTab, param));
        }

        private void FileContextMenu_FileNewTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewerPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this.GetImageFilesGetAction,
                e.FilePath,
                this.SortInfo,
                this.Title,
                this.Icon,
                this.Parameter.VisibleBookmarkMenuItem);
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.AddTab, param));
        }

        private void FileContextMenu_FileNewWindowOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewerPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this.GetImageFilesGetAction,
                e.FilePath,
                this.SortInfo,
                this.Title,
                this.Icon,
                this.Parameter.VisibleBookmarkMenuItem);
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.NewWindow, param));
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
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.OverlapTab, param));
        }

        private void FileContextMenu_DirectoryNewTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new DirectoryFileListPageParameter(e.FilePath);
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.AddTab, param));
        }

        private void FileContextMenu_DirectoryNewWindowOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new DirectoryFileListPageParameter(e.FilePath);
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.NewWindow, param));
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

            Instance<JobCaller>.Value.StartBookmarkAddJob(this, paramter);
        }
    }
}
