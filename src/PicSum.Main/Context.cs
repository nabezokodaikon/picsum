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
        private readonly BrowserManager browserManager = new();

        public bool IsHandleCreated { get; private set; } = true;
        public bool IsDisposed { get; private set; }

        public Context()
        {
            this._resourceManager = new();

            this.browserManager.BrowserNothing += new(this.BrowserManager_BrowserNothing);

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

                    var form = this.browserManager.GetActiveBrowser();
                    var directoryPath = FileUtil.GetParentDirectoryPath(_.Value);

                    var sortInfo = new SortInfo();
                    sortInfo.SetSortType(SortTypeID.FilePath, true);

                    var parameter = new ImageViewerPageParameter(
                        DirectoryFileListPageParameter.PAGE_SOURCES,
                        directoryPath,
                        BrowserMainPanel.GetImageFilesAction(new ImageFileGetByDirectoryParameter(_.Value)),
                        _.Value,
                        sortInfo,
                        FileUtil.GetFileName(directoryPath),
                        Instance<IFileIconCacher>.Value.SmallDirectoryIcon,
                        true);

                    form.AddImageViewerPageTab(parameter);
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

            var form = this.browserManager.GetActiveBrowser();
            form.Show();
        }

        private async void BrowserManager_BrowserNothing(object sender, EventArgs e)
        {
            this.IsDisposed = true;
            await this._resourceManager.DisposeAsync();
            this.ExitThread();
        }
    }
}
