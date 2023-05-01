using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ブラウザコンテンツ表示イベントクラス
    /// </summary>
    public sealed class BrowserContentsOpenEventArgs
        : EventArgs
    {
        public IContentsParameter ContentsParameter { get; private set; }

        public BrowserContentsOpenEventArgs(IContentsParameter contentsParameter)
        {
            if (contentsParameter == null)
            {
                throw new ArgumentNullException(nameof(contentsParameter));
            }

            this.ContentsParameter = contentsParameter;
        }
    }
}
