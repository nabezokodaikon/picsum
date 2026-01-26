using System.Collections.Concurrent;

namespace SWF.Core.ResourceAccessor
{
    public static class FontCacher
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
        private static readonly ConcurrentDictionary<float, Font> REGULAR_FONT_CACHE = [];
        private static readonly ConcurrentDictionary<float, Font> BOLD_FONT_CACHE = [];

        public static Font GetRegularFont(Size srcSize, float scale)
        {
            var size = ToSize(srcSize) * scale;

            if (REGULAR_FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            return REGULAR_FONT_CACHE.GetOrAdd(size, key =>
            {
                return new Font(
                    FONT_FAMILY,
                    size,
                    FontStyle.Regular,
                    UNIT);
            });
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

            return BOLD_FONT_CACHE.GetOrAdd(size, key =>
            {
                return new Font(
                    FONT_FAMILY,
                    size,
                    FontStyle.Bold,
                    UNIT);
            });
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
                _ => throw new NotSupportedException("未定義のフォントサイズです。"),
            };
        }
    }
}
