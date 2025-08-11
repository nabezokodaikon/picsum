using PicSum.Job.Common;
using PicSum.Job.Parameters;
using PicSum.Main.Mng;
using PicSum.Main.UIComponent;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using System;
using System.Windows.Forms;

namespace PicSum.Main
{
    internal sealed class Context
        : ApplicationContext, ISender
    {
        private readonly ResourceManager _resourceManager;
        private readonly BrowseManager _browseManager = new();

        public bool IsLoaded { get; private set; } = true;
        public bool IsDisposed { get; private set; } = false;

        public Context()
        {
            this._resourceManager = new();

            this._browseManager.BrowseNothing += this.BrowseManager_BrowseNothing;

            Instance<JobCaller>.Value.GCCollectRunJob.Value
                .StartJob(this);

            Instance<JobCaller>.Value.PipeServerJob.Value
                .StartJob(this, _ =>
                {
                    if (this.IsDisposed)
                    {
                        return;
                    }

                    if (!FileUtil.CanAccess(_.Value) || !ImageUtil.IsImageFile(_.Value))
                    {
                        return;
                    }

                    var form = this._browseManager.GetActiveBrowse();
                    var directoryPath = FileUtil.GetParentDirectoryPath(_.Value);

                    var sortInfo = new SortParameter();
                    sortInfo.SetSortMode(FileSortMode.FilePath, true);

                    var parameter = new ImageViewPageParameter(
                        DirectoryFileListPageParameter.PAGE_SOURCES,
                        directoryPath,
                        BrowsePanel.GetImageFilesAction(new ImageFileGetByDirectoryParameter(_.Value)),
                        _.Value,
                        sortInfo,
                        FileUtil.GetFileName(directoryPath),
                        Instance<IFileIconCacher>.Value.SmallDirectoryIcon,
                        true);

                    form.AddImageViewPageTab(parameter);
                    if (form.WindowState == FormWindowState.Minimized)
                    {
                        form.RestoreWindowState();
                    }

                    form.Show();
                    form.TopMost = true;
                    form.TopMost = false;
                    form.BringToFront();
                    form.Activate();
                    form.Focus();
                });

            var form = this._browseManager.GetActiveBrowse();
            form.Show();
        }

        private void BrowseManager_BrowseNothing(object sender, EventArgs e)
        {
            this.IsDisposed = true;
            this._resourceManager.Dispose();
            this.ExitThread();
        }
    }
}
