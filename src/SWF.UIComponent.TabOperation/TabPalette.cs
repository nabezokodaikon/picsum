using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブパレットクラス
    /// </summary>
    public class TabPalette : Component
    {
        private static Font _titleFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
        private static Color _titleColor = Color.FromArgb(0, 64, 64, 64);
        private static TextFormatFlags _titleFormatFlags = TextFormatFlags.Left |
                                                           TextFormatFlags.VerticalCenter |
                                                           TextFormatFlags.SingleLine |
                                                           TextFormatFlags.EndEllipsis;

        public Font TitleFont
        {
            get
            {
                return _titleFont;
            }
            set
            {
                _titleFont = value;
            }
        }

        public Color TitleColor
        {
            get
            {
                return _titleColor;
            }
            set
            {
                _titleColor = value;
            }
        }

        public TextFormatFlags TitleFormatFlags
        {
            get
            {
                return _titleFormatFlags;
            }
            set
            {
                _titleFormatFlags = value;
            }
        }
    }
}
