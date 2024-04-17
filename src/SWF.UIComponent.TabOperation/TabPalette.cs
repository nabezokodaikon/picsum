using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブパレットクラス
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class TabPalette
        : Component
    {
        private static Font titleFont = new("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
        private static Color titleColor = Color.FromArgb(0, 64, 64, 64);
        private static TextFormatFlags titleFormatFlags = TextFormatFlags.Left |
                                                          TextFormatFlags.VerticalCenter |
                                                          TextFormatFlags.SingleLine |
                                                          TextFormatFlags.EndEllipsis;

        public Font TitleFont
        {
            get
            {
                return titleFont;
            }
            set
            {
                titleFont = value;
            }
        }

        public Color TitleColor
        {
            get
            {
                return titleColor;
            }
            set
            {
                titleColor = value;
            }
        }

        public TextFormatFlags TitleFormatFlags
        {
            get
            {
                return titleFormatFlags;
            }
            set
            {
                titleFormatFlags = value;
            }
        }
    }
}
