using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    internal sealed class BookmarkFileListContents
        : FileListContentsBase
    {
        private BookmarkFileListContentsParameter paramter = null;
        private TwoWayProcess<GetBookmarkAsyncFacade, ListEntity<FileShallowInfoEntity>> searchProcess = null;
        private OneWayProcess<DeleteBookmarkAsyncFacade, ListEntity<string>> deleteProcess = null;

        private TwoWayProcess<GetBookmarkAsyncFacade, ListEntity<FileShallowInfoEntity>> SearchProcess
        {
            get
            {
                if (this.searchProcess == null)
                {
                    this.searchProcess = TaskManager.CreateTwoWayProcess<GetBookmarkAsyncFacade, ListEntity<FileShallowInfoEntity>>(this.ProcessContainer);
                    this.searchProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(this.SearchProcess_Callback);
                }

                return this.searchProcess;
            }
        }

        private OneWayProcess<DeleteBookmarkAsyncFacade, ListEntity<string>> DeleteProcess
        {
            get
            {
                if (this.deleteProcess == null)
                {
                    this.deleteProcess = TaskManager.CreateOneWayProcess<DeleteBookmarkAsyncFacade, ListEntity<string>>(this.ProcessContainer);
                }

                return this.deleteProcess;
            }
        }

        public BookmarkFileListContents(BookmarkFileListContentsParameter parameter)
            : base(parameter)
        {
            this.paramter = parameter;
            this.InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.SearchProcess.Execute(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.paramter.SelectedFilePath = base.SelectedFilePath;
            }

            base.Dispose(disposing);
        }

        protected override void OnDrawTabContents(DrawTabEventArgs e)
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
            var parameter = new ListEntity<string>();
            parameter.AddRange(filePathList);
            this.DeleteProcess.Execute(this, parameter);

            base.RemoveFile(filePathList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action GetImageFilesAction(ImageViewerContentsParameter paramter)
        {
            return () =>
            {
                var proces = TaskManager.CreateTwoWayProcess<GetFilesByDirectoryAsyncFacade, SingleValueEntity<string>, GetDirectoryResult>(this.ProcessContainer);
                proces.Callback += ((sender, e) =>
                {
                    if (e.DirectoryNotFoundException != null)
                    {
                        ExceptionUtil.ShowErrorDialog(e.DirectoryNotFoundException);
                        return;
                    }

                    var imageFiles = e.FileInfoList
                        .Where(fileInfo => fileInfo.IsImageFile)                        
                        .OrderBy(fileInfo => fileInfo.FilePath)
                        .Select(fileInfo => fileInfo.FilePath)
                        .ToArray();

                    var selectedFilePath = FileUtil.IsImageFile(this.SelectedFilePath) ?
                        this.SelectedFilePath : string.Empty;

                    var title = FileUtil.GetFileName(FileUtil.GetParentDirectoryPath(selectedFilePath));

                    var eventArgs = new GetImageFilesEventArgs(
                        imageFiles, selectedFilePath, title, FileIconCash.SmallDirectoryIcon);
                    paramter.OnGetImageFiles(eventArgs);
                });

                var dir = FileUtil.GetParentDirectoryPath(paramter.SelectedFilePath);
                proces.Execute(this, new SingleValueEntity<string>() { Value = dir });
            };
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InitializeComponent()
        {
            this.Title = "Bookmark";
            this.Icon = Resources.BookmarkIcon;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            this.IsBookmarkMenuItem = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        private void SearchProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFiles(e, this.paramter.SelectedFilePath, PicSum.Core.Base.Conf.SortTypeID.RgistrationDate, false);

            if (string.IsNullOrEmpty(this.paramter.SelectedFilePath))
            {
                base.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
            }
        }
    }
}
