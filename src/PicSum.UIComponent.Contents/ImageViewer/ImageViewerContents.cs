using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using PicSum.UIComponent.Common;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Common;
using SWF.UIComponent.ImagePanel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ImageViewer
{
    /// <summary>
    /// 画像ビューアコンテンツ
    /// </summary>
    public partial class ImageViewerContents
        : BrowserContents
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private readonly ImageViewerContentsParameter parameter = null;
        private string leftImageFilePath = string.Empty;
        private string rightImageFilePath = string.Empty;
        private ImageDisplayMode displayMode = ImageDisplayMode.LeftFacing;
        private ImageSizeMode sizeMode = ImageSizeMode.FitOnlyBigImage;
        private IList<string> filePathList = null;

        private TwoWayProcess<GetImageFileAsyncFacade, GetImageFileParameter, GetImageFileResult> readImageFileProcess = null;
        private OneWayProcess<AddKeepAsyncFacade, ListEntity<KeepFileEntity>> addKeepProcess = null;
        private OneWayProcess<ExportFileAsyncFacade, ExportFileParameter> exportFileProcess = null;

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

        private TwoWayProcess<GetImageFileAsyncFacade, GetImageFileParameter, GetImageFileResult> ReadImageFileProcess
        {
            get
            {
                if (this.readImageFileProcess == null)
                {
                    this.readImageFileProcess = TaskManager.CreateTwoWayProcess<GetImageFileAsyncFacade, GetImageFileParameter, GetImageFileResult>(this.ProcessContainer);
                    this.readImageFileProcess.Callback += new AsyncTaskCallbackEventHandler<GetImageFileResult>(this.ReadImageFileProcess_Callback);
                    this.readImageFileProcess.SuccessEnd += new EventHandler(this.ReadImageFileProcess_SuccessEnd);
                    this.readImageFileProcess.ErrorEnd += new EventHandler(this.ReadImageFileProcess_ErrorEnd);
                }

                return this.readImageFileProcess;
            }
        }

        private OneWayProcess<AddKeepAsyncFacade, ListEntity<KeepFileEntity>> AddKeepProcess
        {
            get
            {
                if (this.addKeepProcess == null)
                {
                    this.addKeepProcess = TaskManager.CreateOneWayProcess<AddKeepAsyncFacade, ListEntity<KeepFileEntity>>(this.ProcessContainer);
                }

                return this.addKeepProcess;
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

        #endregion

        #region コンストラクタ

        public ImageViewerContents(ImageViewerContentsParameter parameter)
            : base(parameter)
        {
            this.InitializeComponent();

            this.parameter = parameter;
            this.Title = this.parameter.ContentsTitle;
            this.Icon = this.parameter.ContentsIcon;
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
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
        }

        protected override void OnResize(EventArgs e)
        {
            this.ChangeImagePanelSize();
            base.OnResize(e);
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
                    this.FilePathListIndex = this.GetPreviewIndex();
                }
                else
                {
                    this.FilePathListIndex = this.GetNextIndex();
                }
            }
            catch (ImageUtilException ex)
            {
                ExceptionUtil.ShowErrorDialog(ex);
            }

            base.OnMouseWheel(e);
        }

        [UIPermission(SecurityAction.Demand, Window = UIPermissionWindow.AllWindows)]
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
                this.FilePathListIndex = this.GetNextIndex();
                return true;
            }
            else if ((keyData & Keys.KeyCode) == Keys.Left)
            {
                this.FilePathListIndex = this.GetPreviewIndex();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnDrawTabContents(SWF.UIComponent.TabOperation.DrawTabEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.parameter.ContentsTitle) && this.parameter.ContentsIcon != null)
            {
                e.Graphics.DrawImage(this.parameter.ContentsIcon, e.IconRectangle);
                DrawTextUtil.DrawText(e.Graphics, this.parameter.ContentsTitle, e.Font, e.TextRectangle, e.TitleColor, e.TitleFormatFlags, e.TextStyle);
            }
            else
            {
                e.Graphics.DrawImage(this.Icon, e.IconRectangle);
                DrawTextUtil.DrawText(e.Graphics, this.Title, e.Font, e.TextRectangle, e.TitleColor, e.TitleFormatFlags, e.TextStyle);
            }
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

        private Size GetImageSize(string filePath)
        {
            var size = ImageUtil.GetImageSize(filePath);
            return size;
        }

        private int GetNextIndex()
        {
            if (this.displayMode == ImageDisplayMode.Single)
            {
                if (this.FilePathListIndex == MaximumIndex)
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
                var currentImageSize = this.GetImageSize(currentFilePath);
                if (currentImageSize.Width < currentImageSize.Height)
                {
                    var nextIndex = currentIndex + 1;
                    if (nextIndex > MaximumIndex)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = this.filePathList[nextIndex];
                    var nextImageSize = this.GetImageSize(nextFilePath);
                    if (nextImageSize.Width < nextImageSize.Height)
                    {
                        if (nextIndex == MaximumIndex)
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

        private int GetPreviewIndex()
        {
            if (this.displayMode == ImageDisplayMode.Single)
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
                    prevIndex1 = MaximumIndex;
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
            this.Cursor = Cursors.WaitCursor;

            var param = new GetImageFileParameter();
            param.CurrentIndex = this.FilePathListIndex;
            param.FilePathList = this.filePathList;
            param.ImageDisplayMode = this.displayMode;
            param.ImageSizeMode = this.sizeMode;
            param.DrawSize = this.checkPatternPanel.Size;
            param.ThumbnailSize = this.leftImagePanel.ThumbnailSize;

            this.ReadImageFileProcess.Cancel();
            this.ReadImageFileProcess.Execute(this, param);
        }

        private void AddKeep(IList<KeepFileEntity> filePathList)
        {
            var param = new ListEntity<KeepFileEntity>(filePathList);
            this.AddKeepProcess.Execute(this, param);
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
            this.singleViewToolStripMenuItem.Checked = mode == ImageDisplayMode.Single;
            this.leftFacingViewToolStripMenuItem.Checked = mode == ImageDisplayMode.LeftFacing;
            this.rightFacingViewToolStripMenuItem.Checked = mode == ImageDisplayMode.RightFacing;

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

        private ImageDisplayMode GetNextDisplayMode()
        {
            if (this.displayMode == ImageDisplayMode.Single)
            {
                return ImageDisplayMode.LeftFacing;
            }
            else if (this.displayMode == ImageDisplayMode.LeftFacing)
            {
                return ImageDisplayMode.RightFacing;
            }
            else
            {
                return ImageDisplayMode.Single;
            }
        }

        private ImageSizeMode GetNextSizeMode()
        {
            if (this.sizeMode == ImageSizeMode.Original)
            {
                return ImageSizeMode.FitAllImage;
            }
            else if (this.sizeMode == ImageSizeMode.FitAllImage)
            {
                return ImageSizeMode.FitOnlyBigImage;
            }
            else
            {
                return ImageSizeMode.Original;
            }
        }

        #endregion

        #region プロセスイベント

        private void Parameter_GetImageFiles(object sender, GetImageFilesEventArgs e)
        {
            this.filePathList = e.FilePathList;

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

            this.SubInitializeComponent();
        }

        private void ReadImageFileProcess_Callback(object sender, GetImageFileResult e)
        {
            if (e.ReadImageFileException != null)
            {
                ExceptionUtil.ShowErrorDialog(e.ReadImageFileException);
                return;
            }

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

            this.SelectedFilePath = e.Image1.FilePath;
            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(e.Image1.FilePath));

            if (this.displayMode == ImageDisplayMode.Single)
            {
                this.leftImageFilePath = e.Image1.FilePath;
                this.leftImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
            }
            else if (displayMode == ImageDisplayMode.LeftFacing)
            {
                this.leftImageFilePath = e.Image1.FilePath;
                this.leftImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
                if (e.Image2 != null)
                {
                    this.rightImageFilePath = e.Image2.FilePath;
                    this.rightImagePanel.SetImage(e.Image2.Image, e.Image2.Thumbnail);
                }
            }
            else if (this.displayMode == ImageDisplayMode.RightFacing)
            {
                this.rightImageFilePath = e.Image1.FilePath;
                this.rightImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
                if (e.Image2 != null)
                {
                    this.leftImageFilePath = e.Image2.FilePath;
                    this.leftImagePanel.SetImage(e.Image2.Image, e.Image2.Thumbnail);
                }
            }

            this.ChangeImagePanelSize();

            this.leftImagePanel.Invalidate();
            this.rightImagePanel.Invalidate();

            this.Focus();
        }

        private void ReadImageFileProcess_SuccessEnd(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void ReadImageFileProcess_ErrorEnd(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        #endregion

        #region ツールバーイベント

        private void ViewToolStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (!this.CanOperation)
            {
                return;
            }

            if (this.SetDisplayMode(GetNextDisplayMode()))
            {
                ImageViewerContentsConfig.ImageDisplayMode = this.displayMode;
                this.ReadImage();
            }
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

        private void SizeToolStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (!this.CanOperation)
            {
                return;
            }

            if (this.SetSizeMode(GetNextSizeMode()))
            {
                ImageViewerContentsConfig.ImageSizeMode = this.sizeMode;
                this.SetThumbnailPanelVisible();
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

        private void PreviewIndexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            try
            {
                this.FilePathListIndex = this.GetPreviewIndex();
            }
            catch (ImageUtilException ex)
            {
                ExceptionUtil.ShowErrorDialog(ex);
            }
        }

        private void NextIndexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            try
            {
                this.FilePathListIndex = this.GetNextIndex();
            }
            catch (ImageUtilException ex)
            {
                ExceptionUtil.ShowErrorDialog(ex);
            }
        }

        private void IndexToolStripSlider_ValueChanging(object sender, EventArgs e)
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

        private void IndexToolStripSlider_ValueChanged(object sender, EventArgs e)
        {
            if (!this.CanOperation)
            {
                return;
            }

            this.filePathToolTip.Hide(this);
            this.ReadImage();
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

            if (e.Button == MouseButtons.Middle)
            {
                if (!string.IsNullOrEmpty(this.leftImageFilePath))
                {
                    this.AddKeep(new KeepFileEntity[] { new KeepFileEntity(this.leftImageFilePath, DateTime.Now) });
                }
            }
        }

        private void RightImagePanel_ImageMouseClick(object sender, MouseEventArgs e)
        {
            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.rightImageFilePath));

            if (e.Button == MouseButtons.Middle)
            {
                if (!string.IsNullOrEmpty(this.rightImageFilePath))
                {
                    this.AddKeep(new KeepFileEntity[] { new KeepFileEntity(this.rightImageFilePath, DateTime.Now) });
                }
            }
        }

        private void LeftImagePanel_DragStart(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.leftImageFilePath))
            {
                this.DoDragDrop(leftImageFilePath);
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
            using (var fbd = new FolderBrowserDialog())
            {
                if (FileUtil.IsExists(CommonConfig.ExportDirectoryPath))
                {
                    fbd.SelectedPath = CommonConfig.ExportDirectoryPath;
                }

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var param = new ExportFileParameter();
                    param.ExportDirectoryPath = fbd.SelectedPath;
                    param.FilePathList = e.FilePathList;
                    this.ExportFileProcess.Execute(this, param);

                    CommonConfig.ExportDirectoryPath = fbd.SelectedPath;
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

        private void FileContextMenu_AddKeep(object sender, ExecuteFileListEventArgs e)
        {
            this.AddKeep(e.FilePathList.Select(filePath => new KeepFileEntity(filePath, DateTime.Now)).ToList());
        }

        #endregion
    }
}
