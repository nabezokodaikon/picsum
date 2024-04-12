using PicSum.Core.Base.Conf;
using PicSum.Core.Base.Exception;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using PicSum.Task.Paramters;
using PicSum.Task.Tasks;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// タグファイルリストコンテンツ
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class TagFileListPage
        : AbstractFileListPage
    {
        private static Action GetImageFilesAction(ImageViewerPageParameter param)
        {
            return () =>
            {
                var task = new TwoWayTask<GetFilesByTagTask, ValueParameter<string>, ListResult<FileShallowInfoEntity>>();

                task
                .Callback(e =>
                {
                    var imageFiles = e
                        .Where(fileInfo => fileInfo.IsImageFile);
                    var sortImageFiles = GetSortFiles(imageFiles, param.SortInfo)
                        .Select(fileInfo => fileInfo.FilePath)
                        .ToArray();

                    if (!FileUtil.IsImageFile(param.SelectedFilePath))
                    {
                        throw new PicSumException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                    }

                    var eventArgs = new GetImageFilesEventArgs(
                        sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                    param.OnGetImageFiles(eventArgs);
                })
                .StartThread();

                task.StartTask(new ValueParameter<string>() { Value = param.SourcesKey });
                task.Wait();
                task.Dispose();
            };
        }

        #region インスタンス変数

        private readonly TagFileListPageParameter parameter = null;
        private TwoWayTask<GetFilesByTagTask, ValueParameter<string>, ListResult<FileShallowInfoEntity>> searchTask = null;
        private OneWayTask<DeleteFileTagTask, UpdateFileTagParameter> deleteTask = null;

        #endregion

        #region プライベートプロパティ

        private TwoWayTask<GetFilesByTagTask, ValueParameter<string>, ListResult<FileShallowInfoEntity>> SearchTask
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

        private OneWayTask<DeleteFileTagTask, UpdateFileTagParameter> DeleteTask
        {
            get
            {
                if (this.deleteTask == null)
                {
                    this.deleteTask = new();
                    this.deleteTask
                        .StartThread();
                }

                return this.deleteTask;
            }
        }

        #endregion

        #region コンストラクタ

        public TagFileListPage(TagFileListPageParameter param)
            : base(param)
        {
            this.parameter = param;
            this.InitializeComponent();
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var param = new ValueParameter<string>
            {
                Value = this.parameter.Tag
            };
            this.SearchTask.StartTask(param);
        }

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

                GC.Collect();
            }

            base.Dispose(disposing);
        }

        protected override void OnDrawTabPage(DrawTabEventArgs e)
        {
            e.Graphics.DrawImage(this.Icon, e.IconRectangle);
            DrawTextUtil.DrawText(e.Graphics, this.Title, e.Font, e.TextRectangle, e.TitleColor, e.TitleFormatFlags, e.TextStyle);
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            // 処理無し。
        }

        protected override void OnRemoveFile(IList<string> filePathList)
        {
            var param = new UpdateFileTagParameter
            {
                FilePathList = filePathList,
                Tag = this.parameter.Tag
            };
            this.DeleteTask.StartTask(param);

            this.RemoveFile(filePathList);
        }

        protected override Action GetGetImageFilesAction(ImageViewerPageParameter param)
        {
            return GetImageFilesAction(param);
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
            this.Title = this.parameter.Tag;
            this.Icon = Resources.TagIcon;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        #endregion

        #region プロセスイベント

        private void SearchTask_Callback(ListResult<FileShallowInfoEntity> e)
        {
            base.SetFiles(e, this.parameter.SelectedFilePath, SortTypeID.RgistrationDate, false);
        }

        #endregion
    }
}
