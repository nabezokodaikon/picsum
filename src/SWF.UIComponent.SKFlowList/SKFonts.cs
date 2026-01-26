using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SWF.UIComponent.SKFlowList
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

        //private const string FONT_FAMILY = "Yu Gothic UI";
        private const string FONT_FAMILY = "Microsoft YaHei";
        private const string FONT_RESOURCE_NAME
            = "SWF.UIComponent.SKFlowList.Fonts.NotoSansCJKjp-Regular.otf";

        private static readonly Dictionary<float, SKFont> FONT_CACHE = [];

        public static SKFont GetFont(Size srcSize, float scale)
        {
            var size = ToSize(srcSize) * scale;

            if (FONT_CACHE.TryGetValue(size, out var font))
            {
                return font;
            }

            var typeface = SKTypeface.FromFamilyName(FONT_FAMILY, SKFontStyle.Normal);
            if (typeface == null)
            {
                typeface = SKTypeface.FromFamilyName("Yu Gothic UI", SKFontStyle.Normal);
            }

            //var typeface = LoadFontFromResource(FONT_RESOURCE_NAME);

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
