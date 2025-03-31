using SWF.Core.Base;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class AddTabButtonDrawArea
    {
        private static float GetScale(TabSwitch tabSwitch)
        {
            var scale = AppConstants.GetCurrentWindowScale(tabSwitch);
            return scale;
        }

        private static RectangleF GetDefaultRectangle(TabSwitch tabSwitch)
        {
            const int PAGE_SIZE = 29;
            const int TAB_HEIGHT = 29;

            var scale = AppConstants.GetCurrentWindowScale(tabSwitch);

            var x = 0;
            var y = (TAB_HEIGHT * scale - PAGE_SIZE * scale) / 2f;
            var w = PAGE_SIZE * scale;
            var h = PAGE_SIZE * scale;
            return new RectangleF(x, y, w, h);
        }

        private static readonly SolidBrush MOUSE_POINT_BRUSH = new(Color.FromArgb(128, 255, 255, 255));
        private static readonly SolidBrush NORMAL_BRUSH = new(Color.FromArgb(0, 0, 0, 0));
        private static readonly Pen MOUSE_POINT_PEN = new(Color.Black, 2f);
        private static readonly Pen NORMAL_PEN = new(Color.LightGray, 2f);

        private float width;
        private float height;
        private PointF drawPoint;
        private readonly TabSwitch tabSwitch;

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
        }

        public float Height
        {
            get
            {
                return this.height;
            }
        }

        public AddTabButtonDrawArea(TabSwitch tabSwitch)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));

            this.tabSwitch = tabSwitch;
        }

        public bool Page(PointF p)
        {
            return this.Page(p.X, p.Y);
        }

        public bool Page(float x, float y)
        {
            var defaultRect = GetDefaultRectangle(this.tabSwitch);
            this.width = defaultRect.Width;
            this.height = defaultRect.Height;
            var rect = new RectangleF(this.drawPoint.X, this.drawPoint.Y, this.width, this.height);
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

        private void Draw(Graphics g, bool isMousePoint)
        {
            var scale = GetScale(this.tabSwitch);
            var OFFSET = 6.75f * scale;

            var rect = new RectangleF(this.drawPoint.X, this.drawPoint.Y, this.width, this.height);
            var bgRect = new RectangleF(rect.Left + OFFSET / 2f, rect.Top + OFFSET / 2f, rect.Width - OFFSET, rect.Height - OFFSET);
            var vp1 = new PointF(bgRect.Left + OFFSET + bgRect.Width / 6f + 0.75f, bgRect.Top + OFFSET);
            var vp2 = new PointF(bgRect.Left + OFFSET + bgRect.Width / 6f + 0.75f, bgRect.Bottom - OFFSET);
            var hp1 = new PointF(bgRect.Left + OFFSET, bgRect.Top + OFFSET + bgRect.Height / 6f + 0.75f);
            var hp2 = new PointF(bgRect.Right - OFFSET, bgRect.Top + OFFSET + bgRect.Height / 6f + 0.75f);

            if (isMousePoint)
            {
                g.FillEllipse(MOUSE_POINT_BRUSH, bgRect);
                g.DrawLine(MOUSE_POINT_PEN, vp1, vp2);
                g.DrawLine(MOUSE_POINT_PEN, hp1, hp2);
            }
            else
            {
                g.FillEllipse(NORMAL_BRUSH, bgRect);
                g.DrawLine(NORMAL_PEN, vp1, vp2);
                g.DrawLine(NORMAL_PEN, hp1, hp2);
            }
        }

    }
}
