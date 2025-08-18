using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ描画領域
    /// </summary>

    internal sealed class TabDrawArea
    {
        private static readonly SolidBrush TAB_CLOSE_ACTIVE_BUTTON_BRUSH
            = new(Color.FromArgb(64, 0, 0, 0));

        private static readonly SolidBrush TAB_CLOSE_INACTIVE_BUTTON_BRUSH
            = new(Color.FromArgb(64, 0, 0, 0));

        private static readonly Pen TAB_CLOSE_BUTTON_SLASH_PEN
            = new(Color.Black, 2f);

        private static readonly SolidBrush ACTIVE_TAB_BRUSH
            = new(Color.FromArgb(250, 250, 250));

        private static readonly SolidBrush INACTIVE_TAB_BRUSH
            = new(Color.FromArgb(200, 200, 200));

        private static readonly SolidBrush MOUSE_POINT_TAB_BRUSH
            = new(Color.FromArgb(220, 220, 220));

        private static readonly Pen TAB_OUTLINE_PEN
            = new(Color.FromArgb(32, 32, 32), 0.1f);

        private PointF _drawPoint = new(0, 0);
        private float _width = 256;
        private readonly TabDrawAreaParameter _parameter;

        public float X
        {
            get
            {
                return this._drawPoint.X;
            }
            set
            {
                this._drawPoint.X = value;
            }
        }

        public float Y
        {
            get
            {
                return this._drawPoint.Y;
            }
            set
            {
                this._drawPoint.Y = value;
            }
        }

        public float Left
        {
            get
            {
                return this._drawPoint.X;
            }
            set
            {
                this._drawPoint.X = value;
            }
        }

        public float Top
        {
            get
            {
                return this._drawPoint.Y;
            }
            set
            {
                this._drawPoint.Y = value;
            }
        }

        public float Right
        {
            get
            {
                return this._drawPoint.X + this._width;
            }
            set
            {
                this._drawPoint.X = value - this._width;
            }
        }

        public float Bottom
        {
            get
            {
                return this._drawPoint.Y + this._parameter.Height;
            }
            set
            {
                this._drawPoint.Y = value - this._parameter.Height;
            }
        }

        public float Width
        {
            get
            {
                return this._width;
            }
            set
            {
                this._width = value;
            }
        }

        public float Height
        {
            get
            {
                return this._parameter.Height;
            }
        }

        public TabDrawArea(Control owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            this._parameter = new(owner);
        }

        public void DrawActiveTab(Graphics g, float scale)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this._parameter.Update(scale);
            this.DrawTab(ACTIVE_TAB_BRUSH, g, scale, true);
        }

        public void DrawActiveTab(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this._parameter.Update(tabSwitch);
            this.DrawTab(ACTIVE_TAB_BRUSH, g, false);
        }

        public void DrawInactiveTab(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));

            this._parameter.Update(tabSwitch);
            this.DrawTab(INACTIVE_TAB_BRUSH, g, false);
        }

        public void DrawMousePointTab(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this._parameter.Update(tabSwitch);
            this.DrawTab(MOUSE_POINT_TAB_BRUSH, g, false);
        }

        public void DrawNothingTabCloseButton(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
        }

        public void DrawActiveMousePointTabCloseButton(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this._parameter.Update(tabSwitch);
            this.DrawTabCloseButton(g, true, true);
        }

        public void DrawInactiveMousePointTabCloseButton(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this._parameter.Update(tabSwitch);
            this.DrawTabCloseButton(g, true, false);
        }

        public void DrawInactiveTabCloseButton(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this._parameter.Update(tabSwitch);
            this.DrawTabCloseButton(g, false, false);
        }

        public void DrawActiveTabCloseButton(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this._parameter.Update(tabSwitch);
            this.DrawTabCloseButton(g, false, true);
        }

        public bool Contains(int x, int y)
        {
            if (x < this.Left || this.Right < x || y < this.Top || this.Bottom < y)
            {
                return false;
            }

            return true;
        }

        public bool Contains(Point p)
        {
            return this.Contains(p.X, p.Y);
        }

        public RectangleF GetRectangle()
        {
            var x = this._drawPoint.X;
            var y = this._drawPoint.Y;
            var w = this._width;
            var h = this._parameter.Height;
            return new RectangleF(x, y, w, h);
        }

        public RectangleF GetIconRectangle(float scale)
        {
            var margin = 8 * scale;
            var rect = this.GetIconRectangle();
            var w = rect.Width - margin;
            var h = rect.Height - margin;
            var x = rect.X + (rect.Width - w) / 2f;
            var y = rect.Y + (rect.Height - h) / 2f;
            return new RectangleF(x, y, w, h);
        }

        public RectangleF GetCloseButtonRectangle()
        {
            var w = this._parameter.CloseButtonRectangle.Width;
            var h = this._parameter.CloseButtonRectangle.Height;

            if (this._width < TabSwitch.GetTabCloseButtonCanDrawWidth(this._parameter.GetOwner()))
            {
                var x = this.X + (this.Width - this._parameter.CloseButtonRectangle.Width) / 2f;
                var y = this._parameter.CloseButtonRectangle.Y + (this._parameter.CloseButtonRectangle.Height - h) / 2f;
                return new RectangleF(x, y, w, h);
            }
            else
            {
                var x = this._parameter.CloseButtonRectangle.X - (this._parameter.TabWidth - this._width) + this._drawPoint.X;
                var y = this._parameter.CloseButtonRectangle.Y + this._drawPoint.Y;
                return new RectangleF(x, y, w, h);
            }
        }

        public RectangleF GetPageRectangle()
        {
            var x = this._parameter.IconRectangle.Right + this._parameter.PageOffset + this._drawPoint.X;
            var y = this._parameter.IconRectangle.Y + this._drawPoint.Y;
            return RectangleF.FromLTRB(
                x,
                y,
                this._parameter.CloseButtonRectangle.X - (this._parameter.TabWidth - this._width) + this._drawPoint.X - this._parameter.PageOffset,
                y + this._parameter.IconRectangle.Height);
        }

        private RectangleF GetIconRectangle()
        {
            var x = this._parameter.IconRectangle.X + this._drawPoint.X;
            var y = this._parameter.IconRectangle.Y + this._drawPoint.Y;
            var w = this._parameter.IconRectangle.Width;
            var h = this._parameter.IconRectangle.Height;
            return new RectangleF(x, y, w, h);
        }

        private void DrawTab(SolidBrush brush, Graphics g, bool isDrawOutline)
        {
            this.DrawTab(brush, g, 1f, isDrawOutline);
        }

        private void DrawTab(SolidBrush brush, Graphics g, float scale, bool isDrawOutline)
        {
            var destRect = this.GetDestCenterRectangle(scale);

            g.FillRectangle(brush, destRect);

            if (isDrawOutline)
            {
                g.DrawLines(TAB_OUTLINE_PEN, [
                    new PointF(destRect.Left, destRect.Bottom - 1f),
                new PointF(destRect.Left, destRect.Top + 0.5f),
                new PointF(destRect.Right, destRect.Top + 0.5f),
                new PointF(destRect.Right, destRect.Bottom - 1f)]);
            }
        }

        private void DrawTabCloseButton(Graphics g, bool isMousePoint, bool isActiveTab)
        {
            float OFFSET = 8 * this._parameter.Scale;

            var rect = this.GetCloseButtonRectangle();
            var bgRect = new RectangleF(rect.Left + OFFSET / 2f, rect.Top + OFFSET / 2f, rect.Width - OFFSET, rect.Height - OFFSET);
            var slashP1 = new PointF(rect.Left + OFFSET, rect.Top + OFFSET);
            var backSlashP1 = new PointF(rect.Right - OFFSET, rect.Bottom - OFFSET);
            var slashP2 = new PointF(rect.Right - OFFSET, rect.Top + OFFSET);
            var backSlashP2 = new PointF(rect.Left + OFFSET, rect.Bottom - OFFSET);

            if (isMousePoint)
            {
                if (isActiveTab)
                {
                    g.FillEllipse(TAB_CLOSE_ACTIVE_BUTTON_BRUSH, bgRect);
                }
                else
                {
                    g.FillEllipse(TAB_CLOSE_INACTIVE_BUTTON_BRUSH, bgRect);
                }
            }

            if (isActiveTab)
            {
                g.DrawLine(TAB_CLOSE_BUTTON_SLASH_PEN, slashP1, backSlashP1);
                g.DrawLine(TAB_CLOSE_BUTTON_SLASH_PEN, slashP2, backSlashP2);
            }
            else
            {
                g.DrawLine(TAB_CLOSE_BUTTON_SLASH_PEN, slashP1, backSlashP1);
                g.DrawLine(TAB_CLOSE_BUTTON_SLASH_PEN, slashP2, backSlashP2);
            }
        }

        private RectangleF GetDestCenterRectangle(float scale)
        {
            var x = this._drawPoint.X;
            var y = this._drawPoint.Y;
            var w = this._width * scale;
            var h = this._parameter.Height;
            return new RectangleF(x, y, w, h);
        }
    }
}
