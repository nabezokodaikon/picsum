using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
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
    /// <summary>
    /// 評価値ファイルリストコンテンツ
    /// </summary>

    public sealed partial class RatingFileListPage
        : AbstractFileListPage
    {
        private bool _disposed = false;
        private readonly RatingFileListPageParameter _parameter = null;

        public RatingFileListPage(RatingFileListPageParameter param)
            : base(param)
        {
            this._parameter = param;

            this.Title = "Star";
            this.Icon = ResourceFiles.ActiveRatingIcon.Value;
            this.IsMoveControlVisible = false;
            this.fileContextMenu.VisibleRemoveFromListMenuItem = true;
            base.toolBar.AddDateSortButtonEnabled = true;

            this.Loaded += this.RatingFileListPage_Loaded;
            this.DrawTabPage += this.RatingFileListPage_DrawTabPage;
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

        private void RatingFileListPage_Loaded(object sender, EventArgs e)
        {
            var param = new FilesGetByRatingParameter()
            {
                RatingValue = this._parameter.RatingValue,
                IsGetThumbnail = true,
            };

            Instance<JobCaller>.Value.EnqueueFilesGetByRatingJob(this, param, _ =>
            {
                if (this._disposed)
                {
                    return;
                }

                this.SearchJob_Callback(_);
            });
        }

        private void RatingFileListPage_DrawTabPage(object sender, DrawTabEventArgs e)
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
            var param = new FileRatingUpdateParameter
            {
                FilePathList = filePathList,
                RatingValue = 0
            };
            Instance<JobCaller>.Value.EnqueueFileRatingUpdateJob(this, param);

            this.RemoveFile(filePathList);
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewPageParameter parameter)
        {
            return FileListUtil.ImageFilesGetActionForRating(parameter);
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
                base.SetFiles(
                    [.. e],
                    this._parameter.SelectedFilePath,
                    this._parameter.ScrollInfo,
                    FileSortMode.AddDate,
                    false);
            }
            else
            {
                base.SetFiles(
                    [.. e],
                    this._parameter.SelectedFilePath,
                    this._parameter.ScrollInfo,
                    this._parameter.SortInfo.ActiveSortMode,
                    this._parameter.SortInfo.IsAscending(this._parameter.SortInfo.ActiveSortMode));
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
