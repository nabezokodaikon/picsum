using System.Drawing;

namespace PicSum.UIComponent.AddressBar
{
    // TODO: フォントキャッシュのDispose処理。

    internal static class Palette
    {
        public static readonly Color TEXT_COLOR = Color.FromArgb(255, 0, 0, 0);
        public static readonly Color OUTLINE_COLOR = Color.LightGray;
        public static readonly Color INNER_COLOR = Color.FromArgb(255, 255, 255, 255);

        public static readonly Color SELECTED_ITEM_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 4,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        public static readonly Color MOUSE_POINT_ITEM_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        public static readonly SolidBrush MOUSE_POINT_BRUSH = new(MOUSE_POINT_ITEM_COLOR);
        public static readonly Pen MOUSE_POINT_PEN = new(MOUSE_POINT_ITEM_COLOR);
        public static readonly SolidBrush MOUSE_DOWN_BRUSH = new(SELECTED_ITEM_COLOR);
        public static readonly SolidBrush OUT_LINE_BRUSH = new(OUTLINE_COLOR);
        public static readonly SolidBrush INNER_BRUSH = new(INNER_COLOR);
    }
}
