using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows")]
    public sealed class ItemMouseClickEventArgs
        : MouseEventArgs
    {
        public string Item { get; private set; }

        public ItemMouseClickEventArgs(
            MouseButtons button, int clicks, int x, int y, int delta, string item)
            : base(button, clicks, x, y, delta)
        {
            this.Item = item ?? throw new ArgumentNullException(nameof(item));
        }
    }
}
