using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.Resource;
using SWF.UIComponent.TabOperation;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class FavoriteDirectoryListPage
        : AbstractFileListPage
    {

        private bool disposed = false;
        private readonly FavoriteDirectoryListPageParameter parameter = null;

        public FavoriteDirectoryListPage(FavoriteDirectoryListPageParameter param)
            : base(param)
        {
            this.parameter = param;

            this.Title = "Home";
            this.Icon = ResourceFiles.HomeIcon.Value;
            this.IsMoveControlVisible = false;
            this.fileContextMenu.VisibleRemoveFromListMenuItem = true;
            base.toolBar.RegistrationSortButtonEnabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.parameter.SelectedFilePath = base.SelectedFilePath;
                this.parameter.SortInfo = base.SortInfo;
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            var param = new FavoriteDirectoriesGetParameter
            {
                IsOnlyDirectory = true,
                Count = FileListPageConfig.Instance.FavoriteDirectoryCount
            };

            Instance<JobCaller>.Value.FavoriteDirectoriesGetJob.Value
                .StartJob(this, param, _ =>
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.SearchJob_Callback(_);
                });

            ConsoleUtil.Write(true, $"FavoriteDirectoryListPage.OnLoad End");

            base.OnLoad(e);
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
            // 処理無し。
        }

        protected override void OnRemoveFile(string[] directoryList)
        {
            var param = new ListParameter<string>(directoryList);
            Instance<JobCaller>.Value.StartDirectoryViewCounterDeleteJob(this, param);
            this.RemoveFile(directoryList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewerPageParameter paramter)
        {
            throw new NotImplementedException();
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SearchJob_Callback(ListResult<FileShallowInfoEntity> e)
        {
            if (this.parameter.SortInfo == null)
            {
                base.SetFile([.. e], this.parameter.SelectedFilePath);
            }
            else
            {
                base.SetFiles(
                    [.. e],
                    this.parameter.SelectedFilePath,
                    this.parameter.SortInfo.ActiveSortType,
                    this.parameter.SortInfo.IsAscending(this.parameter.SortInfo.ActiveSortType));
            }
        }

        protected override void FileContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var filePathList = this.GetSelectedFiles();
            if (filePathList.Length > 0)
            {
                this.fileContextMenu.SetFile(filePathList);
                this.fileContextMenu.VisibleBookmarkMenuItem = false;
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
