using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブのドラッグ操作を制御するクラスです。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class TabDragOperation
    {
        private const int DEFAULT_WIDTH_OFFSET = 8;
        private static readonly List<Form> FORM_LIST = [];
        private static TabDragForm tabDragForm = null;
        private static TabInfo tab = null;
        private static Point fromScreenPoint = Point.Empty;
        private static float widthOffset = 0;
        private static float heightOffset = 0;
        private static bool isBegin = false;
        private static bool isMoving = false;

        public static TabDragForm TabDragForm
        {
            get
            {
                tabDragForm ??= new TabDragForm();
                return tabDragForm;
            }
        }

        public static TabInfo Tab
        {
            get
            {
                return tab;
            }
        }

        /// <summary>
        /// 操作中か確認します。
        /// </summary>
        public static bool IsBegin
        {
            get
            {
                return isBegin;
            }
        }

        /// <summary>
        /// フォームをリストに追加します。
        /// </summary>
        /// <param name="form"></param>
        public static void AddForm(Form form)
        {
            ArgumentNullException.ThrowIfNull(form, nameof(form));

            if (FORM_LIST.Contains(form))
            {
                throw new ArgumentException("既に追加されているフォームをリストに追加しようとしました。", nameof(form));
            }

            FORM_LIST.Insert(0, form);

            AddFormHandler(form);
        }

        /// <summary>
        /// フォームがリスト内に存在するか確認します。
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool ContainsForm(Form form)
        {
            return FORM_LIST.Contains(form);
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

            if (isBegin)
            {
                throw new InvalidOperationException("既にドラッグ操作が開始されています。");
            }

            fromScreenPoint = Cursor.Position;
            var clientPoint = tab.Owner.PointToClient(fromScreenPoint);
            widthOffset = clientPoint.X - tab.DrawArea.X;
            heightOffset = clientPoint.Y - tab.DrawArea.Y;

            TabDragOperation.tab = tab;
            isBegin = true;

            TabDragForm.SetTab(tab);
        }

        /// <summary>
        /// タブのドラッグ操作を終了します。
        /// </summary>
        /// <returns>対象となったタブを返します。</returns>
        public static TabInfo EndTabDragOperation()
        {
            if (!isBegin)
            {
                throw new InvalidOperationException("ドラッグ操作が開始されていません。");
            }

            var targetTab = tab;

            isBegin = false;
            isMoving = false;
            tab = null;

            TabDragForm.Visible = false;
            TabDragForm.Clear();

            return targetTab;
        }

        /// <summary>
        /// タブを移動します。
        /// </summary>
        public static void MoveTab()
        {
            if (!isBegin)
            {
                throw new InvalidOperationException("ドラッグ操作が開始されていません。");
            }

            var toScreenPoint = Cursor.Position;
            var moveWidth = toScreenPoint.X - fromScreenPoint.X;
            var moveHeight = toScreenPoint.Y - fromScreenPoint.Y;

            if (!isMoving)
            {
                if ((SystemInformation.DragSize.Width < Math.Abs(moveWidth) ||
                     SystemInformation.DragSize.Height < Math.Abs(moveHeight)))
                {
                    isMoving = true;
                }
                else
                {
                    return;
                }
            }

            TabDragForm.SetLocation(widthOffset, heightOffset);

            if (tab == null)
            {
                return;
            }

            if (tab.Owner != null)
            {
                // タブが所有されている場合。
                var tabsScreenRectangle = tab.Owner.GetTabsScreenRectangle();
                if (new RectangleF(
                        tabsScreenRectangle.X - 24,
                        tabsScreenRectangle.Y - 8,
                        tabsScreenRectangle.Width + 48,
                        tabsScreenRectangle.Height + 32)
                    .Contains(toScreenPoint))
                {
                    var clientPoint = tab.Owner.PointToClient(toScreenPoint);

                    float toX;
                    if (tab.DrawArea.Width > widthOffset)
                    {
                        toX = clientPoint.X - widthOffset;
                    }
                    else
                    {
                        toX = clientPoint.X - DEFAULT_WIDTH_OFFSET;
                    }

                    var rect = tab.Owner.GetTabsClientRectangle();
                    if (toX < rect.X)
                    {
                        tab.DrawArea.X = rect.X;
                    }
                    else if (toX + tab.DrawArea.Width > rect.Right)
                    {
                        tab.DrawArea.Right = rect.Right;
                    }
                    else
                    {
                        tab.DrawArea.X = toX;
                    }

                    tab.Owner.InvalidateHeader();
                    return;
                }
                else
                {
                    tab.Owner.RemoveTab(tab);
                    TabDragForm.Visible = true;
                    return;
                }
            }
            else
            {
                // タブが所有されていない場合。
                for (var i = 0; i < FORM_LIST.Count; i++)
                {
                    var form = FORM_LIST[i];

                    if (form.Bounds.Contains(toScreenPoint))
                    {
                        var owner = GetTabSwitchControl(form);
                        if (owner.GetTabsScreenRectangle().Contains(toScreenPoint))
                        {
                            var clientPoint = owner.PointToClient(toScreenPoint);
                            tab.DrawArea.X = clientPoint.X;
                            TabDragForm.Visible = false;
                            form.Activate();
                            owner.AddTab(tab);
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
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

            return tab == TabDragOperation.tab;
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
                FORM_LIST.Remove(form);
                FORM_LIST.Insert(0, form);
            }
        }

        private static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sender != null && sender is Form form)
            {
                FORM_LIST.Remove(form);
                RemoveFormHandler(form);
            }
        }

    }
}
