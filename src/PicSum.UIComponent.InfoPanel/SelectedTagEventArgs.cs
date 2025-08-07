using SWF.Core.Base;
using System;

namespace PicSum.UIComponent.InfoPanel
{
    public sealed class SelectedTagEventArgs
        : EventArgs
    {
        public PageOpenMode OpenMode { get; private set; }
        public string Tag { get; private set; }

        public SelectedTagEventArgs(PageOpenMode openMode, string tag)
        {
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            this.OpenMode = openMode;
            this.Tag = tag;
        }
    }
}
