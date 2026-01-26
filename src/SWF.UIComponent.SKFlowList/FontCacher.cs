using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SWF.UIComponent.SKFlowList
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

        private const string FONT_RESOURCE_NAME
            = "SWF.UIComponent.SKFlowList.Fonts.NotoSansCJKjp-Regular.otf";
        private const string FONT_FAMILY = "Yu Gothic UI";
        //private const string SK_FONT_FAMILY = "Yu Gothic UI";
        private const string SK_FONT_FAMILY = "Microsoft YaHei";
        private const GraphicsUnit UNIT = GraphicsUnit.Pixel;

        private static readonly PrivateFontCollection PRIVATE_FONT_COLLECTION = new();
        private static readonly ConcurrentDictionary<float, Font> REGULAR_FONT_CACHE = [];
        private static readonly ConcurrentDictionary<float, Font> BOLD_FONT_CACHE = [];
        private static readonly ConcurrentDictionary<float, SKFont> REGULAR_SK_FONT_CACHE = [];
        private static readonly ConcurrentDictionary<float, SKFont> BOLD_SK_FONT_CACHE = [];

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
                //return LoadFontFromResource(
                //    FONT_RESOURCE_NAME,
                //    size,
                //    FontStyle.Regular,
                //    UNIT);
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
                ////return LoadFontFromResource(
                ////    FONT_RESOURCE_NAME,
                ////    size,
                ////    FontStyle.Bold,
                ////    UNIT);
            });
        }

        public static Font GetBoldFont(Size srcSize)
        {
            return GetBoldFont(srcSize, 1f);
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
                //var typeface = LoadSKFontFromResource(FONT_RESOURCE_NAME);
                var typeface = SKTypeface.FromFamilyName(SK_FONT_FAMILY, SKFontStyle.Normal);
                if (typeface == null)
                {
                    typeface = SKTypeface.FromFamilyName("Yu Gothic UI", SKFontStyle.Normal);
                }

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
                //var typeface = LoadSKFontFromResource(FONT_RESOURCE_NAME);
                var typeface = SKTypeface.FromFamilyName(SK_FONT_FAMILY, SKFontStyle.Bold);
                if (typeface == null)
                {
                    typeface = SKTypeface.FromFamilyName("Yu Gothic UI", SKFontStyle.Bold);
                }

                return new SKFont
                {
                    Typeface = typeface,
                    Size = size,
                    Edging = SKFontEdging.Antialias,
                    Hinting = SKFontHinting.Normal,
                };
            });
        }

        private static Font LoadFontFromResource(
             string resourceName,
             float size,
             FontStyle style,
             GraphicsUnit unit)
        {
            if (PRIVATE_FONT_COLLECTION.Families.Length == 0)
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    throw new InvalidOperationException("Resource not found");
                }

                var fontData = new byte[stream.Length];
                _ = stream.Read(fontData, 0, (int)stream.Length);

                var fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

                PRIVATE_FONT_COLLECTION.AddMemoryFont(fontPtr, fontData.Length);

                Marshal.FreeCoTaskMem(fontPtr);
            }

            return new Font(PRIVATE_FONT_COLLECTION.Families[0], size, style, unit);
        }

        private static SKTypeface LoadSKFontFromResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
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
