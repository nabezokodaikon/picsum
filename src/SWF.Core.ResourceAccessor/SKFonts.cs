using SkiaSharp;
using System.Reflection;

namespace SWF.Core.ResourceAccessor
{
    public static class SKFonts
    {
        public enum Size
        {
            Small,
            Medium,
            Large,
            ExtraLarge,
        }

        private const string FONT_RESOURCE_NAME
            = "SWF.Core.ResourceAccessor.Fonts.NotoSansCJKjp-Regular.otf";

        public static readonly Dictionary<float, SKFont> FONT_CACHE = [];

        public static SKFont GetFont(Size srcSize, float scale)
        {
            var size = ToSize(srcSize) * scale;

            if (FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            //var typeface = LoadFontFromResource(FONT_RESOURCE_NAME);
            var typeface = SKTypeface.FromFamilyName("Microsoft YaHei", SKFontStyle.Normal);
            if (typeface.FamilyName != "Microsoft YaHei")
            {
                typeface = SKTypeface.FromFamilyName("Yu Gothic UI", SKFontStyle.Normal);
            }

            var newFont = new SKFont
            {
                Typeface = typeface,
                Size = size,
                Edging = SKFontEdging.Antialias,
                Hinting = SKFontHinting.Normal,
            };

            FONT_CACHE.Add(size, newFont);
            return newFont;
        }

        private static SKTypeface LoadFontFromResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new InvalidOperationException("Resource not found");
            }

            return SKTypeface.FromStream(stream);
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
