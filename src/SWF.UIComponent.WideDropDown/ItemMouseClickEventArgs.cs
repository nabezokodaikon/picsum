using System;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{

    public sealed class ItemMouseClickEventArgs(
        MouseButtons button, int clicks, int x, int y, int delta, string item)
        : MouseEventArgs(button, clicks, x, y, delta)
    {
        public string Item { get; private set; }
            = item ?? throw new ArgumentNullException(nameof(item));
    }
}
