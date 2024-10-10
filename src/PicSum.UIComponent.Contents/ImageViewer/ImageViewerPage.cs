using NLog;
using PicSum.Core.Base.Conf;
using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Jobs;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.ContextMenu;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.UIComponent.ImagePanel;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
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

        private static ImageFileGetResult CreateEmptyResult(
            string filePath, bool isMain, bool hasSub, int thumbnailSize, ImageSizeMode imageSizeMode, Size imageSize)
        {
            var image = new CvImage(CreateEmptyImage(imageSize));
            var thumbnail = ThumbnailGetLogic.CreateThumbnail(image, thumbnailSize, imageSizeMode);

            return new()
            {
                IsMain = isMain,
                HasSub = hasSub,
                IsUpdate = false,
                Image = new()
                {
                    FilePath = filePath,
                    Thumbnail = thumbnail,
                    Image = image,
                    Size = imageSize,
                    IsEmpty = true,
                    IsError = false,
                }
            };
        }

        private static Bitmap CreateEmptyImage(Size imageSize)
        {
            var bmp = new Bitmap(imageSize.Width, imageSize.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.Gray, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            return bmp;
        }

        #region インスタンス変数

        private bool disposing = false;

        private readonly ImageViewerPageParameter parameter = null;
        private string leftImageFilePath = string.Empty;
        private string rightImageFilePath = string.Empty;
        private ImageDisplayMode displayMode = ImageDisplayMode.LeftFacing;
        private ImageSizeMode sizeMode = ImageSizeMode.FitOnlyBigImage;
        private IList<string> filePathList = null;

        private TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileGetResult> imageFileReadJob = null;
        private OneWayJob<BookmarkAddJob, ValueParameter<string>> addBookmarkJob = null;
        private OneWayJob<SingleFileExportJob, SingleFileExportParameter> singleFileExportJob = null;
        private OneWayJob<ImageInfoCacheJob, ListParameter<string>> imageInfoCacheJob = null;

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

        private TwoWayJob<ImageFileReadJob, ImageFileReadParameter, ImageFileGetResult> ImageFileReadJob
        {
            get
            {
                if (this.imageFileReadJob == null)
                {
                    this.imageFileReadJob = new();
                    this.imageFileReadJob
                        .Callback(r =>
                        {
                            var sw = Stopwatch.StartNew();
                            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageViewerPage.ImageFileReadJob_Callback: Start IsMain = {r.IsMain}");

                            this.ImageFileReadJob_Callback(r);

                            sw.Stop();
                            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageViewerPage.ImageFileReadJob_Callback: {sw.ElapsedMilliseconds} ms");
                        })
                        .Cancel(() =>
                        {
                            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageViewerPage.ImageFileReadJob.Cancel");
                        })
                        .Complete(() =>
                        {
                            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageViewerPage.ImageFileReadJob.Complete");
                        })
                        .StartThread();
                }

                return this.imageFileReadJob;
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

        private OneWayJob<SingleFileExportJob, SingleFileExportParameter> SingleFileExportJob
        {
            get
            {
                if (this.singleFileExportJob == null)
                {
                    this.singleFileExportJob = new();
                    this.singleFileExportJob
                        .StartThread();
                }

                return this.singleFileExportJob;
            }
        }

        private OneWayJob<ImageInfoCacheJob, ListParameter<string>> ImageInfoCacheJob
        {
            get
            {
                if (this.imageInfoCacheJob == null)
                {
                    this.imageInfoCacheJob = new();
                    this.imageInfoCacheJob
                        .StartThread();
                }

                return this.imageInfoCacheJob;
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
            this.SelectedFilePath = parameter.SelectedFilePath;
        }

        #endregion

        #region パブリックメソッド

        public override void RedrawPage()
        {
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

            this.ChangeImagePanelSize();
        }

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.disposing = true;

                if (this.addBookmarkJob != null)
                {
                    this.addBookmarkJob.Dispose();
                    this.addBookmarkJob = null;
                }

                if (this.singleFileExportJob != null)
                {
                    this.singleFileExportJob.Dispose();
                    this.singleFileExportJob = null;
                }

                if (this.imageFileReadJob != null)
                {
                    this.imageFileReadJob.Dispose();
                    this.imageFileReadJob = null;
                }

                if (this.imageInfoCacheJob != null)
                {
                    this.imageInfoCacheJob.Dispose();
                    this.imageInfoCacheJob = null;
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
            if (this.disposing)
            {
                return;
            }

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
                e.TitleColor, e.TitleFormatFlags, e.TextStyle);
        }

        protected override void OnResize(EventArgs e)
        {
            this.RedrawPage();
            base.OnResize(e);
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

        private Size GetImageSize(string filePath)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return ImageUtil.GetImageInfo(filePath).Size;
            }
            catch (FileUtilException ex)
            {
                Logger.Error(ex);
                return new Size(this.checkPatternPanel.Size.Width / 2, this.checkPatternPanel.Size.Height);
            }
            catch (ImageUtilException ex)
            {
                Logger.Error(ex);
                return new Size(this.checkPatternPanel.Size.Width / 2, this.checkPatternPanel.Size.Height);
            }
            finally
            {
                sw.Stop();
                Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageViewerPage.GetImageSize: {sw.ElapsedMilliseconds} ms");
            }
        }

        private void ChangeImagePanelSize()
        {
            var mainImageDrawAction = () =>
            {
                var w = this.checkPatternPanel.Width;
                var h = this.checkPatternPanel.Height;
                var x = 0;
                var y = 0;

                this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                this.leftImagePanel.ImageAlign = ImageAlign.Center;
                this.leftImagePanel.Update(true);

                this.rightImagePanel.Visible = false;
            };

            var bothImageDrawAction = () =>
            {
                var w = (int)(this.checkPatternPanel.Width / 2f);
                var h = this.checkPatternPanel.Height;
                var lx = 0;
                var rx = this.checkPatternPanel.Width - w;
                var y = 0;

                this.leftImagePanel.SetBounds(lx, y, w, h, BoundsSpecified.All);
                this.leftImagePanel.ImageAlign = ImageAlign.Right;
                this.leftImagePanel.Update(true);

                this.rightImagePanel.SetBounds(rx, y, w, h, BoundsSpecified.All);
                this.rightImagePanel.ImageAlign = ImageAlign.Left;
                this.rightImagePanel.Update(true);
            };

            if (this.displayMode == ImageDisplayMode.LeftFacing)
            {
                if (this.leftImagePanel.HasImage
                    && this.rightImagePanel.HasImage)
                {
                    bothImageDrawAction();
                }
                else
                {
                    mainImageDrawAction();
                }
            }
            else if (this.displayMode == ImageDisplayMode.RightFacing)
            {
                if (this.leftImagePanel.HasImage
                    && this.rightImagePanel.HasImage)
                {
                    bothImageDrawAction();
                }
                else
                {
                    mainImageDrawAction();
                }
            }
            else if (this.displayMode == ImageDisplayMode.Single)
            {
                mainImageDrawAction();
            }
        }

        private void ChangeImagePanelSize(ImageFileGetResult e)
        {
            if (e.IsMain && e.HasSub)
            {
                var w = (int)(this.checkPatternPanel.Width / 2f);
                var h = this.checkPatternPanel.Height;
                var y = 0;

                if (this.displayMode == ImageDisplayMode.LeftFacing)
                {
                    var x = 0;
                    this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                    this.leftImagePanel.ImageAlign = ImageAlign.Right;
                    this.leftImagePanel.Update(e.IsUpdate);
                }
                else
                {
                    var x = this.checkPatternPanel.Width - w;
                    this.rightImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                    this.rightImagePanel.ImageAlign = ImageAlign.Left;
                    this.rightImagePanel.Update(e.IsUpdate);
                }
            }
            else if (!e.IsMain)
            {
                var w = (int)(this.checkPatternPanel.Width / 2f);
                var h = this.checkPatternPanel.Height;
                var y = 0;

                if (this.displayMode == ImageDisplayMode.LeftFacing)
                {
                    var x = this.checkPatternPanel.Width - w;
                    this.rightImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                    this.rightImagePanel.ImageAlign = ImageAlign.Left;
                    this.rightImagePanel.Update(e.IsUpdate);
                }
                else
                {
                    var x = 0;
                    this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                    this.leftImagePanel.ImageAlign = ImageAlign.Right;
                    this.leftImagePanel.Update(e.IsUpdate);
                }
            }
            else if (e.IsMain && !e.HasSub)
            {
                var w = this.checkPatternPanel.Width;
                var h = this.checkPatternPanel.Height;
                var x = 0;
                var y = 0;

                this.leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                this.leftImagePanel.ImageAlign = ImageAlign.Center;
                this.leftImagePanel.Update(e.IsUpdate);
                this.rightImagePanel.Visible = false;
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
            var sw = Stopwatch.StartNew();

            var mainFilePath = this.filePathList[this.FilePathListIndex];
            this.SelectedFilePath = mainFilePath;

            var nextFiles = new List<string>(10);
            var nextIndex = this.GetNextIndex(this.FilePathListIndex, true);
            nextFiles.Add(this.filePathList[nextIndex]);
            while (nextFiles.Count < nextFiles.Capacity)
            {
                nextIndex = this.GetNextIndex(nextIndex, true);
                nextFiles.Add(this.filePathList[nextIndex]);
            }

            var prevFiles = new List<string>(4);
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

            this.ImageInfoCacheJob.StartJob([.. nextFiles, .. prevFiles]);
            this.ImageFileReadJob.StartJob(param);
            this.DrawLoadingImage(param, mainFilePath);

            sw.Stop();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageViewerPage.ReadImage: {sw.ElapsedMilliseconds} ms");
        }

        private async void DrawLoadingImage(ImageFileReadParameter parameter, string mainFilePath)
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageViewerPage.DrawLoadingImage: Start");

            var mainSize = this.GetImageSize(mainFilePath);

            if (parameter.ImageDisplayMode != ImageDisplayMode.Single
                && mainSize != ImageUtil.EMPTY_SIZE
                && mainSize.Width < mainSize.Height)
            {
                var subtIndex = parameter.CurrentIndex + 1;
                if (subtIndex > parameter.FilePathList.Count - 1)
                {
                    subtIndex = 0;
                }

                var subFilePath = parameter.FilePathList[subtIndex];
                var subSize = this.GetImageSize(subFilePath);

                if (subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width < subSize.Height)
                {
                    this.ImageFileReadJob_Callback(CreateEmptyResult(
                        mainFilePath, true, true, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));
                    this.ImageFileReadJob_Callback(CreateEmptyResult(
                        subFilePath, false, true, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));
                }
                else
                {
                    this.ImageFileReadJob_Callback(CreateEmptyResult(
                        mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));
                }
            }
            else
            {
                this.ImageFileReadJob_Callback(CreateEmptyResult(
                    mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));
            }

            var context = SynchronizationContext.Current;
            await Task.Run(() =>
            {
                Thread.Sleep(100);

                context.Send(_ =>
                {
                    this.ChangeImagePanelSize();
                }, null);
            });

            sw.Stop();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageViewerPage.DrawLoadingImage: {sw.ElapsedMilliseconds} ms");
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

        private void ImageFileReadJob_Callback(ImageFileGetResult e)
        {
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
                this.SelectedFilePath = e.Image.FilePath;
                this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(e.Image.FilePath));
            }

            if (this.displayMode == ImageDisplayMode.Single)
            {
                this.leftImagePanel.ClearImage();
                this.leftImageFilePath = e.Image.FilePath;
                this.rightImagePanel.ClearImage();
                this.rightImageFilePath = string.Empty;

                if (e.Image.IsError)
                {
                    this.leftImagePanel.SetError();
                }
                else
                {
                    this.leftImagePanel.SetImage(
                        this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.IsEmpty, e.Image.FilePath);
                    var scale = GetImageScale(e.Image.Image.Size, bgSize, this.sizeMode);
                    this.leftImagePanel.SetScale(scale);
                }
            }
            else if (this.displayMode == ImageDisplayMode.LeftFacing)
            {
                if (e.IsMain && e.HasSub)
                {
                    this.leftImagePanel.ClearImage();
                    this.leftImageFilePath = e.Image.FilePath;
                    this.rightImageFilePath = string.Empty;

                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.IsEmpty, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Image.Size, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
                else if (!e.IsMain)
                {
                    this.rightImagePanel.ClearImage();
                    this.rightImageFilePath = e.Image.FilePath;

                    if (e.Image.IsError)
                    {
                        this.rightImagePanel.SetError();
                    }
                    else
                    {
                        this.rightImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.IsEmpty, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Image.Size, bgSize, this.sizeMode);
                        this.rightImagePanel.SetScale(scale);
                    }
                }
                else if (e.IsMain)
                {
                    this.leftImagePanel.ClearImage();
                    this.leftImageFilePath = e.Image.FilePath;
                    this.rightImagePanel.ClearImage();
                    this.rightImageFilePath = string.Empty;

                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.IsEmpty, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Image.Size, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
            }
            else if (this.displayMode == ImageDisplayMode.RightFacing)
            {
                if (e.IsMain && e.HasSub)
                {
                    this.rightImagePanel.ClearImage();
                    this.rightImageFilePath = e.Image.FilePath;
                    this.leftImageFilePath = string.Empty;

                    if (e.Image.IsError)
                    {
                        this.rightImagePanel.SetError();
                    }
                    else
                    {
                        this.rightImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.IsEmpty, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Image.Size, bgSize, this.sizeMode);
                        this.rightImagePanel.SetScale(scale);
                    }
                }
                else if (!e.IsMain)
                {
                    this.leftImagePanel.ClearImage();
                    this.leftImageFilePath = e.Image.FilePath;

                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.IsEmpty, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Image.Size, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
                else if (e.IsMain)
                {
                    this.leftImagePanel.ClearImage();
                    this.leftImageFilePath = e.Image.FilePath;
                    this.rightImagePanel.ClearImage();
                    this.rightImageFilePath = string.Empty;

                    if (e.Image.IsError)
                    {
                        this.leftImagePanel.SetError();
                    }
                    else
                    {
                        this.leftImagePanel.SetImage(
                            this.sizeMode, e.Image.Image, e.Image.Thumbnail, e.Image.IsEmpty, e.Image.FilePath);
                        var scale = GetImageScale(e.Image.Image.Size, bgSize, this.sizeMode);
                        this.leftImagePanel.SetScale(scale);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"不正な画像表示モードです。DisplayMode: '{this.displayMode}'");
            }

            this.ChangeImagePanelSize(e);
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
            if (string.IsNullOrEmpty(this.leftImageFilePath))
            {
                return;
            }

            if (this.SelectedFilePath != this.leftImageFilePath)
            {
                this.SelectedFilePath = this.leftImageFilePath;
                this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.leftImageFilePath));
            }
        }

        private void RightImagePanel_ImageMouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(this.rightImageFilePath))
            {
                return;
            }

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

        #endregion
    }
}
