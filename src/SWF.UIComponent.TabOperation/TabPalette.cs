using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    // TODO: フォントキャッシュのDispose処理。
    /// <summary>
    /// タブパレットクラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class TabPalette
    {
        private static readonly Font TITLE_FONT = new("Yu Gothic UI", 10F);
        private static readonly Dictionary<float, Font> FONT_CACHE = new();

        public static readonly Color TITLE_COLOR = Color.FromArgb(0, 64, 64, 64);
        public static readonly TextFormatFlags TITLE_FORMAT_FLAGS
            = TextFormatFlags.Left |
              TextFormatFlags.VerticalCenter |
              TextFormatFlags.SingleLine |
              TextFormatFlags.EndEllipsis;

        public static Font GetFont(float scale)
        {
            if (FONT_CACHE.TryGetValue(scale, out var font))
            {
                return font;
            }

            var newFont = new Font(TITLE_FONT.FontFamily, TITLE_FONT.Size * scale);
            FONT_CACHE.Add(scale, newFont);
            return newFont;
        }
    }
}
