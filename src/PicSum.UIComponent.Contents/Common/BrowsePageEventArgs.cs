using SWF.Core.Base;
using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.UIComponent.Contents.Common
{
    public sealed class BrowsePageEventArgs
        : EventArgs
    {
        public PageOpenMode OpenType { get; private set; }
        public IPageParameter Parameter { get; private set; }

        public BrowsePageEventArgs(PageOpenMode openType, IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this.OpenType = openType;
            this.Parameter = param;
        }
    }
}
