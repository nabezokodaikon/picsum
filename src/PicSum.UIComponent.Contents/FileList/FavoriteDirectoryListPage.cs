using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
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
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class FavoriteDirectoryListPage
        : AbstractFileListPage
    {

        private bool _disposed = false;
        private readonly FavoriteDirectoryListPageParameter _parameter = null;

        public FavoriteDirectoryListPage(FavoriteDirectoryListPageParameter param)
            : base(param)
        {
            this._parameter = param;

            this.Title = "Home";
            this.Icon = ResourceFiles.HomeIcon.Value;
            this.IsMoveControlVisible = false;
            this.fileContextMenu.VisibleRemoveFromListMenuItem = true;
            base.toolBar.RegistrationSortButtonEnabled = false;

            this.Load += this.FavoriteDirectoryListPage_Load;
            this.DrawTabPage += this.FavoriteDirectoryListPage_DrawTabPage;
        }

        protected override void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._parameter.SelectedFilePath = base.SelectedFilePath;
                this._parameter.SortInfo = base.SortInfo;
            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        private void FavoriteDirectoryListPage_Load(object sender, EventArgs e)
        {
            var param = new FavoriteDirectoriesGetParameter
            {
                IsOnlyDirectory = true,
                Count = FileListPageConfig.INSTANCE.FavoriteDirectoryCount
            };

            Instance<JobCaller>.Value.EnqueueFavoriteDirectoriesGetJob(this, param, _ =>
                {
                    if (this._disposed)
                    {
                        return;
                    }

                    this.SearchJob_Callback(_);
                });
        }

        private void FavoriteDirectoryListPage_DrawTabPage(object sender, DrawTabEventArgs e)
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
            Instance<JobCaller>.Value.EnqueueDirectoryViewCounterDeleteJob(this, param);
            this.RemoveFile(directoryList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewPageParameter paramter)
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
            if (this._parameter.SortInfo == null)
            {
                base.SetFile([.. e], this._parameter.SelectedFilePath);
            }
            else
            {
                base.SetFiles(
                    [.. e],
                    this._parameter.SelectedFilePath,
                    this._parameter.SortInfo.ActiveSortType,
                    this._parameter.SortInfo.IsAscending(this._parameter.SortInfo.ActiveSortType));
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
