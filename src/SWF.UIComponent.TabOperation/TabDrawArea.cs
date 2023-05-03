using SWF.UIComponent.TabOperation.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ描画領域
    /// </summary>
    internal class TabDrawArea
    {
        #region 定数・列挙

        private const int MINIMUM_WIDHT = 64;
        private const int SIDE_WIDTH = 8;
        private const int CONTENTS_SIZE = 24;
        private const int CONTENTS_OFFSET = 2;

        #endregion

        #region クラスメンバ

        private readonly static IList<Point> LeftTransparentPoints = GetLeftTransparentPoints(Resources.ActiveTab);
        private readonly static IList<Point> RightTransparentPoints = GetRightTransparentPoints(Resources.ActiveTab);
        private readonly static Rectangle IconRectangle = new Rectangle(SIDE_WIDTH, CONTENTS_OFFSET, CONTENTS_SIZE, CONTENTS_SIZE);
        private readonly static Rectangle CloseButtonRectangle = new Rectangle(Resources.ActiveTab.Width - SIDE_WIDTH - CONTENTS_SIZE,
                                                                               CONTENTS_OFFSET,
                                                                               CONTENTS_SIZE,
                                                                               CONTENTS_SIZE);

        private readonly static SolidBrush tabCloseActiveButtonBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 0));
        private readonly static SolidBrush tabCloseInactiveButtonBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 0));
        private readonly static Pen tabCloseButtonSlashPen = new Pen(Color.Black, 2f);


        private static IList<Point> GetLeftTransparentPoints(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException(string.Format("ピクセルフォーマットが{0}ではありません。", PixelFormat.Format32bppArgb));
            }

            List<Point> pList = new List<Point>();
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int w = bd.Width;
            int h = bd.Height;

            unsafe
            {
                byte* p = (byte*)(void*)bd.Scan0;

                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                    {
                        if (x < SIDE_WIDTH && p[3] == 0)
                        {
                            pList.Add(new Point(x, y));
                        }

                        p += 4;
                    }
                }
            }

            bmp.UnlockBits(bd);

            return pList;
        }

        private static IList<Point> GetRightTransparentPoints(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException(string.Format("ピクセルフォーマットが{0}ではありません。", PixelFormat.Format32bppArgb));
            }

            List<Point> pList = new List<Point>();
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int w = bd.Width;
            int h = bd.Height;

            unsafe
            {
                byte* p = (byte*)(void*)bd.Scan0;

                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                    {
                        if (x > w - SIDE_WIDTH && p[3] == 0)
                        {
                            pList.Add(new Point(x, y));
                        }

                        p += 4;
                    }
                }
            }

            bmp.UnlockBits(bd);

            return pList;
        }

        #endregion

        #region インスタンス変数

        private readonly IList<Point> _leftTransparentPoints = LeftTransparentPoints;
        private readonly IList<Point> _rightTransparentPoints = RightTransparentPoints;
        private readonly Bitmap _activeTabImage = Resources.ActiveTab;
        private readonly Bitmap _inactiveTabImage = Resources.InactiveTab;
        private readonly Bitmap _mousePointTabImage = Resources.MousePointTab;
        private readonly Rectangle _iconRectangle = IconRectangle;
        private readonly Rectangle _closeButtonRectangle = CloseButtonRectangle;
        private Point _drawPoint = new Point(0, 0);
        private int _width = Resources.ActiveTab.Width;
        private readonly int _height = Resources.ActiveTab.Height;

        #endregion

        #region プロパティ

        public Bitmap TabImage
        {
            get
            {
                return _activeTabImage;
            }
        }

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
            set
            {
                if (value < MINIMUM_WIDHT)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _width = value;
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

        public TabDrawArea()
        {

        }

        #endregion

        #region メソッド

        public void DrawActiveTab(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            drawTab(_activeTabImage, g);
        }

        public void DrawInactiveTab(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            drawTab(_inactiveTabImage, g);
        }

        public void DrawMousePointTab(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            drawTab(_mousePointTabImage, g);
        }

        public void DrawActiveMousePointTabCloseButton(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            drawTabCloseButton(g, true, true);
        }

        public void DrawInactiveMousePointTabCloseButton(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            drawTabCloseButton(g, true, false);
        }

        public void DrawInactiveTabCloseButton(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            drawTabCloseButton(g, false, false);
        }

        public void DrawActiveTabCloseButton(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            drawTabCloseButton(g, false, true);
        }

        public bool Contents(int x, int y)
        {
            if (x < this.Left || this.Right < x || y < this.Top || this.Bottom < y)
            {
                return false;
            }

            foreach (Point p in _leftTransparentPoints)
            {
                if (p.X + _drawPoint.X == x && p.Y + _drawPoint.Y == y)
                {
                    return false;
                }
            }

            int offset = _activeTabImage.Width - _width;
            foreach (Point p in _rightTransparentPoints)
            {
                if ((p.X + _drawPoint.X) - offset == x && p.Y + _drawPoint.Y == y)
                {
                    return false;
                }
            }

            return true;
        }

        public bool Contents(Point p)
        {
            return Contents(p.X, p.Y);
        }

        public Rectangle GetRectangle()
        {
            int x = _drawPoint.X;
            int y = _drawPoint.Y;
            int w = _width;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        public Rectangle GetIconRectangle()
        {
            int x = _iconRectangle.X + _drawPoint.X;
            int y = _iconRectangle.Y + _drawPoint.Y;
            int w = _iconRectangle.Width;
            int h = _iconRectangle.Height;
            return new Rectangle(x, y, w, h);
        }

        public Rectangle GetIconRectangle(Image icon)
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }

            Rectangle rect = GetIconRectangle();
            int w = Math.Min(icon.Width, rect.Width);
            int h = Math.Min(icon.Height, rect.Height);
            int x = rect.X + (rect.Width - w) / 2;
            int y = rect.Y + (rect.Height - h) / 2;
            return new Rectangle(x, y, w, h);
        }

        public Rectangle GetCloseButtonRectangle()
        {
            int x = _closeButtonRectangle.X - (_activeTabImage.Width - _width) + _drawPoint.X;
            int y = _closeButtonRectangle.Y + _drawPoint.Y;
            int w = _closeButtonRectangle.Width;
            int h = _closeButtonRectangle.Height;
            return new Rectangle(x, y, w, h);
        }

        public Rectangle GetContentsRectangle()
        {
            int x = _iconRectangle.Right + CONTENTS_OFFSET + _drawPoint.X;
            int y = _iconRectangle.Y + _drawPoint.Y;
            return Rectangle.FromLTRB(x, y, _closeButtonRectangle.X - (_activeTabImage.Width - _width) + _drawPoint.X - CONTENTS_OFFSET, y + _iconRectangle.Height);
        }

        private void drawTab(Bitmap bmp, Graphics g)
        {
            g.DrawImage(bmp, getDestLeftRectangle(), getSourceLeftRectangle(), GraphicsUnit.Pixel);
            g.DrawImage(bmp, getDestRightRectangle(), getSourceRightRectangle(), GraphicsUnit.Pixel);
            g.DrawImage(bmp, getDestCenterRectangle(), getSourceCenterRectangle(), GraphicsUnit.Pixel);
        }

        private void drawTabCloseButton(Graphics g, bool isMousePoint, bool isActiveTab)
        {
            const int OFFSET = 8;
            var rect = GetCloseButtonRectangle();
            var bgRect = new RectangleF(rect.Left + OFFSET / 2f, rect.Top + OFFSET / 2f, rect.Width - OFFSET, rect.Height - OFFSET);
            var slashP1 = new Point(rect.Left + OFFSET, rect.Top + OFFSET);
            var backSlashP1 = new Point(rect.Right - OFFSET, rect.Bottom - OFFSET);
            var slashP2 = new Point(rect.Right - OFFSET, rect.Top + OFFSET);
            var backSlashP2 = new Point(rect.Left + OFFSET, rect.Bottom - OFFSET);

            if (isMousePoint)
            {
                if (isActiveTab)
                {
                    g.FillEllipse(tabCloseActiveButtonBrush, bgRect);
                }
                else
                {
                    g.FillEllipse(tabCloseInactiveButtonBrush, bgRect);
                }
            }

            if (isActiveTab)
            {
                g.DrawLine(tabCloseButtonSlashPen, slashP1, backSlashP1);
                g.DrawLine(tabCloseButtonSlashPen, slashP2, backSlashP2);
            }
            else
            {
                g.DrawLine(tabCloseButtonSlashPen, slashP1, backSlashP1);
                g.DrawLine(tabCloseButtonSlashPen, slashP2, backSlashP2);
            }
        }

        private Rectangle getDestCenterRectangle()
        {
            int x = _drawPoint.X + SIDE_WIDTH;
            int y = _drawPoint.Y;
            int w = _width - SIDE_WIDTH * 2;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getDestLeftRectangle()
        {
            int x = _drawPoint.X;
            int y = _drawPoint.Y;
            int w = SIDE_WIDTH;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getDestRightRectangle()
        {
            int x = _drawPoint.X + _width - SIDE_WIDTH;
            int y = _drawPoint.Y;
            int w = SIDE_WIDTH;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getSourceCenterRectangle()
        {
            int x = SIDE_WIDTH;
            int y = 0;
            int w = _activeTabImage.Width - SIDE_WIDTH * 2;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getSourceLeftRectangle()
        {
            int x = 0;
            int y = 0;
            int w = SIDE_WIDTH;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getSourceRightRectangle()
        {
            int x = _activeTabImage.Width - SIDE_WIDTH;
            int y = 0;
            int w = SIDE_WIDTH;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        #endregion
    }
}
