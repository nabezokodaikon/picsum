using SkiaSharp;
using System;
using System.Drawing;

namespace SWF.UIComponent.SKFlowList
{
    public sealed class SKDrawItemsEventArgs
        : EventArgs
    {
        public SKCanvas Canvas { get; private set; }
        public Rectangle ClipRectangle { get; private set; }
        public SKDrawItemInfo[] DrawItemInfos { get; private set; }

        public SKDrawItemsEventArgs(
            SKCanvas canvas,
            SKDrawItemInfo[] drawItemInfos)
        {
            ArgumentNullException.ThrowIfNull(canvas);
            ArgumentNullException.ThrowIfNull(drawItemInfos);

            this.Canvas = canvas;
            this.DrawItemInfos = drawItemInfos;
        }
    }
}
