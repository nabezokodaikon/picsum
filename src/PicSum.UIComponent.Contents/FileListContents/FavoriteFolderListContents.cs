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

namespace PicSum.UIComponent.Contents.FileListContents
{
    internal class FavoriteFolderListContents : FileListContentsBase
    {
        #region インスタンス変数

        private FavoriteFolderListContentsParameter _parameter = null;
        private TwoWayProcess<SearchFavoriteFolderAsyncFacade, SearchFavoriteFolderParameterEntity, ListEntity<FileShallowInfoEntity>> _searchFavoriteFolderProcess = null;
        private OneWayProcess<DeleteFolderViewHistoryAsyncFacade, ListEntity<string>> removeItemProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<SearchFavoriteFolderAsyncFacade, SearchFavoriteFolderParameterEntity, ListEntity<FileShallowInfoEntity>> searchFavoriteFolderProcess
        {
            get
            {
                if (_searchFavoriteFolderProcess == null)
                {
                    _searchFavoriteFolderProcess = TaskManager.CreateTwoWayProcess<SearchFavoriteFolderAsyncFacade, SearchFavoriteFolderParameterEntity, ListEntity<FileShallowInfoEntity>>(ProcessContainer);
                    _searchFavoriteFolderProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(searchFavoriteFolderProcess_Callback);
                }

                return _searchFavoriteFolderProcess;
            }
        }

        private OneWayProcess<DeleteFolderViewHistoryAsyncFacade, ListEntity<string>> DeleteFolderViewHistoryProcess
        {
            get
            {
                if (this.removeItemProcess == null)
                {
                    this.removeItemProcess
                        = TaskManager.CreateOneWayProcess<DeleteFolderViewHistoryAsyncFacade, ListEntity<string>>(this.ProcessContainer);

                }

                return this.removeItemProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public FavoriteFolderListContents(FavoriteFolderListContentsParameter param)
            : base(param)
        {
            _parameter = param;
            initializeComponent();
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SearchFavoriteFolderParameterEntity param = new SearchFavoriteFolderParameterEntity();
            param.IsOnlyFolder = true;
            param.Count = FileListContentsConfig.FavoriteFolderCount;
            searchFavoriteFolderProcess.Execute(this, param);
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

        protected override void OnRemoveFile(IList<string> directoryList)
        {
            var param = new ListEntity<string>(directoryList);
            this.DeleteFolderViewHistoryProcess.Execute(this, param);
            this.RemoveFile(directoryList);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Title = "よく開くフォルダ";
            this.Icon = Resources.ActiveRatingIcon;
            this.IsAddKeepMenuItemVisible = true;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
        }

        #endregion

        #region プロセスイベント

        private void searchFavoriteFolderProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFile(e, _parameter.SelectedFilePath);
        }

        #endregion
    }
}
