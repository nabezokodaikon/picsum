using SWF.UIComponent.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブのドラッグ操作を制御するクラスです。
    /// </summary>
    internal class TabDragOperation
    {
        private const int DEFAULT_WIDTH_OFFSET = 8;

        public static bool IsCompletelyHidden(Form targetForm)
        {
            var formBounds = targetForm.Bounds;
            var visibleRegion = new Region(formBounds);

            var hWnd = targetForm.Handle;
            var hWndAbove = WinApiMembers.GetWindow(hWnd, WinApiMembers.GW_HWNDPREV);

            // 自分より前面にあるウィンドウをすべてチェック
            while (hWndAbove != IntPtr.Zero)
            {
                if (WinApiMembers.IsWindowVisible(hWndAbove))
                {
                    if (WinApiMembers.GetWindowRect(hWndAbove, out WinApiMembers.RECT rect))
                    {
                        var aboveRect = rect.ToRectangle();

                        // 重なっている部分を除外
                        if (formBounds.IntersectsWith(aboveRect))
                        {
                            visibleRegion.Exclude(aboveRect);
                        }
                    }
                }

                hWndAbove = WinApiMembers.GetWindow(hWndAbove, WinApiMembers.GW_HWNDPREV);
            }

            // 見えている領域が空かチェック
            var isEmpty = visibleRegion.IsEmpty(Graphics.FromHwnd(targetForm.Handle));
            visibleRegion.Dispose();

            return isEmpty;
        }

        private readonly List<Form> _formList = [];
        private Point _dragCursorPoint = Point.Empty;
        private float _widthOffset = 0;
        private float _heightOffset = 0;
        private bool _isMoving = false;
        private TabInfo _targetTab = null;

        /// <summary>
        /// 操作中か確認します。
        /// </summary>
        public bool IsBegin { get; private set; } = false;

        /// <summary>
        /// フォームをリストに追加します。
        /// </summary>
        /// <param name="form"></param>
        public void AddForm(Form form)
        {
            ArgumentNullException.ThrowIfNull(form, nameof(form));

            if (this._formList.Contains(form))
            {
                throw new ArgumentException("既に追加されているフォームをリストに追加しようとしました。", nameof(form));
            }

            this._formList.Insert(0, form);

            this.AddFormHandler(form);
        }

        /// <summary>
        /// タブのドラッグ操作を試みます。
        /// </summary>
        /// <param name="tab"></param>
        public void BeginTabDragOperation(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            if (this.IsBegin)
            {
                throw new InvalidOperationException("既にドラッグ操作が開始されています。");
            }

            if (tab.Owner == null)
            {
                throw new ArgumentException("タブはどこにも所有されていません。", nameof(tab));
            }

            this._dragCursorPoint = Cursor.Position;

            var clientPoint = tab.Owner.PointToClient(this._dragCursorPoint);
            this._widthOffset = clientPoint.X - tab.DrawArea.X;
            this._heightOffset = clientPoint.Y - tab.DrawArea.Y;

            this._targetTab = tab;
            this.IsBegin = true;
        }

        /// <summary>
        /// タブのドラッグ操作を終了します。
        /// </summary>
        /// <returns>対象となったタブを返します。</returns>
        public TabInfo EndTabDragOperation()
        {
            if (!this.IsBegin)
            {
                throw new InvalidOperationException("ドラッグ操作が開始されていません。");
            }

            var targetTab = this._targetTab;

            this.IsBegin = false;
            this._isMoving = false;
            this._targetTab = null;

            return targetTab;
        }

        /// <summary>
        /// タブを移動します。
        /// </summary>
        public void MoveTab(Point tabSwitchMouseLocation)
        {
            if (!this.IsBegin)
            {
                throw new InvalidOperationException("ドラッグ操作が開始されていません。");
            }

            var cursorPosition = Cursor.Position;
            var moveWidth = cursorPosition.X - this._dragCursorPoint.X;
            var moveHeight = cursorPosition.Y - this._dragCursorPoint.Y;

            if (!this._isMoving)
            {
                if ((SystemInformation.DragSize.Width < Math.Abs(moveWidth) ||
                     SystemInformation.DragSize.Height < Math.Abs(moveHeight)))
                {
                    this._isMoving = true;
                }
                else
                {
                    return;
                }
            }

            if (this._targetTab == null)
            {
                return;
            }

            if (this._targetTab.Owner == null)
            {
                return;
            }

            if (this._targetTab.Owner.TabCount == 1)
            {
                // タブが一つ。
                var currentTab = this._targetTab;
                var currentTabSwitch = currentTab.Owner;
                var currentForm = (BaseForm)currentTabSwitch.GetForm();

                void movingEvent(object sender, EventArgs e)
                {
                    if (currentTabSwitch.IsDisposed)
                    {
                        return;
                    }

                    var cursorPosition = Cursor.Position;

                    foreach (var form in this._formList)
                    {
                        if (form == currentForm)
                        {
                            continue;
                        }

                        if (IsCompletelyHidden(form))
                        {
                            continue;
                        }

                        if (!form.Bounds.Contains(cursorPosition))
                        {
                            continue;
                        }

                        if (currentForm.IsDisposed)
                        {
                            continue;
                        }

                        if (!currentTabSwitch.Contains(currentTab))
                        {
                            continue;
                        }

                        var tabSwitch = this.GetTabSwitchControl(form);
                        if (tabSwitch.Contains(currentTab))
                        {
                            continue;
                        }

                        var tabsScreenRectangle = tabSwitch.GetTabsScreenRectangle();
                        if (!tabsScreenRectangle.Contains(cursorPosition))
                        {
                            continue;
                        }

                        _ = WinApiMembers.SendMessage(currentForm.Handle, WinApiMembers.WM_CANCELMODE, 0, 0);
                        WinApiMembers.ReleaseCapture();

                        currentTabSwitch.RemoveTab(currentTab);
                        tabSwitch.AddTab(currentTab);

                        currentTabSwitch.OnTabDropouted(new TabDropoutedEventArgs(currentTab));

                        form.Activate();
                        tabSwitch.Focus();
                        tabSwitch.Capture = true;
                        tabSwitch.InvalidateHeader(true);

                        return;
                    }
                }

                void movedEvent(object sender, EventArgs e)
                {
                    if (this.IsBegin)
                    {
                        _ = this.EndTabDragOperation();
                    }

                    currentTabSwitch.InvalidateHeaderWithAnimation();
                }

                currentTabSwitch.InvalidateHeaderWithAnimation();

                currentForm.Moving -= movingEvent;
                currentForm.Moving += movingEvent;
                currentForm.Moved -= movedEvent;
                currentForm.Moved += movedEvent;

                WinApiMembers.ReleaseCapture();
                _ = WinApiMembers.SendMessage(
                    currentForm.Handle,
                    WinApiMembers.WM_NCLBUTTONDOWN,
                    WinApiMembers.HTCAPTION,
                    0);
            }
            else
            {
                // タブが複数。
                var tabsScreenRectangle = this._targetTab.Owner.GetTabsScreenRectangleWithOffset();

                if (tabsScreenRectangle.Contains(cursorPosition))
                {
                    // タブヘッダー内の移動。
                    var clientPoint = this._targetTab.Owner.PointToClient(cursorPosition);

                    float toX;
                    if (this._targetTab.DrawArea.Width > this._widthOffset)
                    {
                        toX = clientPoint.X - this._widthOffset;
                    }
                    else
                    {
                        toX = clientPoint.X - DEFAULT_WIDTH_OFFSET;
                    }

                    var rect = this._targetTab.Owner.GetTabsClientRectangle();
                    if (toX < rect.X)
                    {
                        this._targetTab.DrawArea.X = rect.X;
                    }
                    else if (toX + this._targetTab.DrawArea.Width > rect.Right)
                    {
                        this._targetTab.DrawArea.Right = rect.Right;
                    }
                    else
                    {
                        this._targetTab.DrawArea.X = toX;
                    }

                    this._targetTab.Owner.InvalidateHeaderWithAnimation();
                    return;
                }
                else
                {
                    // タブヘッダー外への移動。
                    var ownerTabSwitch = this._targetTab.Owner;
                    var tabsRectange = ownerTabSwitch.GetTabsScreenRectangleWithOffset();
                    var tagSwitchPoint = ownerTabSwitch.PointToScreen(tabSwitchMouseLocation);
                    var formSize = ownerTabSwitch.GetForm().Size;

                    ownerTabSwitch.RemoveTab(this._targetTab);
                    ownerTabSwitch.InvalidateHeader(false);

                    ownerTabSwitch.OnTabDropouted(new TabDropoutedEventArgs(
                        this._targetTab,
                        tabsRectange,
                        tagSwitchPoint,
                        formSize,
                        FormWindowState.Normal));

                    return;
                }
            }
        }

        /// <summary>
        /// 対象のタブであるか確認します。
        /// </summary>
        /// <param name="tab">比較するタブ</param>
        /// <returns></returns>
        public bool IsTarget(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            return tab == this._targetTab;
        }

        private TabSwitch GetTabSwitchControl(Form form)
        {
            var tabSwitch = this.GetTabSwitchControl(form.Controls);
            if (tabSwitch != null)
            {
                return tabSwitch;
            }
            else
            {
                throw new InvalidOperationException("フォーム内にタブスイッチコントロールが存在しません。");
            }
        }

        private TabSwitch GetTabSwitchControl(Control.ControlCollection controls)
        {
            foreach (Control child in controls)
            {
                if (child is TabSwitch @switch)
                {
                    return @switch;
                }
                else if (child.Controls.Count > 0)
                {
                    var result = this.GetTabSwitchControl(child.Controls);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private void AddFormHandler(Form form)
        {
            form.Activated += new(this.Form_Activated);
            form.FormClosing += new(this.Form_FormClosing);
        }

        private void RemoveFormHandler(Form form)
        {
            form.Activated -= new(this.Form_Activated);
            form.FormClosing -= new(this.Form_FormClosing);
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            if (sender != null && sender is Form form)
            {
                this._formList.Remove(form);
                this._formList.Insert(0, form);
            }
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sender != null && sender is Form form)
            {
                this._formList.Remove(form);
                this.RemoveFormHandler(form);
            }
        }
    }
}
