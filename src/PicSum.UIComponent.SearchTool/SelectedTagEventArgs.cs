using System;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.SearchTool
{
    public class SelectedTagEventArgs : SelectedItemEventArgs<string>
    {
        public SelectedTagEventArgs(ContentsOpenType openType, string tag)
            : base(openType, tag) { }
    }
}
