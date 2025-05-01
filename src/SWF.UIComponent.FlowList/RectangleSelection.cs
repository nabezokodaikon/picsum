using System;
using System.Drawing;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 短形選択クラス
    /// </summary>
    internal sealed class RectangleSelection
    {
        private bool _isUse = false;
        private bool _isBegun = false;
        private Point _drawFromPoint = Point.Empty;
        private Rectangle _virtualRectangle = Rectangle.Empty;

        public bool IsUse
        {
            get
            {
                return this._isUse;
            }
            set
            {
                this._isUse = value;
            }
        }

        public bool IsBegun
        {
            get
            {
                return this._isBegun;
            }
        }

        public Rectangle VirtualRectangle
        {
            get
            {
                return this._virtualRectangle;
            }
        }

        public Rectangle GetDrawRectangle(int scrollValue)
        {
            return new Rectangle(this._virtualRectangle.X,
                                 this._virtualRectangle.Y - scrollValue,
                                 this._virtualRectangle.Width,
                                 this._virtualRectangle.Height);
        }

        public void BeginSelection(int drawX, int drawY, int scrollValue)
        {
            if (!this._isUse)
            {
                return;
            }

            if (this._isBegun)
            {
                throw new InvalidOperationException("短形選択は開始されています。");
            }

            this._drawFromPoint = new Point(drawX, drawY + scrollValue);
            this._isBegun = true;
        }

        public void ChangeSelection(int drawX, int drawY, int scrollValue)
        {
            var x = Math.Min(this._drawFromPoint.X, drawX);
            var y = Math.Min(this._drawFromPoint.Y, drawY + scrollValue);
            var w = Math.Abs(this._drawFromPoint.X - drawX);
            var h = Math.Abs(this._drawFromPoint.Y - (drawY + scrollValue));
            this._virtualRectangle = new Rectangle(x, y, w, h);
        }

        public void EndSelection()
        {
            this._drawFromPoint = Point.Empty;
            this._virtualRectangle = Rectangle.Empty;
            this._isBegun = false;
        }
    }
}
