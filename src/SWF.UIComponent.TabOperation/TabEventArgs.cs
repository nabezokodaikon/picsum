using System;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブイベントクラス
    /// </summary>
    public class TabEventArgs
        : EventArgs
    {
        public TabInfo Tab { get; private set; }

        public TabEventArgs(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException(nameof(tab));
            }

            this.Tab = tab;
        }
    }
}
