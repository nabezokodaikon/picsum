using PicSum.Core.Base.Conf;
using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.UIComponent.Contents.Common
{
    public sealed class BrowserPageEventArgs
        : EventArgs
    {
        public PageOpenType OpenType { get; private set; }
        public IPageParameter Parameter { get; private set; }

        public BrowserPageEventArgs(PageOpenType openType, IPageParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            this.OpenType = openType;
            this.Parameter = param;
        }
    }
}
