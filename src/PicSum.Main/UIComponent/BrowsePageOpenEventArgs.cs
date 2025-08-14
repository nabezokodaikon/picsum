using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ブラウザコンテンツ表示イベントクラス
    /// </summary>
    internal sealed class BrowsePageOpenEventArgs
        : EventArgs
    {
        public IPageParameter PageParameter { get; private set; }

        public BrowsePageOpenEventArgs(IPageParameter pageParameter)
        {
            ArgumentNullException.ThrowIfNull(pageParameter, nameof(pageParameter));

            this.PageParameter = pageParameter;
        }
    }
}
