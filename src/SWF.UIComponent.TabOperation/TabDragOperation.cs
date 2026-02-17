using SWF.Core.Base;
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
        private Point _dragStartScreenPoint = Point.Empty;
        private float _widthOffset = 0;
        private float _heightOffset = 0;
        private bool _isMoving = false;
        private TabInfo _currentTab = null;

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
        /// <param name="currentTab"></param>
        public void BeginTabDragOperation(TabInfo currentTab)
        {
            ArgumentNullException.ThrowIfNull(currentTab, nameof(currentTab));

            if (this.IsBegin)
            {
                throw new InvalidOperationException("既にドラッグ操作が開始されています。");
            }

            if (currentTab.Owner == null)
            {
                throw new ArgumentException("タブはどこにも所有されていません。", nameof(currentTab));
            }

            this._currentTab = currentTab;
            this._dragStartScreenPoint = Cursor.Position;

            var dragStartPoint = this._currentTab.Owner.PointToClient(this._dragStartScreenPoint);
            this._widthOffset = dragStartPoint.X - this._currentTab.DrawArea.X;
            this._heightOffset = dragStartPoint.Y - this._currentTab.DrawArea.Y;

            this.IsBegin = true;
        }

        /// <summary>
        /// タブのドラッグ操作を終了します。
        /// </summary>
        public void EndTabDragOperation()
        {
            if (!this.IsBegin)
            {
                throw new InvalidOperationException("ドラッグ操作が開始されていません。");
            }

            this.IsBegin = false;
            this._isMoving = false;
            this._currentTab = null;
        }

        /// <summary>
        /// タブを移動します。
        /// </summary>
        public void MoveTab(Point mousePoint)
        {
            if (!this.IsBegin)
            {
                throw new InvalidOperationException("ドラッグ操作が開始されていません。");
            }

            var currentScreenPoint = Cursor.Position;
            var moveWidth = currentScreenPoint.X - this._dragStartScreenPoint.X;
            var moveHeight = currentScreenPoint.Y - this._dragStartScreenPoint.Y;

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

            var currentTab = this._currentTab;
            var currentTabSwitch = currentTab.Owner;
            var currentForm = (BaseForm)currentTabSwitch.GetForm();

            if (currentTab == null)
            {
                return;
            }

            if (currentTabSwitch == null)
            {
                return;
            }

            if (currentTabSwitch.TabCount == 1)
            {
                // タブが一つ。
                void movingEvent(object sender, EventArgs e)
                {
                    if (!currentTabSwitch.Contains(currentTab))
                    {
                        return;
                    }

                    var screenPoint = Cursor.Position;

                    for (var i = 0; i < this._formList.Count; i++)
                    {
                        var otherForm = this._formList[i];

                        if (otherForm == currentForm)
                        {
                            continue;
                        }

                        if (IsCompletelyHidden(otherForm))
                        {
                            continue;
                        }

                        if (!otherForm.Bounds.Contains(screenPoint))
                        {
                            continue;
                        }

                        var otherTabSwitch = this.GetTabSwitchControl(otherForm);
                        var otherTabsScreenRectangle = otherTabSwitch.GetTabsScreenRectangle();
                        if (!otherTabsScreenRectangle.Contains(screenPoint))
                        {
                            continue;
                        }

                        _ = WinApiMembers.SendMessage(currentForm.Handle, WinApiMembers.WM_CANCELMODE, 0, 0);
                        WinApiMembers.ReleaseCapture();

                        currentTabSwitch.RemoveTab(currentTab);
                        otherTabSwitch.AddTab(currentTab);

                        currentTabSwitch.OnTabDropouted(new TabDropoutedEventArgs(currentTab));

                        otherForm.Activate();
                        otherTabSwitch.Focus();
                        otherTabSwitch.Capture = true;
                        otherTabSwitch.InvalidateHeader(true);
                    }
                }

                void movedEvent(object sender, EventArgs e)
                {
                    if (this.IsBegin)
                    {
                        this.EndTabDragOperation();
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
                var tabsScreenRectangle = currentTab.Owner.GetTabsScreenRectangleWithOffset();

                if (tabsScreenRectangle.Contains(currentScreenPoint))
                {
                    // タブヘッダー内の移動。
                    var currentPoint = currentTabSwitch.PointToClient(currentScreenPoint);

                    float toX;
                    if (currentTab.DrawArea.Width > this._widthOffset)
                    {
                        toX = currentPoint.X - this._widthOffset;
                    }
                    else
                    {
                        toX = currentPoint.X - this.GetWithOffset(currentForm);
                    }

                    var rect = currentTab.Owner.GetTabsClientRectangle();
                    if (toX < rect.X)
                    {
                        currentTab.DrawArea.X = rect.X;
                    }
                    else if (toX + currentTab.DrawArea.Width > rect.Right)
                    {
                        currentTab.DrawArea.Right = rect.Right;
                    }
                    else
                    {
                        currentTab.DrawArea.X = toX;
                    }

                    currentTabSwitch.InvalidateHeaderWithAnimation();
                    return;
                }
                else
                {
                    // タブヘッダー外への移動。
                    var tabsRectange = currentTabSwitch.GetTabsScreenRectangleWithOffset();
                    var tagSwitchPoint = currentTabSwitch.PointToScreen(mousePoint);
                    var formSize = currentForm.Size;

                    currentTabSwitch.RemoveTab(this._currentTab);
                    currentTabSwitch.InvalidateHeader(false);

                    currentTabSwitch.OnTabDropouted(new TabDropoutedEventArgs(
                        this._currentTab,
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

            return tab == this._currentTab;
        }

        private int GetWithOffset(Control control)
        {
            const int WIDTH_OFFSET = 8;
            var scale = WindowUtil.GetCurrentWindowScale(control);
            return (int)(WIDTH_OFFSET * scale);
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
