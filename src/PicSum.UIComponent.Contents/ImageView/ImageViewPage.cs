using PicSum.Job.Common;
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
using SWF.UIComponent.Core;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ImageView
{
    /// <summary>
    /// 画像ビューアコンテンツ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class ImageViewPage
        : BrowsePage, ISender
    {
        private static readonly Rectangle TOOL_BAR_DEFAULT_BOUNDS = new(0, 0, 767, 29);
        private static readonly Rectangle CHECK_PATTERN_PANEL_DEFAULT_BOUNDS = new(0, 29, 767, 0);

        private static float GetImageScale(SizeF imageSize, Size backgroudSize, ImageSizeMode mode)
        {
            if (mode == ImageSizeMode.Original ||
                mode == ImageSizeMode.FitOnlyBigImage
                && imageSize.Width <= backgroudSize.Width
                && imageSize.Height <= backgroudSize.Height)
            {
                return AppConstants.DEFAULT_ZOOM_VALUE;
            }
            else
            {
                var scale = Math.Min(
                    backgroudSize.Width / imageSize.Width,
                    backgroudSize.Height / imageSize.Height);
                return scale;
            }
        }

        private bool _disposed = false;
        private float _scale = 0f;
        private readonly ImageViewPageParameter _parameter = null;
        private ImageDisplayMode _displayMode = ImageDisplayMode.LeftFacing;
        private ImageSizeMode _sizeMode = ImageSizeMode.FitOnlyBigImage;
        private string[] _filePathList = null;
        private bool _isInitializing = true;
        private bool _isMainLoading = false;
        private bool _isSubLoading = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string SelectedFilePath { get; protected set; } = string.Empty;

        private bool CanOperation
        {
            get
            {
                return this._filePathList != null &&
                    this._filePathList.Length > 0;
            }
        }

        private int MaximumIndex
        {
            get
            {
                return this.toolBar.IndexSliderMaximumValue;
            }
            set
            {
                this.toolBar.IndexSliderMaximumValue = value;
            }
        }

        private int FilePathListIndex
        {
            get
            {
                return this.toolBar.IndexSliderValue;
            }
            set
            {
                this.toolBar.IndexSliderValue = value;
            }
        }

        public ImageViewPage(ImageViewPageParameter parameter)
            : base(parameter)
        {
            this.InitializeComponent();

            this.checkPatternPanel.Resize += this.CheckPatternPanel_Resize;

            this.SetDisplayMode(ImageViewPageConfig.INSTANCE.ImageDisplayMode);

            if (ImageViewPageConfig.INSTANCE.ImageSizeMode == ImageSizeMode.Original)
            {
                ImageViewPageConfig.INSTANCE.ImageSizeMode = ImageSizeMode.FitOnlyBigImage;
            }

            this.SetSizeMode(ImageViewPageConfig.INSTANCE.ImageSizeMode);
            this.SetThumbnailPanelVisible();

            this._parameter = parameter;
            this.SelectedFilePath = parameter.SelectedFilePath;
        }

        public override void RedrawPage(float scale)
        {
            using (TimeMeasuring.Run(false, "ImageViewPage.RedrawPage"))
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

                    this.checkPatternPanel.Anchor
                        = AnchorStyles.Top
                        | AnchorStyles.Left
                        | AnchorStyles.Right
                        | AnchorStyles.Bottom;

                    this.toolBar.SetBounds(
                        0,
                        0,
                        this.Width,
                        (int)(TOOL_BAR_DEFAULT_BOUNDS.Height * this._scale));
                    this.toolBar.SetControlsBounds(this._scale);

                    this.checkPatternPanel.SetBounds(
                        0,
                        this.toolBar.Bottom,
                        this.Width,
                        baseHeigth - this.toolBar.Bottom);

                    this.toolBar.ResumeLayout(false);
                    this.ResumeLayout(false);
                }

                Size leftBgSize;
                if (this.leftImagePanel.HasImage && this.rightImagePanel.HasImage)
                {
                    leftBgSize = new Size(
                        this.GetLeftImagePanelWidth(),
                        this.checkPatternPanel.Size.Height);
                }
                else
                {
                    leftBgSize = this.checkPatternPanel.Size;
                }

                var rightBgSize = new Size(
                    this.GetRightImagePanelWidth(leftBgSize.Width),
                    this.checkPatternPanel.Height);

                if (this.leftImagePanel.HasImage)
                {
                    var leftImageScale = GetImageScale(
                        this.leftImagePanel.ImageSize, leftBgSize, this._sizeMode);
                    this.leftImagePanel.SetScale(leftImageScale);
                }

                if (this.rightImagePanel.HasImage)
                {
                    var rightImageScale = GetImageScale(
                        this.rightImagePanel.ImageSize, rightBgSize, this._sizeMode);
                    this.rightImagePanel.SetScale(rightImageScale);
                }

                if (this._isInitializing)
                {
                    return;
                }

                if (this._displayMode == ImageDisplayMode.LeftFacing)
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
                else if (this._displayMode == ImageDisplayMode.RightFacing)
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
                else if (this._displayMode == ImageDisplayMode.Single)
                {
                    this.SettingLeftImagePanelLayout();
                }

                this.leftImagePanel.Focus();
            }
        }

        public override void StopPageDraw()
        {
            //throw new NotImplementedException();
        }

        public override string[] GetSelectedFiles()
        {
            return [this.SelectedFilePath];
        }

        protected override void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this._displayMode == ImageDisplayMode.Single)
                {
                    this._parameter.SelectedFilePath = this.leftImagePanel.FilePath;
                }
                else if (this._displayMode == ImageDisplayMode.LeftFacing
                    && this.leftImagePanel.Visible)
                {
                    this._parameter.SelectedFilePath = this.leftImagePanel.FilePath;
                }
                else if (this._displayMode == ImageDisplayMode.RightFacing
                    && this.rightImagePanel.Visible)
                {
                    this._parameter.SelectedFilePath = this.rightImagePanel.FilePath;
                }
                else
                {
                    this._parameter.SelectedFilePath = this.SelectedFilePath;
                }

                this._parameter.GetImageFiles -= this.Parameter_GetImageFiles;

                this.fileContextMenu.Close();

                this.leftImagePanel.Dispose();
                this.rightImagePanel.Dispose();
            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        private void ImageViewPage_HandleCreated(object sender, EventArgs e)
        {
            if (this._disposed)
            {
                return;
            }

            this._isInitializing = true;

            this._parameter.GetImageFiles += this.Parameter_GetImageFiles;
            this._parameter.ImageFilesGetAction(this._parameter)(this);
        }

        private void ImageViewPage_ParentChanged(object sender, EventArgs e)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            this.RedrawPage(scale);
        }

        private void ImageViewPage_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            if (e.Delta > 0)
            {
                this.ReadImage(this.FilePathListIndex, false, false);
            }
            else
            {
                this.ReadImage(this.FilePathListIndex, true, false);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            using (TimeMeasuring.Run(false, "ImageViewPage.ProcessDialogKey"))
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
                    this.ReadImage(this.FilePathListIndex, true, false);
                    return true;
                }
                else if ((keyData & Keys.KeyCode) == Keys.Left)
                {
                    this.ReadImage(this.FilePathListIndex, false, false);
                    return true;
                }

                return base.ProcessDialogKey(keyData);
            }
        }

        private void ImageViewPage_DrawTabPage(object sender, DrawTabEventArgs e)
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

        private void CheckPatternPanel_Resize(object sender, EventArgs e)
        {
            this.RedrawPage(this._scale);
        }

        private void BeginResumeLayout(bool isLeft, bool isRight, Action action)
        {
            if (isLeft && isRight)
            {
                this.leftImagePanel.SuspendLayout();
                this.rightImagePanel.SuspendLayout();

                action();

                this.leftImagePanel.ResumeLayout(true);
                this.leftImagePanel.Invalidate();
                this.leftImagePanel.Update();

                this.rightImagePanel.ResumeLayout(true);
                this.rightImagePanel.Invalidate();
                this.rightImagePanel.Update();
            }
            else if (isLeft)
            {
                this.leftImagePanel.SuspendLayout();

                action();

                this.leftImagePanel.ResumeLayout(true);
                this.leftImagePanel.Invalidate();
                this.leftImagePanel.Update();
            }
            else if (isRight)
            {
                this.rightImagePanel.SuspendLayout();

                action();

                this.rightImagePanel.ResumeLayout(true);
                this.rightImagePanel.Invalidate();
                this.rightImagePanel.Update();
            }
        }

        private void SettingLeftImagePanelLayout()
        {
            var w = this.checkPatternPanel.Width;
            var h = this.checkPatternPanel.Height;
            var x = 0;
            var y = 0;

            this.leftImagePanel.SuspendLayout();
            this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
            this.leftImagePanel.ImageAlign = ImageAlign.Center;
            this.leftImagePanel.Visible = true;
            this.leftImagePanel.ResumeLayout(true);
            this.leftImagePanel.Invalidate();

            this.rightImagePanel.Visible = false;
        }

        private int GetLeftImagePanelWidth()
        {
            return (int)Math.Ceiling(this.checkPatternPanel.Width / 2d);
        }

        private int GetRightImagePanelWidth(int lw)
        {
            return this.checkPatternPanel.Width - lw;
        }

        private void SettingImagePanelsLayout()
        {
            var lw = this.GetLeftImagePanelWidth();
            var rw = this.GetRightImagePanelWidth(lw);
            var h = this.checkPatternPanel.Height;
            var lx = 0;
            var rx = this.checkPatternPanel.Width - lw;
            var y = 0;

            this.leftImagePanel.SuspendLayout();
            this.leftImagePanel.SetBounds(lx, y, lw, h, BoundsSpecified.All);
            this.leftImagePanel.ImageAlign = ImageAlign.Right;
            this.leftImagePanel.Visible = true;
            this.leftImagePanel.ResumeLayout(true);
            this.leftImagePanel.Invalidate();

            this.rightImagePanel.SuspendLayout();
            this.rightImagePanel.SetBounds(rx, y, rw, h, BoundsSpecified.All);
            this.rightImagePanel.ImageAlign = ImageAlign.Left;
            this.rightImagePanel.Visible = true;
            this.rightImagePanel.ResumeLayout(true);
            this.rightImagePanel.Invalidate();
        }

        private void SettingImagePanelLayout(ImageFileReadResult e)
        {
            if (e.IsMain && e.HasSub)
            {
                var lw = this.GetLeftImagePanelWidth();
                var h = this.checkPatternPanel.Height;
                var y = 0;

                if (this._displayMode == ImageDisplayMode.LeftFacing)
                {
                    this.BeginResumeLayout(true, false, () =>
                    {
                        var x = 0;
                        this.leftImagePanel.SetBounds(x, y, lw, h, BoundsSpecified.All);
                        this.leftImagePanel.ImageAlign = ImageAlign.Right;
                        this.leftImagePanel.Visible = true;
                    });
                }
                else
                {
                    this.BeginResumeLayout(false, true, () =>
                    {
                        var rw = this.GetRightImagePanelWidth(lw);
                        var x = this.checkPatternPanel.Width - rw;
                        this.rightImagePanel.SetBounds(x, y, rw, h, BoundsSpecified.All);
                        this.rightImagePanel.ImageAlign = ImageAlign.Left;
                        this.rightImagePanel.Visible = true;
                    });
                }
            }
            else if (!e.IsMain)
            {
                var lw = this.GetLeftImagePanelWidth();
                var h = this.checkPatternPanel.Height;
                var y = 0;

                if (this._displayMode == ImageDisplayMode.LeftFacing)
                {
                    this.BeginResumeLayout(false, true, () =>
                    {
                        var rw = this.GetRightImagePanelWidth(lw);
                        var x = this.checkPatternPanel.Width - rw;
                        this.rightImagePanel.SetBounds(x, y, rw, h, BoundsSpecified.All);
                        this.rightImagePanel.ImageAlign = ImageAlign.Left;
                        this.rightImagePanel.Visible = true;
                    });
                }
                else
                {
                    this.BeginResumeLayout(true, false, () =>
                    {
                        var x = 0;
                        this.leftImagePanel.SetBounds(x, y, lw, h, BoundsSpecified.All);
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

        private ThumbnailsGetParameter CreateThumbnailsGetParameter(int currentIndex)
        {
            int GetNextIndex(int currentIndex, string[] files)
            {
                var nextIndex = currentIndex + 1;
                if (nextIndex >= files.Length)
                {
                    return 0;
                }
                else
                {
                    return nextIndex;
                }
            }

            int GetPreviewIndex(int currentIndex, string[] files)
            {
                var previewIndex = currentIndex - 1;
                if (previewIndex < 0)
                {
                    return files.Length - 1;
                }
                else
                {
                    return previewIndex;
                }
            }

            var files = this._filePathList;

            const int NEXT_COUNT = 8;
            var nextFiles = new List<string>(NEXT_COUNT);
            var nextIndex = GetNextIndex(currentIndex, files);
            if (NEXT_COUNT > 0)
            {
                nextFiles.Add(files[nextIndex]);
            }
            while (nextFiles.Count < NEXT_COUNT)
            {
                nextIndex = GetNextIndex(nextIndex, files);
                nextFiles.Add(files[nextIndex]);
            }

            const int PREVIEW_COUNT = 8;
            var previewFiles = new List<string>(PREVIEW_COUNT);
            var previewIndex = GetPreviewIndex(currentIndex, files);
            if (PREVIEW_COUNT > 0)
            {
                previewFiles.Add(files[previewIndex]);
            }
            while (previewFiles.Count < PREVIEW_COUNT)
            {
                previewIndex = GetPreviewIndex(previewIndex, files);
                previewFiles.Add(files[previewIndex]);
            }

            string[] targets = [files[currentIndex], .. nextFiles, .. previewFiles];

            return new ThumbnailsGetParameter
            {
                FilePathList = targets,
                FirstIndex = 0,
                LastIndex = targets.Length - 1,
                ThumbnailWidth = ThumbnailUtil.THUMBNAIL_MAXIMUM_SIZE,
                ThumbnailHeight = ThumbnailUtil.THUMBNAIL_MAXIMUM_SIZE,
                IsExecuteCallback = false,
            };
        }

        private ImageFileReadParameter CreateImageFileReadParameter(
            int currentIndex, bool? isNext, bool isForceSingle, float zoomValue)
        {
            return new ImageFileReadParameter
            {
                CurrentIndex = currentIndex,
                FilePathList = this._filePathList,
                ImageDisplayMode = this._displayMode,
                ImageSizeMode = this._sizeMode,
                IsNext = isNext,
                IsForceSingle = isForceSingle,
                ZoomValue = zoomValue,
                ThumbnailSize = this.leftImagePanel.ThumbnailSize,
            };
        }

        private void ReadImage(int currentIndex, bool? isNext, bool isForceSingle)
        {
            this.ReadImage(currentIndex, isNext, isForceSingle, this.toolBar.GetZoomValue());
        }

        private void ReadImage(int currentIndex, bool? isNext, bool isForceSingle, float zoomValue)
        {
            this.fileContextMenu.Close();

            var filePathList = this._filePathList;
            if (filePathList.Length < 1)
            {
                return;
            }

            this._isMainLoading = true;
            this._isSubLoading = true;

            using (TimeMeasuring.Run(false, "ImageViewPage.ReadImage"))
            {
                var mainFilePath = filePathList[currentIndex];
                this.SelectedFilePath = mainFilePath;

                var thumbnailsGetParameter
                    = this.CreateThumbnailsGetParameter(currentIndex);

                var imageFileReadParameter
                    = this.CreateImageFileReadParameter(currentIndex, isNext, isForceSingle, zoomValue);

                Instance<JobCaller>.Value.ThumbnailsGetJob.Value
                    .StartJob(this, thumbnailsGetParameter);

                Instance<JobCaller>.Value.ImageFileCacheJob.Value
                    .StartJob(this, new ImageFileCacheParameter(
                        currentIndex, 5, 5, filePathList));

                Instance<JobCaller>.Value.ImageFileLoadingJob.Value
                    .StartJob(this, imageFileReadParameter, result =>
                    {
                        if (this._disposed)
                        {
                            return;
                        }

                        if (result.IsMain && !this._isMainLoading)
                        {
                            return;
                        }
                        else if (!result.IsMain && !this._isSubLoading)
                        {
                            return;
                        }

                        this.ImageFileReadJob_Callback(result);
                    });

                Instance<JobCaller>.Value.ImageFileReadJob.Value
                    .StartJob(this, imageFileReadParameter, result =>
                    {
                        if (this._disposed)
                        {
                            return;
                        }

                        if (result.IsMain && !result.HasSub)
                        {
                            this._isInitializing = false;
                        }
                        else if (!result.IsMain)
                        {
                            this._isInitializing = false;
                        }

                        if (result.IsMain)
                        {
                            this._isMainLoading = false;
                        }
                        else if (!result.IsMain)
                        {
                            this._isSubLoading = false;
                        }

                        using (TimeMeasuring.Run(false, "ImageViewPage.ImageFileReadJob_Callback"))
                        {
                            this.ImageFileReadJob_Callback(result);
                        }
                    });
            }

            this.leftImagePanel.Focus();
        }

        private void DoDragDrop(string currentFilePath)
        {
            var dragData = new DragEntity(
                this,
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                currentFilePath,
                this._parameter.SortInfo,
                this._parameter.ImageFilesGetAction,
                this._parameter.PageTitle,
                this._parameter.PageIcon,
                this._parameter.VisibleBookmarkMenuItem);

            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.FileDrop, new string[] { currentFilePath });
            dataObject.SetData(typeof(DragEntity), dragData);
            this.DoDragDrop(dataObject, DragDropEffects.Copy);
        }

        private bool SetDisplayMode(ImageDisplayMode mode)
        {
            this.toolBar.SingleViewMenuItemChecked = false;
            this.toolBar.SpreadLeftFeedMenuItemChecked = false;
            this.toolBar.SpreadRightFeedMenuItemChecked = false;

            switch (mode)
            {
                case ImageDisplayMode.Single:
                    this.toolBar.SingleViewMenuItemChecked = true;
                    this.toolBar.DoublePreviewButtonEnabled = false;
                    this.toolBar.DoubleNextButtonEnabled = false;
                    break;
                case ImageDisplayMode.LeftFacing:
                    this.toolBar.SpreadLeftFeedMenuItemChecked = true;
                    this.toolBar.DoublePreviewButtonEnabled = true;
                    this.toolBar.DoubleNextButtonEnabled = true;
                    break;
                case ImageDisplayMode.RightFacing:
                    this.toolBar.SpreadRightFeedMenuItemChecked = true;
                    this.toolBar.DoublePreviewButtonEnabled = true;
                    this.toolBar.DoubleNextButtonEnabled = true;
                    break;
            }

            if (this._displayMode != mode)
            {
                this._displayMode = mode;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SetSizeMode(ImageSizeMode mode)
        {
            this.toolBar.OriginalSizeMenuItemChecked = mode == ImageSizeMode.Original;
            this.toolBar.FitWindowMenuItemChecked = mode == ImageSizeMode.FitAllImage;
            this.toolBar.FitWindowLargeOnlyMenuItemChecked = mode == ImageSizeMode.FitOnlyBigImage;

            var zoomValue = this.toolBar.GetZoomValue();

            foreach (var item in this.toolBar.GetZoomMenuItems())
            {
                item.Checked = false;
            }

            if (this._sizeMode != mode || zoomValue != AppConstants.DEFAULT_ZOOM_VALUE)
            {
                this._sizeMode = mode;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SetSizeMode(float zoomValue)
        {
            this.toolBar.OriginalSizeMenuItemChecked = false;
            this.toolBar.FitWindowMenuItemChecked = false;
            this.toolBar.FitWindowLargeOnlyMenuItemChecked = false;

            var prevZoomValue = this.toolBar.GetZoomValue();

            foreach (var item in this.toolBar.GetZoomMenuItems())
            {
                item.Checked = item.ZoomValue == zoomValue;
            }

            this._sizeMode = ImageSizeMode.Original;

            if (prevZoomValue != zoomValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetThumbnailPanelVisible()
        {
            this.leftImagePanel.IsShowThumbnailPanel = this._sizeMode == ImageSizeMode.Original;
            this.rightImagePanel.IsShowThumbnailPanel = this.leftImagePanel.IsShowThumbnailPanel;
        }

        private void Parameter_GetImageFiles(object sender, GetImageFilesEventArgs e)
        {
            if (this._disposed)
            {
                return;
            }

            this._filePathList = e.FilePathList;

            this.Title = e.PageTitle;
            this.Icon = e.PageIcon;

            if (this._filePathList.Length > 0)
            {
                this.MaximumIndex = this._filePathList.Length - 1;
            }
            else
            {
                this.MaximumIndex = 0;
            }

            var selectedFilePath = this._parameter.SelectedFilePath != string.Empty ?
                this._parameter.SelectedFilePath : e.SelectedFilePath;
            var index = Array.IndexOf(this._filePathList, selectedFilePath);
            if (index < 0)
            {
                this.ReadImage(0, null, false);
            }
            else
            {
                this.ReadImage(index, null, false);
            }
        }

        private void ImageFileReadJob_Callback(ImageFileReadResult e)
        {
            var leftBgSize = e.HasSub switch
            {
                true => new Size(
                    this.GetLeftImagePanelWidth(),
                    this.checkPatternPanel.Size.Height),
                false => this.checkPatternPanel.Size,
            };

            var rightBgSize = new Size(
                this.GetRightImagePanelWidth(leftBgSize.Width),
                this.checkPatternPanel.Height);

            if (e.IsMain)
            {
                this.FilePathListIndex = e.Index;
                this.SelectedFilePath = e.Image.FilePath;
                this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(e.Image.FilePath));
            }

            if (this._displayMode == ImageDisplayMode.Single)
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
                        e.Image.FilePath, this._sizeMode, e.Image.Image, e.Image.Thumbnail);
                    var scale = GetImageScale(e.Image.Image.Size, leftBgSize, this._sizeMode);
                    this.leftImagePanel.SetScale(scale);
                }
            }
            else if (this._displayMode == ImageDisplayMode.LeftFacing)
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
                             e.Image.FilePath, this._sizeMode, e.Image.Image, e.Image.Thumbnail);
                        var scale = GetImageScale(e.Image.Image.Size, leftBgSize, this._sizeMode);
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
                            e.Image.FilePath, this._sizeMode, e.Image.Image, e.Image.Thumbnail);
                        var scale = GetImageScale(e.Image.Image.Size, rightBgSize, this._sizeMode);
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
                            e.Image.FilePath, this._sizeMode, e.Image.Image, e.Image.Thumbnail);
                        var scale = GetImageScale(e.Image.Image.Size, leftBgSize, this._sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
            }
            else if (this._displayMode == ImageDisplayMode.RightFacing)
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
                            e.Image.FilePath, this._sizeMode, e.Image.Image, e.Image.Thumbnail);
                        var scale = GetImageScale(e.Image.Image.Size, rightBgSize, this._sizeMode);
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
                            e.Image.FilePath, this._sizeMode, e.Image.Image, e.Image.Thumbnail);
                        var scale = GetImageScale(e.Image.Image.Size, leftBgSize, this._sizeMode);
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
                            e.Image.FilePath, this._sizeMode, e.Image.Image, e.Image.Thumbnail);
                        var scale = GetImageScale(e.Image.Image.Size, leftBgSize, this._sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"不正な画像表示モードです。DisplayMode: '{this._displayMode}'");
            }

            this.SettingImagePanelLayout(e);
            this.leftImagePanel.Focus();
        }

        private void ToolBar_SingleViewMenuItemClick(object sender, EventArgs e)
        {
            this.leftImagePanel.Focus();

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
                ImageViewPageConfig.INSTANCE.ImageDisplayMode = this._displayMode;
                this.ReadImage(this.FilePathListIndex, null, false);
            }
        }

        private void ToolBar_SpreadLeftFeedMenuItemClick(object sender, EventArgs e)
        {
            this.leftImagePanel.Focus();

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
                ImageViewPageConfig.INSTANCE.ImageDisplayMode = this._displayMode;
                this.ReadImage(this.FilePathListIndex, null, false);
            }
        }

        private void ToolBar_SpreadRightFeedMenuItemClick(object sender, EventArgs e)
        {
            this.leftImagePanel.Focus();

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
                ImageViewPageConfig.INSTANCE.ImageDisplayMode = this._displayMode;
                this.ReadImage(this.FilePathListIndex, null, false);
            }
        }

        private void ToolBar_OriginalSizeMenuItemClick(object sender, EventArgs e)
        {
            this.leftImagePanel.Focus();

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
                ImageViewPageConfig.INSTANCE.ImageSizeMode = this._sizeMode;
                this.SetThumbnailPanelVisible();
                this.ReadImage(this.FilePathListIndex, null, false);
            }
        }

        private void ToolBar_FitWindowMenuItemClick(object sender, EventArgs e)
        {
            this.leftImagePanel.Focus();

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
                ImageViewPageConfig.INSTANCE.ImageSizeMode = this._sizeMode;
                this.SetThumbnailPanelVisible();
                this.ReadImage(this.FilePathListIndex, null, false);
            }
        }

        private void ToolBar_FitWindowLargeOnlyMenuItemClick(object sender, EventArgs e)
        {
            this.leftImagePanel.Focus();

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
                ImageViewPageConfig.INSTANCE.ImageSizeMode = this._sizeMode;
                this.SetThumbnailPanelVisible();
                this.ReadImage(this.FilePathListIndex, null, false);
            }
        }

        private void ToolBar_ZoomMenuItemClick(object sender, ZoomMenuItemClickEventArgs e)
        {
            this.leftImagePanel.Focus();

            if (!this.IsHandleCreated)
            {
                return;
            }

            if (!this.CanOperation)
            {
                return;
            }

            if (this.SetSizeMode(e.ZoomValue))
            {
                this.SetThumbnailPanelVisible();
                this.ReadImage(this.FilePathListIndex, null, false, e.ZoomValue);
            }
        }

        private void ToolBar_DoublePreviewButtonClick(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.ReadImage(this.FilePathListIndex, false, false);
        }

        private void ToolBar_DoubleNextButtonClick(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.ReadImage(this.FilePathListIndex, true, false);
        }

        private void ToolBar_SinglePreviewButtonClick(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.ReadImage(this.FilePathListIndex, false, true);
        }

        private void ToolBar_SingleNextButtonClick(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.ReadImage(this.FilePathListIndex, true, true);
        }

        private void ToolBar_IndexSliderValueChanging(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            var index = this.FilePathListIndex;
            if (index < 0 || this._filePathList.Length - 1 < index)
            {
                return;
            }

            var filePath = this._filePathList[index];
            this.toolBar.ShowToolTip(filePath);
            this.ReadImage(index, null, false);
        }

        private void ToolBar_IndexSliderValueChanged(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }
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

            if (e.Button == MouseButtons.Left)
            {
                if (this.SelectedFilePath != this.leftImagePanel.FilePath)
                {
                    this.SelectedFilePath = this.leftImagePanel.FilePath;
                    this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.leftImagePanel.FilePath));
                }
            }
        }

        private void RightImagePanel_ImageMouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(this.rightImagePanel.FilePath))
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (this.SelectedFilePath != this.rightImagePanel.FilePath)
                {
                    this.SelectedFilePath = this.rightImagePanel.FilePath;
                    this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.rightImagePanel.FilePath));
                }
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
            if (this.fileContextMenu.SourceControl == this.leftImagePanel)
            {
                if (!string.IsNullOrEmpty(this.leftImagePanel.FilePath))
                {
                    if (this.SelectedFilePath != this.leftImagePanel.FilePath)
                    {
                        this.SelectedFilePath = this.leftImagePanel.FilePath;
                        this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.leftImagePanel.FilePath));
                    }

                    this.fileContextMenu.SetFile(this.leftImagePanel.FilePath);
                    this.fileContextMenu.VisibleFileActiveTabOpenMenuItem = false;
                    this.fileContextMenu.VisibleBookmarkMenuItem = this._parameter.VisibleBookmarkMenuItem;
                    return;
                }
            }
            else if (this.fileContextMenu.SourceControl == this.rightImagePanel)
            {
                if (!string.IsNullOrEmpty(this.rightImagePanel.FilePath))
                {
                    if (this.SelectedFilePath != this.rightImagePanel.FilePath)
                    {
                        this.SelectedFilePath = this.rightImagePanel.FilePath;
                        this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.rightImagePanel.FilePath));
                    }

                    this.fileContextMenu.SetFile(this.rightImagePanel.FilePath);
                    this.fileContextMenu.VisibleFileActiveTabOpenMenuItem = false;
                    this.fileContextMenu.VisibleBookmarkMenuItem = this._parameter.VisibleBookmarkMenuItem;
                    return;
                }
            }

            e.Cancel = true;
        }

        private void FileContextMenu_FileNewTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this._parameter.ImageFilesGetAction,
                e.FilePath,
                this._parameter.SortInfo,
                this.Title,
                this.Icon,
                this.Parameter.VisibleBookmarkMenuItem);
            this.OnOpenPage(new BrowsePageEventArgs(PageOpenType.AddTab, param));
        }

        private void FileContextMenu_FileNewWindowOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewPageParameter(
                this.Parameter.PageSources,
                this.Parameter.SourcesKey,
                this._parameter.ImageFilesGetAction,
                e.FilePath,
                this._parameter.SortInfo,
                this.Title,
                this.Icon,
                this.Parameter.VisibleBookmarkMenuItem);
            this.OnOpenPage(new BrowsePageEventArgs(PageOpenType.NewWindow, param));
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

            Instance<JobCaller>.Value.EnqueueBookmarkAddJob(this, paramter);
        }
    }
}
