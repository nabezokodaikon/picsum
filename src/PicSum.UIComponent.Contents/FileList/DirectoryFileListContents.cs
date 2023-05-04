using PicSum.Core.Base.Conf;
using PicSum.Core.Base.Exception;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Common;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// フォルダファイルリストコンテンツ
    /// </summary>
    internal sealed class DirectoryFileListContents
        : FileListContentsBase
    {
        #region インスタンス変数

        private readonly DirectoryFileListContentsParameter parameter = null;
        private TwoWayProcess<GetFilesByDirectoryAsyncFacade, SingleValueEntity<string>, GetDirectoryResult> searchDirectoryProcess = null;
        private OneWayProcess<UpdateDirectoryStateAsynceFacade, DirectoryStateEntity> updateDirectoryStateProcess = null;
        private OneWayProcess<AddDirectoryViewHistoryAsyncFacade, SingleValueEntity<string>> addDirectoryHistoryProcess = null;
        private TwoWayProcess<GetNextDirectoryAsyncFacade, GetNextContentsParameter<string>, SingleValueEntity<string>> getNextDirectoryProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFilesByDirectoryAsyncFacade, SingleValueEntity<string>, GetDirectoryResult> SearchDirectoryProcess
        {
            get
            {
                if (this.searchDirectoryProcess == null)
                {
                    this.searchDirectoryProcess = TaskManager.CreateTwoWayProcess<GetFilesByDirectoryAsyncFacade, SingleValueEntity<string>, GetDirectoryResult>(this.ProcessContainer);
                    this.searchDirectoryProcess.Callback += new AsyncTaskCallbackEventHandler<GetDirectoryResult>(this.SearchDirectoryProcess_Callback);
                }

                return this.searchDirectoryProcess;
            }
        }

        private OneWayProcess<UpdateDirectoryStateAsynceFacade, DirectoryStateEntity> UpdateDirectoryStateProcess
        {
            get
            {
                if (this.updateDirectoryStateProcess == null)
                {
                    this.updateDirectoryStateProcess = TaskManager.CreateOneWayProcess<UpdateDirectoryStateAsynceFacade, DirectoryStateEntity>(this.ProcessContainer);
                }

                return this.updateDirectoryStateProcess;
            }
        }

        private OneWayProcess<AddDirectoryViewHistoryAsyncFacade, SingleValueEntity<string>> AddDirectoryHistoryProcess
        {
            get
            {
                if (this.addDirectoryHistoryProcess == null)
                {
                    this.addDirectoryHistoryProcess = TaskManager.CreateOneWayProcess<AddDirectoryViewHistoryAsyncFacade, SingleValueEntity<string>>(this.ProcessContainer);
                }

                return this.addDirectoryHistoryProcess;
            }
        }

        private TwoWayProcess<GetNextDirectoryAsyncFacade, GetNextContentsParameter<string>, SingleValueEntity<string>> GetNextDirectoryProcess
        {
            get
            {
                if (this.getNextDirectoryProcess == null)
                {
                    this.getNextDirectoryProcess = TaskManager.CreateTwoWayProcess<GetNextDirectoryAsyncFacade, GetNextContentsParameter<string>, SingleValueEntity<string>>(this.ProcessContainer);
                    this.getNextDirectoryProcess.Callback += new AsyncTaskCallbackEventHandler<SingleValueEntity<string>>(this.GetNextDirectoryProcess_Callback);
                }

                return this.getNextDirectoryProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public DirectoryFileListContents(DirectoryFileListContentsParameter param)
            : base(param)
        {
            this.parameter = param;
            this.InitializeComponent();
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var param = new SingleValueEntity<string>();
            param.Value = this.parameter.DirectoryPath;
            this.SearchDirectoryProcess.Execute(this, param);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SaveCurrentDirectoryState();
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
            if (!FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                base.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.parameter.DirectoryPath));
            }
        }

        protected override void OnRemoveFile(System.Collections.Generic.IList<string> filePathList)
        {
            // 処理無し。
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            if (FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                return;
            }

            var param = new GetNextContentsParameter<string>();
            param.CurrentParameter = new SingleValueEntity<string>();
            param.CurrentParameter.Value = this.parameter.DirectoryPath;
            param.IsNext = false;
            this.GetNextDirectoryProcess.Cancel();
            this.GetNextDirectoryProcess.Execute(this, param);
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            if (FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                return;
            }

            var param = new GetNextContentsParameter<string>();
            param.CurrentParameter = new SingleValueEntity<string>();
            param.CurrentParameter.Value = this.parameter.DirectoryPath;
            param.IsNext = true;
            this.GetNextDirectoryProcess.Cancel();
            this.GetNextDirectoryProcess.Execute(this, param);
        }

        protected override Action GetImageFilesAction(ImageViewerContentsParameter paramter)
        {
            return () =>
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
                        .Where(fileInfo => fileInfo.IsImageFile);
                    var sortImageFiles = base.GetSortFiles(imageFiles)
                        .Select(fileInfo => fileInfo.FilePath)
                        .ToArray();

                    if (!FileUtil.IsImageFile(this.SelectedFilePath))
                    {
                        throw new PicSumException(string.Format("画像ファイルが選択されていません。'{0}'", this.SelectedFilePath));
                    }

                    var eventArgs = new GetImageFilesEventArgs(
                        sortImageFiles, this.SelectedFilePath, this.Title, this.Icon);
                    paramter.OnGetImageFiles(eventArgs);
                });

                proces.Execute(this, new SingleValueEntity<string>() { Value = this.parameter.DirectoryPath });
            };
        }

        protected override void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var filePathList = this.GetSelectedFiles();
            if (filePathList.Count > 0)
            {
                this.IsDirectoryActiveTabOpenMenuItemVisible = true;
                this.SetContextMenuFiles(filePathList);
            }
            else if (!FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                this.IsDirectoryActiveTabOpenMenuItemVisible = false;
                this.SetContextMenuFiles(this.parameter.DirectoryPath);
            }
            else
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.Title = FileUtil.GetFileName(this.parameter.DirectoryPath);

            if (string.IsNullOrEmpty(this.parameter.DirectoryPath))
            {
                this.Icon = FileIconCash.SmallMyComputerIcon;
            }
            else if (FileUtil.IsDrive(this.parameter.DirectoryPath))
            {
                this.Icon = FileIconCash.GetSmallDriveIcon(this.parameter.DirectoryPath);
            }
            else
            {
                this.Icon = FileIconCash.SmallDirectoryIcon;
            }

            this.IsRemoveFromListMenuItemVisible = false;
            this.IsMoveControlVisible = !string.IsNullOrEmpty(this.parameter.DirectoryPath);
            base.sortFileRgistrationDateToolStripButton.Enabled = false;
        }

        private void SaveCurrentDirectoryState()
        {
            var param = new DirectoryStateEntity();

            param.DirectoryPath = this.parameter.DirectoryPath;

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

            this.UpdateDirectoryStateProcess.Execute(this, param);
        }

        #endregion

        #region プロセスイベント

        private void SearchDirectoryProcess_Callback(object sender, GetDirectoryResult e)
        {
            if (e.DirectoryNotFoundException != null)
            {
                ExceptionUtil.ShowErrorDialog(e.DirectoryNotFoundException);
                return;
            }

            if (!string.IsNullOrEmpty(this.parameter.SelectedFilePath))
            {
                if (e.DirectoryState != null)
                {
                    base.SetFiles(e.FileInfoList, this.parameter.SelectedFilePath, e.DirectoryState.SortTypeID, e.DirectoryState.IsAscending);
                }
                else
                {
                    base.SetFiles(e.FileInfoList, this.parameter.SelectedFilePath, SortTypeID.FilePath, true);
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

            var param = new SingleValueEntity<string>();
            param.Value = e.DirectoryPath;
            this.AddDirectoryHistoryProcess.Execute(this, param);
        }

        private void GetNextDirectoryProcess_Callback(object sender, SingleValueEntity<string> e)
        {
            var param = new DirectoryFileListContentsParameter(e.Value);
            this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        #endregion
    }
}
