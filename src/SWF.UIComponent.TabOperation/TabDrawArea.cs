using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ描画領域
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class TabDrawArea
    {
        private const float TAB_WIDTH = 256;
        private const float SIDE_WIDTH = 8;
        private const float PAGE_SIZE = 24;
        private const float PAGE_OFFSET = 2;

        private static readonly RectangleF ICON_RECTANGLE
            = new(SIDE_WIDTH, PAGE_OFFSET, PAGE_SIZE, PAGE_SIZE);

        private static readonly RectangleF CLOSE_BUTTON_RECTANGLE
            = new(TAB_WIDTH - SIDE_WIDTH - PAGE_SIZE,
                  PAGE_OFFSET,
                  PAGE_SIZE,
                  PAGE_SIZE);

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
            = new(Color.FromArgb(128, 128, 128), 0.5f);

        private PointF drawPoint = new(0, 0);
        private float width = 256;
        private readonly float height = 29;

        public float X
        {
            get
            {
                return this.drawPoint.X;
            }
            set
            {
                this.drawPoint.X = value;
            }
        }

        public float Y
        {
            get
            {
                return this.drawPoint.Y;
            }
            set
            {
                this.drawPoint.Y = value;
            }
        }

        public float Left
        {
            get
            {
                return this.drawPoint.X;
            }
            set
            {
                this.drawPoint.X = value;
            }
        }

        public float Top
        {
            get
            {
                return this.drawPoint.Y;
            }
            set
            {
                this.drawPoint.Y = value;
            }
        }

        public float Right
        {
            get
            {
                return this.drawPoint.X + this.width;
            }
            set
            {
                this.drawPoint.X = value - this.width;
            }
        }

        public float Bottom
        {
            get
            {
                return this.drawPoint.Y + this.height;
            }
            set
            {
                this.drawPoint.Y = value - this.height;
            }
        }

        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public float Height
        {
            get
            {
                return this.height;
            }
        }

        public void DrawActiveTab(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.DrawTab(ACTIVE_TAB_BRUSH, g);
        }

        public void DrawInactiveTab(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.DrawTab(INACTIVE_TAB_BRUSH, g);
        }

        public void DrawMousePointTab(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.DrawTab(MOUSE_POINT_TAB_BRUSH, g);
        }

        public void DrawNothingTabCloseButton(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
        }

        public void DrawActiveMousePointTabCloseButton(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.DrawTabCloseButton(g, true, true);
        }

        public void DrawInactiveMousePointTabCloseButton(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.DrawTabCloseButton(g, true, false);
        }

        public void DrawInactiveTabCloseButton(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.DrawTabCloseButton(g, false, false);
        }

        public void DrawActiveTabCloseButton(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

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
            var x = this.drawPoint.X;
            var y = this.drawPoint.Y;
            var w = this.width;
            var h = this.height;
            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetIconRectangle()
        {
            var x = ICON_RECTANGLE.X + this.drawPoint.X;
            var y = ICON_RECTANGLE.Y + this.drawPoint.Y;
            var w = ICON_RECTANGLE.Width;
            var h = ICON_RECTANGLE.Height;
            return new RectangleF(x, y, w, h);
        }

        public RectangleF GetIconRectangle(Image icon)
        {
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            var rect = this.GetIconRectangle();
            var w = Math.Min(icon.Width, rect.Width);
            var h = Math.Min(icon.Height, rect.Height);
            var x = rect.X + (rect.Width - w) / 2f;
            var y = rect.Y + (rect.Height - h) / 2f;
            return new RectangleF(x, y, w, h);
        }

        public RectangleF GetCloseButtonRectangle()
        {
            var w = CLOSE_BUTTON_RECTANGLE.Width;
            var h = CLOSE_BUTTON_RECTANGLE.Height;

            if (this.width < TabSwitch.TAB_CLOSE_BUTTON_CAN_DRAW_WIDTH)
            {
                var x = this.X + (this.Width - CLOSE_BUTTON_RECTANGLE.Width) / 2f;
                var y = CLOSE_BUTTON_RECTANGLE.Y + (CLOSE_BUTTON_RECTANGLE.Height - h) / 2f;
                return new RectangleF(x, y, w, h);
            }
            else
            {
                var x = CLOSE_BUTTON_RECTANGLE.X - (TAB_WIDTH - this.width) + this.drawPoint.X;
                var y = CLOSE_BUTTON_RECTANGLE.Y + this.drawPoint.Y;
                return new RectangleF(x, y, w, h);
            }
        }

        public RectangleF GetPageRectangle()
        {
            var x = ICON_RECTANGLE.Right + PAGE_OFFSET + this.drawPoint.X;
            var y = ICON_RECTANGLE.Y + this.drawPoint.Y;
            return RectangleF.FromLTRB(x, y, CLOSE_BUTTON_RECTANGLE.X - (TAB_WIDTH - this.width) + this.drawPoint.X - PAGE_OFFSET, y + ICON_RECTANGLE.Height);
        }

        private void DrawTab(SolidBrush brush, Graphics g)
        {
            var destRect = this.GetDestCenterRectangle();

            g.FillRectangle(brush, destRect);
            g.DrawLines(TAB_OUTLINE_PEN, [
                new PointF(destRect.Left, destRect.Bottom - 1f),
                new PointF(destRect.Left, destRect.Top + 0.5f),
                new PointF(destRect.Right, destRect.Top + 0.5f),
                new PointF(destRect.Right, destRect.Bottom - 1f)]);
        }

        private void DrawTabCloseButton(Graphics g, bool isMousePoint, bool isActiveTab)
        {
            const float OFFSET = 8;
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

        private RectangleF GetDestCenterRectangle()
        {
            var x = this.drawPoint.X;
            var y = this.drawPoint.Y;
            var w = this.width;
            var h = this.height;
            return new RectangleF(x, y, w, h);
        }

    }
}
