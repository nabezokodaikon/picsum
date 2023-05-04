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
using System;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// 評価値ファイルリストコンテンツ
    /// </summary>
    internal sealed class RatingFileListContents
        : FileListContentsBase
    {
        #region インスタンス変数

        private RatingFileListContentsParameter parameter = null;
        private TwoWayProcess<GetFilesByRatingAsyncFacade, SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>> searchFileProcess = null;
        private OneWayProcess<UpdateFileRatingAsyncFacade, UpdateFileRatingParameter> updateFileRatingProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFilesByRatingAsyncFacade, SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>> SearchFileProcess
        {
            get
            {
                if (this.searchFileProcess == null)
                {
                    this.searchFileProcess = TaskManager.CreateTwoWayProcess<GetFilesByRatingAsyncFacade, SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>>(this.ProcessContainer);
                    this.searchFileProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(this.SearchFileProcess_Callback);
                }

                return this.searchFileProcess;
            }
        }

        private OneWayProcess<UpdateFileRatingAsyncFacade, UpdateFileRatingParameter> UpdateFileRatingProcess
        {
            get
            {
                if (this.updateFileRatingProcess == null)
                {
                    this.updateFileRatingProcess = TaskManager.CreateOneWayProcess<UpdateFileRatingAsyncFacade, UpdateFileRatingParameter>(this.ProcessContainer);
                }

                return this.updateFileRatingProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public RatingFileListContents(RatingFileListContentsParameter param)
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
            var param = new SingleValueEntity<int>();
            param.Value = this.parameter.RagingValue;
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

        protected override void OnDrawTabContents(SWF.UIComponent.TabOperation.DrawTabEventArgs e)
        {
            e.Graphics.DrawImage(this.Icon, e.IconRectangle);
            DrawTextUtil.DrawText(e.Graphics, this.Title, e.Font, e.TextRectangle, e.TitleColor, e.TitleFormatFlags, e.TextStyle);
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            // 処理無し。
        }

        protected override void OnRemoveFile(System.Collections.Generic.IList<string> filePathList)
        {
            var param = new UpdateFileRatingParameter();
            param.FilePathList = filePathList;
            param.RatingValue = 0;
            this.UpdateFileRatingProcess.Execute(this, param);

            this.RemoveFile(filePathList);
        }

        protected override Action GetImageFilesAction(ImageViewerContentsParameter paramter)
        {
            return () =>
            {
                var proces = TaskManager.CreateTwoWayProcess<GetFilesByRatingAsyncFacade, SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>>(this.ProcessContainer);
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

                proces.Execute(this, new SingleValueEntity<int>() { Value = this.parameter.RagingValue });
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
            this.Title = "Star";
            this.Icon = Resources.ActiveRatingIcon;
            this.IsMoveControlVisible = false;
            this.IsRemoveFromListMenuItemVisible = true;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        #endregion

        #region プロセスイベント

        private void SearchFileProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFiles(e, this.parameter.SelectedFilePath, SortTypeID.RgistrationDate, false);

            if (string.IsNullOrEmpty(this.parameter.SelectedFilePath))
            {
                base.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
            }
        }

        #endregion
    }
}
