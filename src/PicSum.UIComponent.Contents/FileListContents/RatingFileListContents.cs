using System;
using System.Drawing;
using System.Windows.Forms;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Contents.ContentsParameter;
using PicSum.UIComponent.Contents.Properties;
using PicSum.Core.Base.Conf;

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
            RectangleF rect = new RectangleF(e.IconRectangle.X,
                                             e.IconRectangle.Y,
                                             e.TextRectangle.Right - e.IconRectangle.X,
                                             e.IconRectangle.Height);

            for (int i = 0; i < _parameter.RagingValue; i++)
            {
                float w = Math.Min(this.Icon.Width, e.TextRectangle.Height);
                float h = w;
                float x = rect.X + rect.Height * i + (rect.Height - w) / 2f;
                float y = rect.Y + (rect.Height - h) / 2f;

                if (x + w > rect.Right)
                {
                    break;
                }

                e.Graphics.DrawImage(this.Icon, x, y, w, h);
            }
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            // 処理無し。
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            int ratingValue = 0;
            if (_parameter.RagingValue <= ApplicationConst.MinimumRatingValue)
            {
                ratingValue = ApplicationConst.MaximumRatingValue;
            }
            else
            {
                ratingValue = _parameter.RagingValue - 1;
            }

            RatingFileListContentsParameter param = new RatingFileListContentsParameter(ratingValue);
            OnOpenContents(new BrowserContentsEventArgs(PicSum.Core.Base.Conf.ContentsOpenType.OverlapTab, param));
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            int ratingValue = 0;
            if (_parameter.RagingValue >= ApplicationConst.MaximumRatingValue)
            {
                ratingValue = ApplicationConst.MinimumRatingValue;
            }
            else
            {
                ratingValue = _parameter.RagingValue + 1;
            }

            RatingFileListContentsParameter param = new RatingFileListContentsParameter(ratingValue);
            OnOpenContents(new BrowserContentsEventArgs(PicSum.Core.Base.Conf.ContentsOpenType.OverlapTab, param));
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Title = string.Empty;
            this.Icon = Resources.ActiveRatingIcon;
            this.IsAddKeepMenuItemVisible = true;
            this.IsRemoveFromListMenuItemVisible = false;
            this.IsMoveControlVisible = true;
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
