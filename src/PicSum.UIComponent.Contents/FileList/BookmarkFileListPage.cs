using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows")]
    internal sealed partial class BookmarkFileListPage
        : AbstractFileListPage
    {
        private static Action<ISender> ImageFilesGetAction(ImageViewerPageParameter param)
        {
            return sender =>
            {
                var dir = FileUtil.GetParentDirectoryPath(param.SelectedFilePath);

                CommonJobs.Instance.FilesGetByDirectoryJob.Initialize()
                    .Callback(e =>
                    {
                        if (!FileUtil.IsImageFile(param.SelectedFilePath))
                        {
                            throw new SWFException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                        }

                        var title = FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(param.SelectedFilePath));

                        var imageFiles = e.FileInfoList
                            .Where(fileInfo => fileInfo.IsImageFile)
                            .OrderBy(fileInfo => fileInfo.FilePath, NaturalStringComparer.Windows)
                            .Select(fileInfo => fileInfo.FilePath)
                            .ToArray();

                        var eventArgs = new GetImageFilesEventArgs(
                            imageFiles, param.SelectedFilePath, title, FileIconCash.SmallDirectoryIcon);
                        param.OnGetImageFiles(eventArgs);
                    })
                    .BeginCancel()
                    .StartJob(sender, new ValueParameter<string>(dir));
            };
        }

        private bool disposed = false;
        private readonly BookmarkFileListPageParameter parameter = null;

        public BookmarkFileListPage(BookmarkFileListPageParameter parameter)
            : base(parameter)
        {
            this.parameter = parameter;

            this.Title = "Bookmark";
            this.Icon = ResourceFiles.BookmarkIcon.Value;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            CommonJobs.Instance.BookmarksGetJob.Initialize()
                .Callback(_ =>
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.SearchJob_Callback(_);
                })
                .BeginCancel()
                .StartJob(this);

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

        protected override void OnRemoveFile(IList<string> filePathList)
        {
            var parameter = new ListParameter<string>();
            parameter.AddRange(filePathList);
            CommonJobs.Instance.StartBookmarkDeleteJob(this, parameter);

            base.RemoveFile(filePathList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewerPageParameter param)
        {
            return ImageFilesGetAction(param);
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
            base.FileContextMenu_Opening(sender, e);
            this.IsBookmarkMenuItem = false;
        }

        private void SearchJob_Callback(ListResult<FileShallowInfoEntity> result)
        {
            if (this.parameter.SortInfo == null)
            {
                base.SetFiles(result, this.parameter.SelectedFilePath, SortTypeID.RegistrationDate, false);
            }
            else
            {
                base.SetFiles(
                    result,
                    this.parameter.SelectedFilePath,
                    this.parameter.SortInfo.ActiveSortType,
                    this.parameter.SortInfo.IsAscending(this.parameter.SortInfo.ActiveSortType));
            }
        }
    }
}
