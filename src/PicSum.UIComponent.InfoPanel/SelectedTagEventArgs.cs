using PicSum.Core.Base.Conf;
using System;

namespace PicSum.UIComponent.InfoPanel
{
    public class SelectedTagEventArgs
        : EventArgs
    {
        public ContentsOpenType OpenType { get; private set; }
        public string Tag { get; private set; }

        public SelectedTagEventArgs(ContentsOpenType openType, string tag)
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
