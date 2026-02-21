using PicSum.Main.UIComponent;
using SWF.Core.Base;
using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        private void InitializeBrowseDelegate(BrowseForm browse)
        {
            browse.FormClosed += new(this.Browse_FormClosed);
            browse.Activated += new(this.Browse_Activated);
            browse.TabDropouted += new(this.Browse_TabDropouted);
            browse.NewWindowPageOpen += new(this.Browse_NewWindowPageOpen);
        }

        private void Browse_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.DoEvents();

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
            var browse = this.CreateBrowse();
            browse.WindowState = e.WindowState;
            browse.StartPosition = FormStartPosition.Manual;
            browse.Size = e.WindowSize;

            void setLocation(object s, EventArgs arg)
            {
                var screenPoint = Cursor.Position;

                float x;
                float y;
                if (e.TabSwitchMouseLocation.X < e.TabsRectange.Left)
                {
                    x = screenPoint.X - e.TabsRectange.X - e.Tab.DrawArea.Width / 2f;
                    y = screenPoint.Y - e.TabsRectange.Height / 2f;
                }
                else if (e.TabSwitchMouseLocation.X > e.TabsRectange.Right)
                {
                    x = screenPoint.X - e.TabsRectange.Width + e.Tab.DrawArea.Width / 2f;
                    y = screenPoint.Y - e.TabsRectange.Height / 2f;
                }
                else
                {
                    x = screenPoint.X - e.TabSwitchMouseLocation.X;
                    y = screenPoint.Y - e.TabsRectange.Height / 2f;
                }

                browse.Location = new((int)x, (int)y);

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
            var activeBrowse = this.GetActiveBrowse();
            var scale = WindowUtil.GetCurrentWindowScale(activeBrowse);
            var offset = 16 * scale;
            var browse = this.CreateBrowse();
            browse.Location = new Point(
                (int)(activeBrowse.Location.X + offset),
                (int)(activeBrowse.Location.Y + offset));
            browse.Size = activeBrowse.Size;

            this.browseList.Add(browse);
            browse.Show();
            browse.AddTab(e.PageParameter);
        }

    }
}
