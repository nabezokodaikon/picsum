using SkiaSharp;
using System;

namespace SWF.UIComponent.SKFlowList
{
    public sealed class SKDrawItemsEventArgs
        : EventArgs
    {
        public SKCanvas Canvas { get; private set; }
        public SKRect LocalClipBounds { get; private set; }
        public SKDrawItemInfo[] DrawItemInfos { get; private set; }

        public SKDrawItemsEventArgs(
            SKCanvas canvas,
            SKRect localClipBounds,
            SKDrawItemInfo[] drawItemInfos)
        {
            ArgumentNullException.ThrowIfNull(canvas);
            ArgumentNullException.ThrowIfNull(drawItemInfos);

            this.Canvas = canvas;
            this.LocalClipBounds = localClipBounds;
            this.DrawItemInfos = drawItemInfos;
        }
    }
}
