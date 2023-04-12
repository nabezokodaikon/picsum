using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PicSum.Main.Properties;
using PicSum.Main.UIComponent;
using PicSum.Task.Entity;
using SWF.UIComponent.TabOperation;

namespace PicSum.Main.Mng
{
    /// <summary>
    /// ブラウザ管理クラス
    /// </summary>
    public class BrowserManager
    {
        #region イベント

        /// <summary>
        /// ブラウザが無くなったに発生するイベント
        /// </summary>
        public event EventHandler BrowserNothing;

        #endregion

        #region インスタンス変数

        private readonly List<BrowserForm> _browserList = new List<BrowserForm>();

        #endregion

        #region コンストラクタ

        public BrowserManager()
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// アクティブなブラウザを取得します。
        /// </summary>
        /// <returns></returns>
        public BrowserForm GetActiveBrowser()
        {
            return getActiveBrowser();
        }

        #endregion

        #region イベント発生メソッド

        protected virtual void OnBrowserNothing(EventArgs e)
        {
            if (BrowserNothing != null)
            {
                BrowserNothing(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {

        }

        private BrowserForm getActiveBrowser()
        {
            if (_browserList.Count > 0)
            {
                return _browserList.First();
            }
            else
            {
                BrowserForm browser = createBrowser();
                _browserList.Add(browser);
                return browser;
            }
        }

        #endregion

        #region ブラウザを作成メソッド

        private BrowserForm createBrowser()
        {
            BrowserForm browser = new BrowserForm();
            initializeBrowserDelegate(browser);
            return browser;
        }

        private BrowserForm createBrowser(Point windowLocation, Size windowSize, FormWindowState windowState)
        {
            BrowserForm browser = new BrowserForm();

            browser.Location = windowLocation;
            browser.Size = windowSize;
            browser.WindowState = windowState;

            initializeBrowserDelegate(browser);

            return browser;
        }

        private void initializeBrowserDelegate(BrowserForm browser)
        {
            browser.FormClosing += new System.Windows.Forms.FormClosingEventHandler(browser_FormClosing);
            browser.Activated += new EventHandler(browser_Activated);
            browser.TabDropouted += new EventHandler<TabDropoutedEventArgs>(browser_TabDropouted);
            browser.NewWindowContentsOpen += new EventHandler<BrowserContentsOpenEventArgs>(browser_NewWindowContentsOpen);
            browser.KeyDown += browser_KeyDown;
        }

        #endregion

        #region ブラウザイベント

        private void browser_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: キーボードショートカットを追加する。
            // 1. Backspace で戻る。
            // 2. Alt + 左 で戻る。
            // 3. Alt + 右 で進む。
            switch (e.KeyCode)
            {
                case Keys.W:
                    if (e.Control)
                    {
                        var browser = (BrowserForm)sender;
                        browser.RemoveTabOrWindow();
                        browser.Focus();
                    }
                    break;
                case Keys.T:
                    if (e.Control)
                    {
                        var browser = (BrowserForm)sender;
                        browser.AddFavoriteFolderListTab();
                        browser.Focus();
                    }
                    break;
            }
        }

        private void browser_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            BrowserForm browser = (BrowserForm)sender;

            _browserList.Remove(browser);

            if (_browserList.Count == 0)
            {
                OnBrowserNothing(new EventArgs());
            }
        }

        private void browser_Activated(object sender, EventArgs e)
        {
            BrowserForm browser = (BrowserForm)sender;
            _browserList.Remove(browser);
            _browserList.Insert(0, browser);
        }

        private void browser_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            BrowserForm browser = createBrowser(e.WindowLocation, e.WindowSize, e.WindowState);
            _browserList.Add(browser);
            browser.Show();
            browser.AddTab(e.Tab);
        }

        private void browser_NewWindowContentsOpen(object sender, BrowserContentsOpenEventArgs e)
        {
            BrowserForm browser = createBrowser();
            _browserList.Add(browser);
            browser.Show();
            browser.AddTab(e.ContentsParameter);
        }

        #endregion
    }
}
