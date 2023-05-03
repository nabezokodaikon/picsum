using PicSum.Core.Base.Conf;
using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.UIComponent.Contents.Common
{
    public sealed class BrowserContentsEventArgs
        : EventArgs
    {
        public ContentsOpenType OpenType { get; private set; }
        public IContentsParameter Parameter { get; private set; }

        public BrowserContentsEventArgs(ContentsOpenType openType, IContentsParameter param)
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
