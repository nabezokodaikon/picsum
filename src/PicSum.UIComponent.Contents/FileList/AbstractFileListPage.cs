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
using System.IO;
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
        private bool disposed = false;
        private Dictionary<string, FileEntity> masterFileDictionary = null;
        private string[] filterFilePathList = null;

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
                return this.movePreviewToolStripButton.Visible;
            }
            set
            {
                this.movePreviewToolStripButton.Visible = value;
                this.moveNextToolStripButton.Visible = value;
            }
        }

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

        private int ItemTextHeight
        {
            get
            {
                return base.FontHeight * 2;
            }
        }

        public AbstractFileListPage(IPageParameter param)
            : base(param)
        {
            this.InitializeComponent();

            this.Font = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
            this.IsShowFileName = FileListPageConfig.Instance.IsShowFileName;
            this.IsShowDirectory = FileListPageConfig.Instance.IsShowDirectory;
            this.IsShowImageFile = FileListPageConfig.Instance.IsShowImageFile;
            this.IsShowOtherFile = FileListPageConfig.Instance.IsShowOtherFile;
            this.ThumbnailSize = FileListPageConfig.Instance.ThumbnailSize;
            this.SetFlowListItemSize();
        }

        public override void RedrawPage()
        {
            this.flowList.SetDrawParameter(true);
            this.Invalidate();
            this.Update();
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
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
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

        protected string[] GetSelectedFiles()
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

        private ToolStripButton GetSortToolStripButton(SortTypeID sortType)
        {
            return sortType switch
            {
                SortTypeID.FileName => this.sortFileNameToolStripButton,
                SortTypeID.FilePath => this.sortFilePathToolStripButton,
                SortTypeID.UpdateDate => this.sortFileUpdateDateToolStripButton,
                SortTypeID.RegistrationDate => this.sortFileRgistrationDateToolStripButton,
                _ => null,
            };
        }

        private void SetSort()
        {
            this.sortFileNameToolStripButton.Image = null;
            this.sortFilePathToolStripButton.Image = null;
            this.sortFileUpdateDateToolStripButton.Image = null;
            this.sortFileRgistrationDateToolStripButton.Image = null;

            var sortButton = this.GetSortToolStripButton(this.SortInfo.ActiveSortType);
            if (sortButton != null)
            {
                var isAscending = this.SortInfo.IsAscending(this.SortInfo.ActiveSortType);
                sortButton.Image = this.SortInfo.GetSortDirectionImage(isAscending);
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

            if (item.ThumbnailImage == null)
            {
                ThumbnailUtil.DrawIcon(e.Graphics, item.JumboIcon, this.GetIconRectangle(e));
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
                        ThumbnailUtil.AdjustDrawFileThumbnail(
                            e.Graphics, item.ThumbnailImage, thumbRect, new SizeF(item.SourceImageWidth, item.SourceImageHeight));
                    }
                }
                else
                {
                    var thumbRect = this.GetThumbnailRectangle(e);
                    if (item.ThumbnailWidth == thumbRect.Width && item.ThumbnailHeight == thumbRect.Height)
                    {
                        ThumbnailUtil.DrawDirectoryThumbnail(e.Graphics, item.ThumbnailImage, thumbRect, item.JumboIcon);
                    }
                    else
                    {
                        ThumbnailUtil.AdjustDrawDirectoryThumbnail(
                            e.Graphics, item.ThumbnailImage, thumbRect, new SizeF(item.SourceImageWidth, item.SourceImageHeight), item.JumboIcon);
                    }
                }

                if (this.IsShowFileName)
                {
                    e.Graphics.DrawString(item.FileName, this.Font, this.flowList.ItemTextBrush, this.GetTextRectangle(e), this.flowList.ItemTextFormat);
                }
            }
        }

        private RectangleF GetIconRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            return new RectangleF(e.ItemRectangle.X,
                                  e.ItemRectangle.Y,
                                  e.ItemRectangle.Width,
                                  e.ItemRectangle.Height - this.ItemTextHeight);
        }

        private RectangleF GetThumbnailRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.IsShowFileName)
            {
                return new RectangleF(e.ItemRectangle.X,
                                     e.ItemRectangle.Y,
                                     e.ItemRectangle.Width,
                                     e.ItemRectangle.Height - this.ItemTextHeight);
            }
            else
            {
                return e.ItemRectangle;
            }
        }

        private RectangleF GetTextRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            return new RectangleF(e.ItemRectangle.X,
                                  e.ItemRectangle.Bottom - this.ItemTextHeight,
                                  e.ItemRectangle.Width,
                                  this.ItemTextHeight);
        }

        private void ConvertFiles(string[] files, FileUtil.ImageFileFormat imageFileFormat)
        {
            if (files.Length == 1)
            {
                this.ConvertSingleFile(files.First(), imageFileFormat);
            }
            else if (files.Length > 1)
            {
                this.ConvertMultipleFile(files, imageFileFormat);
            }
        }

        private void ConvertSingleFile(string file, FileUtil.ImageFileFormat imageFileFormat)
        {
            using (var ofd = new SaveFileDialog())
            {
                var destFileName =
                    $"{FileUtil.GetFileNameWithoutExtension(file)}{FileUtil.GetImageFileExtension(imageFileFormat).ToLower()}";
                var srcFilePath = file;
                ofd.InitialDirectory = CommonConfig.Instance.ExportDirectoryPath;
                ofd.FileName = destFileName;
                ofd.CheckPathExists = true;
                ofd.Filter = FileUtil.GetConvertFilterText(srcFilePath, imageFileFormat);
                ofd.FilterIndex = 0;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var dir = FileUtil.GetParentDirectoryPath(ofd.FileName);
                    var param = new SingleFileConvertParameter
                    {
                        SrcFilePath = srcFilePath,
                        ExportFilePath = Path.Combine(dir, destFileName),
                        ImageFileFormat = imageFileFormat,
                    };
                    Instance<JobCaller>.Value.StartSilgeFileConvertJob(this, param);

                    CommonConfig.Instance.ExportDirectoryPath = dir;
                }
            }
        }

        private void ConvertMultipleFile(string[] files, FileUtil.ImageFileFormat imageFileFormat)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = CommonConfig.Instance.ExportDirectoryPath;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var param = new MultiFilesConvertParameter
                    {
                        SrcFiles = files,
                        ExportDirecotry = fbd.SelectedPath,
                        ImageFileFormat = imageFileFormat,
                    };

                    Instance<JobCaller>.Value.MultiFilesConvertJob.Value
                        .StartJob(this, param, _ =>
                        {
                            if (this.disposed)
                            {
                                return;
                            }

                            this.MultiFilesConvertJob_Callback(_);
                        });
                    CommonConfig.Instance.ExportDirectoryPath = fbd.SelectedPath;
                }
            }
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

        private void MultiFilesExportJob_Callback(ValueResult<string> result)
        {
            ConsoleUtil.Write(result.Value);
        }

        private void MultiFilesConvertJob_Callback(ValueResult<string> result)
        {
            ConsoleUtil.Write(result.Value);
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
                    true => this.flowList.ItemHeight - this.flowList.ItemSpace * 2 - this.ItemTextHeight,
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
                    this.Parameter.VisibleBookmarkMenuItem,
                    this.Parameter.VisibleClipMenuItem);
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
                    this.Parameter.VisibleBookmarkMenuItem,
                    this.Parameter.VisibleClipMenuItem);
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
                    this.Parameter.VisibleBookmarkMenuItem,
                    this.Parameter.VisibleClipMenuItem);
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
                    this.Parameter.VisibleBookmarkMenuItem,
                    this.Parameter.VisibleClipMenuItem);

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
                    this.Parameter.VisibleBookmarkMenuItem,
                    this.Parameter.VisibleClipMenuItem);
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
                this.Parameter.VisibleBookmarkMenuItem,
                this.Parameter.VisibleClipMenuItem);
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
                this.Parameter.VisibleBookmarkMenuItem,
                this.Parameter.VisibleClipMenuItem);
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
                this.Parameter.VisibleBookmarkMenuItem,
                this.Parameter.VisibleClipMenuItem);
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

        private void FileContextMenu_Clip(object sender, ExecuteFileListEventArgs e)
        {
            Instance<JobCaller>.Value.StartClipFilesAddJob(
                this, new ListParameter<string>(e.FilePathList));
        }

        private void FileContextMenu_Bookmark(object sender, ExecuteFileEventArgs e)
        {
            var paramter = new ValueParameter<string>(e.FilePath);

            Instance<JobCaller>.Value.StartBookmarkAddJob(this, paramter);
        }

        private void FileContextMenu_ConvertToAvif(object sender, ExecuteFileListEventArgs e)
        {
            this.ConvertFiles(e.FilePathList, FileUtil.ImageFileFormat.Avif);
        }

        private void FileContextMenu_ConvertToBitmap(object sender, ExecuteFileListEventArgs e)
        {
            this.ConvertFiles(e.FilePathList, FileUtil.ImageFileFormat.Bitmap);
        }

        private void FileContextMenu_ConvertToHeif(object sender, ExecuteFileListEventArgs e)
        {
            this.ConvertFiles(e.FilePathList, FileUtil.ImageFileFormat.Heif);
        }

        private void FileContextMenu_ConvertToIcon(object sender, ExecuteFileListEventArgs e)
        {
            this.ConvertFiles(e.FilePathList, FileUtil.ImageFileFormat.Icon);
        }

        private void FileContextMenu_ConvertToJpeg(object sender, ExecuteFileListEventArgs e)
        {
            this.ConvertFiles(e.FilePathList, FileUtil.ImageFileFormat.Jpeg);
        }

        private void FileContextMenu_ConvertToPng(object sender, ExecuteFileListEventArgs e)
        {
            this.ConvertFiles(e.FilePathList, FileUtil.ImageFileFormat.Png);
        }

        private void FileContextMenu_ConvertToSvg(object sender, ExecuteFileListEventArgs e)
        {
            this.ConvertFiles(e.FilePathList, FileUtil.ImageFileFormat.Svg);
        }

        private void FileContextMenu_ConvertToWebp(object sender, ExecuteFileListEventArgs e)
        {
            this.ConvertFiles(e.FilePathList, FileUtil.ImageFileFormat.Webp);
        }
    }
}
