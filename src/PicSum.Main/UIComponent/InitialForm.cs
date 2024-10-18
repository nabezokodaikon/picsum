using PicSum.Job.Jobs;
using PicSum.Main.Mng;
using SWF.Core.Job;
using SWF.UIComponent.Core;
using System;
using System.Runtime.Versioning;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ダミーフォーム
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed partial class InitialForm : HideForm
    {
        private readonly BrowserManager browserManager = new();
        private OneWayJob<GCCollectRunJob> gcCollectRunJob = null;

        private OneWayJob<GCCollectRunJob> GCCollectRunJob
        {
            get
            {
                this.gcCollectRunJob ??= new();
                return this.gcCollectRunJob;
            }
        }

        public InitialForm()
        {
            this.browserManager.BrowserNothing += new(this.BrowserManager_BrowserNothing);
        }

        private void BrowserManager_BrowserNothing(object sender, EventArgs e)
        {
            this.gcCollectRunJob?.Dispose();
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.GCCollectRunJob.StartJob();

            var form = this.browserManager.GetActiveBrowser();
            form.Show();

            base.OnLoad(e);
        }
    }
}
