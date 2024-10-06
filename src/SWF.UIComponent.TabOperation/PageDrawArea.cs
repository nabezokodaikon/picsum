using SWF.Core.ImageAccessor;
using SWF.UIComponent.TabOperation.Properties;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows")]
    public sealed class PageDrawArea
    {
        #region 定数・列挙

        private const int TAB_OVERLAP = 1;

        #endregion

        #region クラスメンバ

        private static Color GetOutlineColor()
        {
            return Color.FromArgb(128, 128, 128);
        }

        private static Color GetInnerColor()
        {
            return Color.FromArgb(241, 244, 250);
        }

        #endregion

        #region インスタンス変数

        private readonly SolidBrush outlineBrush = new(GetOutlineColor());
        private readonly SolidBrush innerBrush = new(GetInnerColor());
        private readonly int top = 29 - TAB_OVERLAP;

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

        public Color PageColor
        {
            get
            {
                return this.innerBrush.Color;
            }
        }

        public SolidBrush PageBrush
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
            ArgumentNullException.ThrowIfNull(nameof(g));

            this.DrawOutline(g);
            this.DrawInnerRectangle(g);
        }

        private void DrawOutline(Graphics g)
        {
            var x = (int)g.ClipBounds.X;
            var y = this.top;
            var w = (int)g.ClipBounds.Width;
            var h = 1;
            g.FillRectangle(this.outlineBrush, x, y, w, h);
        }

        private void DrawInnerRectangle(Graphics g)
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
