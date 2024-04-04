using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncTask;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows")]
    internal sealed class FavoriteDirectoryListContents
        : FileListContentsBase
    {
        #region インスタンス変数

        private FavoriteDirectoryListContentsParameter parameter = null;
        private TwoWayProcess<GetFavoriteDirectoryAsyncTask, GetFavoriteFolderParameter, ListEntity<FileShallowInfoEntity>> searchFavoriteDirectoryProcess = null;
        private OneWayProcess<DeleteDirectoryViewCounterAsyncTask, ListEntity<string>> deleteDirectoryViewCounterProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFavoriteDirectoryAsyncTask, GetFavoriteFolderParameter, ListEntity<FileShallowInfoEntity>> SearchFavoriteDirectoryProcess
        {
            get
            {
                if (this.searchFavoriteDirectoryProcess == null)
                {
                    this.searchFavoriteDirectoryProcess = TaskManager.CreateTwoWayProcess<GetFavoriteDirectoryAsyncTask, GetFavoriteFolderParameter, ListEntity<FileShallowInfoEntity>>(this.ProcessContainer);
                    this.searchFavoriteDirectoryProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(this.SearchFavoriteDirectoryProcess_Callback);
                }

                return this.searchFavoriteDirectoryProcess;
            }
        }

        private OneWayProcess<DeleteDirectoryViewCounterAsyncTask, ListEntity<string>> DeleteDirectoryViewCounterProcess
        {
            get
            {
                if (this.deleteDirectoryViewCounterProcess == null)
                {
                    this.deleteDirectoryViewCounterProcess
                        = TaskManager.CreateOneWayProcess<DeleteDirectoryViewCounterAsyncTask, ListEntity<string>>(this.ProcessContainer);

                }

                return this.deleteDirectoryViewCounterProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public FavoriteDirectoryListContents(FavoriteDirectoryListContentsParameter param)
            : base(param)
        {
            this.parameter = param;
            this.InitializeComponent();
        }

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.parameter.SelectedFilePath = base.SelectedFilePath;
            }

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var param = new GetFavoriteFolderParameter();
            param.IsOnlyDirectory = true;
            param.Count = FileListContentsConfig.FavoriteDirectoryCount;
            this.SearchFavoriteDirectoryProcess.Execute(this, param);
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

        private void InitializeComponent()
        {
            this.Title = "Home";
            this.Icon = Resources.HomeIcon;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = false;
        }

        #endregion

        #region プロセスイベント

        private void SearchFavoriteDirectoryProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFile(e, this.parameter.SelectedFilePath);
        }

        #endregion
    }
}
