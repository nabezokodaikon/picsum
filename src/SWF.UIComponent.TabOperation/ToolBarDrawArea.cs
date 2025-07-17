using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using WinApi;

namespace SWF.UIComponent.TabOperation
{
    public class ToolBarDrawArea
    {
        #region 定数・列挙

        private const int TAB_OVERLAP = 12;
        private const int SIDE_WIDTH = 8;

        #endregion

        #region インスタンス変数

        private readonly Bitmap _toolBarImage = Resources.ToolBar;
        private readonly int _top = Resources.TabMask.Height - TAB_OVERLAP;
        private int _width = Resources.ToolBarMask.Width;
        private readonly int _height = Resources.ToolBarMask.Height;

        #endregion

        #region プロパティ

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        #endregion

        #region コンストラクタ

        public ToolBarDrawArea()
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

            //g.DrawImage(_toolBarImage, getDestLeftRectangle(), getSourceLeftRectangle(), GraphicsUnit.Pixel);
            //g.DrawImage(_toolBarImage, getDestRightRectangle(), getSourceRightRectangle(), GraphicsUnit.Pixel);
            //g.DrawImage(_toolBarImage, getDestCenterRectangle(), getSourceCenterRectangle(), GraphicsUnit.Pixel);
        }

        private Rectangle getDestCenterRectangle()
        {
            int x = SIDE_WIDTH;
            int y = _top;
            int w = _width - SIDE_WIDTH * 2;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getDestLeftRectangle()
        {
            int x = -4;
            int y = _top;
            int w = SIDE_WIDTH;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getDestRightRectangle()
        {
            int x = _width - SIDE_WIDTH;
            int y = _top;
            int w = SIDE_WIDTH;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getSourceCenterRectangle()
        {
            int x = SIDE_WIDTH;
            int y = 0;
            int w = _toolBarImage.Width - SIDE_WIDTH * 2;
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
            int x = _toolBarImage.Width - SIDE_WIDTH;
            int y = 0;
            int w = SIDE_WIDTH;
            int h = _height;
            return new Rectangle(x, y, w, h);
        }

        #endregion
    }
}
