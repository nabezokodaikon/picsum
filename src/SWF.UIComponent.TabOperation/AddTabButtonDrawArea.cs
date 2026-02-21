using SWF.Core.Base;
using System;
using System.Drawing;

namespace SWF.UIComponent.TabOperation
{

    internal sealed class AddTabButtonDrawArea
    {
        private static float GetScale(TabSwitch tabSwitch)
        {
            var scale = WindowUtil.GetCurrentWindowScale(tabSwitch);
            return scale;
        }

        private static RectangleF GetDefaultRectangle(TabSwitch tabSwitch)
        {
            const int PAGE_SIZE = 29;
            const int TAB_HEIGHT = 29;

            var scale = WindowUtil.GetCurrentWindowScale(tabSwitch);

            var x = 0;
            var y = (TAB_HEIGHT * scale - PAGE_SIZE * scale) / 2f;
            var w = PAGE_SIZE * scale;
            var h = PAGE_SIZE * scale;
            return new RectangleF(x, y, w, h);
        }

        private float _width;
        private float _height;
        private PointF _targetPoint;
        private float _currentX = -1f;
        private readonly TabSwitch _tabSwitch;

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

        public float Left
        {
            get
            {
                return this._targetPoint.X;
            }
            set
            {
                this._targetPoint.X = value;
            }
        }

        public float Top
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
                return this._targetPoint.X + this._width;
            }
            set
            {
                this._targetPoint.X = value - this._width;
            }
        }

        public float Bottom
        {
            get
            {
                return this._targetPoint.Y + this._height;
            }
            set
            {
                this._targetPoint.Y = value - this._height;
            }
        }

        public float Width
        {
            get
            {
                return this._width;
            }
        }

        public float Height
        {
            get
            {
                return this._height;
            }
        }

        public AddTabButtonDrawArea(TabSwitch tabSwitch)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));

            this._tabSwitch = tabSwitch;
        }

        public bool Page(PointF p)
        {
            return this.Page(p.X, p.Y);
        }

        public bool Page(float x, float y)
        {
            var defaultRect = GetDefaultRectangle(this._tabSwitch);
            this._width = defaultRect.Width;
            this._height = defaultRect.Height;
            var rect = new RectangleF(this._targetPoint.X, this._targetPoint.Y, this._width, this._height);
            return rect.Contains(x, y);
        }

        public void DrawInactiveImage(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.Draw(g, false);
        }

        public void DrawMousePointImage(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.Draw(g, true);
        }

        public bool IsCompleteAnimation()
        {
            return this._targetPoint.X == this._currentX;
        }

        public void DoNotAnimation()
        {
            this._currentX = this._targetPoint.X;
        }

        public void DoAnimation()
        {
            var currentLeft = this._currentX;
            var targetLeft = this._targetPoint.X;
            var distanceLeft = targetLeft - currentLeft;

            if (Math.Abs(distanceLeft) < 1.0f)
            {
                this._currentX = targetLeft;
                return;
            }

            const float lerpFactor = 0.25f;

            var nextLeft = currentLeft + (distanceLeft * lerpFactor);
            if (distanceLeft > 0)
            {
                this._currentX = (int)Math.Ceiling(nextLeft);
            }
            else
            {
                this._currentX = (int)Math.Floor(nextLeft);
            }

            return;
        }

        private void Draw(Graphics g, bool isMousePoint)
        {
            var scale = GetScale(this._tabSwitch);
            var OFFSET = 6.75f * scale;

            var rect = new RectangleF(this._currentX, this._targetPoint.Y, this._width, this._height);
            var bgRect = new RectangleF(rect.Left + OFFSET / 2f, rect.Top + OFFSET / 2f, rect.Width - OFFSET, rect.Height - OFFSET);
            var vp1 = new PointF(bgRect.Left + OFFSET + bgRect.Width / 6f + 0.75f, bgRect.Top + OFFSET);
            var vp2 = new PointF(bgRect.Left + OFFSET + bgRect.Width / 6f + 0.75f, bgRect.Bottom - OFFSET);
            var hp1 = new PointF(bgRect.Left + OFFSET, bgRect.Top + OFFSET + bgRect.Height / 6f + 0.75f);
            var hp2 = new PointF(bgRect.Right - OFFSET, bgRect.Top + OFFSET + bgRect.Height / 6f + 0.75f);

            if (isMousePoint)
            {
                g.FillEllipse(TabSwitchResources.TAB_ADD_BUTTON_MOUSE_POINT_BRUSH, bgRect);
                g.DrawLine(TabSwitchResources.TAB_ADD_BUTTON_MOUSE_POINT_PEN, vp1, vp2);
                g.DrawLine(TabSwitchResources.TAB_ADD_BUTTON_MOUSE_POINT_PEN, hp1, hp2);
            }
            else
            {
                g.FillEllipse(TabSwitchResources.TAB_ADD_BUTTON_NORMAL_BRUSH, bgRect);
                g.DrawLine(TabSwitchResources.TAB_ADD_BUTTON_NORMAL_PEN, vp1, vp2);
                g.DrawLine(TabSwitchResources.TAB_ADD_BUTTON_NORMAL_PEN, hp1, hp2);
            }
        }

    }
}
