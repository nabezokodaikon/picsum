using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    // TODO: フォントキャッシュのDispose処理。
    /// <summary>
    /// タブパレットクラス
    /// </summary>

    public static class TabPalette
    {
        public static readonly Color TITLE_COLOR = Color.FromArgb(0, 64, 64, 64);
        public static readonly TextFormatFlags TITLE_FORMAT_FLAGS
            = TextFormatFlags.Left |
              TextFormatFlags.VerticalCenter |
              TextFormatFlags.SingleLine |
              TextFormatFlags.EndEllipsis;
    }
}
