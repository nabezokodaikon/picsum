using System;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブイベントクラス
    /// </summary>
    public class TabEventArgs : EventArgs
    {
        private TabInfo _tab = null;

        public TabInfo Tab
        {
            get
            {
                return _tab;
            }
        }

        public TabEventArgs(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            _tab = tab;
        }
    }
}
