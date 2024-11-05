using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class PageDrawArea
    {
        private static readonly SolidBrush OUTLINE_BRUSH = new(Color.FromArgb(128, 128, 128));
        private static readonly SolidBrush INNER_BRUSH = new(Color.FromArgb(241, 244, 250));
        private static readonly int TOP = 28;

        public Color OutlineColor
        {
            get
            {
                return OUTLINE_BRUSH.Color;
            }
        }

        public SolidBrush OutlineBrush
        {
            get
            {
                return OUTLINE_BRUSH;
            }
        }

        public Color PageColor
        {
            get
            {
                return INNER_BRUSH.Color;
            }
        }

        public SolidBrush PageBrush
        {
            get
            {
                return INNER_BRUSH;
            }
        }

        public void Draw(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(nameof(g));

            this.DrawOutline(g);
            this.DrawInnerRectangle(g);
        }

        private void DrawOutline(Graphics g)
        {
            var x = g.ClipBounds.X;
            var y = TOP;
            var w = g.ClipBounds.Width;
            var h = 1;
            g.FillRectangle(OUTLINE_BRUSH, x, y, w, h);
        }

        private void DrawInnerRectangle(Graphics g)
        {
            var x = g.ClipBounds.X;
            var y = TOP + 1;
            var w = g.ClipBounds.Width;
            var h = g.ClipBounds.Height - y;
            g.FillRectangle(INNER_BRUSH, x, y, w, h);
        }
    }
}
