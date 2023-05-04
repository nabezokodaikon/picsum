using SWF.Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    public sealed class DrawTabEventArgs
        : EventArgs
    {
        public Graphics Graphics { get; set; }
        public Font Font { get; set; }
        public Color TitleColor { get; set; }
        public TextFormatFlags TitleFormatFlags { get; set; }
        public Rectangle TextRectangle { get; set; }
        public Rectangle IconRectangle { get; set; }
        public Rectangle CloseButtonRectangle { get; set; }
        public DrawTextUtil.TextStyle TextStyle { get; set; }
    }
}
