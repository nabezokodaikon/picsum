using PicSum.Main.UIComponent;
using SWF.Core.Base;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.Main.Mng
{
    /// <summary>
    /// ブラウザ管理クラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class BrowserManager
    {

        /// <summary>
        /// ブラウザが無くなったに発生するイベント
        /// </summary>
        public event EventHandler BrowserNothing;

        private readonly List<BrowserForm> browserList = [];

        public BrowserManager()
        {

        }

        /// <summary>
        /// アクティブなブラウザを取得します。
        /// </summary>
        /// <returns></returns>
        public BrowserForm GetActiveBrowser()
        {
            if (this.browserList.Count > 0)
            {
                return this.browserList.First();
            }
            else
            {
                var browser = this.CreateBrowser();
                this.browserList.Add(browser);
                return browser;
            }
        }

        public void OnBrowserNothing(EventArgs e)
        {
            this.BrowserNothing?.Invoke(this, e);
        }

        private BrowserForm CreateBrowser()
        {
            ConsoleUtil.Write(true, $"BrowserManager.CreateBrowser Start");

            var browser = new BrowserForm(false);
            this.InitializeBrowserDelegate(browser);

            ConsoleUtil.Write(true, $"BrowserManager.CreateBrowser End");

            return browser;
        }

        private BrowserForm CreateBrowser(Point windowLocation, Size windowSize, FormWindowState windowState)
        {
            ConsoleUtil.Write(true, $"BrowserManager.CreateBrowser Start");

            var browser = new BrowserForm(true)
            {
                Location = windowLocation,
                Size = windowSize,
                WindowState = windowState
            };

            this.InitializeBrowserDelegate(browser);

            ConsoleUtil.Write(true, $"BrowserManager.CreateBrowser End");

            return browser;
        }

        private void InitializeBrowserDelegate(BrowserForm browser)
        {
            browser.FormClosing += new(this.Browser_FormClosing);
            browser.Activated += new(this.Browser_Activated);
            browser.TabDropouted += new(this.Browser_TabDropouted);
            browser.NewWindowPageOpen += new(this.Browser_NewWindowPageOpen);
        }

        private void Browser_FormClosing(object sender, FormClosingEventArgs e)
        {
            var browser = (BrowserForm)sender;

            this.browserList.Remove(browser);

            if (this.browserList.Count == 0)
            {
                this.OnBrowserNothing(new EventArgs());
            }
        }

        private void Browser_Activated(object sender, EventArgs e)
        {
            var browser = (BrowserForm)sender;
            this.browserList.Remove(browser);
            this.browserList.Insert(0, browser);
        }

        private void Browser_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            var browser = this.CreateBrowser(e.WindowLocation, e.WindowSize, e.WindowState);
            this.browserList.Add(browser);
            browser.Show();
            browser.AddTab(e.Tab);
        }

        private void Browser_NewWindowPageOpen(object sender, BrowserPageOpenEventArgs e)
        {
            var browser = this.CreateBrowser();
            this.browserList.Add(browser);
            browser.Show();
            browser.AddTab(e.PageParameter);
        }

    }
}
