using SWF.Core.Base;
using System;

namespace PicSum.UIComponent.InfoPanel
{
    public sealed class SelectedTagEventArgs
        : EventArgs
    {
        public PageOpenMode OpenType { get; private set; }
        public string Tag { get; private set; }

        public SelectedTagEventArgs(PageOpenMode openType, string tag)
        {
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            this.OpenType = openType;
            this.Tag = tag;
        }
    }
}
