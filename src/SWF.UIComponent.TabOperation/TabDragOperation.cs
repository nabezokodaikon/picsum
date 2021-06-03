using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブのドラッグ操作を制御するクラスです。
    /// </summary>
    internal static class TabDragOperation
    {
        private const int DEFAULT_WIDTH_OFFSET = 8;
        private static readonly List<Form> _formList = new List<Form>();
        private static TabDragForm _tabDragForm = null;
        private static TabInfo _tab = null;
        private static Point _fromScreenPoint = Point.Empty;
        private static int _widthOffset = 0;
        private static int _heightOffset = 0;
        private static bool _isBegin = false;
        private static bool _isMoving = false;

        private static TabDragForm tabDragForm
        {
            get
            {
                if (_tabDragForm == null)
                {
                    _tabDragForm = new TabDragForm();
                }

                return _tabDragForm;
            }
        }

        /// <summary>
        /// フォームをリストに追加します。
        /// </summary>
        /// <param name="form"></param>
        public static void AddForm(Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }

            if (_formList.Contains(form))
            {
                throw new ArgumentException("既に追加されているフォームをリストに追加しようとしました。", "form");
            }

            _formList.Insert(0, form);

            addFormHandler(form);
        }

        /// <summary>
        /// フォームがリスト内に存在するか確認します。
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool ContentsForm(Form form)
        {
            return _formList.Contains(form);
        }

        /// <summary>
        /// 操作中か確認します。
        /// </summary>
        public static bool IsBegin
        {
            get
            {
                return _isBegin;
            }
        }

        /// <summary>
        /// タブのドラッグ操作を試みます。
        /// </summary>
        /// <param name="tab"></param>
        public static void BeginTabDragOperation(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            if (tab.Owner == null)
            {
                throw new ArgumentException("タブはどこにも所有されていません。", "tab");
            }

            if (_isBegin)
            {
                throw new SystemException("既にドラッグ操作が開始されています。");
            }

            _fromScreenPoint = Cursor.Position;
            Point clientPoint = tab.Owner.PointToClient(_fromScreenPoint);
            _widthOffset = clientPoint.X - tab.DrawArea.X;
            _heightOffset = clientPoint.Y - tab.DrawArea.Y;

            _tab = tab;
            _isBegin = true;

            tabDragForm.SetTab(tab);
        }

        /// <summary>
        /// タブのドラッグ操作を終了します。
        /// </summary>
        /// <returns>対象となったタブを返します。</returns>
        public static TabInfo EndTabDragOperation()
        {
            if (!_isBegin)
            {
                throw new SystemException("ドラッグ操作が開始されていません。");
            }

            TabInfo targetTab = _tab;

            _isBegin = false;
            _isMoving = false;
            _tab = null;

            tabDragForm.Hide();
            tabDragForm.Clear();

            return targetTab;
        }

        /// <summary>
        /// タブを移動します。
        /// </summary>
        public static void MoveTab()
        {
            if (!_isBegin)
            {
                throw new SystemException("ドラッグ操作が開始されていません。");
            }

            Point toScreenPoint = Cursor.Position;
            float moveWidth = toScreenPoint.X - _fromScreenPoint.X;
            float moveHeight = toScreenPoint.Y - _fromScreenPoint.Y;

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

            tabDragForm.SetLocation(_widthOffset, _heightOffset);

            if (_tab.Owner != null)
            {
                // タブが所有されている場合。
                if (_tab.Owner.GetTabsScreenRectangle().Contains(toScreenPoint))
                {
                    Point clientPoint = _tab.Owner.PointToClient(toScreenPoint);

                    int toX = clientPoint.X;
                    if (_tab.DrawArea.Width > _widthOffset)
                    {
                        toX = clientPoint.X - _widthOffset;
                    }
                    else
                    {
                        toX = clientPoint.X - DEFAULT_WIDTH_OFFSET;
                    }

                    Rectangle rect = _tab.Owner.GetTabsClientRectangle();
                    if (toX < rect.X)
                    {
                        _tab.DrawArea.X = rect.X;
                    }
                    else if (toX + _tab.DrawArea.Width > rect.Right)
                    {
                        _tab.DrawArea.Right = rect.Right;
                    }
                    else
                    {
                        _tab.DrawArea.X = toX;
                    }

                    _tab.Owner.InvalidateHeader();
                    return;
                }
                else
                {
                    _tab.Owner.RemoveTab(_tab);
                    tabDragForm.Show();
                    return;
                }
            }
            else
            {
                // タブが所有されていない場合。
                foreach (Form form in _formList)
                {
                    if (form.Bounds.Contains(toScreenPoint))
                    {
                        TabSwitch owner = getTabSwitchControl(form);
                        if (owner.GetTabsScreenRectangle().Contains(toScreenPoint))
                        {
                            form.Activate();
                            Point clientPoint = owner.PointToClient(toScreenPoint);
                            _tab.DrawArea.X = clientPoint.X;
                            owner.AddTab(_tab);
                            tabDragForm.Hide();
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
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            return tab.Equals(_tab);
        }

        private static TabSwitch getTabSwitchControl(Form form)
        {
            TabSwitch tabSwitch = getTabSwitchControl(form.Controls);
            if (tabSwitch != null)
            {
                return tabSwitch;
            }
            else
            {
                throw new NullReferenceException("フォーム内にタブスイッチコントロールが存在しません。");
            }
        }

        private static TabSwitch getTabSwitchControl(Control.ControlCollection controls)
        {
            foreach (Control child in controls)
            {
                if (child is TabSwitch)
                {
                    return (TabSwitch)child;
                }
                else if (child.Controls.Count > 0)
                {
                    TabSwitch result = getTabSwitchControl(child.Controls);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private static void addFormHandler(Form form)
        {
            form.Activated += new EventHandler(form_Activated);
            form.FormClosing += new FormClosingEventHandler(form_FormClosing);
        }

        private static void removeFormHandler(Form form)
        {
            form.Activated -= new EventHandler(form_Activated);
            form.FormClosing -= new FormClosingEventHandler(form_FormClosing);
        }

        #region フォームイベント

        private static void form_Activated(object sender, EventArgs e)
        {
            if (sender != null && sender is Form)
            {
                Form form = (Form)sender;
                _formList.Remove(form);
                _formList.Insert(0, form);
            }
        }

        private static void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sender != null && sender is Form)
            {
                Form form = (Form)sender;
                _formList.Remove(form);
                removeFormHandler(form);
            }
        }

        #endregion
    }
}
