using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.UIComponent.Contents.Common;
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

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class BookmarkFileListPage
        : AbstractFileListPage
    {
        private bool _disposed = false;
        private readonly BookmarkFileListPageParameter _parameter = null;

        public BookmarkFileListPage(BookmarkFileListPageParameter parameter)
            : base(parameter)
        {
            this._parameter = parameter;

            this.Title = "Bookmark";
            this.Icon = ResourceFiles.BookmarkIcon.Value;
            this.IsMoveControlVisible = false;
            this.fileContextMenu.VisibleRemoveFromListMenuItem = true;
            base.toolBar.AddDateSortButtonEnabled = true;

            this.Loaded += this.BookmarkFileListPage_Loaded;
            this.DrawTabPage += this.BookmarkFileListPage_DrawTabPage;
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
                this._parameter.ScrollInfo
                    = new(this.ScrollValue, this.FlowListSize, this.ItemSize);
                this._parameter.SortInfo = base.SortInfo;
            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        private void BookmarkFileListPage_DrawTabPage(object sender, DrawTabEventArgs e)
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

        protected override void OnRemoveFile(string[] filePathList)
        {
            var parameter = new ListParameter<string>();
            parameter.AddRange(filePathList);
            Instance<JobCaller>.Value.EnqueueBookmarkDeleteJob(this, parameter);

            base.RemoveFile(filePathList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewPageParameter param)
        {
            return FileListUtil.ImageFilesGetActionForBookmark(param);
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
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

        private void BookmarkFileListPage_Loaded(object sender, EventArgs e)
        {
            Instance<JobCaller>.Value.EnqueueBookmarksGetJob(this, _ =>
            {
                if (this._disposed)
                {
                    return;
                }

                this.SearchJob_Callback(_);
            });
        }

        private void SearchJob_Callback(ListResult<FileShallowInfoEntity> result)
        {
            if (this._parameter.SortInfo == null)
            {
                base.SetFiles(
                    [.. result],
                    this._parameter.SelectedFilePath,
                    this._parameter.ScrollInfo,
                    FileSortMode.AddDate,
                    false);
            }
            else
            {
                base.SetFiles(
                    [.. result],
                    this._parameter.SelectedFilePath,
                    this._parameter.ScrollInfo,
                    this._parameter.SortInfo.ActiveSortMode,
                    this._parameter.SortInfo.IsAscending(this._parameter.SortInfo.ActiveSortMode));
            }
        }
    }
}
