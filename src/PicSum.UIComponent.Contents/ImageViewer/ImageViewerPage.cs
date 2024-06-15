using NLog;
using PicSum.Core.Base.Conf;
using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Jobs;
using PicSum.Job.Paramters;
using PicSum.Job.Results;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.ContextMenu;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Common;
using SWF.UIComponent.ImagePanel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ImageViewer
{
    /// <summary>
    /// 画像ビューアコンテンツ
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed partial class ImageViewerPage
        : BrowserPage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Size GetImageSize(string filePath)
        {
            try
            {
                return ImageUtil.GetImageSize(filePath);
            }
            catch (ImageUtilException ex)
            {
                Logger.Error(ex);
                return ImageUtil.EMPTY_SIZE;
            }
        }

        private static float GetImageScale(Size imageSize, Size backgroudSize, ImageSizeMode mode)
        {
            if (mode == ImageSizeMode.Original ||
                mode == ImageSizeMode.FitOnlyBigImage
                && imageSize.Width <= backgroudSize.Width
                && imageSize.Height <= backgroudSize.Height)
            {
                return 1.0f;
            }
            else
            {
                var scale = Math.Min(
                    backgroudSize.Width / (float)imageSize.Width,
                    backgroudSize.Height / (float)imageSize.Height);
                return scale;
            }
        }

        #region インスタンス変数

        private readonly ImageViewerPageParameter parameter = null;
        private string leftImageFilePath = string.Empty;
        private string rightImageFilePath = string.Empty;
        private ImageDisplayMode displayMode = ImageDisplayMode.LeftFacing;
        private ImageSizeMode sizeMode = ImageSizeMode.FitOnlyBigImage;
        private IList<string> filePathList = null;

        private TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileGetResult> getImageFileJob = null;
        private TwoWayJob<ImageFileReadJobV2, ImageFileReadParameter, ImageFileGetResultV2> getImageFileJobV2 = null;
        private OneWayJob<BookmarkAddJob, ValueParameter<string>> addBookmarkJob = null;
        private OneWayJob<FileExportJob, ExportFileParameter> exportFileJob = null;
        private OneWayJob<CreateImageCacheJob, ListParameter<string>> createCacheJob = null;

        #endregion

        #region パブリックプロパティ

        public override string SelectedFilePath { get; protected set; } = string.Empty;

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private bool CanOperation
        {
            get
            {
                return this.filePathList != null &&
                    this.filePathList.Count > 0;
            }
        }

        private int MaximumIndex
        {
            get
            {
                return this.indexSlider.MaximumValue;
            }
            set
            {
                this.indexSlider.MaximumValue = value;
            }
        }

        private int FilePathListIndex
        {
            get
            {
                return this.indexSlider.Value;
            }
            set
            {
                this.indexSlider.Value = value;
            }
        }

        private TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileGetResult> GetImageFileJob
        {
            get
            {
                if (this.getImageFileJob == null)
                {
                    this.getImageFileJob = new();
                    this.getImageFileJob
                        .Callback(this.GetImageFileJob_Callback)
                        .Catch(_ =>
                        {
                            this.Cursor = Cursors.Default;
                        })
                        .Complete(() =>
                        {
                            this.Cursor = Cursors.Default;
                            this.CreateImageCache();
                        })
                        .StartThread();
                }

                return this.getImageFileJob;
            }
        }

        private TwoWayJob<ImageFileReadJobV2, ImageFileReadParameter, ImageFileGetResultV2> GetImageFileJobV2
        {
            get
            {
                if (this.getImageFileJobV2 == null)
                {
                    this.getImageFileJobV2 = new();
                    this.getImageFileJobV2
                        .Callback(this.GetImageFileJobV2_Callback)
                        .Catch(_ =>
                        {
                            this.Cursor = Cursors.Default;
                        })
                        .Complete(() =>
                        {
                            this.Cursor = Cursors.Default;
                            this.CreateImageCache();
                        })
                        .StartThread();
                }

                return this.getImageFileJobV2;
            }
        }

        private OneWayJob<BookmarkAddJob, ValueParameter<string>> AddBookmarkJob
        {
            get
            {
                if (this.addBookmarkJob == null)
                {
                    this.addBookmarkJob = new();
                    this.addBookmarkJob
                        .StartThread();
                }

                return this.addBookmarkJob;
            }
        }

        private OneWayJob<FileExportJob, ExportFileParameter> ExportFileJob
        {
            get
            {
                if (this.exportFileJob == null)
                {
                    this.exportFileJob = new();
                    this.exportFileJob
                        .StartThread();
                }

                return this.exportFileJob;
            }
        }

        private OneWayJob<CreateImageCacheJob, ListParameter<string>> CreateCacheJob
        {
            get
            {
                if (this.createCacheJob == null)
                {
                    this.createCacheJob = new();
                    this.createCacheJob
                        .StartThread();
                }

                return this.createCacheJob;
            }
        }

        #endregion

        #region コンストラクタ

        public ImageViewerPage(ImageViewerPageParameter parameter)
            : base(parameter)
        {
            this.InitializeComponent();
            this.SubInitializeComponent();

            this.parameter = parameter;
        }

        #endregion

        #region パブリックメソッド

        public override void RedrawPage()
        {
            this.ChangeImagePanelSize();

            Size backgroudSize;
            if (this.leftImagePanel.HasImage && this.rightImagePanel.HasImage)
            {
                backgroudSize = new Size(
                    (int)(this.checkPatternPanel.Size.Width / 2f),
                    this.checkPatternPanel.Size.Height);
            }
            else
            {
                backgroudSize = this.checkPatternPanel.Size;
            }

            if (this.leftImagePanel.HasImage)
            {
                var leftImageScale = GetImageScale(
                    this.leftImagePanel.ImageSize, backgroudSize, this.sizeMode);
                this.leftImagePanel.SetScale(leftImageScale);
            }

            if (this.rightImagePanel.HasImage)
            {
                var rightImageScale = GetImageScale(
                    this.rightImagePanel.ImageSize, backgroudSize, this.sizeMode);
                this.rightImagePanel.SetScale(rightImageScale);
            }

            this.leftImagePanel.Invalidate();
            this.rightImagePanel.Invalidate();
        }

        #endregion

        #region 継承メソッド

        protected override void OnResize(EventArgs e)
        {
            this.RedrawPage();

            base.OnResize(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.getImageFileJob != null)
                {
                    this.getImageFileJob.Dispose();
                    this.getImageFileJob = null;
                }

                if (this.addBookmarkJob != null)
                {
                    this.addBookmarkJob.Dispose();
                    this.addBookmarkJob = null;
                }

                if (this.exportFileJob != null)
                {
                    this.exportFileJob.Dispose();
                    this.exportFileJob = null;
                }

                this.leftImagePanel.Dispose();
                this.rightImagePanel.Dispose();

                this.parameter.SelectedFilePath = this.SelectedFilePath;
                this.parameter.GetImageFiles -= this.Parameter_GetImageFiles;

                this.components?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.parameter.GetImageFiles += this.Parameter_GetImageFiles;
            this.parameter.ImageFilesGetAction(this.parameter)();
            base.OnLoad(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            if (e.Delta > 0)
            {
                this.FilePathListIndex = this.GetPreviewIndex(this.FilePathListIndex, false);
            }
            else
            {
                this.FilePathListIndex = this.GetNextIndex(this.FilePathListIndex, false);
            }

            base.OnMouseWheel(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (!this.CanOperation)
            {
                return false;
            }

            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                return false;
            }

            if ((keyData & Keys.KeyCode) == Keys.Right)
            {
                this.FilePathListIndex = this.GetNextIndex(this.FilePathListIndex, false);
                return true;
            }
            else if ((keyData & Keys.KeyCode) == Keys.Left)
            {
                this.FilePathListIndex = this.GetPreviewIndex(this.FilePathListIndex, false);
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnDrawTabPage(SWF.UIComponent.TabOperation.DrawTabEventArgs e)
        {
            e.Graphics.DrawImage(this.Icon, e.IconRectangle);
            DrawTextUtil.DrawText(e.Graphics, this.Title, e.Font, e.TextRectangle, e.TitleColor, e.TitleFormatFlags, e.TextStyle);
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            // 処理無し。
        }

        #endregion

        #region プライベートメソッド

        private void SubInitializeComponent()
        {
            this.SetDisplayMode(ImageViewerPageConfig.ImageDisplayMode);
            this.SetSizeMode(ImageViewerPageConfig.ImageSizeMode);
            this.SetThumbnailPanelVisible();
        }

        private void CreateImageCache()
        {
            var nextFiles = new List<string>(5);
            var nextIndex = this.GetNextIndex(this.FilePathListIndex, true);
            nextFiles.Add(this.filePathList[nextIndex]);
            while (nextFiles.Count < nextFiles.Capacity)
            {
                nextIndex = this.GetNextIndex(nextIndex, true);
                nextFiles.Add(this.filePathList[nextIndex]);
            }

            var prevFiles = new List<string>(5);
            var prevIndex = this.GetPreviewIndex(this.FilePathListIndex, true);
            prevFiles.Add(this.filePathList[prevIndex]);
            while (prevFiles.Count < prevFiles.Capacity)
            {
                prevIndex = this.GetPreviewIndex(prevIndex, true);
                prevFiles.Add(this.filePathList[prevIndex]);
            }

            var parameter = new ListParameter<string>(nextFiles.Concat(prevFiles));
            this.CreateCacheJob.StartJob(parameter);
        }

        private void ChangeImagePanelSize()
        {
            this.leftImagePanel.Visible = false;
            this.rightImagePanel.Visible = false;

            if ((this.leftImagePanel.HasImage || this.leftImagePanel.IsError)
                && (this.rightImagePanel.HasImage || this.rightImagePanel.IsError))
            {
                var w = (int)(this.checkPatternPanel.Width / 2f);
                var h = this.checkPatternPanel.Height;
                var lx = 0;
                var rx = this.checkPatternPanel.Width - w;
                var y = 0;

                this.leftImagePanel.SetBounds(lx, y, w, h, BoundsSpecified.All);
                this.rightImagePanel.SetBounds(rx, y, w, h, BoundsSpecified.All);

                this.leftImagePanel.ImageAlign = ImageAlign.Right;
                this.rightImagePanel.ImageAlign = ImageAlign.Left;

                this.leftImagePanel.Visible = true;
                this.rightImagePanel.Visible = true;
            }
            else if (this.leftImagePanel.HasImage || this.leftImagePanel.IsError)
            {
                var w = this.checkPatternPanel.Width;
                var h = this.checkPatternPanel.Height;
                var x = 0;
                var y = 0;

                this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                this.leftImagePanel.ImageAlign = ImageAlign.Center;

                this.leftImagePanel.Visible = true;
                this.rightImagePanel.Visible = false;
            }
            else if (this.rightImagePanel.HasImage || this.rightImagePanel.IsError)
            {
                var w = this.checkPatternPanel.Width;
                var h = this.checkPatternPanel.Height;
                var x = 0;
                var y = 0;

                this.rightImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                this.rightImagePanel.ImageAlign = ImageAlign.Center;

                this.leftImagePanel.Visible = false;
                this.rightImagePanel.Visible = true;
            }
        }

        private int GetNextIndex(int currentIndex, bool isForceSingle)
        {
            if (isForceSingle || this.displayMode == ImageDisplayMode.Single)
            {
                if (currentIndex == this.MaximumIndex)
                {
                    return 0;
                }
                else
                {
                    return currentIndex + 1;
                }
            }
            else
            {
                var currentFilePath = this.filePathList[currentIndex];
                var currentImageSize = GetImageSize(currentFilePath);
                if (currentImageSize.Width < currentImageSize.Height)
                {
                    var nextIndex = currentIndex + 1;
                    if (nextIndex > this.MaximumIndex)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = this.filePathList[nextIndex];
                    var nextImageSize = GetImageSize(nextFilePath);
                    if (nextImageSize.Width < nextImageSize.Height)
                    {
                        if (nextIndex == this.MaximumIndex)
                        {
                            return 0;
                        }
                        else
                        {
                            return nextIndex + 1;
                        }
                    }
                    else
                    {
                        return nextIndex;
                    }
                }
                else
                {
                    if (currentIndex == this.MaximumIndex)
                    {
                        return 0;
                    }
                    else
                    {
                        return currentIndex + 1;
                    }
                }
            }
        }

        private int GetPreviewIndex(int currentIndex, bool isSingle)
        {
            if (this.displayMode == ImageDisplayMode.Single || isSingle)
            {
                if (currentIndex == 0)
                {
                    return this.MaximumIndex;
                }
                else
                {
                    return currentIndex - 1;
                }
            }
            else
            {
                var prevIndex1 = currentIndex - 1;
                if (prevIndex1 < 0)
                {
                    prevIndex1 = this.MaximumIndex;
                }

                var prevFilePath1 = this.filePathList[prevIndex1];
                var prevImageSize1 = GetImageSize(prevFilePath1);
                if (prevImageSize1.Width < prevImageSize1.Height)
                {
                    var prevIndex2 = prevIndex1 - 1;
                    if (prevIndex2 < 0)
                    {
                        prevIndex2 = this.MaximumIndex;
                    }

                    var prevFilePath2 = this.filePathList[prevIndex2];
                    var prevImageSize2 = GetImageSize(prevFilePath2);
                    if (prevImageSize2.Width < prevImageSize2.Height)
                    {
                        return prevIndex2;
                    }
                    else
                    {
                        return prevIndex1;
                    }
                }
                else
                {
                    return prevIndex1;
                }
            }
        }

        private void ReadImage()
        {
            this.Cursor = Cursors.WaitCursor;

            var param = new ImageFileReadParameter
            {
                CurrentIndex = this.FilePathListIndex,
                FilePathList = this.filePathList,
                ImageDisplayMode = this.displayMode,
                ImageSizeMode = this.sizeMode,
                ThumbnailSize = this.leftImagePanel.ThumbnailSize
            };

            this.GetImageFileJob.StartJob(param);
            //this.GetImageFileJobV2.StartJob(param);
        }

        private void DoDragDrop(string currentFilePath)
        {
            var dragData = new DragEntity(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                currentFilePath,
                this.parameter.SortInfo,
                this.parameter.ImageFilesGetAction,
                this.parameter.PageTitle,
                this.parameter.PageIcon);
            this.DoDragDrop(dragData, DragDropEffects.All);
        }

        private bool SetDisplayMode(ImageDisplayMode mode)
        {
            this.singleViewToolStripMenuItem.Checked = false;
            this.leftFacingViewToolStripMenuItem.Checked = false;
            this.rightFacingViewToolStripMenuItem.Checked = false;

            switch (mode)
            {
                case ImageDisplayMode.Single:
                    this.singleViewToolStripMenuItem.Checked = true;
                    this.doublePreviewIndexToolStripButton.Enabled = false;
                    this.doubleNextIndexToolStripButton.Enabled = false;
                    break;
                case ImageDisplayMode.LeftFacing:
                    this.leftFacingViewToolStripMenuItem.Checked = true;
                    this.doublePreviewIndexToolStripButton.Enabled = true;
                    this.doubleNextIndexToolStripButton.Enabled = true;
                    break;
                case ImageDisplayMode.RightFacing:
                    this.rightFacingViewToolStripMenuItem.Checked = true;
                    this.doublePreviewIndexToolStripButton.Enabled = true;
                    this.doubleNextIndexToolStripButton.Enabled = true;
                    break;
            }

            if (this.displayMode != mode)
            {
                this.displayMode = mode;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SetSizeMode(ImageSizeMode mode)
        {
            this.originalSizeToolStripMenuItem.Checked = mode == ImageSizeMode.Original;
            this.allFitSizeToolStripMenuItem.Checked = mode == ImageSizeMode.FitAllImage;
            this.onlyBigImageFitSizeToolStripMenuItem.Checked = mode == ImageSizeMode.FitOnlyBigImage;

            if (this.sizeMode != mode)
            {
                this.sizeMode = mode;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetThumbnailPanelVisible()
        {
            this.leftImagePanel.IsShowThumbnailPanel = this.sizeMode == ImageSizeMode.Original;
            this.rightImagePanel.IsShowThumbnailPanel = this.leftImagePanel.IsShowThumbnailPanel;
        }

        #endregion

        #region プロセスイベント

        private void Parameter_GetImageFiles(object sender, GetImageFilesEventArgs e)
        {
            this.filePathList = e.FilePathList;

            this.Title = e.PageTitle;
            this.Icon = e.PageIcon;

            if (this.filePathList.Count > 0)
            {
                this.MaximumIndex = this.filePathList.Count - 1;
            }
            else
            {
                this.MaximumIndex = 0;
            }

            var selectedFilePath = this.parameter.SelectedFilePath != string.Empty ?
                this.parameter.SelectedFilePath : e.SelectedFilePath;
            var index = this.filePathList.IndexOf(selectedFilePath);
            if (index < 0)
            {
                this.FilePathListIndex = 0;
            }
            else
            {
                this.FilePathListIndex = index;
            }
        }

        private void GetImageFileJob_Callback(ImageFileGetResult e)
        {
            this.leftImageFilePath = string.Empty;
            this.leftImagePanel.ClearImage();
            this.rightImageFilePath = string.Empty;
            this.rightImagePanel.ClearImage();

            var index = this.filePathList.IndexOf(e.Image1.FilePath);
            if (index != this.FilePathListIndex)
            {
                return;
            }

            Size bgSize;
            if (e.Image1 != null && e.Image2 != null)
            {
                bgSize = new Size(
                    (int)(this.checkPatternPanel.Size.Width / 2f),
                    this.checkPatternPanel.Size.Height);
            }
            else
            {
                bgSize = this.checkPatternPanel.Size;
            }

            if (e.Image2 == null)
            {
                if (this.SelectedFilePath != e.Image1.FilePath)
                {
                    this.SelectedFilePath = e.Image1.FilePath;
                    this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(e.Image1.FilePath));
                }
            }
            else
            {
                if (this.SelectedFilePath != e.Image1.FilePath
                    && this.SelectedFilePath != e.Image2.FilePath)
                {
                    this.SelectedFilePath = e.Image1.FilePath;
                    this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(e.Image1.FilePath));
                }
            }

            if (this.displayMode == ImageDisplayMode.Single)
            {
                this.leftImageFilePath = e.Image1.FilePath;
                if (e.Image1.IsError)
                {
                    this.leftImagePanel.SetError();
                }
                else
                {
                    this.leftImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
                    var scale = GetImageScale(
                        this.leftImagePanel.ImageSize, bgSize, this.sizeMode);
                    this.leftImagePanel.SetScale(scale);
                }
            }
            else if (this.displayMode == ImageDisplayMode.LeftFacing)
            {
                this.leftImageFilePath = e.Image1.FilePath;
                if (e.Image1.IsError)
                {
                    this.leftImagePanel.SetError();
                }
                else
                {
                    this.leftImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
                    var leftImageScale = GetImageScale(
                        this.leftImagePanel.ImageSize, bgSize, this.sizeMode);
                    this.leftImagePanel.SetScale(leftImageScale);
                }

                if (e.Image2 != null)
                {
                    this.rightImageFilePath = e.Image2.FilePath;
                    if (e.Image2.IsError)
                    {
                        this.rightImagePanel.SetError();
                    }
                    else
                    {
                        this.rightImagePanel.SetImage(e.Image2.Image, e.Image2.Thumbnail);
                        var rightImageScale = GetImageScale(
                            this.rightImagePanel.ImageSize, bgSize, this.sizeMode);
                        this.rightImagePanel.SetScale(rightImageScale);
                    }
                }
            }
            else if (this.displayMode == ImageDisplayMode.RightFacing)
            {
                this.rightImageFilePath = e.Image1.FilePath;
                if (e.Image1.IsError)
                {
                    this.rightImagePanel.SetError();
                }
                else
                {
                    this.rightImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
                    var rightImageScale = GetImageScale(
                        this.rightImagePanel.ImageSize, bgSize, this.sizeMode);
                    this.rightImagePanel.SetScale(rightImageScale);
                }

                if (e.Image2 != null)
                {
                    this.leftImageFilePath = e.Image2.FilePath;
                    if (e.Image2.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(e.Image2.Image, e.Image2.Thumbnail);
                        var leftImageScale = GetImageScale(
                            this.leftImagePanel.ImageSize, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(leftImageScale);
                    }
                }
            }

            this.ChangeImagePanelSize();
            this.Focus();
        }

        private void GetImageFileJobV2_Callback(ImageFileGetResultV2 e)
        {
            //if (index != this.FilePathListIndex)
            //{
            //    this.leftImageFilePath = string.Empty;
            //    this.leftImagePanel.ClearImage();
            //    this.rightImageFilePath = string.Empty;
            //    this.rightImagePanel.ClearImage();
            //    return;
            //}

            Size bgSize;
            if (e.HasSub)
            {
                bgSize = new Size(
                    (int)(this.checkPatternPanel.Size.Width / 2f),
                    this.checkPatternPanel.Size.Height);
            }
            else
            {
                bgSize = this.checkPatternPanel.Size;
            }

            if (e.IsMain)
            {
                if (this.SelectedFilePath != e.Image.FilePath)
                {
                    this.SelectedFilePath = e.Image.FilePath;
                    this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(e.Image.FilePath));
                }
            }

            if (this.displayMode == ImageDisplayMode.Single)
            {
                this.leftImagePanel.ClearImage();
                this.rightImagePanel.ClearImage();
                this.leftImageFilePath = e.Image.FilePath;
                this.rightImageFilePath = string.Empty;

                if (e.Image.IsError)
                {
                    this.leftImagePanel.SetError();
                }
                else
                {
                    this.leftImagePanel.SetImage(e.Image.Image, e.Image.Thumbnail);
                    var scale = GetImageScale(
                        this.leftImagePanel.ImageSize, bgSize, this.sizeMode);
                    this.leftImagePanel.SetScale(scale);
                }
            }
            else if (this.displayMode == ImageDisplayMode.LeftFacing)
            {
                if (e.IsMain)
                {
                    this.leftImagePanel.ClearImage();
                    this.leftImageFilePath = e.Image.FilePath;
                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(e.Image.Image, e.Image.Thumbnail);
                        var leftImageScale = GetImageScale(
                            this.leftImagePanel.ImageSize, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(leftImageScale);
                    }
                }
                else
                {
                    this.rightImagePanel.ClearImage();
                    this.rightImageFilePath = e.Image.FilePath;
                    if (e.Image.IsError)
                    {
                        this.rightImagePanel.SetError();
                    }
                    else
                    {
                        this.rightImagePanel.SetImage(e.Image.Image, e.Image.Thumbnail);
                        var rightImageScale = GetImageScale(
                            this.rightImagePanel.ImageSize, bgSize, this.sizeMode);
                        this.rightImagePanel.SetScale(rightImageScale);
                    }
                }
            }
            else if (this.displayMode == ImageDisplayMode.RightFacing)
            {
                if (e.IsMain)
                {
                    this.rightImagePanel.ClearImage();
                    this.rightImageFilePath = e.Image.FilePath;
                    if (e.Image.IsError)
                    {
                        this.rightImagePanel.SetError();
                    }
                    else
                    {
                        this.rightImagePanel.SetImage(e.Image.Image, e.Image.Thumbnail);
                        var rightImageScale = GetImageScale(
                            this.rightImagePanel.ImageSize, bgSize, this.sizeMode);
                        this.rightImagePanel.SetScale(rightImageScale);
                    }
                }
                else
                {
                    this.leftImagePanel.ClearImage();
                    this.leftImageFilePath = e.Image.FilePath;
                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(e.Image.Image, e.Image.Thumbnail);
                        var leftImageScale = GetImageScale(
                            this.leftImagePanel.ImageSize, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(leftImageScale);
                    }
                }
            }
            else
            {
                this.leftImageFilePath = string.Empty;
                this.leftImagePanel.ClearImage();
                this.rightImageFilePath = string.Empty;
                this.rightImagePanel.ClearImage();
            }

            this.ChangeImagePanelSize();
            this.Focus();
        }

        #endregion

        #region ツールバーイベント

        private void SingleViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (!this.CanOperation)
            {
                return;
            }

            if (this.SetDisplayMode(ImageDisplayMode.Single))
            {
                ImageViewerPageConfig.ImageDisplayMode = this.displayMode;
                this.ReadImage();
            }
        }

        private void LeftFacingViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (!this.CanOperation)
            {
                return;
            }

            if (this.SetDisplayMode(ImageDisplayMode.LeftFacing))
            {
                ImageViewerPageConfig.ImageDisplayMode = this.displayMode;
                this.ReadImage();
            }
        }

        private void RightFacingViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (!this.CanOperation)
            {
                return;
            }

            if (this.SetDisplayMode(ImageDisplayMode.RightFacing))
            {
                ImageViewerPageConfig.ImageDisplayMode = this.displayMode;
                this.ReadImage();
            }
        }

        private void OriginalSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (!this.CanOperation)
            {
                return;
            }

            if (this.SetSizeMode(ImageSizeMode.Original))
            {
                ImageViewerPageConfig.ImageSizeMode = this.sizeMode;
                this.SetThumbnailPanelVisible();
                this.ReadImage();
            }
        }

        private void AllFitSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (!this.CanOperation)
            {
                return;
            }

            if (this.SetSizeMode(ImageSizeMode.FitAllImage))
            {
                ImageViewerPageConfig.ImageSizeMode = this.sizeMode;
                this.SetThumbnailPanelVisible();
                this.ReadImage();
            }
        }

        private void OnlyBigImageFitSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (!this.CanOperation)
            {
                return;
            }

            if (this.SetSizeMode(ImageSizeMode.FitOnlyBigImage))
            {
                ImageViewerPageConfig.ImageSizeMode = this.sizeMode;
                this.SetThumbnailPanelVisible();
                this.ReadImage();
            }
        }

        private void DoublePreviewIndexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.FilePathListIndex = this.GetPreviewIndex(this.FilePathListIndex, false);
        }

        private void DoubleNextIndexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.FilePathListIndex = this.GetNextIndex(this.FilePathListIndex, false);
        }

        private void SinglePreviewIndexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.FilePathListIndex = this.GetPreviewIndex(this.FilePathListIndex, true);
        }

        private void SingleNextIndexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.FilePathListIndex = this.GetNextIndex(this.FilePathListIndex, true);
        }

        private void IndexSlider_ValueChanging(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            var index = this.FilePathListIndex;
            if (index < 0 || this.filePathList.Count - 1 < index)
            {
                return;
            }

            var filePath = this.filePathList[index];
            var p = this.PointToClient(Cursor.Position);
            this.filePathToolTip.Show(filePath, this, p.X, -16, 5000);
        }

        private void IndexSlider_ValueChanged(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.filePathToolTip.Hide(this);
            this.ReadImage();
        }

        #endregion

        #region 画像パネルイベント

        private void LeftImagePanel_MouseDown(object sender, MouseEventArgs e)
        {
            this.leftImagePanel.Focus();
        }

        private void RightImagePanel_MouseDown(object sender, MouseEventArgs e)
        {
            this.rightImagePanel.Focus();
        }

        private void RightImagePanel_MouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        private void LeftImagePanel_MouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        private void LeftImagePanel_ImageMouseClick(object sender, MouseEventArgs e)
        {
            if (this.SelectedFilePath != this.leftImageFilePath)
            {
                this.SelectedFilePath = this.leftImageFilePath;
                this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.leftImageFilePath));
            }
        }

        private void RightImagePanel_ImageMouseClick(object sender, MouseEventArgs e)
        {
            if (this.SelectedFilePath != this.rightImageFilePath)
            {
                this.SelectedFilePath = this.rightImageFilePath;
                this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.rightImageFilePath));
            }
        }

        private void LeftImagePanel_DragStart(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.leftImageFilePath))
            {
                this.DoDragDrop(this.leftImageFilePath);
            }
        }

        private void RightImagePanel_DragStart(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.rightImageFilePath))
            {
                this.DoDragDrop(this.rightImageFilePath);
            }
        }

        #endregion

        #region ファイルコンテキストメニューイベント

        private void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (this.fileContextMenu.SourceControl.Equals(this.leftImagePanel))
            {
                if (!string.IsNullOrEmpty(this.leftImageFilePath))
                {
                    if (this.SelectedFilePath != this.leftImageFilePath)
                    {
                        this.SelectedFilePath = this.leftImageFilePath;
                        this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.leftImageFilePath));
                    }

                    this.fileContextMenu.SetFile(this.leftImageFilePath);
                    return;
                }
            }
            else if (this.fileContextMenu.SourceControl.Equals(this.rightImagePanel))
            {
                if (!string.IsNullOrEmpty(this.rightImageFilePath))
                {
                    if (this.SelectedFilePath != this.rightImageFilePath)
                    {
                        this.SelectedFilePath = this.rightImageFilePath;
                        this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.rightImageFilePath));
                    }

                    this.fileContextMenu.SetFile(this.rightImageFilePath);
                    return;
                }
            }

            e.Cancel = true;
        }

        private void FileContextMenu_FileNewTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewerPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this.parameter.ImageFilesGetAction,
                e.FilePath,
                this.parameter.SortInfo,
                this.Title,
                this.Icon);
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.AddTab, param));
        }

        private void FileContextMenu_FileNewWindowOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewerPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this.parameter.ImageFilesGetAction,
                e.FilePath,
                this.parameter.SortInfo,
                this.Title,
                this.Icon);
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.NewWindow, param));
        }

        private void FileContextMenu_FileOpen(object sender, ExecuteFileEventArgs e)
        {
            FileUtil.OpenFile(e.FilePath);
        }

        private void FileContextMenu_SaveDirectoryOpen(object sender, ExecuteFileEventArgs e)
        {
            FileUtil.OpenExplorerSelect(e.FilePath);
        }

        private void FileContextMenu_Export(object sender, ExecuteFileListEventArgs e)
        {
            using (var ofd = new SaveFileDialog())
            {
                var srcFilePath = e.FilePathList.First();
                ofd.InitialDirectory = CommonConfig.ExportDirectoryPath;
                ofd.FileName = FileUtil.GetExportFileName(
                    ofd.InitialDirectory,
                    srcFilePath);
                ofd.CheckFileExists = false;
                ofd.Filter = FileUtil.GetExportFilterText(srcFilePath);
                ofd.FilterIndex = 0;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var dir = FileUtil.GetParentDirectoryPath(ofd.FileName);
                    var param = new ExportFileParameter
                    {
                        SrcFilePath = srcFilePath,
                        ExportFilePath = ofd.FileName
                    };
                    this.ExportFileJob.StartJob(param);

                    CommonConfig.ExportDirectoryPath = dir;
                }
            }
        }

        private void FileContextMenu_PathCopy(object sender, ExecuteFileListEventArgs e)
        {
            Clipboard.SetText(e.FilePathList[0]);
        }

        private void FileContextMenu_NameCopy(object sender, ExecuteFileListEventArgs e)
        {
            Clipboard.SetText(FileUtil.GetFileName(e.FilePathList[0]));
        }

        private void FileContextMenu_Bookmark(object sender, ExecuteFileEventArgs e)
        {
            var paramter = new ValueParameter<string>(e.FilePath);

            this.AddBookmarkJob.StartJob(paramter);
        }

        #endregion
    }
}
