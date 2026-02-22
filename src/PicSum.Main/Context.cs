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
        private readonly WindowManager _windowManager;

        public bool IsHandleCreated { get; private set; } = true;
        public bool IsLoaded { get; private set; } = true;
        public bool IsDisposed { get; private set; } = false;

        public Context()
        {
            AppConstants.ThrowIfNotUIThread();

            this._resourceManager = new();
            this._windowManager = new();

            this._windowManager.WindowNothing += this.WindowManager_BrowseNothing;

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

                    var window = this._windowManager.GetActiveWindow();
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

                    window.AddImageViewPageTab(parameter);
                    if (window.WindowState == FormWindowState.Minimized)
                    {
                        window.RestoreWindowState();
                    }

                    window.Show();
                    window.TopMost = true;
                    window.TopMost = false;
                    window.BringToFront();
                    window.Activate();
                    window.Focus();
                });

            var window = this._windowManager.GetActiveWindow();
            window.Show();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this._resourceManager.Dispose();
            }

            this.IsDisposed = true;

            base.Dispose(disposing);
        }

        private void WindowManager_BrowseNothing(object sender, EventArgs e)
        {
            this.ExitThread();
        }
    }
}
