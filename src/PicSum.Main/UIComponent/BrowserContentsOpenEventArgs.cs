using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.UIComponent.Contents;
using SWF.UIComponent.TabOperation;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ブラウザコンテンツ表示イベントクラス
    /// </summary>
    public class BrowserContentsOpenEventArgs : EventArgs
    {
        private IContentsParameter _contentsParameter;

        public IContentsParameter ContentsParameter
        {
            get
            {
                return _contentsParameter;
            }
        }

        public BrowserContentsOpenEventArgs(IContentsParameter contentsParameter)
        {
            if (contentsParameter == null)
            {
                throw new ArgumentNullException("contentsParameter");
            }

            _contentsParameter = contentsParameter;
        }
    }
}
