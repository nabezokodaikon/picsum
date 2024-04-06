using PicSum.Core.Base.Conf;
using PicSum.Core.Base.Exception;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using PicSum.Task.Parameters;
using PicSum.Task.Paramters;
using PicSum.Task.Results;
using PicSum.Task.Tasks;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Common;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// フォルダファイルリストコンテンツ
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryFileListContents
        : AbstractFileListContents
    {
        #region インスタンス変数

        private readonly DirectoryFileListContentsParameter parameter = null;
        private TaskWrapper<GetFilesByDirectoryTask, ValueParameter<string>, GetDirectoryResult> searchTask = null;
        private TaskWrapper<UpdateDirectoryStateTask, DirectoryStateParameter> updateDirectoryStateTask = null;
        private TaskWrapper<AddDirectoryViewHistoryTask, ValueParameter<string>> addDirectoryHistoryTask = null;
        private TaskWrapper<GetNextDirectoryTask, GetNextContentsParameter<string>, ValueResult<string>> getNextDirectoryTask = null;
        private TaskWrapper<GetFilesByDirectoryTask, ValueParameter<string>, GetDirectoryResult> getFilesTask = null;

        #endregion

        #region プライベートプロパティ

        private TaskWrapper<GetFilesByDirectoryTask, ValueParameter<string>, GetDirectoryResult> SearchTask
        {
            get
            {
                if (this.searchTask == null)
                {
                    this.searchTask = new();
                    this.searchTask
                        .Callback(this.SearchTask_Callback)
                        .Catch(e =>
                        {
                            ExceptionUtil.ShowErrorDialog(e.InnerException);
                        })
                        .StartThread();
                }

                return this.searchTask;
            }
        }

        private TaskWrapper<UpdateDirectoryStateTask, DirectoryStateParameter> UpdateDirectoryStateTask
        {
            get
            {
                if (this.updateDirectoryStateTask == null)
                {
                    this.updateDirectoryStateTask = new();
                    this.updateDirectoryStateTask.StartThread();
                }

                return this.updateDirectoryStateTask;
            }
        }

        private TaskWrapper<AddDirectoryViewHistoryTask, ValueParameter<string>> AddDirectoryHistoryTask
        {
            get
            {
                if (this.addDirectoryHistoryTask == null)
                {
                    this.addDirectoryHistoryTask = new();
                    this.addDirectoryHistoryTask.StartThread();
                }

                return this.addDirectoryHistoryTask;
            }
        }

        private TaskWrapper<GetNextDirectoryTask, GetNextContentsParameter<string>, ValueResult<string>> GetNextDirectoryTask
        {
            get
            {
                if (this.getNextDirectoryTask == null)
                {
                    this.getNextDirectoryTask = new();
                    this.getNextDirectoryTask
                        .Callback(this.GetNextDirectoryProcess_Callback)
                        .StartThread();
                }

                return this.getNextDirectoryTask;
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
            var param = new ValueParameter<string>();
            param.Value = this.parameter.DirectoryPath;
            this.SearchTask.StartTask(param);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SaveCurrentDirectoryState();

                if (this.searchTask != null)
                {
                    this.searchTask.Dispose();
                    this.searchTask = null;
                }

                if (this.updateDirectoryStateTask != null)
                {
                    this.updateDirectoryStateTask.Dispose();
                    this.updateDirectoryStateTask = null;
                }

                if (this.addDirectoryHistoryTask != null)
                {
                    this.addDirectoryHistoryTask.Dispose();
                    this.addDirectoryHistoryTask = null;
                }

                if (this.getNextDirectoryTask != null)
                {
                    this.getNextDirectoryTask.Dispose();
                    this.getNextDirectoryTask = null;
                }

                if (this.getFilesTask != null)
                {
                    this.getFilesTask.Dispose();
                    this.getFilesTask = null;
                }
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
            this.GetNextDirectoryTask.StartTask(param);
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
            this.GetNextDirectoryTask.StartTask(param);
        }

        protected override Action GetImageFilesAction(ImageViewerContentsParameter paramter)
        {
            return () =>
            {
                var task = this.CreateNewGetFilesTask();
                task
                .Callback(e =>
                {
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
                })
                .Catch(e => ExceptionUtil.ShowErrorDialog(e.InnerException))
                .StartThread();

                task.StartTask(new ValueParameter<string>() { Value = this.parameter.DirectoryPath });
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

            if (FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
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

        private TaskWrapper<GetFilesByDirectoryTask, ValueParameter<string>, GetDirectoryResult> CreateNewGetFilesTask()
        {
            if (this.getFilesTask != null)
            {
                this.getFilesTask.Dispose();
                this.getFilesTask = null;
            }

            this.getFilesTask = new();
            return this.getFilesTask;
        }

        private void SaveCurrentDirectoryState()
        {
            var param = new DirectoryStateParameter();

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

            this.UpdateDirectoryStateTask.StartTask(param);
        }

        #endregion

        #region プロセスイベント

        private void SearchTask_Callback(GetDirectoryResult e)
        {
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

            var param = new ValueParameter<string>();
            param.Value = e.DirectoryPath;
            this.AddDirectoryHistoryTask.StartTask(param);
        }

        private void GetNextDirectoryProcess_Callback(ValueResult<string> e)
        {
            var param = new DirectoryFileListContentsParameter(e.Value);
            this.OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        #endregion
    }
}
