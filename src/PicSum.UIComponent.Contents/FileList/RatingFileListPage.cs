using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.UIComponent.TabOperation;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// 評価値ファイルリストコンテンツ
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class RatingFileListPage
        : AbstractFileListPage
    {
        private static Action ImageFilesGetAction(ImageViewerPageParameter param)
        {
            return () =>
            {
                var job = new TwoWayJob<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>>();
                job.Callback(e =>
                {
                    var imageFiles = e
                        .Where(fileInfo => fileInfo.IsImageFile);
                    var sortImageFiles = GetSortFiles(imageFiles, param.SortInfo)
                        .Select(fileInfo => fileInfo.FilePath)
                        .ToArray();

                    if (!FileUtil.IsImageFile(param.SelectedFilePath))
                    {
                        throw new SWFException($"画像ファイルが選択されていません。'{param.SelectedFilePath}'");
                    }

                    var eventArgs = new GetImageFilesEventArgs(
                        sortImageFiles, param.SelectedFilePath, param.PageTitle, param.PageIcon);
                    param.OnGetImageFiles(eventArgs);
                })
                .StartThread();

                job.StartJob(new ValueParameter<int>(int.Parse(param.SourcesKey)));
                job.Wait();
                job.Dispose();
            };
        }

        private bool disposing = false;
        private readonly RatingFileListPageParameter parameter = null;
        private TwoWayJob<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>> searchJob = null;
        private OneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter> deleteJob = null;

        private TwoWayJob<FilesGetByRatingJob, ValueParameter<int>, ListResult<FileShallowInfoEntity>> SearchJob
        {
            get
            {
                if (this.searchJob == null)
                {
                    this.searchJob = new();
                    this.searchJob
                        .Callback(_ =>
                        {
                            if (this.disposing)
                            {
                                return;
                            }

                            this.SearchJob_Callback(_);
                        })
                        .StartThread();
                }

                return this.searchJob;
            }
        }

        private OneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter> DeleteJob
        {
            get
            {
                if (this.deleteJob == null)
                {
                    this.deleteJob = new();
                    this.deleteJob.StartThread();
                }

                return this.deleteJob;
            }
        }

        public RatingFileListPage(RatingFileListPageParameter param)
            : base(param)
        {
            this.parameter = param;
            this.InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var param = new ValueParameter<int>(this.parameter.RatingValue);
            this.SearchJob.StartJob(param);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.disposing = true;

                this.parameter.SelectedFilePath = base.SelectedFilePath;
                this.parameter.SortInfo = base.SortInfo;

                if (this.searchJob != null)
                {
                    this.searchJob.Dispose();
                    this.searchJob = null;
                }

                if (this.deleteJob != null)
                {
                    this.deleteJob.Dispose();
                    this.deleteJob = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnDrawTabPage(DrawTabEventArgs e)
        {
            e.Graphics.DrawImage(this.Icon, e.IconRectangle);
            DrawTextUtil.DrawText(
                e.Graphics, this.Title, e.Font,
                new Rectangle(
                    (int)e.TextRectangle.X,
                    (int)e.TextRectangle.Y,
                    (int)e.TextRectangle.Width,
                    (int)e.TextRectangle.Height),
                e.TitleColor, e.TitleFormatFlags, e.TextStyle);
        }

        protected override void OnBackgroundMouseClick(MouseEventArgs e)
        {
            // 処理無し。
        }

        protected override void OnRemoveFile(System.Collections.Generic.IList<string> filePathList)
        {
            var param = new FileRatingUpdateParameter
            {
                FilePathList = filePathList,
                RatingValue = 0
            };
            this.DeleteJob.StartJob(param);

            this.RemoveFile(filePathList);
        }

        protected override Action GetImageFilesGetAction(ImageViewerPageParameter param)
        {
            return ImageFilesGetAction(param);
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InitializeComponent()
        {
            this.Title = "Star";
            this.Icon = Resources.ActiveRatingIcon;
            this.IsMoveControlVisible = false;
            this.IsRemoveFromListMenuItemVisible = true;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        private void SearchJob_Callback(ListResult<FileShallowInfoEntity> e)
        {
            if (this.parameter.SortInfo == null)
            {
                base.SetFiles(e, this.parameter.SelectedFilePath, SortTypeID.RgistrationDate, false);
            }
            else
            {
                base.SetFiles(
                    e,
                    this.parameter.SelectedFilePath,
                    this.parameter.SortInfo.ActiveSortType,
                    this.parameter.SortInfo.IsAscending(this.parameter.SortInfo.ActiveSortType));
            }
        }

    }
}
