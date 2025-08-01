using PicSum.Job.Common;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.TabOperation;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// フォルダファイルリストコンテンツ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class DirectoryFileListPage
        : AbstractFileListPage
    {
        private bool _disposed = false;
        private readonly DirectoryFileListPageParameter _parameter = null;

        public DirectoryFileListPage(DirectoryFileListPageParameter param)
            : base(param)
        {
            this._parameter = param;

            this.Title = FileUtil.GetFileName(this._parameter.DirectoryPath);

            if (FileUtil.IsSystemRoot(this._parameter.DirectoryPath))
            {
                this.Icon = Instance<IFileIconCacher>.Value.LargePCIcon;
            }
            else if (FileUtil.IsExistsDrive(this._parameter.DirectoryPath))
            {
                this.Icon = Instance<IFileIconCacher>.Value.GetJumboDriveIcon(this._parameter.DirectoryPath);
            }
            else
            {
                this.Icon = Instance<IFileIconCacher>.Value.JumboDirectoryIcon;
            }

            this.IsMoveControlVisible = !string.IsNullOrEmpty(this._parameter.DirectoryPath);
            this.fileContextMenu.VisibleRemoveFromListMenuItem = false;
            base.toolBar.RegistrationSortButtonEnabled = false;

            this.Loaded += this.DirectoryFileListPage_Loaded;
            this.DrawTabPage += this.DirectoryFileListPage_DrawTabPage;
        }

        protected override void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this.SaveCurrentDirectoryState();
            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        private void DirectoryFileListPage_Loaded(object sender, EventArgs e)
        {
            var dirParam = new ValueParameter<string>(this._parameter.DirectoryPath);
            Instance<JobCaller>.Value.EnqueueDirectoryViewCounterIncrementJob(this, dirParam);
            Instance<JobCaller>.Value.EnqueueDirectoryViewHistoryAddJob(this, dirParam);

            var getParam = new FilesGetByDirectoryParameter()
            {
                DirectoryPath = this._parameter.DirectoryPath,
                IsGetThumbnail = true,
            };

            Instance<JobCaller>.Value.EnqueueFilesGetByDirectoryJob(this, getParam, _ =>
            {
                if (this._disposed)
                {
                    return;
                }

                this.SearchJob_Callback(_);
            });
        }

        private void DirectoryFileListPage_DrawTabPage(object sender, DrawTabEventArgs e)
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

        protected override void OnSelectedItemChange()
        {
            var filePathList = this.GetSelectedFiles();
            if (filePathList.Length < 1)
            {
                if (!FileUtil.IsSystemRoot(this._parameter.DirectoryPath))
                {
                    base.OnSelectedFileChanged(
                        new SelectedFileChangeEventArgs(this._parameter.DirectoryPath));
                    return;
                }
            }
            else
            {
                this.SelectedFilePath = filePathList.First();
            }

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs(filePathList));
        }

        protected override void OnRemoveFile(string[] filePathList)
        {
            // 処理無し。
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            if (FileUtil.IsSystemRoot(this._parameter.DirectoryPath))
            {
                return;
            }

            var param = new NextDirectoryGetParameter
            {
                CurrentParameter = this._parameter.DirectoryPath,
                IsNext = false,
            };

            Instance<JobCaller>.Value.EnqueueNextDirectoryGetJob(this, param, _ =>
                {
                    if (this._disposed)
                    {
                        return;
                    }

                    this.GetNextDirectoryProcess_Callback(_);
                });
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            if (FileUtil.IsSystemRoot(this._parameter.DirectoryPath))
            {
                return;
            }

            var param = new NextDirectoryGetParameter
            {
                IsNext = true,
                CurrentParameter = this._parameter.DirectoryPath,
            };

            Instance<JobCaller>.Value.EnqueueNextDirectoryGetJob(this, param, _ =>
                {
                    if (this._disposed)
                    {
                        return;
                    }

                    this.GetNextDirectoryProcess_Callback(_);
                });
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewPageParameter param)
        {
            return FileListUtil.ImageFilesGetActionForDirectory(param);
        }

        protected override void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var filePathList = this.GetSelectedFiles();
            if (filePathList.Length > 0)
            {
                this.SetContextMenuFiles(filePathList);
            }
            else if (!FileUtil.IsSystemRoot(this._parameter.DirectoryPath))
            {
                this.SetContextMenuFiles(this._parameter.DirectoryPath);
                this.fileContextMenu.VisibleDirectoryActiveTabOpenMenuItem = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void SaveCurrentDirectoryState()
        {
            if (base.SortTypeID == SortTypeID.Default)
            {
                var param = new DirectoryStateParameter(
                    this._parameter.DirectoryPath,
                    SortTypeID.FileName,
                    true,
                    base.SelectedFilePath);
                Instance<JobCaller>.Value.EnqueueDirectoryStateUpdateJob(this, param);
            }
            else
            {
                var param = new DirectoryStateParameter(
                    this._parameter.DirectoryPath,
                    base.SortTypeID,
                    base.IsAscending,
                    base.SelectedFilePath);
                Instance<JobCaller>.Value.EnqueueDirectoryStateUpdateJob(this, param);
            }
        }

        private void SearchJob_Callback(DirectoryGetResult e)
        {
            if (!string.IsNullOrEmpty(this._parameter.SelectedFilePath))
            {
                if (e.DirectoryState == DirectoryStateParameter.EMPTY)
                {
                    base.SetFiles(e.FileInfoList, this._parameter.SelectedFilePath, SortTypeID.FilePath, true);
                }
                else
                {
                    base.SetFiles(e.FileInfoList, this._parameter.SelectedFilePath, e.DirectoryState.SortTypeID, e.DirectoryState.IsAscending);
                }
            }
            else
            {
                if (e.DirectoryState == DirectoryStateParameter.EMPTY)
                {
                    base.SetFiles(e.FileInfoList, this._parameter.DirectoryPath, SortTypeID.FilePath, true);
                }
                else
                {
                    base.SetFiles(e.FileInfoList, e.DirectoryState.SelectedFilePath, e.DirectoryState.SortTypeID, e.DirectoryState.IsAscending);
                }

                if (e.FileInfoList.Length < 1)
                {
                    base.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this._parameter.DirectoryPath));
                }
            }
        }

        private void GetNextDirectoryProcess_Callback(ValueResult<string> e)
        {
            var param = new DirectoryFileListPageParameter(e.Value);
            this.OnOpenPage(new BrowsePageEventArgs(PageOpenType.OverlapTab, param));
        }

    }
}
