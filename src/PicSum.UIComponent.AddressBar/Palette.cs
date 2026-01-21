using System.Drawing;

namespace PicSum.UIComponent.AddressBar
{
    // TODO: フォントキャッシュのDispose処理。

    internal static class Palette
    {
        public static readonly Color OUTLINE_COLOR = Color.LightGray;
        public static readonly Color INNER_COLOR = Color.FromArgb(255, 255, 255, 255);

        public static readonly SolidBrush OUT_LINE_BRUSH = new(OUTLINE_COLOR);
        public static readonly SolidBrush INNER_BRUSH = new(INNER_COLOR);
    }
}
