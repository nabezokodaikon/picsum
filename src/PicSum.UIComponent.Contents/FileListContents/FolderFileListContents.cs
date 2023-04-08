using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PicSum.Core.Base.Conf;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Contents.ContentsParameter;
using SWF.Common;

namespace PicSum.UIComponent.Contents.FileListContents
{
    /// <summary>
    /// フォルダファイルリストコンテンツ
    /// </summary>
    internal class FolderFileListContents : FileListContentsBase
    {
        #region インスタンス変数

        private readonly FolderFileListContentsParameter _parameter = null;
        private TwoWayProcess<SearchFileByFolderAsyncFacade, SingleValueEntity<string>, SearchFolderResultEntity> _searchFolderProcess = null;
        private OneWayProcess<UpdateFolderStateAsynceFacade, FolderStateEntity> _updateFolderStateProcess = null;
        private OneWayProcess<AddFolderViewHistoryAsyncFacade, SingleValueEntity<string>> _addFolderHistoryProcess = null;
        private TwoWayProcess<GetNextFolderAsyncFacade, GetNextContentsParameterEntity<string>, SingleValueEntity<string>> _getNextFolderProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<SearchFileByFolderAsyncFacade, SingleValueEntity<string>, SearchFolderResultEntity> searchFolderProcess
        {
            get
            {
                if (_searchFolderProcess == null)
                {
                    _searchFolderProcess = TaskManager.CreateTwoWayProcess<SearchFileByFolderAsyncFacade, SingleValueEntity<string>, SearchFolderResultEntity>(ProcessContainer);
                    _searchFolderProcess.Callback += new AsyncTaskCallbackEventHandler<SearchFolderResultEntity>(searchFolderProcess_Callback);
                }

                return _searchFolderProcess;
            }
        }

        private OneWayProcess<UpdateFolderStateAsynceFacade, FolderStateEntity> updateFolderStateProcess
        {
            get
            {
                if (_updateFolderStateProcess == null)
                {
                    _updateFolderStateProcess = TaskManager.CreateOneWayProcess<UpdateFolderStateAsynceFacade, FolderStateEntity>(ProcessContainer);
                }

                return _updateFolderStateProcess;
            }
        }

        private OneWayProcess<AddFolderViewHistoryAsyncFacade, SingleValueEntity<string>> addFolderHistoryProcess
        {
            get
            {
                if (_addFolderHistoryProcess == null)
                {
                    _addFolderHistoryProcess = TaskManager.CreateOneWayProcess<AddFolderViewHistoryAsyncFacade, SingleValueEntity<string>>(ProcessContainer);
                }

                return _addFolderHistoryProcess;
            }
        }

        private TwoWayProcess<GetNextFolderAsyncFacade, GetNextContentsParameterEntity<string>, SingleValueEntity<string>> getNextFolderProcess
        {
            get
            {
                if (_getNextFolderProcess == null)
                {
                    _getNextFolderProcess = TaskManager.CreateTwoWayProcess<GetNextFolderAsyncFacade, GetNextContentsParameterEntity<string>, SingleValueEntity<string>>(ProcessContainer);
                    _getNextFolderProcess.Callback += new AsyncTaskCallbackEventHandler<SingleValueEntity<string>>(getNextFolderProcess_Callback);
                }

                return _getNextFolderProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public FolderFileListContents(FolderFileListContentsParameter param)
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
            param.Value = _parameter.FolderPath;
            searchFolderProcess.Execute(this, param);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                saveCurrentFolderState();
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
            base.OnSelectedFileChanged(new SelectedFileChangeEventArgs(_parameter.FolderPath));
        }

        protected override void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            IList<string> filePathList = GetSelectedFiles();
            if (filePathList.Count > 0)
            {
                IsFolderActiveTabOpenMenuItemVisible = true;
                SetContextMenuFiles(filePathList);
            }
            else
            {
                IsFolderActiveTabOpenMenuItemVisible = false;
                SetContextMenuFiles(_parameter.FolderPath);
            }
        }

        protected override void OnRemoveFile(System.Collections.Generic.IList<string> filePathList)
        {
            // 処理無し。
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            GetNextContentsParameterEntity<string> param = new GetNextContentsParameterEntity<string>();
            param.CurrentParameter = new SingleValueEntity<string>();
            param.CurrentParameter.Value = _parameter.FolderPath;
            param.IsNext = false;
            getNextFolderProcess.Cancel();
            getNextFolderProcess.Execute(this, param);
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            GetNextContentsParameterEntity<string> param = new GetNextContentsParameterEntity<string>();
            param.CurrentParameter = new SingleValueEntity<string>();
            param.CurrentParameter.Value = _parameter.FolderPath;
            param.IsNext = true;
            getNextFolderProcess.Cancel();
            getNextFolderProcess.Execute(this, param);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Title = FileUtil.GetFileName(_parameter.FolderPath);

            if (string.IsNullOrEmpty(_parameter.FolderPath))
            {
                this.Icon = FileIconCash.SmallMyComputerIcon;
            }
            else if (FileUtil.IsDrive(_parameter.FolderPath))
            {
                this.Icon = FileIconCash.GetSmallDriveIcon(_parameter.FolderPath);
            }
            else
            {
                this.Icon = FileIconCash.SmallFolderIcon;
            }

            this.IsAddKeepMenuItemVisible = true;
            this.IsRemoveFromListMenuItemVisible = false;
            this.IsMoveControlVisible = !string.IsNullOrEmpty(_parameter.FolderPath);
        }

        private void saveCurrentFolderState()
        {
            FolderStateEntity param = new FolderStateEntity();

            param.FolderPath = _parameter.FolderPath;

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

            updateFolderStateProcess.Execute(this, param);
        }

        #endregion

        #region プロセスイベント

        private void searchFolderProcess_Callback(object sender, SearchFolderResultEntity e)
        {
            if (e.DirectoryNotFoundException != null)
            {
                MessageBox.Show(e.DirectoryNotFoundException.Message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrEmpty(_parameter.SelectedFilePath))
            {
                if (e.FolderState != null)
                {
                    base.SetFile(e.FileInfoList, _parameter.SelectedFilePath, e.FolderState.SortTypeID, e.FolderState.IsAscending);
                }
                else
                {
                    base.SetFile(e.FileInfoList, _parameter.SelectedFilePath, SortTypeID.FilePath, true);
                }
            }
            else
            {
                if (e.FolderState != null)
                {
                    base.SetFile(e.FileInfoList, e.FolderState.SelectedFilePath, e.FolderState.SortTypeID, e.FolderState.IsAscending);
                }
                else
                {
                    base.SetFile(e.FileInfoList, string.Empty, SortTypeID.FilePath, true);
                }
            }

            SingleValueEntity<string> param = new SingleValueEntity<string>();
            param.Value = e.FolderPath;
            addFolderHistoryProcess.Execute(this, param);
        }

        private void getNextFolderProcess_Callback(object sender, SingleValueEntity<string> e)
        {
            FolderFileListContentsParameter param = new FolderFileListContentsParameter(e.Value);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        #endregion
    }
}
