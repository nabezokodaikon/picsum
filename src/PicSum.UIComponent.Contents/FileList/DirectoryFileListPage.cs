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
    internal sealed class DirectoryFileListPage
        : AbstractFileListPage
    {
        private static Action GetImageFilesAction(ImageViewerPageParameter param)
        {
            return () =>
            {
                var task = new TwoWayTask<FilesGetByDirectoryTask, ValueParameter<string>, DirectoryGetResult>();
                task
                .Callback(e =>
                {
                    var imageFiles = e.FileInfoList
                        .Where(fileInfo => fileInfo.IsImageFile);
                    var sortImageFiles = GetSortFiles(imageFiles, param.SortInfo)
                        .Select(fileInfo => fileInfo.FilePath)
                        .ToArray();

                    if (!FileUtil.IsImageFile(param.SelectedFilePath))
                    {
                        throw new PicSumException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                    }

                    var eventArgs = new GetImageFilesEventArgs(
                        sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                    param.OnGetImageFiles(eventArgs);
                })
                .Catch(e => ExceptionUtil.ShowErrorDialog(e.InnerException))
                .StartThread();

                task.StartTask(new ValueParameter<string>() { Value = param.SourcesKey });
                task.Wait();
                task.Dispose();
            };
        }

        #region インスタンス変数

        private readonly DirectoryFileListPageParameter parameter = null;
        private TwoWayTask<FilesGetByDirectoryTask, ValueParameter<string>, DirectoryGetResult> searchTask = null;
        private OneWayTask<DirectoryStateUpdateTask, DirectoryStateParameter> updateDirectoryStateTask = null;
        private OneWayTask<DirectoryViewHistoryAddTask, ValueParameter<string>> addDirectoryHistoryTask = null;
        private TwoWayTask<NextDirectoryGetTask, NextDirectoryGetParameter<string>, ValueResult<string>> getNextDirectoryTask = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayTask<FilesGetByDirectoryTask, ValueParameter<string>, DirectoryGetResult> SearchTask
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

        private OneWayTask<DirectoryStateUpdateTask, DirectoryStateParameter> UpdateDirectoryStateTask
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

        private OneWayTask<DirectoryViewHistoryAddTask, ValueParameter<string>> AddDirectoryHistoryTask
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

        private TwoWayTask<NextDirectoryGetTask, NextDirectoryGetParameter<string>, ValueResult<string>> GetNextDirectoryTask
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

        public DirectoryFileListPage(DirectoryFileListPageParameter param)
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
            var param = new ValueParameter<string>
            {
                Value = this.parameter.DirectoryPath
            };
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
            }

            base.Dispose(disposing);
        }

        protected override void OnDrawTabPage(SWF.UIComponent.TabOperation.DrawTabEventArgs e)
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

            var param = new NextDirectoryGetParameter<string>
            {
                CurrentParameter = new ValueEntity<string>(),
                IsNext = false,
            };
            param.CurrentParameter.Value = this.parameter.DirectoryPath;
            this.GetNextDirectoryTask.StartTask(param);
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            if (FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                return;
            }

            var param = new NextDirectoryGetParameter<string>
            {
                IsNext = true,
                CurrentParameter = new ValueEntity<string>()
            };
            param.CurrentParameter.Value = this.parameter.DirectoryPath;
            this.GetNextDirectoryTask.StartTask(param);
        }

        protected override Action GetGetImageFilesAction(ImageViewerPageParameter param)
        {
            return GetImageFilesAction(param);
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

        private void SaveCurrentDirectoryState()
        {
            var param = new DirectoryStateParameter
            {
                DirectoryPath = this.parameter.DirectoryPath
            };

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

        private void SearchTask_Callback(DirectoryGetResult e)
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

            var param = new ValueParameter<string>
            {
                Value = e.DirectoryPath
            };
            this.AddDirectoryHistoryTask.StartTask(param);
        }

        private void GetNextDirectoryProcess_Callback(ValueResult<string> e)
        {
            var param = new DirectoryFileListPageParameter(e.Value);
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.OverlapTab, param));
        }

        #endregion
    }
}
