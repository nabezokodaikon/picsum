using SkiaSharp;
using System.Drawing;

namespace PicSum.UIComponent.AddressBar
{
    public static class AddressBarResources
    {
        public static readonly Color OUTLINE_COLOR = Color.LightGray;
        public static readonly Color INNER_COLOR = Color.FromArgb(255, 255, 255, 255);
        public static readonly Color ITEM_TEXT_COLOR = Color.FromArgb(255, 0, 0, 0);

        public static readonly Color MOUSE_POINT_ITEM_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        public static readonly SolidBrush OUT_LINE_BRUSH = new(OUTLINE_COLOR);
        public static readonly SolidBrush INNER_BRUSH = new(INNER_COLOR);
        public static readonly SolidBrush MOUSE_POINT_ITEM_BRUSH = new(MOUSE_POINT_ITEM_COLOR);

        public static readonly SKPaint ICON_PAINT = new()
        {
            IsAntialias = false,
            BlendMode = SKBlendMode.SrcOver,
        };

        public static void Dispose()
        {
            OUT_LINE_BRUSH.Dispose();
            INNER_BRUSH.Dispose();
            MOUSE_POINT_ITEM_BRUSH.Dispose();
            ICON_PAINT.Dispose();
        }
    }
}
