using PicSum.Core.Base.Conf;
using System;

namespace PicSum.UIComponent.InfoPanel
{
    public sealed class SelectedTagEventArgs
        : EventArgs
    {
        public PageOpenType OpenType { get; private set; }
        public string Tag { get; private set; }

        public SelectedTagEventArgs(PageOpenType openType, string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            this.OpenType = openType;
            this.Tag = tag;
        }
    }
}
