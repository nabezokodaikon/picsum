using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows")]
    internal sealed class AddTabButtonDrawArea
    {


        private const int PAGE_SIZE = 24;
        private const int TAB_HEIGHT = 29;





        private readonly static RectangleF DEFAULT_RECTANGLE = GetDefaultRectangle();

        private static RectangleF GetDefaultRectangle()
        {
            var x = 0;
            var y = (TAB_HEIGHT - PAGE_SIZE) / 2f;
            var w = PAGE_SIZE;
            var h = PAGE_SIZE;
            return new RectangleF(x, y, w, h);
        }

        private static readonly SolidBrush MOUSE_POINT_BRUSH = new(Color.FromArgb(128, 255, 255, 255));
        private static readonly SolidBrush NORMAL_BRUSH = new(Color.FromArgb(64, 0, 0, 0));
        private static readonly Pen MOUSE_POINT_PEN = new(Color.Black, 2f);
        private static readonly Pen NORMAL_PEN = new(Color.White, 2f);





        private readonly float width = DEFAULT_RECTANGLE.Width;
        private readonly float height = DEFAULT_RECTANGLE.Height;
        private PointF drawPoint = new(DEFAULT_RECTANGLE.X, DEFAULT_RECTANGLE.Y);





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





        public bool Page(PointF p)
        {
            return this.Page(p.X, p.Y);
        }

        public bool Page(float x, float y)
        {
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
            const float OFFSET = 6f;
            var rect = new RectangleF(this.drawPoint.X, this.drawPoint.Y, this.width, this.height);
            var bgRect = new RectangleF(rect.Left + OFFSET / 2f, rect.Top + OFFSET / 2f, rect.Width - OFFSET, rect.Height - OFFSET);
            var vp1 = new PointF(rect.Left + OFFSET + rect.Width / 4f, rect.Top + OFFSET);
            var vp2 = new PointF(rect.Left + OFFSET + rect.Width / 4f, rect.Bottom - OFFSET);
            var hp1 = new PointF(rect.Left + OFFSET, rect.Top + OFFSET + rect.Height / 4f);
            var hp2 = new PointF(rect.Right - OFFSET, rect.Top + OFFSET + rect.Height / 4f);

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
