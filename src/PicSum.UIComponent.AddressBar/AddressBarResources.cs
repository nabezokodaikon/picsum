using System.Drawing;

namespace PicSum.UIComponent.AddressBar
{
    public static class AddressBarResources
    {
        public static readonly Color OUTLINE_COLOR = Color.LightGray;
        public static readonly Color INNER_COLOR = Color.FromArgb(255, 255, 255, 255);

        public static readonly SolidBrush OUT_LINE_BRUSH = new(OUTLINE_COLOR);
        public static readonly SolidBrush INNER_BRUSH = new(INNER_COLOR);

        public static void Dispose()
        {
            OUT_LINE_BRUSH.Dispose();
            INNER_BRUSH.Dispose();
        }
    }
}
