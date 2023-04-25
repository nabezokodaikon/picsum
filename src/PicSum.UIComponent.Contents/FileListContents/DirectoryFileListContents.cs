using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using PicSum.UIComponent.Contents.ContentsParameter;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileListContents
{
    /// <summary>
    /// フォルダファイルリストコンテンツ
    /// </summary>
    internal class DirectoryFileListContents : FileListContentsBase
    {
        #region インスタンス変数

        private readonly DirectoryFileListContentsParameter _parameter = null;
        private TwoWayProcess<GetFilesByDirectoryAsyncFacade, SingleValueEntity<string>, GetDirectoryResult> _searchDirectoryProcess = null;
        private OneWayProcess<UpdateDirectoryStateAsynceFacade, DirectoryStateEntity> _updateDirectoryStateProcess = null;
        private OneWayProcess<AddDirectoryViewHistoryAsyncFacade, SingleValueEntity<string>> _addDirectoryHistoryProcess = null;
        private TwoWayProcess<GetNextDirectoryAsyncFacade, GetNextContentsParameter<string>, SingleValueEntity<string>> _getNextDirectoryProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFilesByDirectoryAsyncFacade, SingleValueEntity<string>, GetDirectoryResult> searchDirectoryProcess
        {
            get
            {
                if (_searchDirectoryProcess == null)
                {
                    _searchDirectoryProcess = TaskManager.CreateTwoWayProcess<GetFilesByDirectoryAsyncFacade, SingleValueEntity<string>, GetDirectoryResult>(ProcessContainer);
                    _searchDirectoryProcess.Callback += new AsyncTaskCallbackEventHandler<GetDirectoryResult>(searchDirectoryProcess_Callback);
                }

                return _searchDirectoryProcess;
            }
        }

        private OneWayProcess<UpdateDirectoryStateAsynceFacade, DirectoryStateEntity> updateDirectoryStateProcess
        {
            get
            {
                if (_updateDirectoryStateProcess == null)
                {
                    _updateDirectoryStateProcess = TaskManager.CreateOneWayProcess<UpdateDirectoryStateAsynceFacade, DirectoryStateEntity>(ProcessContainer);
                }

                return _updateDirectoryStateProcess;
            }
        }

        private OneWayProcess<AddDirectoryViewHistoryAsyncFacade, SingleValueEntity<string>> addDirectoryHistoryProcess
        {
            get
            {
                if (_addDirectoryHistoryProcess == null)
                {
                    _addDirectoryHistoryProcess = TaskManager.CreateOneWayProcess<AddDirectoryViewHistoryAsyncFacade, SingleValueEntity<string>>(ProcessContainer);
                }

                return _addDirectoryHistoryProcess;
            }
        }

        private TwoWayProcess<GetNextDirectoryAsyncFacade, GetNextContentsParameter<string>, SingleValueEntity<string>> getNextDirectoryProcess
        {
            get
            {
                if (_getNextDirectoryProcess == null)
                {
                    _getNextDirectoryProcess = TaskManager.CreateTwoWayProcess<GetNextDirectoryAsyncFacade, GetNextContentsParameter<string>, SingleValueEntity<string>>(ProcessContainer);
                    _getNextDirectoryProcess.Callback += new AsyncTaskCallbackEventHandler<SingleValueEntity<string>>(getNextDirectoryProcess_Callback);
                }

                return _getNextDirectoryProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public DirectoryFileListContents(DirectoryFileListContentsParameter param)
            : base(param)
        {
            _parameter = param;
            initializeComponent();
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SingleValueEntity<string> param = new SingleValueEntity<string>();
            param.Value = _parameter.DirectoryPath;
            searchDirectoryProcess.Execute(this, param);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                saveCurrentDirectoryState();
            }

            base.Dispose(disposing);
        }

        protected override void OnDrawTabContents(SWF.UIComponent.TabOperation.DrawTabEventArgs e)
        {
            e.Graphics.DrawImage(this.Icon, e.IconRectangle);
            DrawTextUtil.DrawText(e.Graphics, this.Title, e.Font, e.TextRectangle, e.TitleColor, e.TitleFormatFlags, e.TextStyle);
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            base.OnSelectedFileChanged(new SelectedFileChangeEventArgs(_parameter.DirectoryPath));
        }

        protected override void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            IList<string> filePathList = GetSelectedFiles();
            if (filePathList.Count > 0)
            {
                IsDirectoryActiveTabOpenMenuItemVisible = true;
                SetContextMenuFiles(filePathList);
            }
            else
            {
                IsDirectoryActiveTabOpenMenuItemVisible = false;
                SetContextMenuFiles(_parameter.DirectoryPath);
            }
        }

        protected override void OnRemoveFile(System.Collections.Generic.IList<string> filePathList)
        {
            // 処理無し。
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            GetNextContentsParameter<string> param = new GetNextContentsParameter<string>();
            param.CurrentParameter = new SingleValueEntity<string>();
            param.CurrentParameter.Value = _parameter.DirectoryPath;
            param.IsNext = false;
            getNextDirectoryProcess.Cancel();
            getNextDirectoryProcess.Execute(this, param);
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            GetNextContentsParameter<string> param = new GetNextContentsParameter<string>();
            param.CurrentParameter = new SingleValueEntity<string>();
            param.CurrentParameter.Value = _parameter.DirectoryPath;
            param.IsNext = true;
            getNextDirectoryProcess.Cancel();
            getNextDirectoryProcess.Execute(this, param);
        }

        protected override void GetImageFilesAction(ImageViewerContentsParameter paramter)
        {
            var proces = TaskManager.CreateTwoWayProcess<GetFilesByDirectoryAsyncFacade, SingleValueEntity<string>, GetDirectoryResult>(this.ProcessContainer);
            proces.Callback += ((sender, e) => 
            {
                if (e.DirectoryNotFoundException != null)
                {
                    ExceptionUtil.ShowErrorDialog(e.DirectoryNotFoundException);
                    return;
                }

                var imageFiles = e.FileInfoList
                    .Where(fileInfo => fileInfo.IsImageFile)
                    .Select(fileInfo => fileInfo.FilePath)
                    .ToArray();

                var ex = FileUtil.GetExtension(this.SelectedFilePath);
                var selectedFilePath = ImageUtil.ImageFileExtensionList.Contains(ex) ? 
                    this.SelectedFilePath: string.Empty;

                var eventArgs = new GetImageFilesEventArgs(imageFiles, selectedFilePath);
                paramter.OnGetImageFiles(eventArgs);
            });
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Title = FileUtil.GetFileName(_parameter.DirectoryPath);

            if (string.IsNullOrEmpty(_parameter.DirectoryPath))
            {
                this.Icon = FileIconCash.SmallMyComputerIcon;
            }
            else if (FileUtil.IsDrive(_parameter.DirectoryPath))
            {
                this.Icon = FileIconCash.GetSmallDriveIcon(_parameter.DirectoryPath);
            }
            else
            {
                this.Icon = FileIconCash.SmallDirectoryIcon;
            }

            this.IsAddKeepMenuItemVisible = true;
            this.IsRemoveFromListMenuItemVisible = false;
            this.IsMoveControlVisible = !string.IsNullOrEmpty(_parameter.DirectoryPath);
            base.sortFileRgistrationDateToolStripButton.Enabled = false;
        }

        private void saveCurrentDirectoryState()
        {
            DirectoryStateEntity param = new DirectoryStateEntity();

            param.DirectoryPath = _parameter.DirectoryPath;

            if (base.SortTypeID == SortTypeID.Default)
            {
                param.SortTypeID = SortTypeID.FileName;
                param.IsAscending = true;
            }
            else
            {
                param.SortTypeID = base.SortTypeID;
                param.IsAscending = base.IsAscending;
            }

            param.SelectedFilePath = base.SelectedFilePath;

            updateDirectoryStateProcess.Execute(this, param);
        }

        #endregion

        #region プロセスイベント

        private void searchDirectoryProcess_Callback(object sender, GetDirectoryResult e)
        {
            if (e.DirectoryNotFoundException != null)
            {
                ExceptionUtil.ShowErrorDialog(e.DirectoryNotFoundException);
                return;
            }

            if (!string.IsNullOrEmpty(_parameter.SelectedFilePath))
            {
                if (e.DirectoryState != null)
                {
                    base.SetFiles(e.FileInfoList, _parameter.SelectedFilePath, e.DirectoryState.SortTypeID, e.DirectoryState.IsAscending);
                }
                else
                {
                    base.SetFiles(e.FileInfoList, _parameter.SelectedFilePath, SortTypeID.FilePath, true);
                }
            }
            else
            {
                if (e.DirectoryState != null)
                {
                    base.SetFiles(e.FileInfoList, e.DirectoryState.SelectedFilePath, e.DirectoryState.SortTypeID, e.DirectoryState.IsAscending);
                    if (e.FileInfoList.Count < 1) 
                    {
                        base.OnSelectedFileChanged(new SelectedFileChangeEventArgs(e.DirectoryState.DirectoryPath));
                    }
                }
                else
                {
                    base.SetFiles(e.FileInfoList, string.Empty, SortTypeID.FilePath, true);
                }
            }

            SingleValueEntity<string> param = new SingleValueEntity<string>();
            param.Value = e.DirectoryPath;
            addDirectoryHistoryProcess.Execute(this, param);
        }

        private void getNextDirectoryProcess_Callback(object sender, SingleValueEntity<string> e)
        {
            DirectoryFileListContentsParameter param = new DirectoryFileListContentsParameter(e.Value);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        #endregion
    }
}
