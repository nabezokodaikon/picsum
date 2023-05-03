using PicSum.Core.Base.Conf;
using PicSum.Core.Base.Exception;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// タグファイルリストコンテンツ
    /// </summary>
    internal sealed class TagFileListContents
        : FileListContentsBase
    {
        #region インスタンス変数

        private TagFileListContentsParameter parameter = null;
        private TwoWayProcess<GetFilesByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> searchFileProcess = null;
        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameter> deleteFileTagProcess = null;

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFilesByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> SearchFileProcess
        {
            get
            {
                if (this.searchFileProcess == null)
                {
                    this.searchFileProcess = TaskManager.CreateTwoWayProcess<GetFilesByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>(this.ProcessContainer);
                    this.searchFileProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(this.SearchFileProcess_Callback);
                }

                return this.searchFileProcess;
            }
        }

        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameter> DeleteFileTagProcess
        {
            get
            {
                if (this.deleteFileTagProcess == null)
                {
                    this.deleteFileTagProcess = TaskManager.CreateOneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameter>(this.ProcessContainer);
                }

                return this.deleteFileTagProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public TagFileListContents(TagFileListContentsParameter param)
            : base(param)
        {
            this.parameter = param;
            this.InitializeComponent();
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var param = new SingleValueEntity<string>();
            param.Value = parameter.Tag;
            this.SearchFileProcess.Execute(this, param);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.parameter.SelectedFilePath = base.SelectedFilePath;
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
            var param = new UpdateFileTagParameter();
            param.FilePathList = filePathList;
            param.Tag = parameter.Tag;
            this.DeleteFileTagProcess.Execute(this, param);

            this.RemoveFile(filePathList);
        }

        protected override Action GetImageFilesAction(ImageViewerContentsParameter paramter)
        {
            return () =>
            {
                var proces = TaskManager.CreateTwoWayProcess<GetFilesByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>(this.ProcessContainer);
                proces.Callback += ((sender, e) =>
                {
                    var imageFiles = e
                        .Where(fileInfo => fileInfo.IsImageFile);
                    var sortImageFiles = base.GetSortFiles(imageFiles)
                        .Select(fileInfo => fileInfo.FilePath)
                        .ToArray();

                    if (!FileUtil.IsImageFile(this.SelectedFilePath))
                    {
                        throw new PicSumException(string.Format("画像ファイルが選択されていません。'{0}'", this.SelectedFilePath));
                    }

                    var eventArgs = new GetImageFilesEventArgs(
                        sortImageFiles, this.SelectedFilePath, this.Title, this.Icon);
                    paramter.OnGetImageFiles(eventArgs);
                });

                proces.Execute(this, new SingleValueEntity<string>() { Value = this.parameter.Tag });
            };
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.Title = this.parameter.Tag;
            this.Icon = Resources.TagIcon;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        #endregion

        #region プロセスイベント

        private void SearchFileProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFiles(e, this.parameter.SelectedFilePath, SortTypeID.RgistrationDate, false);
        }

        #endregion
    }
}
