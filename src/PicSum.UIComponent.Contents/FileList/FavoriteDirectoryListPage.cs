using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.Conf;
using PicSum.UIComponent.Contents.Parameter;
using PicSum.UIComponent.Contents.Properties;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows")]
    internal sealed partial class FavoriteDirectoryListPage
        : AbstractFileListPage
    {

        private bool disposed = false;
        private readonly FavoriteDirectoryListPageParameter parameter = null;
        private TwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>> searchJob = null;
        private OneWayJob<DirectoryViewCounterDeleteJob, ListParameter<string>> deleteJob = null;

        private TwoWayJob<FavoriteDirectoriesGetJob, FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>> SearchJob
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

        private OneWayJob<DirectoryViewCounterDeleteJob, ListParameter<string>> DeleteJob
        {
            get
            {
                this.deleteJob ??= new();
                return this.deleteJob;
            }
        }

        public FavoriteDirectoryListPage(FavoriteDirectoryListPageParameter param)
            : base(param)
        {
            this.parameter = param;

            this.Title = "Home";
            this.Icon = Resources.HomeIcon;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsMoveControlVisible = false;
            base.sortFileRgistrationDateToolStripButton.Enabled = false;
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

                this.deleteJob?.Dispose();
                this.deleteJob = null;
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var param = new FavoriteDirectoriesGetParameter
            {
                IsOnlyDirectory = true,
                Count = FileListPageConfig.FavoriteDirectoryCount
            };
            this.SearchJob.StartJob(this, param);
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

        protected override void OnRemoveFile(IList<string> directoryList)
        {
            var param = new ListParameter<string>(directoryList);
            this.DeleteJob.StartJob(this, param);
            this.RemoveFile(directoryList);

            this.OnSelectedFileChanged(new SelectedFileChangeEventArgs());
        }

        protected override Action<Control> GetImageFilesGetAction(ImageViewerPageParameter paramter)
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

        private void SearchJob_Callback(ListResult<FileShallowInfoEntity> e)
        {
            if (this.parameter.SortInfo == null)
            {
                base.SetFile(e, this.parameter.SelectedFilePath);
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
