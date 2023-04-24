using System;
using System.Windows.Forms;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Contents.ContentsParameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;

namespace PicSum.UIComponent.Contents.FileListContents
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
        private TwoWayProcess<GetFileByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> _searchFileProcess = null;
        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameterEntity> _deleteFileTagProcess = null;

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFileByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> searchFileProcess
        {
            get
            {
                if (_searchFileProcess == null)
                {
                    _searchFileProcess = TaskManager.CreateTwoWayProcess<GetFileByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>(ProcessContainer);
                    searchFileProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(searchFileProcess_Callback);
                }

                return _searchFileProcess;
            }
        }

        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameterEntity> deleteFileTagProcess
        {
            get
            {
                if (_deleteFileTagProcess == null)
                {
                    _deleteFileTagProcess = TaskManager.CreateOneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameterEntity>(ProcessContainer);
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
            //base.OnBackgroundMouseClick(e);
        }

        protected override void OnRemoveFile(System.Collections.Generic.IList<string> filePathList)
        {
            UpdateFileTagParameterEntity param = new UpdateFileTagParameterEntity();
            param.FilePathList = filePathList;
            param.Tag = _parameter.Tag;
            deleteFileTagProcess.Execute(this, param);

            RemoveFile(filePathList);
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
            base.SetFile(e, _parameter.SelectedFilePath, SortTypeID.RgistrationDate, false);
        }

        #endregion
    }
}
