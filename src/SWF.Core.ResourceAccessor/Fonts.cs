using System.Runtime.Versioning;

namespace SWF.Core.ResourceAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class Fonts
    {
        public enum Size
        {
            Small,
            Medium,
            Large,
            ExtraLarge,
        }

        private const string FONT_FAMILY = "Yu Gothic UI";
        private const GraphicsUnit UNIT = GraphicsUnit.Pixel;

        public static readonly Dictionary<float, Font> REGULAR_FONT_CACHE = [];
        public static readonly Dictionary<float, Font> BOLD_FONT_CACHE = [];

        public static Font GetRegularFont(Size srcSize, float scale)
        {
            var size = ToSize(srcSize) * scale;

            if (REGULAR_FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            var newFont = new Font(
                FONT_FAMILY,
                size,
                FontStyle.Regular,
                UNIT);
            REGULAR_FONT_CACHE.Add(size, newFont);
            return newFont;
        }

        public static Font GetRegularFont(Size srcSize)
        {
            return GetRegularFont(srcSize, 1f);
        }

        public static Font GetBoldFont(Size srcSize, float scale)
        {
            var size = ToSize(srcSize) * scale;

            if (BOLD_FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            var newFont = new Font(
                FONT_FAMILY,
                size,
                FontStyle.Bold,
                UNIT);
            BOLD_FONT_CACHE.Add(size, newFont);
            return newFont;
        }

        public static Font GetBoldFont(Size srcSize)
        {
            return GetBoldFont(srcSize, 1f);
        }

        private static int ToSize(Size size)
        {
            return size switch
            {
                Size.Small => 12,
                Size.Medium => 14,
                Size.Large => 18,
                Size.ExtraLarge => 22,
                _ => throw new InvalidOperationException("未定義のフォントサイズです。"),
            };
        }
    }
}
