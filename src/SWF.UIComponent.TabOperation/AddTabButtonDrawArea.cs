using SWF.UIComponent.TabOperation.Properties;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows")]
    internal sealed class AddTabButtonDrawArea
    {
        #region 定数・列挙

        private const int PAGE_SIZE = 24;

        #endregion

        #region クラスメンバ

        private readonly static Rectangle DEFAULT_RECTANGLE = GetDefaultRectangle();

        private static Rectangle GetDefaultRectangle()
        {
            var tabHeight = Resources.ActiveTab.Height;
            var x = 0;
            var y = (int)((tabHeight - PAGE_SIZE) / 2d);
            var w = PAGE_SIZE;
            var h = PAGE_SIZE;
            return new Rectangle(x, y, w, h);
        }

        private static readonly SolidBrush MOUSE_POINT_BRUSH = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
        private static readonly SolidBrush NORMAL_BRUSH = new SolidBrush(Color.FromArgb(64, 0, 0, 0));
        private static readonly Pen MOUSE_POINT_PEN = new Pen(Color.Black, 2f);
        private static readonly Pen NORMAL_PEN = new Pen(Color.White, 2f);

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private readonly int width = DEFAULT_RECTANGLE.Width;
        private readonly int height = DEFAULT_RECTANGLE.Height;
        private Point drawPoint = new Point(DEFAULT_RECTANGLE.X, DEFAULT_RECTANGLE.Y);

        #endregion

        #region プロパティ

        public int X
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

        public int Y
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

        public int Left
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

        public int Top
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

        public int Right
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

        public int Bottom
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

        public int Width
        {
            get
            {
                return this.width;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
        }

        #endregion

        #region メソッド

        public bool Page(Point p)
        {
            return this.Page(p.X, p.Y);
        }

        public bool Page(int x, int y)
        {
            var rect = new Rectangle(this.drawPoint.X, this.drawPoint.Y, this.width, this.height);
            return rect.Contains(x, y);
        }

        public void DrawInactiveImage(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            this.Draw(g, false);
        }

        public void DrawMousePointImage(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            this.Draw(g, true);
        }

        private void Draw(Graphics g, bool isMousePoint)
        {
            const float OFFSET = 6f;
            var rect = new Rectangle(this.drawPoint.X, this.drawPoint.Y, this.width, this.height);
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

        #endregion
    }
}
