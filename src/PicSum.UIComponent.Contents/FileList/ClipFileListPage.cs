using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
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
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class ClipFileListPage
        : AbstractFileListPage
    {
        private bool disposed = false;
        private readonly ClipFileListPageParameter parameter = null;

        public ClipFileListPage(ClipFileListPageParameter param)
            : base(param)
        {
            this.parameter = param;

            this.Title = "Clip";
            this.Icon = ResourceFiles.ClipIcon.Value;
            this.IsMoveControlVisible = false;
            this.IsRemoveFromListMenuItemVisible = true;
            this.IsClipMenuItem = false;

            base.sortFileRgistrationDateToolStripButton.Enabled = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            Instance<JobCaller>.Value.FilesGetByClipJob.Value
                .Reset()
                .Callback(_ =>
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.SearchJob_Callback(_);
                })
                .BeginCancel()
                .StartJob(this);

            base.OnLoad(e);
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

        }

        protected override void OnRemoveFile(IList<string> filePathList)
        {
            Instance<JobCaller>.Value.StartClipFilesDeleteJob(
                this, new ListParameter<string>(filePathList));

            this.RemoveFile(filePathList);
        }

        protected override Action<ISender> GetImageFilesGetAction(ImageViewerPageParameter paramter)
        {
            return FileListUtil.ImageFilesGetActionForClip(paramter);
        }

        protected override void OnMoveNextButtonClick(EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnMovePreviewButtonClick(EventArgs e)
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
