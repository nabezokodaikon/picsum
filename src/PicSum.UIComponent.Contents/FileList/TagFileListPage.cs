using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    /// <summary>
    /// タグファイルリストコンテンツ
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed partial class TagFileListPage
        : AbstractFileListPage
    {
        private static Action<Control> ImageFilesGetAction(ImageViewerPageParameter param)
        {
            return sender =>
            {
                using (var job = new TwoWayJob<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>>())
                {
                    job
                    .Callback(e =>
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
                    });

                    job.StartJob(sender, new ValueParameter<string>(param.SourcesKey));
                    job.WaitJobComplete();
                }
            };
        }

        private bool disposed = false;
        private readonly TagFileListPageParameter parameter = null;
        private TwoWayJob<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>> searchJob = null;

        private TwoWayJob<FilesGetByTagJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>> SearchJob
        {
            get
            {
                if (this.searchJob == null)
                {
                    this.searchJob = new();
                    this.searchJob
                        .Callback(_ =>
                        {
                            if (this.disposed)
                            {
                                return;
                            }

                            this.SearchJob_Callback(_);
                        });
                }

                return this.searchJob;
            }
        }

        public TagFileListPage(TagFileListPageParameter param)
            : base(param)
        {
            this.parameter = param;

            this.Title = this.parameter.Tag;
            this.Icon = Resources.TagIcon;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var param = new ValueParameter<string>(this.parameter.Tag);
            this.SearchJob.StartJob(this, param);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.parameter.SelectedFilePath = base.SelectedFilePath;
                this.parameter.SortInfo = base.SortInfo;

                this.searchJob?.Dispose();
                this.searchJob = null;
            }

            this.disposed = true;

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
                e.TitleColor, e.TitleFormatFlags);
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
            CommonJobs.Instance.StartFileTagDeleteJob(this, param);

            this.RemoveFile(filePathList);
        }

        protected override Action<Control> GetImageFilesGetAction(ImageViewerPageParameter param)
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

        private void SearchJob_Callback(ListResult<FileShallowInfoEntity> e)
        {
            if (this.parameter.SortInfo == null)
            {
                base.SetFiles(e, this.parameter.SelectedFilePath, SortTypeID.RegistrationDate, false);
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
