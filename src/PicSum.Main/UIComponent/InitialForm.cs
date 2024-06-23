using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Jobs;
using PicSum.Main.Mng;
using SWF.UIComponent.Core;
using System;
using System.Runtime.Versioning;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ダミーフォーム
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class InitialForm : HideForm
    {
        private readonly BrowserManager browserManager = new();
        private OneWayJob<GCRunJob> gcRunJob = null;

        private OneWayJob<GCRunJob> GCRunJob
        {
            get
            {
                if (this.gcRunJob == null)
                {
                    this.gcRunJob = new();
                    this.gcRunJob
                        .StartThread();
                }

                return this.gcRunJob;
            }
        }

        public InitialForm()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.browserManager.BrowserNothing += new(this.BrowserManager_BrowserNothing);
        }

        private void BrowserManager_BrowserNothing(object sender, EventArgs e)
        {
            if (this.gcRunJob != null)
            {
                this.gcRunJob.Dispose();
                this.gcRunJob = null;
            }

            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.GCRunJob.StartJob();

            var form = this.browserManager.GetActiveBrowser();
            form.Show();

            base.OnLoad(e);
        }
    }
}
