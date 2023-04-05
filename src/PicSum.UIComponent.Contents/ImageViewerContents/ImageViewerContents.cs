using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PicSum.Core.Base.Conf;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.ContentsParameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using SWF.UIComponent.ImagePanel;
using PicSum.UIComponent.Contents.ContentsToolBar;
using System.Security.Permissions;
using System.Diagnostics;

namespace PicSum.UIComponent.Contents.ImageViewerContents
{
    /// <summary>
    /// 画像ビューアコンテンツ
    /// </summary>
    public partial class ImageViewerContents : BrowserContents
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private readonly ImageViewerContentsParameter _parameter = null;
        private readonly Dictionary<string, Size> _imageSizeDictionary = new Dictionary<string, Size>();
        private string _leftImageFilePath = string.Empty;
        private string _rightImageFilePath = string.Empty;
        private ImageDisplayMode _displayMode = ImageDisplayMode.LeftFacing;
        private ImageSizeMode _sizeMode = ImageSizeMode.FitOnlyBigImage;

        private TwoWayProcess<ReadImageFileAsyncFacade, ReadImageFileParameterEntity, ReadImageFileResultEntity> _readImageFileProcess = null;
        private TwoWayProcess<GetImageSizeAsyncFacade, ListEntity<string>, ImageSizeEntity> _getImageSizeProcess = null;
        private OneWayProcess<AddFileViewHistoryAsyncFacade, ListEntity<string>> _addHistoryProcess = null;
        private OneWayProcess<AddKeepAsyncFacade, ListEntity<string>> _addKeepProcess = null;
        private OneWayProcess<ExportFileAsyncFacade, ExportFileParameterEntity> _exportFileProcess = null;

        #endregion

        #region パブリックプロパティ

        public override string SelectedFilePath
        {
            get
            {
                return _parameter.SelectedFilePath;
            }
        }

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private int maximumIndex
        {
            get
            {
                return indexToolStripSlider.MaximumValue;
            }
            set
            {
                indexToolStripSlider.MaximumValue = value;
            }
        }

        private int filePathListIndex
        {
            get
            {
                return indexToolStripSlider.Value;
            }
            set
            {
                indexToolStripSlider.Value = value;
            }
        }

        private TwoWayProcess<ReadImageFileAsyncFacade, ReadImageFileParameterEntity, ReadImageFileResultEntity> readImageFileProcess
        {
            get
            {
                if (_readImageFileProcess == null)
                {
                    _readImageFileProcess = TaskManager.CreateTwoWayProcess<ReadImageFileAsyncFacade, ReadImageFileParameterEntity, ReadImageFileResultEntity>(ProcessContainer);
                    _readImageFileProcess.Callback += new AsyncTaskCallbackEventHandler<ReadImageFileResultEntity>(readImageFileProcess_Callback);
                    _readImageFileProcess.SuccessEnd += new EventHandler(readImageFileProcess_SuccessEnd);
                    _readImageFileProcess.ErrorEnd += new EventHandler(readImageFileProcess_ErrorEnd);
                }

                return _readImageFileProcess;
            }
        }

        private TwoWayProcess<GetImageSizeAsyncFacade, ListEntity<string>, ImageSizeEntity> getImageSizeProcess
        {
            get
            {
                if (_getImageSizeProcess == null)
                {
                    _getImageSizeProcess = TaskManager.CreateTwoWayProcess<GetImageSizeAsyncFacade, ListEntity<string>, ImageSizeEntity>(ProcessContainer);
                    _getImageSizeProcess.Callback += new AsyncTaskCallbackEventHandler<ImageSizeEntity>(getImageSizeProcess_Callback);
                }

                return _getImageSizeProcess;
            }
        }

        private OneWayProcess<AddFileViewHistoryAsyncFacade, ListEntity<string>> addHistoryProcess
        {
            get
            {
                if (_addHistoryProcess == null)
                {
                    _addHistoryProcess = TaskManager.CreateOneWayProcess<AddFileViewHistoryAsyncFacade, ListEntity<string>>(ProcessContainer);
                }

                return _addHistoryProcess;
            }
        }

        private OneWayProcess<AddKeepAsyncFacade, ListEntity<string>> addKeepProcess
        {
            get
            {
                if (_addKeepProcess == null)
                {
                    _addKeepProcess = TaskManager.CreateOneWayProcess<AddKeepAsyncFacade, ListEntity<string>>(ProcessContainer);
                }

                return _addKeepProcess;
            }
        }

        private OneWayProcess<ExportFileAsyncFacade, ExportFileParameterEntity> exportFileProcess
        {
            get
            {
                if (_exportFileProcess == null)
                {
                    _exportFileProcess = TaskManager.CreateOneWayProcess<ExportFileAsyncFacade, ExportFileParameterEntity>(ProcessContainer);
                }

                return _exportFileProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public ImageViewerContents()
        {
            InitializeComponent();
        }

        public ImageViewerContents(ImageViewerContentsParameter param)
            : this()
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            _parameter = param;

            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            filePathListIndex = _parameter.FilePathList.IndexOf(_parameter.SelectedFilePath);
        }

        protected override void OnResize(EventArgs e)
        {
            changeImagePanelSize();
            base.OnResize(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                if (e.Delta > 0)
                {
                    filePathListIndex = getPreviewIndex();
                }
                else
                {
                    filePathListIndex = getNextIndex();
                }
            }
            catch (ImageException ex)
            {
                MessageBox.Show(ex.Message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            base.OnMouseWheel(e);
        }

        [UIPermission(SecurityAction.Demand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Right)
            {
                filePathListIndex = getNextIndex();
                return true;
            }
            else if ((keyData & Keys.KeyCode) == Keys.Left)
            {
                filePathListIndex = getPreviewIndex();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }


        protected override void OnDrawTabContents(SWF.UIComponent.TabOperation.DrawTabEventArgs e)
        {
            e.Graphics.DrawImage(this.Icon, e.IconRectangle);
            DrawTextUtil.DrawText(e.Graphics, this.Title, e.Font, e.TextRectangle, e.TitleColor, e.TitleFormatFlags, e.TextStyle);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Icon = Resources.ViewerIcon;
            maximumIndex = _parameter.FilePathList.Count - 1;
            setDisplayMode(ImageViewerContentsConfig.ImageDisplayMode);
            setSizeMode(ImageViewerContentsConfig.ImageSizeMode);
            setThumbnailPanelVisible();
        }

        private void changeImagePanelSize()
        {
            if (leftImagePanel.HasImage && rightImagePanel.HasImage)
            {
                int w = (int)(checkPatternPanel.Width / 2f);
                int h = checkPatternPanel.Height;
                int lx = 0;
                int rx = checkPatternPanel.Width - w;
                int y = 0;

                leftImagePanel.SetBounds(lx, y, w, h, BoundsSpecified.All);
                rightImagePanel.SetBounds(rx, y, w, h, BoundsSpecified.All);

                leftImagePanel.ImageAlign = ImageAlign.Right;
                rightImagePanel.ImageAlign = ImageAlign.Left;

                leftImagePanel.Visible = true;
                rightImagePanel.Visible = true;
            }
            else if (leftImagePanel.HasImage)
            {
                int w = checkPatternPanel.Width;
                int h = checkPatternPanel.Height;
                int x = 0;
                int y = 0;

                leftImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                leftImagePanel.ImageAlign = ImageAlign.Center;

                leftImagePanel.Visible = true;
                rightImagePanel.Visible = false;
            }
            else if (rightImagePanel.HasImage)
            {
                int w = checkPatternPanel.Width;
                int h = checkPatternPanel.Height;
                int x = 0;
                int y = 0;

                rightImagePanel.SetBounds(x, y, w, h, BoundsSpecified.All);
                rightImagePanel.ImageAlign = ImageAlign.Center;

                leftImagePanel.Visible = false;
                rightImagePanel.Visible = true;
            }
            else
            {
                leftImagePanel.Visible = false;
                rightImagePanel.Visible = false;
            }
        }

        private Size getImageSize(string filePath)
        {
            if (_imageSizeDictionary.ContainsKey(filePath))
            {
                return _imageSizeDictionary[filePath];
            }
            else
            {
                return ImageUtil.GetImageSize(filePath);
            }
        }

        private int getNextIndex()
        {
            if (_displayMode == ImageDisplayMode.Single)
            {
                if (filePathListIndex == maximumIndex)
                {
                    return 0;
                }
                else
                {
                    return filePathListIndex + 1;
                }
            }
            else
            {
                int currentIndex = filePathListIndex;
                string currentFilePath = _parameter.FilePathList[currentIndex];
                Size currentImageSize = getImageSize(currentFilePath);
                if (currentImageSize.Width < currentImageSize.Height)
                {
                    int nextIndex = currentIndex + 1;
                    if (nextIndex > maximumIndex)
                    {
                        nextIndex = 0;
                    }

                    string nextFilePath = _parameter.FilePathList[nextIndex];
                    Size nextImageSize = getImageSize(nextFilePath);
                    if (nextImageSize.Width < nextImageSize.Height)
                    {
                        if (nextIndex == maximumIndex)
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
                    if (filePathListIndex == maximumIndex)
                    {
                        return 0;
                    }
                    else
                    {
                        return filePathListIndex + 1;
                    }
                }
            }
        }

        private int getPreviewIndex()
        {
            if (_displayMode == ImageDisplayMode.Single)
            {
                if (filePathListIndex == 0)
                {
                    return maximumIndex;
                }
                else
                {
                    return filePathListIndex - 1;
                }
            }
            else
            {
                int prevIndex1 = filePathListIndex - 1;
                if (prevIndex1 < 0)
                {
                    prevIndex1 = maximumIndex;
                }

                string prevFilePath1 = _parameter.FilePathList[prevIndex1];
                Size prevImageSize1 = getImageSize(prevFilePath1);
                if (prevImageSize1.Width < prevImageSize1.Height)
                {
                    int prevIndex2 = prevIndex1 - 1;
                    if (prevIndex2 < 0)
                    {
                        prevIndex2 = maximumIndex;
                    }

                    string prevFilePath2 = _parameter.FilePathList[prevIndex2];
                    Size prevImageSize2 = getImageSize(prevFilePath2);
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

        private void readImage()
        {
            this.Cursor = Cursors.WaitCursor;

            ReadImageFileParameterEntity param = new ReadImageFileParameterEntity();
            param.CurrentIndex = filePathListIndex;
            param.FilePathList = _parameter.FilePathList;
            param.ImageDisplayMode = _displayMode;
            param.ImageSizeMode = _sizeMode;
            param.DrawSize = checkPatternPanel.Size;
            param.ThumbnailSize = leftImagePanel.ThumbnailSize;

            readImageFileProcess.Cancel();
            readImageFileProcess.Execute(this, param);
        }

        private void readImageSize()
        {
            if (_parameter.FilePathList.Count < 4)
            {
                return;
            }

            Func<int, int> getIndexMethod = (i =>
            {
                if (i < 0)
                {
                    return i + _parameter.FilePathList.Count;
                }
                else if (i > _parameter.FilePathList.Count - 1)
                {
                    return i - _parameter.FilePathList.Count;
                }
                else
                {
                    return i;
                }
            });

            ListEntity<string> param = new ListEntity<string>();
            int startIndex = filePathListIndex - 2;
            int endIndex = filePathListIndex + 1;
            for (int i = startIndex; i <= endIndex; i++)
            {
                int index = getIndexMethod(i);
                string filePath = _parameter.FilePathList[index];
                if (!param.Contains(filePath))
                {
                    param.Add(filePath);
                }
            }

            getImageSizeProcess.Cancel();
            getImageSizeProcess.Execute(this, param);
        }

        private void setTitle(string selectedFilePath)
        {
            this.Title = FileUtil.GetFileName(selectedFilePath);
            OnSelectedFileChanged(new SelectedFileChangeEventArgs(selectedFilePath));
        }

        private void addKeep(IList<string> filePathList)
        {
            ListEntity<string> param = new ListEntity<string>(filePathList);
            addKeepProcess.Execute(this, param);
        }

        private void doDragDrop(string currentFilePath)
        {
            DragEntity dragData = new DragEntity();
            dragData.CurrentFilePath = currentFilePath;
            dragData.FilePathList = _parameter.FilePathList;
            dragData.SourceControl = this;
            this.DoDragDrop(dragData, DragDropEffects.All);
        }

        private bool setDisplayMode(ImageDisplayMode mode)
        {
            singleViewToolStripMenuItem.Checked = mode == ImageDisplayMode.Single;
            leftFacingViewToolStripMenuItem.Checked = mode == ImageDisplayMode.LeftFacing;
            rightFacingViewToolStripMenuItem.Checked = mode == ImageDisplayMode.RightFacing;

            if (_displayMode != mode)
            {
                _displayMode = mode;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool setSizeMode(ImageSizeMode mode)
        {
            originalSizeToolStripMenuItem.Checked = mode == ImageSizeMode.Original;
            allFitSizeToolStripMenuItem.Checked = mode == ImageSizeMode.FitAllImage;
            onlyBigImageFitSizeToolStripMenuItem.Checked = mode == ImageSizeMode.FitOnlyBigImage;

            if (_sizeMode != mode)
            {
                _sizeMode = mode;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void setThumbnailPanelVisible()
        {
            leftImagePanel.IsShowThumbnailPanel = _sizeMode == ImageSizeMode.Original;
            rightImagePanel.IsShowThumbnailPanel = leftImagePanel.IsShowThumbnailPanel;
        }

        private ImageDisplayMode getNextDisplayMode()
        {
            if (_displayMode == ImageDisplayMode.Single)
            {
                return ImageDisplayMode.LeftFacing;
            }
            else if (_displayMode == ImageDisplayMode.LeftFacing)
            {
                return ImageDisplayMode.RightFacing;
            }
            else
            {
                return ImageDisplayMode.Single;
            }
        }

        private ImageSizeMode getNextSizeMode()
        {
            if (_sizeMode == ImageSizeMode.Original)
            {
                return ImageSizeMode.FitAllImage;
            }
            else if (_sizeMode == ImageSizeMode.FitAllImage)
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

        private void readImageFileProcess_Callback(object sender, ReadImageFileResultEntity e)
        {
            if (e.ReadImageFileException != null)
            {
                MessageBox.Show(e.ReadImageFileException.Message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (leftImagePanel.HasImage)
            {
                _leftImageFilePath = string.Empty;
                leftImagePanel.ClearImage();
            }

            if (rightImagePanel.HasImage)
            {
                _rightImageFilePath = string.Empty;
                rightImagePanel.ClearImage();
            }

            int index = _parameter.FilePathList.IndexOf(e.Image1.FilePath);
            if (index != filePathListIndex)
            {
                return;
            }

            _parameter.SelectedFilePath = e.Image1.FilePath;
            setTitle(e.Image1.FilePath);

            if (_displayMode == ImageDisplayMode.Single)
            {
                _leftImageFilePath = e.Image1.FilePath;
                leftImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
            }
            else if (_displayMode == ImageDisplayMode.LeftFacing)
            {
                _leftImageFilePath = e.Image1.FilePath;
                leftImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
                if (e.Image2 != null)
                {
                    _rightImageFilePath = e.Image2.FilePath;
                    rightImagePanel.SetImage(e.Image2.Image, e.Image2.Thumbnail);
                }
            }
            else if (_displayMode == ImageDisplayMode.RightFacing)
            {
                _rightImageFilePath = e.Image1.FilePath;
                rightImagePanel.SetImage(e.Image1.Image, e.Image1.Thumbnail);
                if (e.Image2 != null)
                {
                    _leftImageFilePath = e.Image2.FilePath;
                    leftImagePanel.SetImage(e.Image2.Image, e.Image2.Thumbnail);
                }
            }

            changeImagePanelSize();

            leftImagePanel.Invalidate();
            rightImagePanel.Invalidate();

            // TODO: 2012/05/04 重いので行わないことにした。
            //ListEntity<string> param = new ListEntity<string>();
            //param.Add(e.Image1.FilePath);
            //if (e.Image2 != null)
            //{
            //    param.Add(e.Image2.FilePath);
            //}

            //addHistoryProcess.Execute(this, param);

            this.Focus();
        }

        private void readImageFileProcess_SuccessEnd(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void readImageFileProcess_ErrorEnd(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void getImageSizeProcess_Callback(object sender, ImageSizeEntity e)
        {
            if (!_imageSizeDictionary.ContainsKey(e.FilePath))
            {
                _imageSizeDictionary.Add(e.FilePath, e.ImageSize);
            }
        }

        #endregion

        #region ツールバーイベント

        private void viewToolStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (setDisplayMode(getNextDisplayMode()))
            {
                ImageViewerContentsConfig.ImageDisplayMode = _displayMode;
                readImage();
            }
        }

        private void singleViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (setDisplayMode(ImageDisplayMode.Single))
            {
                ImageViewerContentsConfig.ImageDisplayMode = _displayMode;
                readImage();
            }
        }

        private void leftFacingViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (setDisplayMode(ImageDisplayMode.LeftFacing))
            {
                ImageViewerContentsConfig.ImageDisplayMode = _displayMode;
                readImage();
            }
        }

        private void rightFacingViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (setDisplayMode(ImageDisplayMode.RightFacing))
            {
                ImageViewerContentsConfig.ImageDisplayMode = _displayMode;
                readImage();
            }
        }

        private void sizeToolStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (setSizeMode(getNextSizeMode()))
            {
                ImageViewerContentsConfig.ImageSizeMode = _sizeMode;
                setThumbnailPanelVisible();
                readImage();
            }
        }

        private void originalSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (setSizeMode(ImageSizeMode.Original))
            {
                ImageViewerContentsConfig.ImageSizeMode = _sizeMode;
                setThumbnailPanelVisible();
                readImage();
            }
        }

        private void allFitSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (setSizeMode(ImageSizeMode.FitAllImage))
            {
                ImageViewerContentsConfig.ImageSizeMode = _sizeMode;
                setThumbnailPanelVisible();
                readImage();
            }
        }

        private void onlyBigImageFitSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (setSizeMode(ImageSizeMode.FitOnlyBigImage))
            {
                ImageViewerContentsConfig.ImageSizeMode = _sizeMode;
                setThumbnailPanelVisible();
                readImage();
            }
        }

        private void previewIndexToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                filePathListIndex = getPreviewIndex();
            }
            catch (ImageException ex)
            {
                MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nextIndexToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                filePathListIndex = getNextIndex();
            }
            catch (ImageException ex)
            {
                MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void indexToolStripSlider_ValueChanging(object sender, EventArgs e)
        {
            int index = filePathListIndex;
            if (index < 0 || _parameter.FilePathList.Count - 1 < index)
            {
                return;
            }

            string filePath = _parameter.FilePathList[index];
            Point p = this.PointToClient(Cursor.Position);
            filePathToolTip.Show(filePath, this, p.X, -16, 5000);
        }

        private void indexToolStripSlider_ValueChanged(object sender, EventArgs e)
        {
            filePathToolTip.Hide(this);
            readImageSize();
            readImage();
        }

        #endregion

        #region 画像パネルイベント

        private void leftImagePanel_MouseDown(object sender, MouseEventArgs e)
        {
            leftImagePanel.Focus();
        }

        private void rightImagePanel_MouseDown(object sender, MouseEventArgs e)
        {
            rightImagePanel.Focus();
        }

        private void leftImagePanel_ImageMouseClick(object sender, MouseEventArgs e)
        {
            setTitle(_leftImageFilePath);

            if (e.Button == MouseButtons.Middle)
            {
                if (!string.IsNullOrEmpty(_leftImageFilePath))
                {
                    addKeep(new string[] { _leftImageFilePath });
                }
            }
        }

        private void rightImagePanel_ImageMouseClick(object sender, MouseEventArgs e)
        {
            setTitle(_rightImageFilePath);

            if (e.Button == MouseButtons.Middle)
            {
                if (!string.IsNullOrEmpty(_rightImageFilePath))
                {
                    addKeep(new string[] { _rightImageFilePath });
                }
            }
        }

        private void leftImagePanel_DragStart(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_leftImageFilePath))
            {
                doDragDrop(_leftImageFilePath);
            }
        }

        private void rightImagePanel_DragStart(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_rightImageFilePath))
            {
                doDragDrop(_rightImageFilePath);
            }
        }

        #endregion

        #region ファイルコンテキストメニューイベント

        private void fileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (fileContextMenu.SourceControl.Equals(leftImagePanel))
            {
                if (!string.IsNullOrEmpty(_leftImageFilePath))
                {
                    fileContextMenu.SetFile(_leftImageFilePath);
                    return;
                }
            }
            else if (fileContextMenu.SourceControl.Equals(rightImagePanel))
            {
                if (!string.IsNullOrEmpty(_rightImageFilePath))
                {
                    fileContextMenu.SetFile(_rightImageFilePath);
                    return;
                }
            }

            e.Cancel = true;
        }

        private void fileContextMenu_FileNewTabOpen(object sender, PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs e)
        {
            ImageViewerContentsParameter param = new ImageViewerContentsParameter(_parameter.FilePathList, e.FilePath);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.AddTab, param));
        }

        private void fileContextMenu_FileOtherWindowOpen(object sender, PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs e)
        {
            ImageViewerContentsParameter param = new ImageViewerContentsParameter(_parameter.FilePathList, e.FilePath);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OtherWindow, param));
        }

        private void fileContextMenu_FileNewWindowOpen(object sender, PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs e)
        {
            ImageViewerContentsParameter param = new ImageViewerContentsParameter(_parameter.FilePathList, e.FilePath);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.NewWindow, param));
        }

        private void fileContextMenu_FileOpen(object sender, PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs e)
        {
            FileUtil.OpenFile(e.FilePath);
        }

        private void fileContextMenu_SaveFolderOpen(object sender, PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs e)
        {
            FileUtil.OpenExplorerSelect(e.FilePath);
        }

        private void fileContextMenu_Export(object sender, PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (FileUtil.IsExists(CommonConfig.ExportFolderPath))
                {
                    fbd.SelectedPath = CommonConfig.ExportFolderPath;
                }

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ExportFileParameterEntity param = new ExportFileParameterEntity();
                    param.ExportFolderPath = fbd.SelectedPath;
                    param.FilePathList = e.FilePathList;
                    exportFileProcess.Execute(this, param);

                    CommonConfig.ExportFolderPath = fbd.SelectedPath;
                }
            }
        }

        private void fileContextMenu_PathCopy(object sender, PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs e)
        {
            Clipboard.SetText(e.FilePathList[0]);
        }

        private void fileContextMenu_NameCopy(object sender, PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs e)
        {
            Clipboard.SetText(FileUtil.GetFileName(e.FilePathList[0]));
        }

        private void fileContextMenu_AddKeep(object sender, PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs e)
        {
            addKeep(e.FilePathList);
        }

        #endregion
    }
}
