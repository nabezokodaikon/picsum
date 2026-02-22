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

    internal sealed class WindowManager
    {

        /// <summary>
        /// ブラウザが無くなったに発生するイベント
        /// </summary>
        public event EventHandler WindowNothing;

        private readonly List<BrowseWindow> _windowList = [];

        public WindowManager()
        {

        }

        /// <summary>
        /// アクティブなブラウザを取得します。
        /// </summary>
        /// <returns></returns>
        internal BrowseWindow GetActiveWindow()
        {
            if (this._windowList.Count > 0)
            {
                return this._windowList.First();
            }
            else
            {
                var window = this.CreateWindow();
                this._windowList.Add(window);
                return window;
            }
        }

        public void OnWindowNothing(EventArgs e)
        {
            this.WindowNothing?.Invoke(this, e);
        }

        private BrowseWindow CreateWindow()
        {
            using (Measuring.Time(true, "WindowManager.CreateWindow"))
            {
                var window = new BrowseWindow();
                this.InitializeWindowDelegate(window);

                return window;
            }
        }

        private void InitializeWindowDelegate(BrowseWindow window)
        {
            window.FormClosed += new(this.Window_FormClosed);
            window.Activated += new(this.Window_Activated);
            window.TabDropouted += new(this.Window_TabDropouted);
            window.NewWindowPageOpen += new(this.Window_NewWindowPageOpen);
        }

        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.DoEvents();

            var window = (BrowseWindow)sender;

            this._windowList.Remove(window);

            if (this._windowList.Count == 0)
            {
                this.OnWindowNothing(new EventArgs());
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            var window = (BrowseWindow)sender;
            this._windowList.Remove(window);
            this._windowList.Insert(0, window);
        }

        private void Window_TabDropouted(object sender, TabDropoutedEventArgs e)
        {
            var window = this.CreateWindow();
            window.WindowState = e.WindowState;
            window.StartPosition = FormStartPosition.Manual;
            window.Size = e.WindowSize;

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

                window.Location = new((int)x, (int)y);

                window.Shown -= setLocation;
            }

            this._windowList.Add(window);
            window.AddTab(e.Tab);

            window.Shown += setLocation;

            //window.Opacity = 0;
            window.Show();
            //window.Opacity = 1;
        }

        private void Window_NewWindowPageOpen(object sender, BrowsePageOpenEventArgs e)
        {
            var activeWindow = this.GetActiveWindow();
            var scale = WindowUtil.GetCurrentWindowScale(activeWindow);
            var offset = 16 * scale;
            var newWindow = this.CreateWindow();
            newWindow.Location = new Point(
                (int)(activeWindow.Location.X + offset),
                (int)(activeWindow.Location.Y + offset));
            newWindow.Size = activeWindow.Size;

            this._windowList.Add(newWindow);
            newWindow.Show();
            newWindow.AddTab(e.PageParameter);
        }

    }
}
