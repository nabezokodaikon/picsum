using NLog;
using PicSum.Job.Jobs;
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
using SWF.UIComponent.TabOperation;
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

        private static float GetImageScale(SizeF imageSize, SizeF backgroudSize, ImageSizeMode mode)
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
                    backgroudSize.Width / imageSize.Width,
                    backgroudSize.Height / imageSize.Height);
                return scale;
            }
        }

        private bool disposing = false;

        private readonly ImageViewerPageParameter parameter = null;
        private ImageDisplayMode displayMode = ImageDisplayMode.LeftFacing;
        private ImageSizeMode sizeMode = ImageSizeMode.FitOnlyBigImage;
        private IList<string> filePathList = null;
        private bool isLoading = false;

        private TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult> imageFileReadJob = null;
        private TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult> imageFileLoadingJob = null;
        private OneWayJob<BookmarkAddJob, ValueParameter<string>> addBookmarkJob = null;
        private OneWayJob<SingleFileExportJob, SingleFileExportParameter> singleFileExportJob = null;
        private OneWayJob<ImageFileCacheJob, ListParameter<string>> imageFileCacheJob = null;

        public override string SelectedFilePath { get; protected set; } = string.Empty;

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

        private TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileReadResult> ImageFileReadJob
        {
            get
            {
                if (this.imageFileReadJob == null)
                {
                    this.imageFileReadJob = new();
                    this.imageFileReadJob
                        .Callback(r =>
                        {
                            if (this.disposing)
                            {
                                return;
                            }

                            using (TimeMeasuring.Run(true, "ImageViewerPage.ImageFileReadJob_Callback"))
                            {
                                this.ImageFileReadJob_Callback(r);
                            }
                        })
                        .Cancel(() =>
                        {
                            ConsoleUtil.Write($"ImageViewerPage.ImageFileReadJob.Cancel");
                        })
                        .Complete(() =>
                        {
                            ConsoleUtil.Write($"ImageViewerPage.ImageFileReadJob.Complete");
                        });
                }

                return this.imageFileReadJob;
            }
        }

        private TwoWayJob<ImageFileLoadingJob, ImageFileReadParameter, ImageFileReadResult> ImageFileLoadingJob
        {
            get
            {
                if (this.imageFileLoadingJob == null)
                {
                    this.imageFileLoadingJob = new();
                    this.imageFileLoadingJob
                        .Callback(_ =>
                        {
                            if (this.disposing)
                            {
                                return;
                            }

                            if (!this.isLoading)
                            {
                                return;
                            }

                            this.ImageFileReadJob_Callback(_);
                        });
                }

                return this.imageFileLoadingJob;
            }
        }

        private OneWayJob<BookmarkAddJob, ValueParameter<string>> AddBookmarkJob
        {
            get
            {
                this.addBookmarkJob ??= new();
                return this.addBookmarkJob;
            }
        }

        private OneWayJob<SingleFileExportJob, SingleFileExportParameter> SingleFileExportJob
        {
            get
            {
                this.singleFileExportJob ??= new();
                return this.singleFileExportJob;
            }
        }

        private OneWayJob<ImageFileCacheJob, ListParameter<string>> ImageFileCacheJob
        {
            get
            {
                this.imageFileCacheJob ??= new();
                return this.imageFileCacheJob;
            }
        }

        public ImageViewerPage(ImageViewerPageParameter parameter)
            : base(parameter)
        {
            this.InitializeComponent();

            this.checkPatternPanel.Resize += this.CheckPatternPanel_Resize;

            this.SetDisplayMode(ImageViewerPageConfig.ImageDisplayMode);
            this.SetSizeMode(ImageViewerPageConfig.ImageSizeMode);
            this.SetThumbnailPanelVisible();

            this.parameter = parameter;
            this.SelectedFilePath = parameter.SelectedFilePath;
        }

        public override void RedrawPage()
        {
            using (TimeMeasuring.Run(true, "ImageViewerPage.RedrawPage"))
            {
                SizeF backgroudSize;
                if (this.leftImagePanel.HasImage && this.rightImagePanel.HasImage)
                {
                    backgroudSize = new SizeF(
                        this.checkPatternPanel.Size.Width / 2f,
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

                if (this.displayMode == ImageDisplayMode.LeftFacing)
                {
                    if (this.leftImagePanel.HasImage
                        && this.rightImagePanel.HasImage)
                    {
                        this.SettingImagePanelsLayout();
                    }
                    else
                    {
                        this.SettingLeftImagePanelLayout();
                    }
                }
                else if (this.displayMode == ImageDisplayMode.RightFacing)
                {
                    if (this.leftImagePanel.HasImage
                        && this.rightImagePanel.HasImage)
                    {
                        this.SettingImagePanelsLayout();
                    }
                    else
                    {
                        this.SettingLeftImagePanelLayout();
                    }
                }
                else if (this.displayMode == ImageDisplayMode.Single)
                {
                    this.SettingLeftImagePanelLayout();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.disposing = true;

                this.addBookmarkJob?.Dispose();
                this.addBookmarkJob = null;

                this.singleFileExportJob?.Dispose();
                this.singleFileExportJob = null;

                this.imageFileReadJob?.Dispose();
                this.imageFileReadJob = null;

                this.imageFileCacheJob?.Dispose();
                this.imageFileCacheJob = null;

                this.imageFileLoadingJob?.Dispose();
                this.imageFileLoadingJob = null;

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
            if (this.disposing)
            {
                return;
            }

            this.parameter.GetImageFiles += this.Parameter_GetImageFiles;
            this.parameter.ImageFilesGetAction(this.parameter)();
            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            this.indexSlider.Width = this.ClientSize.Width - 390;

            base.OnResize(e);
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

        protected override void OnDrawTabPage(DrawTabEventArgs e)
        {
            e.Graphics.DrawImage(this.Icon, e.IconRectangle);
            DrawTextUtil.DrawText(
                e.Graphics, this.Title, e.Font,
                new Rectangle(
                    (int)e.TextRectangle.X,
                    (int)e.TextRectangle.Y,
                    (int)e.TextRectangle.Width,
                    (int)e.TextRectangle.Height),
                e.TitleColor, e.TitleFormatFlags);
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            // 処理無し。
        }

        private Size GetImageSize(string filePath)
        {
            try
            {
                using (TimeMeasuring.Run(true, "ImageViewerPage.GetImageSize"))
                {
                    return ImageFileSizeCacheUtil.Get(filePath).Size;
                }
            }
            catch (FileUtilException ex)
            {
                Logger.Error(ex);
                return new Size((int)(this.checkPatternPanel.Size.Width / 2f), this.checkPatternPanel.Size.Height);
            }
            catch (ImageUtilException ex)
            {
                Logger.Error(ex);
                return new Size((int)(this.checkPatternPanel.Size.Width / 2f), this.checkPatternPanel.Size.Height);
            }
        }

        private void CheckPatternPanel_Resize(object sender, EventArgs e)
        {
            this.RedrawPage();
        }

        private void BeginResumeLayout(bool isLeft, bool isRight, Action action)
        {
            if (isLeft && isRight)
            {
                this.leftImagePanel.SuspendLayout();
                this.rightImagePanel.SuspendLayout();
                this.checkPatternPanel.SuspendLayout();
                this.SuspendLayout();

                action();

                this.leftImagePanel.ResumeLayout(false);
                this.leftImagePanel.PerformLayout();
                this.rightImagePanel.ResumeLayout(false);
                this.rightImagePanel.PerformLayout();
                this.checkPatternPanel.ResumeLayout(false);
                this.checkPatternPanel.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
                this.leftImagePanel.Invalidate();
                this.leftImagePanel.Update();
                this.rightImagePanel.Invalidate();
                this.rightImagePanel.Update();
            }
            else if (isLeft)
            {
                this.leftImagePanel.SuspendLayout();
                this.checkPatternPanel.SuspendLayout();
                this.SuspendLayout();

                action();

                this.leftImagePanel.ResumeLayout(false);
                this.leftImagePanel.PerformLayout();
                this.checkPatternPanel.ResumeLayout(false);
                this.checkPatternPanel.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
                this.leftImagePanel.Invalidate();
                this.leftImagePanel.Update();
            }
            else if (isRight)
            {
                this.rightImagePanel.SuspendLayout();
                this.checkPatternPanel.SuspendLayout();
                this.SuspendLayout();

                action();

                this.rightImagePanel.ResumeLayout(false);
                this.rightImagePanel.PerformLayout();
                this.checkPatternPanel.ResumeLayout(false);
                this.checkPatternPanel.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
                this.rightImagePanel.Invalidate();
                this.rightImagePanel.Update();
            }
        }

        private void SettingLeftImagePanelLayout()
        {
            this.BeginResumeLayout(true, false, () =>
            {
                var w = this.checkPatternPanel.Width;
                var h = this.checkPatternPanel.Height;
                var x = 0;
                var y = 0;

                this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                this.leftImagePanel.ImageAlign = ImageAlign.Center;
                this.leftImagePanel.Visible = true;

                this.rightImagePanel.Visible = false;
            });
        }

        private void SettingImagePanelsLayout()
        {
            this.BeginResumeLayout(true, true, () =>
            {
                var w = (int)(this.checkPatternPanel.Width / 2f);
                var h = this.checkPatternPanel.Height;
                var lx = 0;
                var rx = this.checkPatternPanel.Width - w;
                var y = 0;

                this.leftImagePanel.SetBounds(lx, y, w, h, BoundsSpecified.All);
                this.leftImagePanel.ImageAlign = ImageAlign.Right;
                this.leftImagePanel.Visible = true;

                this.rightImagePanel.SetBounds(rx, y, w, h, BoundsSpecified.All);
                this.rightImagePanel.ImageAlign = ImageAlign.Left;
                this.rightImagePanel.Visible = true;
            });
        }

        private void SettingImagePanelLayout(ImageFileReadResult e)
        {
            if (e.IsMain && e.HasSub)
            {
                var w = (int)(this.checkPatternPanel.Width / 2f);
                var h = this.checkPatternPanel.Height;
                var y = 0;

                if (this.displayMode == ImageDisplayMode.LeftFacing)
                {
                    this.BeginResumeLayout(true, false, () =>
                    {
                        var x = 0;
                        this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                        this.leftImagePanel.ImageAlign = ImageAlign.Right;
                        this.leftImagePanel.Visible = true;
                    });
                }
                else
                {
                    this.BeginResumeLayout(false, true, () =>
                    {
                        var x = this.checkPatternPanel.Width - w;
                        this.rightImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                        this.rightImagePanel.ImageAlign = ImageAlign.Left;
                        this.rightImagePanel.Visible = true;
                    });
                }
            }
            else if (!e.IsMain)
            {
                var w = (int)(this.checkPatternPanel.Width / 2f);
                var h = this.checkPatternPanel.Height;
                var y = 0;

                if (this.displayMode == ImageDisplayMode.LeftFacing)
                {
                    this.BeginResumeLayout(false, true, () =>
                    {
                        var x = this.checkPatternPanel.Width - w;
                        this.rightImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                        this.rightImagePanel.ImageAlign = ImageAlign.Left;
                        this.rightImagePanel.Visible = true;
                    });
                }
                else
                {
                    this.BeginResumeLayout(true, false, () =>
                    {
                        var x = 0;
                        this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                        this.leftImagePanel.ImageAlign = ImageAlign.Right;
                        this.leftImagePanel.Visible = true;
                    });
                }
            }
            else if (e.IsMain && !e.HasSub)
            {
                this.BeginResumeLayout(true, false, () =>
                {
                    var w = this.checkPatternPanel.Width;
                    var h = this.checkPatternPanel.Height;
                    var x = 0;
                    var y = 0;

                    this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                    this.leftImagePanel.ImageAlign = ImageAlign.Center;
                    this.leftImagePanel.Visible = true;
                    this.rightImagePanel.Visible = false;
                });
            }
            else
            {
                this.leftImagePanel.Visible = false;
                this.rightImagePanel.Visible = false;
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
                var currentImageSize = this.GetImageSize(currentFilePath);
                if (currentImageSize.Width < currentImageSize.Height)
                {
                    var nextIndex = currentIndex + 1;
                    if (nextIndex > this.MaximumIndex)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = this.filePathList[nextIndex];
                    var nextImageSize = this.GetImageSize(nextFilePath);
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
                var prevImageSize1 = this.GetImageSize(prevFilePath1);
                if (prevImageSize1.Width < prevImageSize1.Height)
                {
                    var prevIndex2 = prevIndex1 - 1;
                    if (prevIndex2 < 0)
                    {
                        prevIndex2 = this.MaximumIndex;
                    }

                    var prevFilePath2 = this.filePathList[prevIndex2];
                    var prevImageSize2 = this.GetImageSize(prevFilePath2);
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
            using (TimeMeasuring.Run(true, "ImageViewerPage.ReadImage"))
            {
                var mainFilePath = this.filePathList[this.FilePathListIndex];
                this.SelectedFilePath = mainFilePath;

                var nextFiles = new List<string>(6);
                var nextIndex = this.GetNextIndex(this.FilePathListIndex, true);
                nextFiles.Add(this.filePathList[nextIndex]);
                while (nextFiles.Count < nextFiles.Capacity)
                {
                    nextIndex = this.GetNextIndex(nextIndex, true);
                    nextFiles.Add(this.filePathList[nextIndex]);
                }

                var prevFiles = new List<string>(6);
                var prevIndex = this.GetPreviewIndex(this.FilePathListIndex, true);
                prevFiles.Add(this.filePathList[prevIndex]);
                while (prevFiles.Count < prevFiles.Capacity)
                {
                    prevIndex = this.GetPreviewIndex(prevIndex, true);
                    prevFiles.Add(this.filePathList[prevIndex]);
                }

                var param = new ImageFileReadParameter
                {
                    CurrentIndex = this.FilePathListIndex,
                    FilePathList = this.filePathList,
                    ImageDisplayMode = this.displayMode,
                    ImageSizeMode = this.sizeMode,
                    ThumbnailSize = this.leftImagePanel.ThumbnailSize,
                };

                this.isLoading = true;

                this.ImageFileCacheJob.StartJob([.. nextFiles, .. prevFiles]);
                this.ImageFileLoadingJob.StartJob(param);
                this.ImageFileReadJob.StartJob(param);
            }
        }

        private void DoDragDrop(string currentFilePath)
        {
            var dragData = new DragEntity(
                this,
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

        private void ImageFileReadJob_Callback(ImageFileReadResult e)
        {
            if (!e.Image.IsEmpty)
            {
                this.isLoading = false;
            }

            var bgSize = e.HasSub switch
            {
                true => new SizeF(
                    this.checkPatternPanel.Size.Width / 2f,
                    this.checkPatternPanel.Size.Height),
                false => this.checkPatternPanel.Size,
            };

            if (e.IsMain)
            {
                this.SelectedFilePath = e.Image.FilePath;
                this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(e.Image.FilePath));
            }

            if (this.displayMode == ImageDisplayMode.Single)
            {
                this.leftImagePanel.ClearImage();
                this.rightImagePanel.ClearImage();
                if (e.Image.IsError)
                {
                    this.leftImagePanel.SetError();
                }
                else
                {
                    this.leftImagePanel.SetImage(
                        this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.FilePath);
                    var scale = GetImageScale(e.Image.Size, bgSize, this.sizeMode);
                    this.leftImagePanel.SetScale(scale);
                }
            }
            else if (this.displayMode == ImageDisplayMode.LeftFacing)
            {
                if (e.IsMain && e.HasSub)
                {
                    this.leftImagePanel.ClearImage();
                    this.rightImagePanel.ClearImage();
                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Size, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
                else if (!e.IsMain)
                {
                    this.rightImagePanel.ClearImage();
                    if (e.Image.IsError)
                    {
                        this.rightImagePanel.SetError();
                    }
                    else
                    {
                        this.rightImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Size, bgSize, this.sizeMode);
                        this.rightImagePanel.SetScale(scale);
                    }
                }
                else if (e.IsMain)
                {
                    this.leftImagePanel.ClearImage();
                    this.rightImagePanel.ClearImage();
                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Size, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
            }
            else if (this.displayMode == ImageDisplayMode.RightFacing)
            {
                if (e.IsMain && e.HasSub)
                {
                    this.rightImagePanel.ClearImage();
                    this.leftImagePanel.ClearImage();
                    if (e.Image.IsError)
                    {
                        this.rightImagePanel.SetError();
                    }
                    else
                    {
                        this.rightImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Size, bgSize, this.sizeMode);
                        this.rightImagePanel.SetScale(scale);
                    }
                }
                else if (!e.IsMain)
                {
                    this.leftImagePanel.ClearImage();
                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Size, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
                else if (e.IsMain)
                {
                    this.leftImagePanel.ClearImage();
                    this.rightImagePanel.ClearImage();
                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Size, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"不正な画像表示モードです。DisplayMode: '{this.displayMode}'");
            }

            this.SettingImagePanelLayout(e);
            this.Focus();
        }

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
            this.ReadImage();
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
            if (string.IsNullOrEmpty(this.leftImagePanel.FilePath))
            {
                return;
            }

            if (this.SelectedFilePath != this.leftImagePanel.FilePath)
            {
                this.SelectedFilePath = this.leftImagePanel.FilePath;
                this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.leftImagePanel.FilePath));
            }
        }

        private void RightImagePanel_ImageMouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(this.rightImagePanel.FilePath))
            {
                return;
            }

            if (this.SelectedFilePath != this.rightImagePanel.FilePath)
            {
                this.SelectedFilePath = this.rightImagePanel.FilePath;
                this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.rightImagePanel.FilePath));
            }
        }

        private void LeftImagePanel_DragStart(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.leftImagePanel.FilePath))
            {
                this.DoDragDrop(this.leftImagePanel.FilePath);
            }
        }

        private void RightImagePanel_DragStart(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.rightImagePanel.FilePath))
            {
                this.DoDragDrop(this.rightImagePanel.FilePath);
            }
        }

        private void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (this.fileContextMenu.SourceControl.Equals(this.leftImagePanel))
            {
                if (!string.IsNullOrEmpty(this.leftImagePanel.FilePath))
                {
                    if (this.SelectedFilePath != this.leftImagePanel.FilePath)
                    {
                        this.SelectedFilePath = this.leftImagePanel.FilePath;
                        this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.leftImagePanel.FilePath));
                    }

                    this.fileContextMenu.SetFile(this.leftImagePanel.FilePath);
                    return;
                }
            }
            else if (this.fileContextMenu.SourceControl.Equals(this.rightImagePanel))
            {
                if (!string.IsNullOrEmpty(this.rightImagePanel.FilePath))
                {
                    if (this.SelectedFilePath != this.rightImagePanel.FilePath)
                    {
                        this.SelectedFilePath = this.rightImagePanel.FilePath;
                        this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.rightImagePanel.FilePath));
                    }

                    this.fileContextMenu.SetFile(this.rightImagePanel.FilePath);
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
                    var param = new SingleFileExportParameter
                    {
                        SrcFilePath = srcFilePath,
                        ExportFilePath = ofd.FileName
                    };
                    this.SingleFileExportJob.StartJob(param);

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

    }
}
