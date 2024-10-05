using SWF.UIComponent.TabOperation.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ描画領域
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class TabDrawArea
    {
        #region 定数・列挙

        private const int MINIMUM_WIDHT = 64;
        private const int SIDE_WIDTH = 8;
        private const int PAGE_SIZE = 24;
        private const int PAGE_OFFSET = 2;

        #endregion

        #region クラスメンバ

        private readonly static IList<Point> LEFT_TRANSPARENT_POINTS
            = GetLeftTransparentPoints(Resources.ActiveTab);
        private readonly static IList<Point> RIGHT_TRANSPARENT_POINTS
            = GetRightTransparentPoints(Resources.ActiveTab);
        private readonly static Rectangle ICON_RECTANGLE
            = new(SIDE_WIDTH, PAGE_OFFSET, PAGE_SIZE, PAGE_SIZE);
        private readonly static Rectangle CLOSE_BUTTON_RECTANGLE
            = new(Resources.ActiveTab.Width - SIDE_WIDTH - PAGE_SIZE,
                  PAGE_OFFSET,
                  PAGE_SIZE,
                  PAGE_SIZE);

        private readonly static SolidBrush TAB_CLOSE_ACTIVE_BUTTON_BRUSH
            = new(Color.FromArgb(64, 0, 0, 0));
        private readonly static SolidBrush TAB_CLOSE_INACTIVE_BUTTON_BRUSH
            = new(Color.FromArgb(64, 0, 0, 0));
        private readonly static Pen TAB_CLOSE_BUTTON_SLASH_PEN
            = new(Color.Black, 2f);


        private static List<Point> GetLeftTransparentPoints(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException($"ピクセルフォーマットが{PixelFormat.Format32bppArgb}ではありません。");
            }

            var pList = new List<Point>();
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var w = bd.Width;
            var h = bd.Height;

            unsafe
            {
                byte* p = (byte*)(void*)bd.Scan0;

                for (var y = 0; y < h; ++y)
                {
                    for (var x = 0; x < w; ++x)
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

        private static List<Point> GetRightTransparentPoints(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException($"ピクセルフォーマットが{PixelFormat.Format32bppArgb}ではありません。");
            }

            var pList = new List<Point>();
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int w = bd.Width;
            int h = bd.Height;

            unsafe
            {
                byte* p = (byte*)(void*)bd.Scan0;

                for (var y = 0; y < h; ++y)
                {
                    for (var x = 0; x < w; ++x)
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

        private readonly IList<Point> leftTransparentPoints = LEFT_TRANSPARENT_POINTS;
        private readonly IList<Point> rightTransparentPoints = RIGHT_TRANSPARENT_POINTS;
        private readonly Bitmap activeTabImage = Resources.ActiveTab;
        private readonly Bitmap inactiveTabImage = Resources.InactiveTab;
        private readonly Bitmap mousePointTabImage = Resources.MousePointTab;
        private readonly Rectangle iconRectangle = ICON_RECTANGLE;
        private readonly Rectangle closeButtonRectangle = CLOSE_BUTTON_RECTANGLE;
        private Point drawPoint = new(0, 0);
        private int width = Resources.ActiveTab.Width;
        private readonly int height = Resources.ActiveTab.Height;

        #endregion

        #region プロパティ

        public Bitmap TabImage
        {
            get
            {
                return this.activeTabImage;
            }
        }

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
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, TabSwitch.TAB_MINIMUM_WIDTH, nameof(value));

                this.width = value;
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

        public void DrawActiveTab(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.DrawTab(this.activeTabImage, g);
        }

        public void DrawInactiveTab(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.DrawTab(this.inactiveTabImage, g);
        }

        public void DrawMousePointTab(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            this.DrawTab(this.mousePointTabImage, g);
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

            foreach (var p in this.leftTransparentPoints)
            {
                if (p.X + this.drawPoint.X == x && p.Y + this.drawPoint.Y == y)
                {
                    return false;
                }
            }

            var offset = this.activeTabImage.Width - this.width;
            foreach (var p in this.rightTransparentPoints)
            {
                if ((p.X + this.drawPoint.X) - offset == x && p.Y + this.drawPoint.Y == y)
                {
                    return false;
                }
            }

            return true;
        }

        public bool Contains(Point p)
        {
            return this.Contains(p.X, p.Y);
        }

        public Rectangle GetRectangle()
        {
            var x = this.drawPoint.X;
            var y = this.drawPoint.Y;
            var w = this.width;
            var h = this.height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle GetIconRectangle()
        {
            var x = this.iconRectangle.X + this.drawPoint.X;
            var y = this.iconRectangle.Y + this.drawPoint.Y;
            var w = this.iconRectangle.Width;
            var h = this.iconRectangle.Height;
            return new Rectangle(x, y, w, h);
        }

        public Rectangle GetIconRectangle(Image icon)
        {
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            var rect = this.GetIconRectangle();
            var w = Math.Min(icon.Width, rect.Width);
            var h = Math.Min(icon.Height, rect.Height);
            var x = rect.X + (rect.Width - w) / 2;
            var y = rect.Y + (rect.Height - h) / 2;
            return new Rectangle(x, y, w, h);
        }

        public Rectangle GetCloseButtonRectangle()
        {
            var x = this.closeButtonRectangle.X - (this.activeTabImage.Width - this.width) + this.drawPoint.X;
            var y = this.closeButtonRectangle.Y + this.drawPoint.Y;
            var w = this.closeButtonRectangle.Width;
            var h = this.closeButtonRectangle.Height;
            return new Rectangle(x, y, w, h);
        }

        public Rectangle GetPageRectangle()
        {
            var x = this.iconRectangle.Right + PAGE_OFFSET + this.drawPoint.X;
            var y = this.iconRectangle.Y + this.drawPoint.Y;
            return Rectangle.FromLTRB(x, y, this.closeButtonRectangle.X - (this.activeTabImage.Width - this.width) + this.drawPoint.X - PAGE_OFFSET, y + this.iconRectangle.Height);
        }

        private void DrawTab(Bitmap bmp, Graphics g)
        {
            g.DrawImage(bmp, this.GetDestCenterRectangle(), this.GetSourceCenterRectangle(), GraphicsUnit.Pixel);
        }

        private void DrawTabCloseButton(Graphics g, bool isMousePoint, bool isActiveTab)
        {
            const int OFFSET = 8;
            var rect = this.GetCloseButtonRectangle();
            var bgRect = new RectangleF(rect.Left + OFFSET / 2f, rect.Top + OFFSET / 2f, rect.Width - OFFSET, rect.Height - OFFSET);
            var slashP1 = new Point(rect.Left + OFFSET, rect.Top + OFFSET);
            var backSlashP1 = new Point(rect.Right - OFFSET, rect.Bottom - OFFSET);
            var slashP2 = new Point(rect.Right - OFFSET, rect.Top + OFFSET);
            var backSlashP2 = new Point(rect.Left + OFFSET, rect.Bottom - OFFSET);

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

        private Rectangle GetDestCenterRectangle()
        {
            var x = this.drawPoint.X;
            var y = this.drawPoint.Y;
            var w = this.width;
            var h = this.height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle GetSourceCenterRectangle()
        {
            var x = 0;
            var y = 0;
            var w = this.activeTabImage.Width;
            var h = this.height;
            return new Rectangle(x, y, w, h);
        }

        #endregion
    }
}
