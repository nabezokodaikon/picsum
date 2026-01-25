using SkiaSharp.Views.Desktop;
using System;
using System.Drawing;

namespace SWF.UIComponent.FlowList
{
    public sealed class SKDrawItemsEventArgs
        : EventArgs
    {
        public SKPaintSurfaceEventArgs Args { get; private set; }
        public Rectangle ClipRectangle { get; private set; }
        public SKDrawItemEventArgs[] DrawItemEventArgs { get; private set; }

        public SKDrawItemsEventArgs(
            SKPaintSurfaceEventArgs args,
            SKDrawItemEventArgs[] drawItemEventArgs)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(drawItemEventArgs);

            this.Args = args;
            this.DrawItemEventArgs = drawItemEventArgs;
        }
    }
}
