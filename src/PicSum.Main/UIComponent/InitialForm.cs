using PicSum.Main.Mng;
using SWF.Common;
using SWF.UIComponent.Common;
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
        private BrowserManager browserManager = new();

        public InitialForm()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.browserManager.BrowserNothing += new EventHandler(this.BrowserManager_BrowserNothing);
        }

        private void BrowserManager_BrowserNothing(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            var form = this.browserManager.GetActiveBrowser();
            form.Show();

            base.OnLoad(e);
        }
    }
}
