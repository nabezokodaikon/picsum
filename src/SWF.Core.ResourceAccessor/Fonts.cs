using System.Collections.Concurrent;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SWF.Core.ResourceAccessor
{
    public static class Fonts
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

        private const string FONT_FAMILY = "Yu Gothic UI";
        private const GraphicsUnit UNIT = GraphicsUnit.Pixel;
        private static readonly PrivateFontCollection PRIVATE_FONT_COLLECTION = new();
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
                //return LoadFontFromResource(
                //    FONT_RESOURCE_NAME,
                //    size,
                //    FontStyle.Regular,
                //    UNIT);
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
                //return LoadFontFromResource(
                //    FONT_RESOURCE_NAME,
                //    size,
                //    FontStyle.Bold,
                //    UNIT);
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

        private static Font LoadFontFromResource(
            string resourceName,
            float size,
            FontStyle style,
            GraphicsUnit unit)
        {
            // 1. すでに読み込み済みか確認（同じフォントを二度読み込まない工夫）
            // ※簡易化のため、初回のみ読み込むロジック
            if (PRIVATE_FONT_COLLECTION.Families.Length == 0)
            {
                var assembly = Assembly.GetExecutingAssembly();
                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    throw new InvalidOperationException("Resource not found");
                }

                // 2. ストリームをバイト配列に変換
                var fontData = new byte[stream.Length];
                _ = stream.Read(fontData, 0, (int)stream.Length);

                // 3. メモリ（アンマネージド領域）を確保してコピー
                var fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

                // 4. コレクションに追加
                PRIVATE_FONT_COLLECTION.AddMemoryFont(fontPtr, fontData.Length);

                // 5. メモリの解放（AddMemoryFont後は解放してOK）
                Marshal.FreeCoTaskMem(fontPtr);
            }

            // 6. Fontオブジェクトの生成
            return new Font(PRIVATE_FONT_COLLECTION.Families[0], size, style, unit);
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
