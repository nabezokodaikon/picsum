using System.Runtime.Versioning;

namespace SWF.Core.ResourceAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class Fonts
    {
        private const string FONT_FAMILY = "Yu Gothic UI";

        public static readonly Font UI_FONT_09 = new(FONT_FAMILY, 9F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_10 = new(FONT_FAMILY, 10F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_11 = new(FONT_FAMILY, 11F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_12 = new(FONT_FAMILY, 12F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_13 = new(FONT_FAMILY, 13F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_14 = new(FONT_FAMILY, 14F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_14_BOLD = new(FONT_FAMILY, 14F, FontStyle.Bold, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_18 = new(FONT_FAMILY, 18F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_22 = new(FONT_FAMILY, 22F, FontStyle.Regular, GraphicsUnit.Pixel);

        public static readonly Dictionary<float, Font> REGULAR_FONT_CACHE = [];
        public static readonly Dictionary<float, Font> BOLD_FONT_CACHE = [];

        public static Font GetRegularFont(Font srcFont, float scale)
        {
            var size = srcFont.Size * scale;

            if (REGULAR_FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            var newFont = new Font(
                srcFont.FontFamily,
                size,
                FontStyle.Regular,
                srcFont.Unit);
            REGULAR_FONT_CACHE.Add(size, newFont);
            return newFont;
        }

        public static Font GetBoldFont(Font srcFont, float scale)
        {
            var size = srcFont.Size * scale;

            if (BOLD_FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            var newFont = new Font(
                srcFont.FontFamily,
                size,
                FontStyle.Bold,
                srcFont.Unit);
            BOLD_FONT_CACHE.Add(size, newFont);
            return newFont;
        }
    }
}
