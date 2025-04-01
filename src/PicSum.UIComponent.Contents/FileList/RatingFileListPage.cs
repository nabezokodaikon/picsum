using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
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
    /// <summary>
    /// 評価値ファイルリストコンテンツ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class RatingFileListPage
        : AbstractFileListPage
    {
        private bool disposed = false;
        private readonly RatingFileListPageParameter parameter = null;

        public RatingFileListPage(RatingFileListPageParameter param)
            : base(param)
        {
            this.parameter = param;

            this.Title = "Star";
            this.Icon = ResourceFiles.ActiveRatingIcon.Value;
            this.IsMoveControlVisible = false;
            this.fileContextMenu.VisibleRemoveFromListMenuItem = true;
            base.toolBar.RegistrationSortButtonEnabled = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            var param = new FilesGetByRatingParameter()
            {
                RatingValue = this.parameter.RatingValue,
                IsGetThumbnail = true,
            };

            Instance<JobCaller>.Value.FilesGetByRatingJob.Value
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
                this.parameter.SelectedFilePath = base.SelectedFilePath;
                this.parameter.SortInfo = base.SortInfo;
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
            // 処理無し。
        }

        protected override void OnRemoveFile(string[] filePathList)
        {
            var param = new FileRatingUpdateParameter
            {
                FilePathList = filePathList,
                RatingValue = 0
            };
            Instance<JobCaller>.Value.StartFileRatingUpdateJob(this, param);

            this.RemoveFile(filePathList);
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewerPageParameter param)
        {
            return FileListUtil.ImageFilesGetActionForRating(param);
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
                base.SetFiles([.. e], this.parameter.SelectedFilePath, SortTypeID.RegistrationDate, false);
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
