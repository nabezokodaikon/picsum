using System;
using System.Drawing;
using System.Windows.Forms;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Contents.ContentsParameter;
using PicSum.UIComponent.Contents.Properties;
using PicSum.Core.Base.Conf;
using SWF.Common;

namespace PicSum.UIComponent.Contents.FileListContents
{
    /// <summary>
    /// 評価値ファイルリストコンテンツ
    /// </summary>
    internal class RatingFileListContents : FileListContentsBase
    {
        #region インスタンス変数

        private RatingFileListContentsParameter _parameter = null;
        private TwoWayProcess<SearchFileByRatingAsyncFacade, SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>> _searchFileProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<SearchFileByRatingAsyncFacade, SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>> searchFileProcess
        {
            get
            {
                if (_searchFileProcess == null)
                {
                    _searchFileProcess = TaskManager.CreateTwoWayProcess<SearchFileByRatingAsyncFacade, SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>>(ProcessContainer);
                    searchFileProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(searchFileProcess_Callback);
                }

                return _searchFileProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public RatingFileListContents(RatingFileListContentsParameter param)
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
            SingleValueEntity<int> param = new SingleValueEntity<int>();
            param.Value = _parameter.RagingValue;
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

        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Title = "スター";
            this.Icon = Resources.ActiveRatingIcon;
            this.IsAddKeepMenuItemVisible = true;
            this.IsRemoveFromListMenuItemVisible = false;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        #endregion

        #region プロセスイベント

        private void searchFileProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFile(e, _parameter.SelectedFilePath);

            if (string.IsNullOrEmpty(_parameter.SelectedFilePath))
            {
                base.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
            }
        }

        #endregion
    }
}
