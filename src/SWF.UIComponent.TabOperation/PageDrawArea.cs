using SWF.Core.Base;
using System;
using System.Drawing;

namespace SWF.UIComponent.TabOperation
{

    public sealed class PageDrawArea
    {
        private const int TOP = 28;

        private readonly TabSwitch _tabSwitch;

        public PageDrawArea(TabSwitch tabSwitch)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));

            this._tabSwitch = tabSwitch;
        }

        public void Draw(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            var scale = WindowUtil.GetCurrentWindowScale(this._tabSwitch);
            this.DrawOutline(g, scale);
            this.DrawInnerRectangle(g, scale);
        }

        private void DrawOutline(Graphics g, float scale)
        {
            var x = g.ClipBounds.X;
            var y = TOP * scale;
            var w = g.ClipBounds.Width;
            var h = 1;
            g.FillRectangle(TabSwitchResources.PAGE_OUTLINE_BRUSH, x, y, w, h);
        }

        private void DrawInnerRectangle(Graphics g, float scale)
        {
            var x = g.ClipBounds.X;
            var y = (TOP + 1) * scale;
            var w = g.ClipBounds.Width;
            var h = g.ClipBounds.Height - y;
            g.FillRectangle(TabSwitchResources.PAGE_INNER_BRUSH, x, y, w, h);
        }
    }
}
