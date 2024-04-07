using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using PicSum.Task.Paramters;
using PicSum.Task.Tasks;
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
        : AbstractFileListContents
    {
        #region インスタンス変数

        private FavoriteDirectoryListContentsParameter parameter = null;
        private TwoWayTask<GetFavoriteDirectoryTask, GetFavoriteDirectoryParameter, ListResult<FileShallowInfoEntity>> searchTask = null;
        private OneWayTask<DeleteDirectoryViewCounterTask, ListParameter<string>> deleteTask = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayTask<GetFavoriteDirectoryTask, GetFavoriteDirectoryParameter, ListResult<FileShallowInfoEntity>> SearchTask
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

        private OneWayTask<DeleteDirectoryViewCounterTask, ListParameter<string>> DeleteTask
        {
            get
            {
                if (this.deleteTask == null)
                {
                    this.deleteTask = new();
                    this.searchTask
                        .StartThread();
                }

                return this.deleteTask;
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var param = new GetFavoriteDirectoryParameter();
            param.IsOnlyDirectory = true;
            param.Count = FileListContentsConfig.FavoriteDirectoryCount;
            this.SearchTask.StartTask(param);
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
            var param = new ListParameter<string>(directoryList);
            this.DeleteTask.StartTask(param);
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

        private void SearchTask_Callback(ListResult<FileShallowInfoEntity> e)
        {
            base.SetFile(e, this.parameter.SelectedFilePath);
        }

        #endregion
    }
}
