using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Main.Mng;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using SWF.UIComponent.Core;
using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ダミーフォーム
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed partial class InitialForm : HideForm
    {
        private bool disposed = false;
        private readonly BrowserManager browserManager = new();
        private OneWayJob<GCCollectRunJob> gcCollectRunJob = null;
        private TwoWayJob<PipeServerJob, ValueResult<string>> pipeServerJob = null;

        private OneWayJob<GCCollectRunJob> GCCollectRunJob
        {
            get
            {
                this.gcCollectRunJob ??= new();
                return this.gcCollectRunJob;
            }
        }

        private TwoWayJob<PipeServerJob, ValueResult<string>> PipeServerJob
        {
            get
            {
                if (this.pipeServerJob == null)
                {
                    this.pipeServerJob = new();
                    this.pipeServerJob.Callback(_ =>
                    {
                        if (this.disposed)
                        {
                            throw new ObjectDisposedException(this.GetType().FullName);
                        }

                        if (!FileUtil.CanAccess(_.Value) || !FileUtil.IsImageFile(_.Value))
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
                            FileIconCash.SmallDirectoryIcon);

                        form.AddImageViewerPageTab(parameter);
                        if (form.WindowState == FormWindowState.Minimized)
                        {
                            form.RestoreWindowState();
                        }
                        form.Activate();
                    });
                }

                return this.pipeServerJob;
            }
        }

        public InitialForm()
        {
            this.browserManager.BrowserNothing += new(this.BrowserManager_BrowserNothing);
        }

        private void BrowserManager_BrowserNothing(object sender, EventArgs e)
        {
            this.gcCollectRunJob?.Dispose();
            this.pipeServerJob?.Dispose();
            this.disposed = true;
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            SynchronizationContextWrapper.SetSynchronizationContext();

            this.GCCollectRunJob.StartJob();
            this.PipeServerJob.StartJob();

            var form = this.browserManager.GetActiveBrowser();
            form.Show();

            base.OnLoad(e);
        }
    }
}
