using PicSum.Main.UIComponent;
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
    [SupportedOSPlatform("windows")]
    public sealed class BrowserManager
    {
        #region イベント

        /// <summary>
        /// ブラウザが無くなったに発生するイベント
        /// </summary>
        public event EventHandler BrowserNothing;

        #endregion

        #region インスタンス変数

        private readonly List<BrowserForm> browserList = [];

        #endregion

        #region コンストラクタ

        public BrowserManager()
        {

        }

        #endregion

        #region パブリックメソッド

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

        #endregion

        #region イベント発生メソッド

        public void OnBrowserNothing(EventArgs e)
        {
            this.BrowserNothing?.Invoke(this, e);
        }

        #endregion

        #region ブラウザ作成メソッド

        private BrowserForm CreateBrowser()
        {
            var browser = new BrowserForm();
            this.InitializeBrowserDelegate(browser);
            return browser;
        }

        private BrowserForm CreateBrowser(Point windowLocation, Size windowSize, FormWindowState windowState)
        {
            var browser = new BrowserForm
            {
                Location = windowLocation,
                Size = windowSize,
                WindowState = windowState
            };

            this.InitializeBrowserDelegate(browser);

            return browser;
        }

        private void InitializeBrowserDelegate(BrowserForm browser)
        {
            browser.FormClosing += new(this.Browser_FormClosing);
            browser.Activated += new(this.Browser_Activated);
            browser.TabDropouted += new(this.Browser_TabDropouted);
            browser.NewWindowPageOpen += new(this.Browser_NewWindowPageOpen);
        }

        #endregion

        #region ブラウザイベント

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

        #endregion
    }
}
