using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ描画領域
    /// </summary>

    public sealed class TabDrawArea
    {
        private readonly TabDrawAreaParameter _parameter;

        private PointF _targetPoint = new(0, 0);
        private float _targetWidth = 256f;
        private float _currentX = -1f;
        private float _currentWidth = -1f;

        public float X
        {
            get
            {
                return this._targetPoint.X;
            }
            set
            {
                if (this._currentX < 0 && value != this._currentX)
                {
                    this._currentX = value;
                }

                this._targetPoint.X = value;
            }
        }

        public float Y
        {
            get
            {
                return this._targetPoint.Y;
            }
            set
            {
                this._targetPoint.Y = value;
            }
        }

        public float Right
        {
            get
            {
                return this.X + this.Width;
            }
            set
            {
                this.X = value - this.Width;
            }
        }

        public float Bottom
        {
            get
            {
                return this.Y + this.Height;
            }
            set
            {
                this.Y = value - this.Height;
            }
        }

        public float Width
        {
            get
            {
                return this._targetWidth;
            }
            set
            {
                this._targetWidth = value;
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
            this.DrawTab(TabSwitchResources.TAB_ACTIVE_BRUSH, g, scale, true);
        }

        public void DrawActiveTab(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this._parameter.Update(tabSwitch);
            this.DrawTab(TabSwitchResources.TAB_ACTIVE_BRUSH, g, false);
        }

        public void DrawInactiveTab(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));

            this._parameter.Update(tabSwitch);
            this.DrawTab(TabSwitchResources.TAB_INACTIVE_BRUSH, g, false);
        }

        public void DrawMousePointTab(TabSwitch tabSwitch, Graphics g)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this._parameter.Update(tabSwitch);
            this.DrawTab(TabSwitchResources.TAB_MOUSE_POINT_BRUSH, g, false);
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
            if (x < this.X || this.Right < x || y < this.Y || this.Bottom < y)
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
            var x = this._targetPoint.X;
            var y = this._targetPoint.Y;
            var w = this._targetWidth;
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

            if (this._targetWidth < TabSwitch.GetTabCloseButtonCanDrawWidth(this._parameter.GetOwner()))
            {
                var x = this.X + (this.Width - this._parameter.CloseButtonRectangle.Width) / 2f;
                var y = this._parameter.CloseButtonRectangle.Y + (this._parameter.CloseButtonRectangle.Height - h) / 2f;
                return new RectangleF(x, y, w, h);
            }
            else
            {
                var x = this._parameter.CloseButtonRectangle.X - (this._parameter.TabWidth - this._currentWidth) + this._currentX;
                var y = this._parameter.CloseButtonRectangle.Y + this._targetPoint.Y;
                return new RectangleF(x, y, w, h);
            }
        }

        public RectangleF GetPageRectangle()
        {
            var x = this._parameter.IconRectangle.Right + this._parameter.PageOffset + this._currentX;
            var y = this._parameter.IconRectangle.Y + this._targetPoint.Y;
            return RectangleF.FromLTRB(
                x,
                y,
                this._parameter.CloseButtonRectangle.X - (this._parameter.TabWidth - this._currentWidth) + this._currentX - this._parameter.PageOffset,
                y + this._parameter.IconRectangle.Height);
        }

        public bool IsCompleteAnimation()
        {
            return this._targetPoint.X == this._currentX
                && this._targetWidth == this._currentWidth;
        }

        public void DoNotAnimation()
        {
            this._currentX = this._targetPoint.X;
            this._currentWidth = this._targetWidth;
        }

        public void DoAnimation()
        {
            var currentLeft = this._currentX;
            var targetLeft = this._targetPoint.X;
            var distanceLeft = targetLeft - currentLeft;

            var currentWidth = this._currentWidth;
            var targetWidth = this._targetWidth;
            var distanceWidth = targetWidth - currentWidth;

            if (Math.Abs(distanceLeft) < 1.0f && Math.Abs(distanceWidth) < 1.0f)
            {
                this._currentX = targetLeft;
                this._currentWidth = targetWidth;
                return;
            }

            const float lerpFactor = 0.2f;

            var nextLeft = currentLeft + (distanceLeft * lerpFactor);
            if (distanceLeft > 0)
            {
                this._currentX = (int)Math.Ceiling(nextLeft);
            }
            else
            {
                this._currentX = (int)Math.Floor(nextLeft);
            }

            var nextWidth = currentWidth + (distanceWidth * lerpFactor);
            if (distanceWidth > 0)
            {
                this._currentWidth = (int)Math.Ceiling(nextWidth);
            }
            else
            {
                this._currentWidth = (int)Math.Floor(nextWidth);
            }

            return;
        }

        private RectangleF GetIconRectangle()
        {
            var x = this._parameter.IconRectangle.X + this._currentX;
            var y = this._parameter.IconRectangle.Y + this._targetPoint.Y;
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
                g.DrawLines(TabSwitchResources.TAB_OUTLINE_PEN, [
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
                    g.FillEllipse(TabSwitchResources.TAB_CLOSE_BUTTON_ACTIVE_BRUSH, bgRect);
                }
                else
                {
                    g.FillEllipse(TabSwitchResources.TAB_CLOSE_BUTTON_INACTIVE_BRUSH, bgRect);
                }
            }

            if (isActiveTab)
            {
                g.DrawLine(TabSwitchResources.TAB_CLOSE_BUTTON_SLASH_PEN, slashP1, backSlashP1);
                g.DrawLine(TabSwitchResources.TAB_CLOSE_BUTTON_SLASH_PEN, slashP2, backSlashP2);
            }
            else
            {
                g.DrawLine(TabSwitchResources.TAB_CLOSE_BUTTON_SLASH_PEN, slashP1, backSlashP1);
                g.DrawLine(TabSwitchResources.TAB_CLOSE_BUTTON_SLASH_PEN, slashP2, backSlashP2);
            }
        }

        private RectangleF GetDestCenterRectangle(float scale)
        {
            var x = this._currentX;
            var y = this.Y;
            var w = this._currentWidth * scale;
            var h = this.Height;
            return new RectangleF(x, y, w, h);
        }
    }
}
