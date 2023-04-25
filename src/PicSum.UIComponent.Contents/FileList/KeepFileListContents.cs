using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Result;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// キープファイルリストコンテンツ
    /// </summary>
    internal class KeepFileListContents : FileListContentsBase
    {
        #region インスタンス変数

        private KeepFileListContentsParameter _parameter = null;
        private TwoWayProcess<GetKeepAsyncFacade, ListEntity<FileShallowInfoEntity>> _searchFileProcess = null;
        private OneWayProcess<DeleteKeepAsyncFacade, ListEntity<string>> _removeKeepProcess = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetKeepAsyncFacade, ListEntity<FileShallowInfoEntity>> searchFileProcess
        {
            get
            {
                if (_searchFileProcess == null)
                {
                    _searchFileProcess = TaskManager.CreateTwoWayProcess<GetKeepAsyncFacade, ListEntity<FileShallowInfoEntity>>(ProcessContainer);
                    searchFileProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(searchFileProcess_Callback);
                }

                return _searchFileProcess;
            }
        }

        private OneWayProcess<DeleteKeepAsyncFacade, ListEntity<string>> removeKeepProcess
        {
            get
            {
                if (_removeKeepProcess == null)
                {
                    _removeKeepProcess = TaskManager.CreateOneWayProcess<DeleteKeepAsyncFacade, ListEntity<string>>(ProcessContainer);
                }

                return _removeKeepProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public KeepFileListContents(KeepFileListContentsParameter param)
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
            searchFileProcess.Execute(this);
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
            ListEntity<string> param = new ListEntity<string>(filePathList);
            removeKeepProcess.Execute(this, param);

            RemoveFile(filePathList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action GetImageFilesAction(ImageViewerContentsParameter paramter)
        {
            return () =>
            {
                var proces = TaskManager.CreateTwoWayProcess<GetKeepAsyncFacade, ListEntity<FileShallowInfoEntity>>(this.ProcessContainer);
                proces.Callback += ((sender, e) =>
                {
                    var imageFiles = e
                        .Where(fileInfo => fileInfo.IsImageFile)
                        .Select(fileInfo => fileInfo.FilePath)
                        .ToArray();

                    var ex = FileUtil.GetExtension(this.SelectedFilePath);
                    var selectedFilePath = ImageUtil.ImageFileExtensionList.Contains(ex) ?
                        this.SelectedFilePath : string.Empty;

                    var eventArgs = new GetImageFilesEventArgs(imageFiles, selectedFilePath);
                    paramter.OnGetImageFiles(eventArgs);
                });

                proces.Execute(this);
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
            this.Title = "Keep";
            this.Icon = Resources.ClipIcon;
            this.IsAddKeepMenuItemVisible = false;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        #endregion

        #region プロセスイベント

        private void searchFileProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            base.SetFiles(e, _parameter.SelectedFilePath, PicSum.Core.Base.Conf.SortTypeID.RgistrationDate, false);

            if (string.IsNullOrEmpty(_parameter.SelectedFilePath))
            {
                base.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
            }
        }

        #endregion
    }
}
