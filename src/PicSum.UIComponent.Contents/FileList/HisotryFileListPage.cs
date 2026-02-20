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

namespace PicSum.UIComponent.Contents.FileList
{

    public sealed partial class HisotryFileListPage
        : AbstractFileListPage
    {
        private bool _disposed = false;
        private readonly HistoryFileListPageParameter _parameter = null;

        public HisotryFileListPage(HistoryFileListPageParameter parameter)
            : base(parameter)
        {
            this._parameter = parameter;

            this.Title = "History";
            this.Icon = ResourceFiles.HistoryIcon.Value;
            this.IsMoveControlVisible = false;
            this.fileContextMenu.VisibleRemoveFromListMenuItem = true;
            base.toolBar.AddDateSortButtonEnabled = true;

            this.Loaded += this.HisotryFileListPage_Loaded;
            this.DrawTabPage += this.HisotryFileListPage_DrawTabPage;
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

        private void HisotryFileListPage_DrawTabPage(object sender, DrawTabEventArgs e)
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
            var param = new ListParameter<string>(filePathList);
            Instance<JobCaller>.Value.EnqueueDirectoryViewHistoryDeleteJob(this, param);

            this.RemoveFile(filePathList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewPageParameter parameter)
        {
            throw new NotImplementedException();
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

        private void HisotryFileListPage_Loaded(object sender, EventArgs e)
        {
            Instance<JobCaller>.Value.EnqueueHisotryDirectoriesGetJob(this, _ =>
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
