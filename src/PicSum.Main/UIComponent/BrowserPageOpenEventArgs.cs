using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ブラウザコンテンツ表示イベントクラス
    /// </summary>
    public sealed class BrowserPageOpenEventArgs
        : EventArgs
    {
        public IPageParameter PageParameter { get; private set; }

        public BrowserPageOpenEventArgs(IPageParameter pageParameter)
        {
            if (pageParameter == null)
            {
                throw new ArgumentNullException(nameof(pageParameter));
            }

            this.PageParameter = pageParameter;
        }
    }
}
