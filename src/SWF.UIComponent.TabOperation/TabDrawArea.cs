using SWF.Core.Base;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ描画領域
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
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
            = new(Color.FromArgb(128, 128, 128), 0.5f);

        private PointF drawPoint = new(0, 0);
        private float width = 256;
        private readonly TabDrawAreaParameter parameter;

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
                return this.drawPoint.Y + this.parameter.Height;
            }
            set
            {
                this.drawPoint.Y = value - this.parameter.Height;
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
                return this.parameter.Height;
            }
        }

        public TabDrawArea(Control owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            this.parameter = new(owner);
        }

        public void DrawActiveTab(Graphics g, float scale)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.parameter.Update(scale);
            this.DrawTab(ACTIVE_TAB_BRUSH, g, scale);
        }

        public void DrawActiveTab(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.parameter.Update(tabSwitch);
            this.DrawTab(ACTIVE_TAB_BRUSH, g);
        }

        public void DrawInactiveTab(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));

            this.parameter.Update(tabSwitch);
            this.DrawTab(INACTIVE_TAB_BRUSH, g);
        }

        public void DrawMousePointTab(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.parameter.Update(tabSwitch);
            this.DrawTab(MOUSE_POINT_TAB_BRUSH, g);
        }

        public void DrawNothingTabCloseButton(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
        }

        public void DrawActiveMousePointTabCloseButton(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.parameter.Update(tabSwitch);
            this.DrawTabCloseButton(g, true, true);
        }

        public void DrawInactiveMousePointTabCloseButton(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.parameter.Update(tabSwitch);
            this.DrawTabCloseButton(g, true, false);
        }

        public void DrawInactiveTabCloseButton(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.parameter.Update(tabSwitch);
            this.DrawTabCloseButton(g, false, false);
        }

        public void DrawActiveTabCloseButton(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.parameter.Update(tabSwitch);
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
            var h = this.parameter.Height;
            return new RectangleF(x, y, w, h);
        }

        public RectangleF GetIconRectangle(Image icon)
        {
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            var hwnd = WinApiMembers.WindowFromPoint(
                new WinApiMembers.POINT(Cursor.Position.X, Cursor.Position.Y));
            var scale = AppConstants.GetCurrentWindowScale(hwnd);
            var margin = 8 * scale;

            var rect = this.GetIconRectangle();
            var w = Math.Min(icon.Width, rect.Width) - margin;
            var h = Math.Min(icon.Height, rect.Height) - margin;
            var x = rect.X + (rect.Width - w) / 2f;
            var y = rect.Y + (rect.Height - h) / 2f;
            return new RectangleF(x, y, w, h);
        }

        public RectangleF GetCloseButtonRectangle()
        {
            var w = this.parameter.CloseButtonRectangle.Width;
            var h = this.parameter.CloseButtonRectangle.Height;

            if (this.width < TabSwitch.GetTabCloseButtonCanDrawWidth(this.parameter.GetOwner()))
            {
                var x = this.X + (this.Width - this.parameter.CloseButtonRectangle.Width) / 2f;
                var y = this.parameter.CloseButtonRectangle.Y + (this.parameter.CloseButtonRectangle.Height - h) / 2f;
                return new RectangleF(x, y, w, h);
            }
            else
            {
                var x = this.parameter.CloseButtonRectangle.X - (this.parameter.TabWidth - this.width) + this.drawPoint.X;
                var y = this.parameter.CloseButtonRectangle.Y + this.drawPoint.Y;
                return new RectangleF(x, y, w, h);
            }
        }

        public RectangleF GetPageRectangle()
        {
            var x = this.parameter.IconRectangle.Right + this.parameter.PageOffset + this.drawPoint.X;
            var y = this.parameter.IconRectangle.Y + this.drawPoint.Y;
            return RectangleF.FromLTRB(
                x,
                y,
                this.parameter.CloseButtonRectangle.X - (this.parameter.TabWidth - this.width) + this.drawPoint.X - this.parameter.PageOffset,
                y + this.parameter.IconRectangle.Height);
        }

        private RectangleF GetIconRectangle()
        {
            var x = this.parameter.IconRectangle.X + this.drawPoint.X;
            var y = this.parameter.IconRectangle.Y + this.drawPoint.Y;
            var w = this.parameter.IconRectangle.Width;
            var h = this.parameter.IconRectangle.Height;
            return new RectangleF(x, y, w, h);
        }

        private void DrawTab(SolidBrush brush, Graphics g)
        {
            this.DrawTab(brush, g, 1f);
        }

        private void DrawTab(SolidBrush brush, Graphics g, float scale)
        {
            var destRect = this.GetDestCenterRectangle(scale);

            g.FillRectangle(brush, destRect);
            g.DrawLines(TAB_OUTLINE_PEN, [
                new PointF(destRect.Left, destRect.Bottom - 1f),
                new PointF(destRect.Left, destRect.Top + 0.5f),
                new PointF(destRect.Right, destRect.Top + 0.5f),
                new PointF(destRect.Right, destRect.Bottom - 1f)]);
        }

        private void DrawTabCloseButton(Graphics g, bool isMousePoint, bool isActiveTab)
        {
            float OFFSET = 8 * this.parameter.Scale;

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
            var x = this.drawPoint.X;
            var y = this.drawPoint.Y;
            var w = this.width * scale;
            var h = this.parameter.Height;
            return new RectangleF(x, y, w, h);
        }
    }
}
