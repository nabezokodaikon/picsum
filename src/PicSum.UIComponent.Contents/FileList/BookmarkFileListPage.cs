using PicSum.Core.Base.Exception;
using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Results;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
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
    internal sealed class BookmarkFileListPage
        : AbstractFileListPage
    {
        private static Action ImageFilesGetAction(ImageViewerPageParameter param)
        {
            return () =>
            {
                var job = new TwoWayJob<FilesGetByDirectoryJob, ValueParameter<string>, DirectoryGetResult>();

                job
                .Callback(e =>
                {
                    if (!FileUtil.IsImageFile(param.SelectedFilePath))
                    {
                        throw new PicSumException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                    }

                    var title = FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(param.SelectedFilePath));

                    var imageFiles = e.FileInfoList
                        .Where(fileInfo => fileInfo.IsImageFile)
                        .OrderBy(fileInfo => fileInfo.FilePath)
                        .Select(fileInfo => fileInfo.FilePath)
                        .ToArray();

                    var eventArgs = new GetImageFilesEventArgs(
                        imageFiles, param.SelectedFilePath, title, FileIconCash.SmallDirectoryIcon);
                    param.OnGetImageFiles(eventArgs);
                })
                .StartThread();

                var dir = FileUtil.GetParentDirectoryPath(param.SelectedFilePath);
                job.StartJob(new ValueParameter<string>(dir));
                job.Wait();
                job.Dispose();
            };
        }

        private readonly BookmarkFileListPageParameter parameter = null;
        private TwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>> searchJob = null;
        private OneWayJob<BookmarkDeleteJob, ListParameter<string>> deleteJob = null;

        private TwoWayJob<BookmarksGetJob, ListResult<FileShallowInfoEntity>> SearchJob
        {
            get
            {
                if (this.searchJob == null)
                {
                    this.searchJob = new();
                    this.searchJob
                        .Callback(this.SearchJob_Callback)
                        .StartThread();
                }

                return this.searchJob;
            }
        }

        private OneWayJob<BookmarkDeleteJob, ListParameter<string>> DeleteJob
        {
            get
            {
                if (this.deleteJob == null)
                {
                    this.deleteJob = new();
                    this.deleteJob
                        .StartThread();
                }

                return this.deleteJob;
            }
        }

        public BookmarkFileListPage(BookmarkFileListPageParameter parameter)
            : base(parameter)
        {
            this.parameter = parameter;
            this.InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.SearchJob.StartJob();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Disposed = true;

                this.parameter.SelectedFilePath = base.SelectedFilePath;
                this.parameter.SortInfo = base.SortInfo;

                if (this.searchJob != null)
                {
                    this.searchJob.Dispose();
                    this.searchJob = null;
                }

                if (this.deleteJob != null)
                {
                    this.deleteJob.Dispose();
                    this.deleteJob = null;
                }
            }

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
                e.TitleColor, e.TitleFormatFlags, e.TextStyle);
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            // 処理無し。
        }

        protected override void OnRemoveFile(IList<string> filePathList)
        {
            var parameter = new ListParameter<string>();
            parameter.AddRange(filePathList);
            this.DeleteJob.StartJob(parameter);

            base.RemoveFile(filePathList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action GetImageFilesGetAction(ImageViewerPageParameter param)
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

        private void InitializeComponent()
        {
            this.Title = "Bookmark";
            this.Icon = Resources.BookmarkIcon;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        private void SearchJob_Callback(ListResult<FileShallowInfoEntity> result)
        {
            if (this.parameter.SortInfo == null)
            {
                base.SetFiles(result, this.parameter.SelectedFilePath, SortTypeID.RgistrationDate, false);
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
