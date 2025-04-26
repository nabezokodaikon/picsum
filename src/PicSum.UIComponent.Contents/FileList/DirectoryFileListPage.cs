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
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// フォルダファイルリストコンテンツ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class DirectoryFileListPage
        : AbstractFileListPage
    {
        private bool disposed = false;
        private readonly DirectoryFileListPageParameter parameter = null;

        public DirectoryFileListPage(DirectoryFileListPageParameter param)
            : base(param)
        {
            this.parameter = param;

            this.Title = FileUtil.GetFileName(this.parameter.DirectoryPath);

            if (FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                this.Icon = Instance<IFileIconCacher>.Value.LargePCIcon;
            }
            else if (FileUtil.IsExistsDrive(this.parameter.DirectoryPath))
            {
                this.Icon = Instance<IFileIconCacher>.Value.GetJumboDriveIcon(this.parameter.DirectoryPath);
            }
            else
            {
                this.Icon = Instance<IFileIconCacher>.Value.JumboDirectoryIcon;
            }

            this.IsMoveControlVisible = !string.IsNullOrEmpty(this.parameter.DirectoryPath);
            this.fileContextMenu.VisibleRemoveFromListMenuItem = false;
            base.toolBar.RegistrationSortButtonEnabled = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            var param = new FilesGetByDirectoryParameter()
            {
                DirectoryPath = this.parameter.DirectoryPath,
                IsGetThumbnail = true,
            };

            Instance<JobCaller>.Value.FilesGetByDirectoryJob.Value
                .StartJob(this, param, _ =>
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.SearchJob_Callback(_);
                });

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.SaveCurrentDirectoryState();
            }

            this.disposed = true;

            base.Dispose(disposing);
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
                e.TitleColor, e.TitleFormatFlags);
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            if (!FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                base.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.parameter.DirectoryPath));
            }
        }

        protected override void OnRemoveFile(string[] filePathList)
        {
            // 処理無し。
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            if (FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                return;
            }

            var param = new NextDirectoryGetParameter
            {
                CurrentParameter = this.parameter.DirectoryPath,
                IsNext = false,
            };

            Instance<JobCaller>.Value.NextDirectoryGetJob.Value
                .StartJob(this, param, _ =>
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.GetNextDirectoryProcess_Callback(_);
                });
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            if (FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                return;
            }

            var param = new NextDirectoryGetParameter
            {
                IsNext = true,
                CurrentParameter = this.parameter.DirectoryPath,
            };

            Instance<JobCaller>.Value.NextDirectoryGetJob.Value
                .StartJob(this, param, _ =>
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.GetNextDirectoryProcess_Callback(_);
                });
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewerPageParameter param)
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
            else if (!FileUtil.IsSystemRoot(this.parameter.DirectoryPath))
            {
                this.SetContextMenuFiles(this.parameter.DirectoryPath);
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
                    this.parameter.DirectoryPath,
                    SortTypeID.FileName,
                    true,
                    base.SelectedFilePath);
                Instance<JobCaller>.Value.StartDirectoryStateUpdateJob(this, param);
            }
            else
            {
                var param = new DirectoryStateParameter(
                    this.parameter.DirectoryPath,
                    base.SortTypeID,
                    base.IsAscending,
                    base.SelectedFilePath);
                Instance<JobCaller>.Value.StartDirectoryStateUpdateJob(this, param);
            }
        }

        private void SearchJob_Callback(DirectoryGetResult e)
        {
            if (!string.IsNullOrEmpty(this.parameter.SelectedFilePath))
            {
                if (e.DirectoryState == DirectoryStateParameter.EMPTY)
                {
                    base.SetFiles(e.FileInfoList, this.parameter.SelectedFilePath, SortTypeID.FilePath, true);
                }
                else
                {
                    base.SetFiles(e.FileInfoList, this.parameter.SelectedFilePath, e.DirectoryState.SortTypeID, e.DirectoryState.IsAscending);
                }
            }
            else
            {
                if (e.DirectoryState == DirectoryStateParameter.EMPTY)
                {
                    base.SetFiles(e.FileInfoList, this.parameter.DirectoryPath, SortTypeID.FilePath, true);
                }
                else
                {
                    base.SetFiles(e.FileInfoList, e.DirectoryState.SelectedFilePath, e.DirectoryState.SortTypeID, e.DirectoryState.IsAscending);
                }

                if (e.FileInfoList.Length < 1)
                {
                    base.OnSelectedFileChanged(new SelectedFileChangeEventArgs(this.parameter.DirectoryPath));
                }
            }

            var param = new ValueParameter<string>(e.DirectoryPath);
            Instance<JobCaller>.Value.StartDirectoryViewHistoryAddJob(this, param);
        }

        private void GetNextDirectoryProcess_Callback(ValueResult<string> e)
        {
            var param = new DirectoryFileListPageParameter(e.Value);
            this.OnOpenPage(new BrowserPageEventArgs(PageOpenType.OverlapTab, param));
        }

    }
}
