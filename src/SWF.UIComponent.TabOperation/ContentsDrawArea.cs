using SWF.Common;
using SWF.UIComponent.TabOperation.Properties;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows")]
    public sealed class ContentsDrawArea
    {
        #region 定数・列挙

        private const int TAB_OVERLAP = 1;
        private const int SIDE_WIDTH = 16;

        #endregion

        #region クラスメンバ

        private static Color GetOutlineColor()
        {
            var bmp = Resources.ActiveTab;
            var x = 0;
            var y = bmp.Height - 1;
            return ImageUtil.GetPixel(bmp, x, y);
        }

        private static Color GetInnerColor()
        {
            var bmp = Resources.ActiveTab;
            var x = bmp.Width / 2;
            var y = bmp.Height - 1;
            return ImageUtil.GetPixel(bmp, x, y);
        }

        #endregion

        #region インスタンス変数

        private readonly SolidBrush outlineBrush = new SolidBrush(GetOutlineColor());
        private readonly SolidBrush innerBrush = new SolidBrush(GetInnerColor());
        private readonly int top = Resources.ActiveTab.Height - TAB_OVERLAP;

        #endregion

        #region プロパティ

        public Color OutlineColor
        {
            get
            {
                return this.outlineBrush.Color;
            }
        }

        public SolidBrush OutlineBrush
        {
            get
            {
                return this.outlineBrush;
            }
        }

        public Color ContentsColor
        {
            get
            {
                return this.innerBrush.Color;
            }
        }

        public SolidBrush ContentsBrush
        {
            get
            {
                return this.innerBrush;
            }
        }

        #endregion

        #region メソッド

        public void Draw(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            this.drawOutline(g);
            this.drawInnerRectangle(g);
        }

        private void drawOutline(Graphics g)
        {
            var x = (int)g.ClipBounds.X;
            var y = this.top;
            var w = (int)g.ClipBounds.Width;
            var h = 1;
            g.FillRectangle(this.outlineBrush, x, y, w, h);
        }

        private void drawInnerRectangle(Graphics g)
        {
            var x = (int)g.ClipBounds.X;
            var y = this.top + 1;
            var w = (int)g.ClipBounds.Width;
            var h = (int)g.ClipBounds.Height - y;
            g.FillRectangle(this.innerBrush, x, y, w, h);
        }

        #endregion
    }
}
