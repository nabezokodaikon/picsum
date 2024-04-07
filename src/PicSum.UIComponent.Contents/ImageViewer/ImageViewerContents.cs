using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Paramters;
using PicSum.Task.Results;
using PicSum.Task.Tasks;
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
    public sealed partial class ImageViewerContents
        : BrowserContents
    {
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

        private readonly ImageViewerContentsParameter parameter = null;
        private string leftImageFilePath = string.Empty;
        private string rightImageFilePath = string.Empty;
        private ImageDisplayMode displayMode = ImageDisplayMode.LeftFacing;
        private ImageSizeMode sizeMode = ImageSizeMode.FitOnlyBigImage;
        private IList<string> filePathList = null;

        private TwoWayTask<GetImageFileTask, GetImageFileParameter, GetImageFileResult> getImageFileTask = null;
        private OneWayTask<AddBookmarkTask, ValueParameter<string>> addBookmarkTask = null;
        private OneWayTask<ExportFileTask, ExportFileParameter> exportFileTask = null;

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

        private TwoWayTask<GetImageFileTask, GetImageFileParameter, GetImageFileResult> GetImageFileTask
        {
            get
            {
                if (this.getImageFileTask == null)
                {
                    this.getImageFileTask = new();
                    this.getImageFileTask
                        .Callback(this.GetImageFileTask_Callback)
                        .Catch(ex =>
                        {
                            this.Cursor = Cursors.Default;
                            ExceptionUtil.ShowErrorDialog(ex.InnerException);
                        })
                        .Complete(() => this.Cursor = Cursors.Default)
                        .StartThread();
                }

                return this.getImageFileTask;
            }
        }

        private OneWayTask<AddBookmarkTask, ValueParameter<string>> AddBookmarkTask
        {
            get
            {
                if (this.addBookmarkTask == null)
                {
                    this.addBookmarkTask = new();
                    this.addBookmarkTask
                        .StartThread();
                }

                return this.addBookmarkTask;
            }
        }

        private OneWayTask<ExportFileTask, ExportFileParameter> ExportFileTask
        {
            get
            {
                if (this.exportFileTask == null)
                {
                    this.exportFileTask = new();
                    this.exportFileTask
                        .StartThread();
                }

                return this.exportFileTask;
            }
        }

        #endregion

        #region コンストラクタ

        public ImageViewerContents(ImageViewerContentsParameter parameter)
            : base(parameter)
        {
            this.InitializeComponent();
            this.SubInitializeComponent();

            this.parameter = parameter;
        }

        #endregion

        #region パブリックメソッド

        public override void RedrawContents()
        {
            this.ChangeImagePanelSize();

            Size backgroudSize = Size.Empty;
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
            this.RedrawContents();

            base.OnResize(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.getImageFileTask != null)
                {
                    this.getImageFileTask.Dispose();
                    this.getImageFileTask = null;
                }

                if (this.addBookmarkTask != null)
                {
                    this.addBookmarkTask.Dispose();
                    this.addBookmarkTask = null;
                }

                if (this.exportFileTask != null)
                {
                    this.exportFileTask.Dispose();
                    this.exportFileTask = null;
                }

                this.parameter.SelectedFilePath = this.SelectedFilePath;
                this.parameter.GetImageFiles -= this.Parameter_GetImageFiles;

                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.parameter.GetImageFiles += this.Parameter_GetImageFiles;
            this.parameter.GetImageFilesAction(this.parameter)();
            base.OnLoad(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            try
            {
                if (e.Delta > 0)
                {
                    this.FilePathListIndex = this.GetPreviewIndex(false);
                }
                else
                {
                    this.FilePathListIndex = this.GetNextIndex(false);
                }
            }
            catch (ImageUtilException ex)
            {
                ExceptionUtil.ShowErrorDialog(ex);
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
                this.FilePathListIndex = this.GetNextIndex(false);
                return true;
            }
            else if ((keyData & Keys.KeyCode) == Keys.Left)
            {
                this.FilePathListIndex = this.GetPreviewIndex(false);
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnDrawTabContents(SWF.UIComponent.TabOperation.DrawTabEventArgs e)
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
            this.SetDisplayMode(ImageViewerContentsConfig.ImageDisplayMode);
            this.SetSizeMode(ImageViewerContentsConfig.ImageSizeMode);
            this.SetThumbnailPanelVisible();
        }

        private void ChangeImagePanelSize()
        {
            this.leftImagePanel.Visible = false;
            this.rightImagePanel.Visible = false;

            if (this.leftImagePanel.HasImage && this.rightImagePanel.HasImage)
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
            else if (this.leftImagePanel.HasImage)
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
            else if (this.rightImagePanel.HasImage)
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
            else
            {
                this.leftImagePanel.Visible = false;
                this.rightImagePanel.Visible = false;
            }
        }

        private int GetNextIndex(bool isForceSingle)
        {
            if (isForceSingle || this.displayMode == ImageDisplayMode.Single)
            {
                if (this.FilePathListIndex == this.MaximumIndex)
                {
                    return 0;
                }
                else
                {
                    return this.FilePathListIndex + 1;
                }
            }
            else
            {
                var currentIndex = this.FilePathListIndex;
                var currentFilePath = this.filePathList[currentIndex];
                var currentImageSize = ImageUtil.GetImageSize(currentFilePath); ;
                if (currentImageSize.Width < currentImageSize.Height)
                {
                    var nextIndex = currentIndex + 1;
                    if (nextIndex > this.MaximumIndex)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = this.filePathList[nextIndex];
                    var nextImageSize = ImageUtil.GetImageSize(nextFilePath);
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
                    if (this.FilePathListIndex == this.MaximumIndex)
                    {
                        return 0;
                    }
                    else
                    {
                        return this.FilePathListIndex + 1;
                    }
                }
            }
        }

        private int GetPreviewIndex(bool isSingle)
        {
            if (this.displayMode == ImageDisplayMode.Single || isSingle)
            {
                if (this.FilePathListIndex == 0)
                {
                    return this.MaximumIndex;
                }
                else
                {
                    return this.FilePathListIndex - 1;
                }
            }
            else
            {
                var prevIndex1 = this.FilePathListIndex - 1;
                if (prevIndex1 < 0)
                {
                    prevIndex1 = this.MaximumIndex;
                }

                var prevFilePath1 = this.filePathList[prevIndex1];
                var prevImageSize1 = ImageUtil.GetImageSize(prevFilePath1);
                if (prevImageSize1.Width < prevImageSize1.Height)
                {
                    var prevIndex2 = prevIndex1 - 1;
                    if (prevIndex2 < 0)
                    {
                        prevIndex2 = this.MaximumIndex;
                    }

                    var prevFilePath2 = this.filePathList[prevIndex2];
                    var prevImageSize2 = ImageUtil.GetImageSize(prevFilePath2);
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

            var param = new GetImageFileParameter();
            param.CurrentIndex = this.FilePathListIndex;
            param.FilePathList = this.filePathList;
            param.ImageDisplayMode = this.displayMode;
            param.ImageSizeMode = this.sizeMode;
            param.ThumbnailSize = this.leftImagePanel.ThumbnailSize;

            this.GetImageFileTask.StartTask(param);
        }

        private void DoDragDrop(string currentFilePath)
        {
            var dragData = new DragEntity(
                this.Parameter.ContentsSources,
                this.Parameter.SourcesKey,
                currentFilePath,
                this.parameter.GetImageFilesAction,
                this.parameter.ContentsTitle,
                this.parameter.ContentsIcon);
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

            this.Title = e.ContentsTitle;
            this.Icon = e.ContentsIcon;

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

        private void GetImageFileTask_Callback(GetImageFileResult e)
        {
            if (this.leftImagePanel.HasImage)
            {
                this.leftImageFilePath = string.Empty;
                this.leftImagePanel.ClearImage();
            }

            if (this.rightImagePanel.HasImage)
            {
                this.rightImageFilePath = string.Empty;
                this.rightImagePanel.ClearImage();
            }

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
                this.leftImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);

                var scale = GetImageScale(
                    this.leftImagePanel.ImageSize, bgSize, this.sizeMode);
                this.leftImagePanel.SetScale(scale);
            }
            else if (this.displayMode == ImageDisplayMode.LeftFacing)
            {
                this.leftImageFilePath = e.Image1.FilePath;
                this.leftImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
                var leftImageScale = GetImageScale(
                    this.leftImagePanel.ImageSize, bgSize, this.sizeMode);

                if (e.Image2 != null)
                {
                    this.leftImagePanel.SetScale(leftImageScale);

                    this.rightImageFilePath = e.Image2.FilePath;
                    this.rightImagePanel.SetImage(e.Image2.Image, e.Image2.Thumbnail);

                    var rightImageScale = GetImageScale(
                        this.rightImagePanel.ImageSize, bgSize, this.sizeMode);
                    this.rightImagePanel.SetScale(rightImageScale);
                }
                else
                {
                    this.leftImagePanel.SetScale(leftImageScale);
                }
            }
            else if (this.displayMode == ImageDisplayMode.RightFacing)
            {
                this.rightImageFilePath = e.Image1.FilePath;
                this.rightImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
                var rightImageScale = GetImageScale(
                    this.rightImagePanel.ImageSize, bgSize, this.sizeMode);

                if (e.Image2 != null)
                {
                    this.rightImagePanel.SetScale(rightImageScale);

                    this.leftImageFilePath = e.Image2.FilePath;
                    this.leftImagePanel.SetImage(e.Image2.Image, e.Image2.Thumbnail);

                    var leftImageScale = GetImageScale(
                        this.leftImagePanel.ImageSize, bgSize, this.sizeMode);
                    this.leftImagePanel.SetScale(leftImageScale);
                }
                else
                {
                    this.rightImagePanel.SetScale(rightImageScale);
                }
            }

            this.ChangeImagePanelSize();

            this.leftImagePanel.Invalidate();
            this.rightImagePanel.Invalidate();

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
                ImageViewerContentsConfig.ImageDisplayMode = this.displayMode;
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
                ImageViewerContentsConfig.ImageDisplayMode = this.displayMode;
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
                ImageViewerContentsConfig.ImageDisplayMode = this.displayMode;
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
                ImageViewerContentsConfig.ImageSizeMode = this.sizeMode;
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
                ImageViewerContentsConfig.ImageSizeMode = this.sizeMode;
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
                ImageViewerContentsConfig.ImageSizeMode = this.sizeMode;
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

            try
            {
                this.FilePathListIndex = this.GetPreviewIndex(false);
            }
            catch (ImageUtilException ex)
            {
                ExceptionUtil.ShowErrorDialog(ex);
            }
        }

        private void DoubleNextIndexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            try
            {
                this.FilePathListIndex = this.GetNextIndex(false);
            }
            catch (ImageUtilException ex)
            {
                ExceptionUtil.ShowErrorDialog(ex);
            }
        }

        private void SinglePreviewIndexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            try
            {
                this.FilePathListIndex = this.GetPreviewIndex(true);
            }
            catch (ImageUtilException ex)
            {
                ExceptionUtil.ShowErrorDialog(ex);
            }
        }

        private void SingleNextIndexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            try
            {
                this.FilePathListIndex = this.GetNextIndex(true);
            }
            catch (ImageUtilException ex)
            {
                ExceptionUtil.ShowErrorDialog(ex);
            }
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
            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.leftImageFilePath));
        }

        private void RightImagePanel_ImageMouseClick(object sender, MouseEventArgs e)
        {
            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.rightImageFilePath));
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
                    this.fileContextMenu.SetFile(this.leftImageFilePath);
                    return;
                }
            }
            else if (this.fileContextMenu.SourceControl.Equals(this.rightImagePanel))
            {
                if (!string.IsNullOrEmpty(this.rightImageFilePath))
                {
                    this.fileContextMenu.SetFile(this.rightImageFilePath);
                    return;
                }
            }

            e.Cancel = true;
        }

        private void FileContextMenu_FileNewTabOpen(object sender, ExecuteFileEventArgs e)
        {
            var param = new ImageViewerContentsParameter(
                this.Parameter.ContentsSources,
                this.Parameter.SourcesKey,
                this.parameter.GetImageFilesAction,
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
                this.parameter.GetImageFilesAction,
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

        private void FileContextMenu_Export(object sender, ExecuteFileListEventArgs e)
        {
            using (var ofd = new SaveFileDialog())
            {
                var srcFilePath = e.FilePathList.First();
                ofd.InitialDirectory = CommonConfig.ExportDirectoryPath;
                ofd.FileName = FileUtil.GetExportFileName(
                    CommonConfig.ExportDirectoryPath,
                    srcFilePath);
                ofd.CheckFileExists = false;
                ofd.Filter = FileUtil.GetExportFilterText(srcFilePath);
                ofd.FilterIndex = 0;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var dir = FileUtil.GetParentDirectoryPath(ofd.FileName);
                    var param = new ExportFileParameter();
                    param.SrcFilePath = srcFilePath;
                    param.ExportFilePath = ofd.FileName;
                    this.ExportFileTask.StartTask(param);

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
            var paramter = new ValueParameter<string>()
            {
                Value = e.FilePath,
            };

            this.AddBookmarkTask.StartTask(paramter);
        }

        #endregion
    }
}
