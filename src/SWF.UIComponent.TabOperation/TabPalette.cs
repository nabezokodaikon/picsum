using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブパレットクラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class TabPalette
        : Component
    {
        private static readonly Font TITLE_FONT = new("Yu Gothic UI", 10F);
        private static readonly Color TITLE_COLOR = Color.FromArgb(0, 64, 64, 64);
        private static readonly TextFormatFlags TITLE_FORMAT_FLAGS
            = TextFormatFlags.Left |
              TextFormatFlags.VerticalCenter |
              TextFormatFlags.SingleLine |
              TextFormatFlags.EndEllipsis;

        public Font TitleFont
        {
            get
            {
                return TITLE_FONT;
            }
        }

        public Color TitleColor
        {
            get
            {
                return TITLE_COLOR;
            }
        }

        public TextFormatFlags TitleFormatFlags
        {
            get
            {
                return TITLE_FORMAT_FLAGS;
            }
        }
    }
}
