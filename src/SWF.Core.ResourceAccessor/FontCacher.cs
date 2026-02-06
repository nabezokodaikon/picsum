using SkiaSharp;
using System.Collections.Concurrent;
using Windows.Globalization;
using Windows.Globalization.Fonts;

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

        private const GraphicsUnit UNIT = GraphicsUnit.Pixel;

        private static readonly Lazy<string> FONT_FAMILY = new(
            static () => GetFotFamily(), LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly ConcurrentDictionary<float, Font> REGULAR_GDI_FONT_CACHE = [];
        private static readonly ConcurrentDictionary<float, Font> BOLD_GDI_FONT_CACHE = [];
        private static readonly ConcurrentDictionary<float, SKFont> REGULAR_SK_FONT_CACHE = [];
        private static readonly ConcurrentDictionary<float, SKFont> BOLD_SK_FONT_CACHE = [];

        public static Font GetRegularGdiFont(Size srcSize, float scale)
        {
            var size = ToSize(srcSize) * scale;

            if (REGULAR_GDI_FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            return REGULAR_GDI_FONT_CACHE.GetOrAdd(size, key =>
            {
                return new Font(
                    FONT_FAMILY.Value,
                    size,
                    FontStyle.Regular,
                    UNIT);
            });
        }

        public static Font GetRegularGdiFont(Size srcSize)
        {
            return GetRegularGdiFont(srcSize, 1f);
        }

        public static Font GetBoldGdiFont(Size srcSize, float scale)
        {
            var size = ToSize(srcSize) * scale;

            if (BOLD_GDI_FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            return BOLD_GDI_FONT_CACHE.GetOrAdd(size, key =>
            {
                return new Font(
                    FONT_FAMILY.Value,
                    size,
                    FontStyle.Bold,
                    UNIT);
            });
        }

        public static Font GetBoldGdiFont(Size srcSize)
        {
            return GetBoldGdiFont(srcSize, 1f);
        }

        public static SKFont GetRegularSKFont(Size srcSize, float scale)
        {
            var size = ToSize(srcSize) * scale;

            if (REGULAR_SK_FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            return REGULAR_SK_FONT_CACHE.GetOrAdd(size, key =>
            {
                var typeface = SKTypeface.FromFamilyName(FONT_FAMILY.Value, SKFontStyle.Normal);

                return new SKFont
                {
                    Typeface = typeface,
                    Size = size,
                    Edging = SKFontEdging.Antialias,
                    Hinting = SKFontHinting.Normal,
                };
            });
        }

        public static SKFont GetBoldSKFont(Size srcSize, float scale)
        {
            var size = ToSize(srcSize) * scale;

            if (BOLD_SK_FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            return BOLD_SK_FONT_CACHE.GetOrAdd(size, key =>
            {
                var typeface = SKTypeface.FromFamilyName(FONT_FAMILY.Value, SKFontStyle.Bold);

                return new SKFont
                {
                    Typeface = typeface,
                    Size = size,
                    Edging = SKFontEdging.Antialias,
                    Hinting = SKFontHinting.Normal,
                };
            });
        }

        private static string GetFotFamily()
        {
            var languages = ApplicationLanguages.Languages.ToArray();
            if (languages.Length < 1)
            {
                throw new InvalidOperationException("カルチャー名が取得できませんでした。");
            }

            var languageTag = ApplicationLanguages.Languages[0];
            var languageFontGroup = new LanguageFontGroup(languageTag);
            return languageFontGroup.UITextFont.FontFamily;
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
