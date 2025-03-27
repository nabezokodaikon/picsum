using SWF.Core.Base;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class PageDrawArea
    {
        private static readonly SolidBrush OUTLINE_BRUSH = new(Color.FromArgb(128, 128, 128));
        private static readonly SolidBrush INNER_BRUSH = new(Color.FromArgb(250, 250, 250));
        private static readonly int TOP = 28;

        private readonly TabSwitch tabSwitch;

        public PageDrawArea(TabSwitch tabSwitch)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));

            this.tabSwitch = tabSwitch;
        }

        public void Draw(Graphics g)
        {
            var scale = AppConstants.GetCurrentWindowScale(this.tabSwitch.Handle);
            this.DrawOutline(g, scale);
            this.DrawInnerRectangle(g, scale);
        }

        private void DrawOutline(Graphics g, float scale)
        {
            var x = g.ClipBounds.X;
            var y = TOP * scale;
            var w = g.ClipBounds.Width;
            var h = 1;
            g.FillRectangle(OUTLINE_BRUSH, x, y, w, h);
        }

        private void DrawInnerRectangle(Graphics g, float scale)
        {
            var x = g.ClipBounds.X;
            var y = (TOP + 1) * scale;
            var w = g.ClipBounds.Width;
            var h = g.ClipBounds.Height - y;
            g.FillRectangle(INNER_BRUSH, x, y, w, h);
        }
    }
}
