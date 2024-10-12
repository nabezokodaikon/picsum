using SWF.Core.ImageAccessor;
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
        public RectangleF TextRectangle { get; set; }
        public RectangleF IconRectangle { get; set; }
        public RectangleF CloseButtonRectangle { get; set; }
        public DrawTextUtil.TextStyle TextStyle { get; set; }
    }
}
