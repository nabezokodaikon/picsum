using System;
using System.Windows.Forms;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Contents.ContentsParameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using PicSum.UIComponent.Contents.Conf;
using System.Collections.Generic;
using PicSum.Task.Paramter;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.Contents.FileListContents
{
    internal class FavoriteDirectoryListContents 
        : FileListContentsBase
    {
        #region インスタンス変数

        private FavoriteDirectoryListContentsParameter _parameter = null;
        private TwoWayProcess<GetFavoriteDirectoryAsyncFacade, GetFavoriteFolderParameter, ListEntity<FileShallowInfoEntity>> _searchFavoriteDirectoryProcess = null;
        private OneWayProcess<DeleteDirectoryViewCounterAsyncFacade, ListEntity<string>> deleteDirectoryViewCounterProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFavoriteDirectoryAsyncFacade, GetFavoriteFolderParameter, ListEntity<FileShallowInfoEntity>> searchFavoriteDirectoryProcess
        {
            get
            {
                if (_searchFavoriteDirectoryProcess == null)
                {
                    _searchFavoriteDirectoryProcess = TaskManager.CreateTwoWayProcess<GetFavoriteDirectoryAsyncFacade, GetFavoriteFolderParameter, ListEntity<FileShallowInfoEntity>>(ProcessContainer);
                    _searchFavoriteDirectoryProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(searchFavoriteDirectoryProcess_Callback);
                }

                return _searchFavoriteDirectoryProcess;
            }
        }

        private OneWayProcess<DeleteDirectoryViewCounterAsyncFacade, ListEntity<string>> DeleteDirectoryViewCounterProcess
        {
            get
            {
                if (this.deleteDirectoryViewCounterProcess == null)
                {
                    this.deleteDirectoryViewCounterProcess
                        = TaskManager.CreateOneWayProcess<DeleteDirectoryViewCounterAsyncFacade, ListEntity<string>>(this.ProcessContainer);

                }

                return this.deleteDirectoryViewCounterProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public FavoriteDirectoryListContents(FavoriteDirectoryListContentsParameter param)
            : base(param)
        {
            _parameter = param;
            initializeComponent();
        }

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._parameter.SelectedFilePath = base.SelectedFilePath;
            }

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GetFavoriteFolderParameter param = new GetFavoriteFolderParameter();
            param.IsOnlyDirectory = true;
            param.Count = FileListContentsConfig.FavoriteDirectoryCount;
            searchFavoriteDirectoryProcess.Execute(this, param);
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

        protected override void OnRemoveFile(IList<string> directoryList)
        {
            var param = new ListEntity<string>(directoryList);
            this.DeleteDirectoryViewCounterProcess.Execute(this, param);
            this.RemoveFile(directoryList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action GetImageFilesAction(ImageViewerContentsParameter paramter)
        {
            throw new NotImplementedException();
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
            this.Title = "Home";
            this.Icon = Resources.HomeIcon;
            this.IsAddKeepMenuItemVisible = true;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = false;
        }

        #endregion

        #region プロセスイベント

        private void searchFavoriteDirectoryProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFile(e, _parameter.SelectedFilePath);
        }

        #endregion
    }
}
