﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using SWF.Common;

namespace SWF.UIComponent.TabOperation
{
    public class DrawTabEventArgs : EventArgs
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
