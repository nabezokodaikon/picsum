using PicSum.Core.Base.Exception;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using PicSum.Task.Results;
using PicSum.Task.Tasks;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows")]
    internal sealed class BookmarkFileListPage
        : AbstractFileListPage
    {
        private static Action GetImageFilesAction(ImageViewerPageParameter param)
        {
            return () =>
            {
                var task = new TwoWayTask<GetFilesByDirectoryTask, ValueParameter<string>, GetDirectoryResult>();

                task
                .Callback(e =>
                {
                    var imageFiles = e.FileInfoList
                        .Where(fileInfo => fileInfo.IsImageFile)
                        .OrderBy(fileInfo => fileInfo.FilePath)
                        .Select(fileInfo => fileInfo.FilePath)
                        .ToArray();

                    if (!FileUtil.IsImageFile(param.SelectedFilePath))
                    {
                        throw new PicSumException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                    }

                    var title = FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(param.SelectedFilePath));

                    var eventArgs = new GetImageFilesEventArgs(
                        imageFiles, param.SelectedFilePath, title, FileIconCash.SmallDirectoryIcon);
                    param.OnGetImageFiles(eventArgs);
                })
                .StartThread();

                var dir = FileUtil.GetParentDirectoryPath(param.SelectedFilePath);
                task.StartTask(new ValueParameter<string>() { Value = dir });
                task.Wait();
                task.Dispose();
            };
        }

        private readonly BookmarkFileListPageParameter paramter = null;
        private TwoWayTask<GetBookmarkTask, ListResult<FileShallowInfoEntity>> searchTask = null;
        private OneWayTask<DeleteBookmarkTask, ListParameter<string>> deleteTask = null;

        private TwoWayTask<GetBookmarkTask, ListResult<FileShallowInfoEntity>> SearchTask
        {
            get
            {
                if (this.searchTask == null)
                {
                    this.searchTask = new();
                    this.searchTask
                        .Callback(this.SearchTask_Callback)
                        .StartThread();
                }

                return this.searchTask;
            }
        }

        private OneWayTask<DeleteBookmarkTask, ListParameter<string>> DeleteTask
        {
            get
            {
                if (this.deleteTask == null)
                {
                    this.deleteTask = new();
                    this.deleteTask
                        .StartThread();
                }

                return this.deleteTask;
            }
        }

        public BookmarkFileListPage(BookmarkFileListPageParameter parameter)
            : base(parameter)
        {
            this.paramter = parameter;
            this.InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.SearchTask.StartTask();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.paramter.SelectedFilePath = base.SelectedFilePath;

                if (this.searchTask != null)
                {
                    this.searchTask.Dispose();
                    this.searchTask = null;
                }

                if (this.deleteTask != null)
                {
                    this.deleteTask.Dispose();
                    this.deleteTask = null;
                }                
            }

            base.Dispose(disposing);
        }

        protected override void OnDrawTabPage(DrawTabEventArgs e)
        {
            e.Graphics.DrawImage(this.Icon, e.IconRectangle);
            DrawTextUtil.DrawText(e.Graphics, this.Title, e.Font, e.TextRectangle, e.TitleColor, e.TitleFormatFlags, e.TextStyle);
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            // 処理無し。
        }

        protected override void OnRemoveFile(IList<string> filePathList)
        {
            var parameter = new ListParameter<string>();
            parameter.AddRange(filePathList);
            this.DeleteTask.StartTask(parameter);

            base.RemoveFile(filePathList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action GetGetImageFilesAction(ImageViewerPageParameter param)
        {
            return GetImageFilesAction(param);
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

        private void SearchTask_Callback(ListResult<FileShallowInfoEntity> result)
        {
            base.SetFiles(result, this.paramter.SelectedFilePath, PicSum.Core.Base.Conf.SortTypeID.RgistrationDate, false);

            if (string.IsNullOrEmpty(this.paramter.SelectedFilePath))
            {
                base.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
            }
        }
    }
}