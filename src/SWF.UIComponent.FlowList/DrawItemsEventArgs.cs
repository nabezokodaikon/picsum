using System;
using System.Drawing;

namespace SWF.UIComponent.FlowList
{
    public sealed class DrawItemsEventArgs
        : EventArgs
    {
        public Graphics Graphics { get; private set; }
        public DrawItemEventArgs[] DrawItemEventArgs { get; private set; }

        public DrawItemsEventArgs(Graphics graphics, DrawItemEventArgs[] drawItemEventArgs)
        {
            ArgumentNullException.ThrowIfNull(graphics);
            ArgumentNullException.ThrowIfNull(drawItemEventArgs);

            this.Graphics = graphics;
            this.DrawItemEventArgs = drawItemEventArgs;
        }
    }
}
