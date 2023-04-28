using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// タグファイルリストコンテンツ
    /// </summary>
    internal class TagFileListContents : FileListContentsBase
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private TagFileListContentsParameter _parameter = null;
        private TwoWayProcess<GetFilesByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> _searchFileProcess = null;
        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameter> _deleteFileTagProcess = null;

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFilesByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> searchFileProcess
        {
            get
            {
                if (_searchFileProcess == null)
                {
                    _searchFileProcess = TaskManager.CreateTwoWayProcess<GetFilesByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>(ProcessContainer);
                    searchFileProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(searchFileProcess_Callback);
                }

                return _searchFileProcess;
            }
        }

        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameter> deleteFileTagProcess
        {
            get
            {
                if (_deleteFileTagProcess == null)
                {
                    _deleteFileTagProcess = TaskManager.CreateOneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameter>(ProcessContainer);
                }

                return _deleteFileTagProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public TagFileListContents(TagFileListContentsParameter param)
            : base(param)
        {
            _parameter = param;
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SingleValueEntity<string> param = new SingleValueEntity<string>();
            param.Value = _parameter.Tag;
            searchFileProcess.Execute(this, param);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _parameter.SelectedFilePath = base.SelectedFilePath;
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
            UpdateFileTagParameter param = new UpdateFileTagParameter();
            param.FilePathList = filePathList;
            param.Tag = _parameter.Tag;
            deleteFileTagProcess.Execute(this, param);

            RemoveFile(filePathList);
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

                    var selectedFilePath = FileUtil.IsImageFile(this.SelectedFilePath) ?
                        this.SelectedFilePath : string.Empty;

                    var eventArgs = new GetImageFilesEventArgs(sortImageFiles, selectedFilePath);
                    paramter.OnGetImageFiles(eventArgs);
                });

                proces.Execute(this, new SingleValueEntity<string>() { Value = this._parameter.Tag });
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

        private void initializeComponent()
        {
            this.Title = _parameter.Tag;
            this.Icon = Resources.TagIcon;
            this.IsAddKeepMenuItemVisible = true;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        #endregion

        #region プロセスイベント

        private void searchFileProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFiles(e, _parameter.SelectedFilePath, SortTypeID.RgistrationDate, false);
        }

        #endregion
    }
}
