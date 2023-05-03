using SWF.Common;
using SWF.UIComponent.TabOperation.Properties;
using System;
using System.Drawing;

namespace SWF.UIComponent.TabOperation
{
    public class ContentsDrawArea
    {
        #region 定数・列挙

        private const int TAB_OVERLAP = 1;
        private const int SIDE_WIDTH = 16;

        #endregion

        #region クラスメンバ

        private static Color GetOutlineColor()
        {
            Bitmap bmp = Resources.ActiveTab;
            int x = 0;
            int y = bmp.Height - 1;
            return ImageUtil.GetPixel(bmp, x, y);
        }

        private static Color GetInnerColor()
        {
            Bitmap bmp = Resources.ActiveTab;
            int x = bmp.Width / 2;
            int y = bmp.Height - 1;
            return ImageUtil.GetPixel(bmp, x, y);
        }

        #endregion

        #region インスタンス変数

        private readonly SolidBrush _outlineBrush = new SolidBrush(GetOutlineColor());
        private readonly SolidBrush _innerBrush = new SolidBrush(GetInnerColor());
        private readonly int _top = Resources.ActiveTab.Height - TAB_OVERLAP;

        #endregion

        #region プロパティ

        public Color OutlineColor
        {
            get
            {
                return _outlineBrush.Color;
            }
        }

        public SolidBrush OutlineBrush
        {
            get
            {
                return _outlineBrush;
            }
        }

        public Color ContentsColor
        {
            get
            {
                return _innerBrush.Color;
            }
        }

        public SolidBrush ContentsBrush
        {
            get
            {
                return _innerBrush;
            }
        }

        #endregion

        #region コンストラクタ

        public ContentsDrawArea()
        {

        }

        #endregion

        #region メソッド

        public void Draw(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            drawOutline(g);
            drawInnerRectangle(g);
        }

        private void drawOutline(Graphics g)
        {
            int x = (int)g.ClipBounds.X;
            int y = _top;
            int w = (int)g.ClipBounds.Width;
            int h = 1;
            g.FillRectangle(_outlineBrush, x, y, w, h);
        }

        private void drawInnerRectangle(Graphics g)
        {
            int x = (int)g.ClipBounds.X;
            int y = _top + 1;
            int w = (int)g.ClipBounds.Width;
            int h = (int)g.ClipBounds.Height - y;
            g.FillRectangle(_innerBrush, x, y, w, h);
        }

        #endregion
    }
}
