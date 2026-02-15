using PicSum.Main.UIComponent;
using SWF.Core.Base;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.Main.Mng
{
    /// <summary>
    /// ブラウザ管理クラス
    /// </summary>

    internal sealed class BrowseManager
    {

        /// <summary>
        /// ブラウザが無くなったに発生するイベント
        /// </summary>
        public event EventHandler BrowseNothing;

        private readonly List<BrowseForm> browseList = [];

        public BrowseManager()
        {

        }

        /// <summary>
        /// アクティブなブラウザを取得します。
        /// </summary>
        /// <returns></returns>
        internal BrowseForm GetActiveBrowse()
        {
            if (this.browseList.Count > 0)
            {
                return this.browseList.First();
            }
            else
            {
                var browse = this.CreateBrowse();
                this.browseList.Add(browse);
                return browse;
            }
        }

        public void OnBrowseNothing(EventArgs e)
        {
            this.BrowseNothing?.Invoke(this, e);
        }

        private BrowseForm CreateBrowse()
        {
            using (Measuring.Time(true, "BrowseManager.CreateBrowse"))
            {
                var browse = new BrowseForm();
                this.InitializeBrowseDelegate(browse);

                return browse;
            }
        }

        private BrowseForm CreateBrowse(FormWindowState windowState)
        {
            using (Measuring.Time(true, "BrowseManager.CreateBrowse"))
            {
                var browse = new BrowseForm()
                {
                    StartPosition = FormStartPosition.Manual,
                    WindowState = windowState,
                };

                this.InitializeBrowseDelegate(browse);

                return browse;
            }
        }

        private void InitializeBrowseDelegate(BrowseForm browse)
        {
            browse.FormClosing += new(this.Browse_FormClosing);
            browse.Activated += new(this.Browse_Activated);
            browse.TabDropouted += new(this.Browse_TabDropouted);
            browse.NewWindowPageOpen += new(this.Browse_NewWindowPageOpen);
        }

        private void Browse_FormClosing(object sender, FormClosingEventArgs e)
        {
            var browse = (BrowseForm)sender;

            this.browseList.Remove(browse);

            if (this.browseList.Count == 0)
            {
                this.OnBrowseNothing(new EventArgs());
            }
        }

        private void Browse_Activated(object sender, EventArgs e)
        {
            var browse = (BrowseForm)sender;
            this.browseList.Remove(browse);
            this.browseList.Insert(0, browse);
        }

        private void Browse_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            var browse = this.CreateBrowse(e.WindowState);
            var tabsRectange = e.TabsRectange;
            var tabSwitchMouseLocation = e.TaSwitchMouseLocation;
            var windowSize = e.WindowSize;
            var tabDrawArea = e.Tab.DrawArea;

            void setLocation(object s, EventArgs arg)
            {
                var cursorPosition = Cursor.Position;

                if (tabsRectange.X <= cursorPosition.X && cursorPosition.X <= tabsRectange.Right)
                {
                    var x = cursorPosition.X + (tabsRectange.X - tabSwitchMouseLocation.X);
                    var y = cursorPosition.Y - tabsRectange.Height / 2f;
                    browse.SetBounds((int)x, (int)y, windowSize.Width, windowSize.Height);
                }
                else
                {
                    var x = cursorPosition.X - tabDrawArea.X - tabDrawArea.Width / 2f;
                    var y = cursorPosition.Y - tabDrawArea.Y - tabDrawArea.Height / 2f;
                    browse.SetBounds((int)x, (int)y, windowSize.Width, windowSize.Height);
                }

                browse.Shown -= setLocation;
            }

            this.browseList.Add(browse);
            browse.AddTab(e.Tab);

            browse.Shown += setLocation;

            //browse.Opacity = 0;
            browse.Show();
            //browse.Opacity = 1;
        }

        private void Browse_NewWindowPageOpen(object sender, BrowsePageOpenEventArgs e)
        {
            var browse = this.CreateBrowse();
            this.browseList.Add(browse);
            browse.Show();
            browse.AddTab(e.PageParameter);
        }

    }
}
