using SWF.UIComponent.TabOperation.Properties;
using System;
using System.Drawing;

namespace SWF.UIComponent.TabOperation
{
    class AddTabButtonDrawArea
    {
        #region 定数・列挙

        private const int CONTENTS_SIZE = 24;
        #endregion

        #region クラスメンバ

        private readonly static Rectangle DefaultRectangle = GetDefaultRectangle();

        private static Rectangle GetDefaultRectangle()
        {
            int tabHeight = Resources.ActiveTab.Height;
            int x = 0;
            int y = (int)((tabHeight - CONTENTS_SIZE) / 2d);
            int w = CONTENTS_SIZE;
            int h = CONTENTS_SIZE;
            return new Rectangle(x, y, w, h);
        }

        private static readonly SolidBrush mousePointBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
        private static readonly SolidBrush normalBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 0));
        private static readonly Pen mousePointPen = new Pen(Color.Black, 2f);
        private static readonly Pen normalPen = new Pen(Color.White, 2f);

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private readonly int _width = DefaultRectangle.Width;
        private readonly int _height = DefaultRectangle.Height;
        private Point _drawPoint = new Point(DefaultRectangle.X, DefaultRectangle.Y);

        #endregion

        #region プロパティ

        public int X
        {
            get
            {
                return _drawPoint.X;
            }
            set
            {
                _drawPoint.X = value;
            }
        }

        public int Y
        {
            get
            {
                return _drawPoint.Y;
            }
            set
            {
                _drawPoint.Y = value;
            }
        }

        public int Left
        {
            get
            {
                return _drawPoint.X;
            }
            set
            {
                _drawPoint.X = value;
            }
        }

        public int Top
        {
            get
            {
                return _drawPoint.Y;
            }
            set
            {
                _drawPoint.Y = value;
            }
        }

        public int Right
        {
            get
            {
                return _drawPoint.X + _width;
            }
            set
            {
                _drawPoint.X = value - _width;
            }
        }

        public int Bottom
        {
            get
            {
                return _drawPoint.Y + _height;
            }
            set
            {
                _drawPoint.Y = value - _height;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        #endregion

        #region コンストラクタ

        public AddTabButtonDrawArea()
        {

        }

        #endregion

        #region メソッド

        public bool Contents(Point p)
        {
            return Contents(p.X, p.Y);
        }

        public bool Contents(int x, int y)
        {
            Rectangle rect = new Rectangle(_drawPoint.X, _drawPoint.Y, _width, _height);
            return rect.Contains(x, y);
        }

        public void DrawInactiveImage(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            draw(g, false);
        }

        public void DrawMousePointImage(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            draw(g, true);
        }

        private void draw(Graphics g, bool isMousePoint)
        {
            const float OFFSET = 6f;
            var rect = new Rectangle(_drawPoint.X, _drawPoint.Y, _width, _height);
            var bgRect = new RectangleF(rect.Left + OFFSET / 2f, rect.Top + OFFSET / 2f, rect.Width - OFFSET, rect.Height - OFFSET);
            var vp1 = new PointF(rect.Left + OFFSET + rect.Width / 4f, rect.Top + OFFSET);
            var vp2 = new PointF(rect.Left + OFFSET + rect.Width / 4f, rect.Bottom - OFFSET);
            var hp1 = new PointF(rect.Left + OFFSET, rect.Top + OFFSET + rect.Height / 4f);
            var hp2 = new PointF(rect.Right - OFFSET, rect.Top + OFFSET + rect.Height / 4f);

            if (isMousePoint)
            {
                g.FillEllipse(mousePointBrush, bgRect);
                g.DrawLine(mousePointPen, vp1, vp2);
                g.DrawLine(mousePointPen, hp1, hp2);
            }
            else
            {
                g.FillEllipse(normalBrush, bgRect);
                g.DrawLine(normalPen, vp1, vp2);
                g.DrawLine(normalPen, hp1, hp2);
            }
        }

        #endregion
    }
}
