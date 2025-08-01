using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ItemMouseClickEventArgs(
        MouseButtons button, int clicks, int x, int y, int delta, string item)
        : MouseEventArgs(button, clicks, x, y, delta)
    {
        public string Item { get; private set; }
            = item ?? throw new ArgumentNullException(nameof(item));
    }
}
