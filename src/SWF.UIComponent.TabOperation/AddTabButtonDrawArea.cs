using System;
using System.Drawing;
using SWF.UIComponent.TabOperation.Properties;

namespace SWF.UIComponent.TabOperation
{
    class AddTabButtonDrawArea
    {
        #region 定数・列挙

        #endregion

        #region クラスメンバ

        private readonly static Rectangle DefaultRectangle = GetDefaultRectangle();

        private static Rectangle GetDefaultRectangle()
        {
            Bitmap buttonImg = Resources.InactiveAddTabButton;
            int tabHeight = Resources.ActiveTab.Height;
            int x = 0;
            int y = (int)((tabHeight - buttonImg.Height) / 2d);
            int w = buttonImg.Width;
            int h = buttonImg.Height;
            return new Rectangle(x, y, w, h);
        }

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private readonly Bitmap _inactiveImage = Resources.InactiveAddTabButton;
        private readonly Bitmap _mousePointImage = Resources.MousePointAddTabButton;
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

            draw(g, _inactiveImage);
        }

        public void DrawMousePointImage(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            draw(g, _mousePointImage);
        }

        private void draw(Graphics g, Bitmap img)
        {
            g.DrawImage(img, _drawPoint.X, _drawPoint.Y, _width, _height);
        }

        #endregion
    }
}
