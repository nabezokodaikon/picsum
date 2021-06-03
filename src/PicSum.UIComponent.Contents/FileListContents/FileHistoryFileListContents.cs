using System;
using System.Windows.Forms;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Contents.ContentsParameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.Contents.FileListContents
{
    /// <summary>
    /// ファイル表示履歴ファイルリストコンテンツ
    /// </summary>
    internal class FileHistoryFileListContents : FileListContentsBase
    {
        #region インスタンス変数

        private FileHistoryFileListContentsParameter _parameter = null;
        private TwoWayProcess<SearchFileByViewDateAsyncFacade, SingleValueEntity<DateTime>, ListEntity<FileShallowInfoEntity>> _searchFileProcess = null;
        private TwoWayProcess<GetNextFileViewHistoryAsyncFacade, GetNextContentsParameterEntity<DateTime>, SingleValueEntity<DateTime>> _getNextHistoryProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<SearchFileByViewDateAsyncFacade, SingleValueEntity<DateTime>, ListEntity<FileShallowInfoEntity>> searchFileProcess
        {
            get
            {
                if (_searchFileProcess == null)
                {
                    _searchFileProcess = TaskManager.CreateTwoWayProcess<SearchFileByViewDateAsyncFacade, SingleValueEntity<DateTime>, ListEntity<FileShallowInfoEntity>>(ProcessContainer);
                    searchFileProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(searchFileProcess_Callback);
                }

                return _searchFileProcess;
            }
        }

        private TwoWayProcess<GetNextFileViewHistoryAsyncFacade, GetNextContentsParameterEntity<DateTime>, SingleValueEntity<DateTime>> getNextHistoryProcess
        {
            get
            {
                if (_getNextHistoryProcess == null)
                {
                    _getNextHistoryProcess = TaskManager.CreateTwoWayProcess<GetNextFileViewHistoryAsyncFacade, GetNextContentsParameterEntity<DateTime>, SingleValueEntity<DateTime>>(ProcessContainer);
                    _getNextHistoryProcess.Callback += new AsyncTaskCallbackEventHandler<SingleValueEntity<DateTime>>(getNextHistoryProcess_Callback);
                }

                return _getNextHistoryProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public FileHistoryFileListContents(FileHistoryFileListContentsParameter param)
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

            SingleValueEntity<DateTime> param = new SingleValueEntity<DateTime>();
            param.Value = _parameter.ViewDate;
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
            // 処理無し。
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            GetNextContentsParameterEntity<DateTime> param = new GetNextContentsParameterEntity<DateTime>();
            param.CurrentParameter = new SingleValueEntity<DateTime>();
            param.CurrentParameter.Value = _parameter.ViewDate;
            param.IsNext = false;
            getNextHistoryProcess.Execute(this, param);
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            GetNextContentsParameterEntity<DateTime> param = new GetNextContentsParameterEntity<DateTime>();
            param.CurrentParameter = new SingleValueEntity<DateTime>();
            param.CurrentParameter.Value = _parameter.ViewDate;
            param.IsNext = true;
            getNextHistoryProcess.Execute(this, param);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Title = _parameter.ViewDate.ToString("yyyy/MM/dd");
            this.Icon = Resources.FileHistoryIcon;
            this.IsAddKeepMenuItemVisible = true;
            this.IsRemoveFromListMenuItemVisible = false;
            this.IsMoveControlVisible = true;
        }

        #endregion

        #region プロセスイベント

        private void searchFileProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFile(e, _parameter.SelectedFilePath);
        }

        private void getNextHistoryProcess_Callback(object sender, SingleValueEntity<DateTime> e)
        {
            FileHistoryFileListContentsParameter param = new FileHistoryFileListContentsParameter(e.Value);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        #endregion
    }
}
