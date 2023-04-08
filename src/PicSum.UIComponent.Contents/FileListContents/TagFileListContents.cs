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
        private TwoWayProcess<SearchFileByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> _searchFileProcess = null;
        private TwoWayProcess<GetNextTagAsyncFacade, GetNextContentsParameterEntity<string>, SingleValueEntity<string>> _getNextTagProcess = null;

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<SearchFileByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> searchFileProcess
        {
            get
            {
                if (_searchFileProcess == null)
                {
                    _searchFileProcess = TaskManager.CreateTwoWayProcess<SearchFileByTagAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>(ProcessContainer);
                    searchFileProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(searchFileProcess_Callback);
                }

                return _searchFileProcess;
            }
        }

        private TwoWayProcess<GetNextTagAsyncFacade, GetNextContentsParameterEntity<string>, SingleValueEntity<string>> getNextTagProcess
        {
            get
            {
                if (_getNextTagProcess == null)
                {
                    _getNextTagProcess = TaskManager.CreateTwoWayProcess<GetNextTagAsyncFacade, GetNextContentsParameterEntity<string>, SingleValueEntity<string>>(ProcessContainer);
                    _getNextTagProcess.Callback += new AsyncTaskCallbackEventHandler<SingleValueEntity<string>>(getNextTagProcess_Callback);
                }

                return _getNextTagProcess;
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

        }


        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            GetNextContentsParameterEntity<string> param = new GetNextContentsParameterEntity<string>();
            param.CurrentParameter = new SingleValueEntity<string>();
            param.CurrentParameter.Value = _parameter.Tag;
            param.IsNext = false;
            getNextTagProcess.Cancel();
            getNextTagProcess.Execute(this, param);
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            GetNextContentsParameterEntity<string> param = new GetNextContentsParameterEntity<string>();
            param.CurrentParameter = new SingleValueEntity<string>();
            param.CurrentParameter.Value = _parameter.Tag;
            param.IsNext = true;
            getNextTagProcess.Cancel();
            getNextTagProcess.Execute(this, param);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Title = _parameter.Tag;
            this.Icon = Resources.TagIcon;
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

        private void getNextTagProcess_Callback(object sender, SingleValueEntity<string> e)
        {
            TagFileListContentsParameter param = new TagFileListContentsParameter(e.Value);
            OnOpenContents(new BrowserContentsEventArgs(ContentsOpenType.OverlapTab, param));
        }

        #endregion
    }
}
