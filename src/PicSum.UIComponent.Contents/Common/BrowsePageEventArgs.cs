using SWF.Core.Base;
using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.UIComponent.Contents.Common
{
    public sealed class BrowsePageEventArgs
        : EventArgs
    {
        public PageOpenMode OpenMode { get; private set; }
        public IPageParameter Parameter { get; private set; }

        public BrowsePageEventArgs(PageOpenMode openMode, IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this.OpenMode = openMode;
            this.Parameter = param;
        }
    }
}
