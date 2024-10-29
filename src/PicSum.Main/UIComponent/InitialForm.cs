using PicSum.Job.Common;
using PicSum.Job.Parameters;
using PicSum.Main.Mng;
using PicSum.UIComponent.Contents.Parameter;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
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

        public InitialForm()
        {
            this.browserManager.BrowserNothing += new(this.BrowserManager_BrowserNothing);
        }

        private void BrowserManager_BrowserNothing(object sender, EventArgs e)
        {
            this.disposed = true;
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            CommonJobs.Instance.StartGCCollectRunJob(this);

            CommonJobs.Instance.PipeServerJob.SetCurrentSender(this)
                .Callback(_ =>
                {
                    if (this.disposed)
                    {
                        return;
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
                })
                .StartJob(this);

            var form = this.browserManager.GetActiveBrowser();
            form.Show();

            base.OnLoad(e);
        }
    }
}
