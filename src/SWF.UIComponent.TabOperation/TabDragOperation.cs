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

    internal static class TabDragOperation
    {
        private const int DEFAULT_WIDTH_OFFSET = 8;
        private static readonly List<Form> _formList = [];
        private static Point _dragCursorPoint = Point.Empty;
        private static float _widthOffset = 0;
        private static float _heightOffset = 0;
        private static bool _isMoving = false;

        /// <summary>
        /// 操作中か確認します。
        /// </summary>
        public static bool IsBegin { get; private set; } = false;

        public static TabInfo TargetTab { get; private set; } = null;

        /// <summary>
        /// フォームをリストに追加します。
        /// </summary>
        /// <param name="form"></param>
        public static void AddForm(Form form)
        {
            ArgumentNullException.ThrowIfNull(form, nameof(form));

            if (_formList.Contains(form))
            {
                throw new ArgumentException("既に追加されているフォームをリストに追加しようとしました。", nameof(form));
            }

            _formList.Insert(0, form);

            AddFormHandler(form);
        }

        /// <summary>
        /// フォームがリスト内に存在するか確認します。
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool ContainsForm(Form form)
        {
            return _formList.Contains(form);
        }

        /// <summary>
        /// タブのドラッグ操作を試みます。
        /// </summary>
        /// <param name="tab"></param>
        public static void BeginTabDragOperation(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            if (tab.Owner == null)
            {
                throw new ArgumentException("タブはどこにも所有されていません。", nameof(tab));
            }

            if (IsBegin)
            {
                throw new InvalidOperationException("既にドラッグ操作が開始されています。");
            }

            _dragCursorPoint = Cursor.Position;

            var clientPoint = tab.Owner.PointToClient(_dragCursorPoint);
            _widthOffset = clientPoint.X - tab.DrawArea.X;
            _heightOffset = clientPoint.Y - tab.DrawArea.Y;

            TargetTab = tab;
            IsBegin = true;
        }

        /// <summary>
        /// タブのドラッグ操作を終了します。
        /// </summary>
        /// <returns>対象となったタブを返します。</returns>
        public static TabInfo EndTabDragOperation()
        {
            if (!IsBegin)
            {
                throw new InvalidOperationException("ドラッグ操作が開始されていません。");
            }

            var targetTab = TargetTab;

            IsBegin = false;
            _isMoving = false;
            TargetTab = null;

            return targetTab;
        }

        /// <summary>
        /// タブを移動します。
        /// </summary>
        public static void MoveTab()
        {
            if (!IsBegin)
            {
                throw new InvalidOperationException("ドラッグ操作が開始されていません。");
            }

            var cursorPosition = Cursor.Position;
            var moveWidth = cursorPosition.X - _dragCursorPoint.X;
            var moveHeight = cursorPosition.Y - _dragCursorPoint.Y;

            if (!_isMoving)
            {
                if ((SystemInformation.DragSize.Width < Math.Abs(moveWidth) ||
                     SystemInformation.DragSize.Height < Math.Abs(moveHeight)))
                {
                    _isMoving = true;
                }
                else
                {
                    return;
                }
            }

            if (TargetTab == null)
            {
                return;
            }

            if (TargetTab.Owner == null)
            {
                return;
            }

            if (TargetTab.Owner.TabCount == 1)
            {
                // タブが一つ。
                var currentTab = TargetTab;
                var ownerTabSwitch = currentTab.Owner;
                var ownerForm = (BaseForm)ownerTabSwitch.GetForm();

                void movingEvent(object sender, EventArgs e)
                {
                    if (ownerTabSwitch.IsDisposed)
                    {
                        return;
                    }

                    var cursorPosition = Cursor.Position;
                    var clientPosition = ownerTabSwitch.PointToClient(cursorPosition);
                    var currentTabRect = currentTab.DrawArea.GetRectangle();
                    if (!currentTabRect.Contains(clientPosition))
                    {
                        ConsoleUtil.Write(true, $"Rect: {currentTabRect}, Point: {clientPosition}");
                        return;
                    }

                    foreach (var form in _formList)
                    {
                        if (form == ownerForm)
                        {
                            continue;
                        }

                        if (!form.Bounds.Contains(cursorPosition))
                        {
                            continue;
                        }

                        if (ownerForm.IsDisposed)
                        {
                            ConsoleUtil.Write(true, "移動元のTabSwitchが破棄されています。");
                            continue;
                        }

                        if (!ownerTabSwitch.Contains(currentTab))
                        {
                            ConsoleUtil.Write(true, "移動元のTabSwitch内に存在しません。");
                            continue;
                        }

                        var tabSwitch = GetTabSwitchControl(form);
                        if (tabSwitch.Contains(currentTab))
                        {
                            ConsoleUtil.Write(true, "移動先のTabSwitchに存在しています。");
                            continue;
                        }

                        var tabsScreenRectangle = tabSwitch.GetTabsScreenRectangle();
                        if (!tabsScreenRectangle.Contains(cursorPosition))
                        {
                            ConsoleUtil.Write(true, "移動先のタブ領域外です。");
                            continue;
                        }

                        _ = WinApiMembers.SendMessage(ownerForm.Handle, WinApiMembers.WM_CANCELMODE, 0, 0);
                        WinApiMembers.ReleaseCapture();

                        ownerTabSwitch.RemoveTab(currentTab);
                        tabSwitch.AddTab(currentTab);

                        ownerTabSwitch.OnTabDropouted(new TabDropoutedEventArgs(currentTab));

                        form.Activate();
                        tabSwitch.Focus();
                        tabSwitch.Capture = true;
                        tabSwitch.InvalidateHeader(true);

                        ConsoleUtil.Write(true, "タブを入れ替えました。");
                        return;
                    }
                }

                void movedEvent(object sender, EventArgs e)
                {
                    if (IsBegin)
                    {
                        _ = EndTabDragOperation();
                    }

                    ownerTabSwitch.InvalidateHeaderWithAnimation();
                }

                ownerTabSwitch.InvalidateHeaderWithAnimation();

                ownerForm.Moving -= movingEvent;
                ownerForm.Moving += movingEvent;
                ownerForm.Moved -= movedEvent;
                ownerForm.Moved += movedEvent;

                WinApiMembers.ReleaseCapture();
                _ = WinApiMembers.SendMessage(
                    ownerForm.Handle,
                    WinApiMembers.WM_NCLBUTTONDOWN,
                    WinApiMembers.HTCAPTION,
                    0);
            }
            else
            {
                // タブが複数。
                var tabsScreenRectangle = TargetTab.Owner.GetTabsScreenRectangleWithOffset();

                if (tabsScreenRectangle.Contains(cursorPosition))
                {
                    // タブヘッダー内の移動。
                    var clientPoint = TargetTab.Owner.PointToClient(cursorPosition);

                    float toX;
                    if (TargetTab.DrawArea.Width > _widthOffset)
                    {
                        toX = clientPoint.X - _widthOffset;
                    }
                    else
                    {
                        toX = clientPoint.X - DEFAULT_WIDTH_OFFSET;
                    }

                    var rect = TargetTab.Owner.GetTabsClientRectangle();
                    if (toX < rect.X)
                    {
                        TargetTab.DrawArea.X = rect.X;
                    }
                    else if (toX + TargetTab.DrawArea.Width > rect.Right)
                    {
                        TargetTab.DrawArea.Right = rect.Right;
                    }
                    else
                    {
                        TargetTab.DrawArea.X = toX;
                    }

                    TargetTab.Owner.InvalidateHeaderWithAnimation();
                    return;
                }
                else
                {
                    // タブヘッダー外への移動。
                    var owner = TargetTab.Owner;
                    var form = owner.GetForm();
                    owner.RemoveTab(TargetTab);
                    owner.InvalidateHeader();

                    var tabDrawArea = new Rectangle(
                        (int)TargetTab.DrawArea.X,
                        (int)TargetTab.DrawArea.Y,
                        (int)TargetTab.DrawArea.Width,
                        (int)TargetTab.DrawArea.Height);

                    owner.OnTabDropouted(new TabDropoutedEventArgs(
                        TargetTab,
                        tabDrawArea,
                        form.ClientSize,
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
        public static bool IsTarget(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            return tab == TargetTab;
        }

        private static TabSwitch GetTabSwitchControl(Form form)
        {
            var tabSwitch = GetTabSwitchControl(form.Controls);
            if (tabSwitch != null)
            {
                return tabSwitch;
            }
            else
            {
                throw new InvalidOperationException("フォーム内にタブスイッチコントロールが存在しません。");
            }
        }

        private static TabSwitch GetTabSwitchControl(Control.ControlCollection controls)
        {
            foreach (Control child in controls)
            {
                if (child is TabSwitch @switch)
                {
                    return @switch;
                }
                else if (child.Controls.Count > 0)
                {
                    var result = GetTabSwitchControl(child.Controls);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private static void AddFormHandler(Form form)
        {
            form.Activated += new(Form_Activated);
            form.FormClosing += new(Form_FormClosing);
        }

        private static void RemoveFormHandler(Form form)
        {
            form.Activated -= new(Form_Activated);
            form.FormClosing -= new(Form_FormClosing);
        }

        private static void Form_Activated(object sender, EventArgs e)
        {
            if (sender != null && sender is Form form)
            {
                _formList.Remove(form);
                _formList.Insert(0, form);
            }
        }

        private static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sender != null && sender is Form form)
            {
                _formList.Remove(form);
                RemoveFormHandler(form);
            }
        }

    }
}
