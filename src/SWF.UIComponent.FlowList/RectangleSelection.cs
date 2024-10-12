using System;
using System.Drawing;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 短形選択クラス
    /// </summary>
    internal sealed class RectangleSelection
    {
        private bool isUse = false;
        private bool isBegun = false;
        private Point drawFromPoint = Point.Empty;
        private Rectangle virtualRectangle = Rectangle.Empty;

        public bool IsUse
        {
            get
            {
                return this.isUse;
            }
            set
            {
                this.isUse = value;
            }
        }

        public bool IsBegun
        {
            get
            {
                return this.isBegun;
            }
        }

        public Rectangle VirtualRectangle
        {
            get
            {
                return this.virtualRectangle;
            }
        }

        public Rectangle GetDrawRectangle(int scrollValue)
        {
            return new Rectangle(this.virtualRectangle.X,
                                 this.virtualRectangle.Y - scrollValue,
                                 this.virtualRectangle.Width,
                                 this.virtualRectangle.Height);
        }

        public void BeginSelection(int drawX, int drawY, int scrollValue)
        {
            if (!this.isUse)
            {
                return;
            }

            if (this.isBegun)
            {
                throw new InvalidOperationException("短形選択は開始されています。");
            }

            this.drawFromPoint = new Point(drawX, drawY + scrollValue);
            this.isBegun = true;
        }

        public void ChangeSelection(int drawX, int drawY, int scrollValue)
        {
            var x = Math.Min(this.drawFromPoint.X, drawX);
            var y = Math.Min(this.drawFromPoint.Y, drawY + scrollValue);
            var w = Math.Abs(this.drawFromPoint.X - drawX);
            var h = Math.Abs(this.drawFromPoint.Y - (drawY + scrollValue));
            this.virtualRectangle = new Rectangle(x, y, w, h);
        }

        public void EndSelection()
        {
            this.drawFromPoint = Point.Empty;
            this.virtualRectangle = Rectangle.Empty;
            this.isBegun = false;
        }
    }
}
