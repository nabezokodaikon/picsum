using System;
using System.Drawing;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 短形選択クラス
    /// </summary>
    internal class RectangleSelection
    {
        private bool _isUse = false;
        private bool _isBegun = false;
        private Point _drawFromPoint = Point.Empty;
        private Rectangle _virtualRectangle = Rectangle.Empty;

        public bool IsUse
        {
            get
            {
                return _isUse;
            }
            set
            {
                _isUse = value;
            }
        }

        public bool IsBegun
        {
            get
            {
                return _isBegun;
            }
        }

        public Rectangle VirtualRectangle
        {
            get
            {
                return _virtualRectangle;
            }
        }

        public Rectangle GetDrawRectangle(int scrollValue)
        {
            return new Rectangle(_virtualRectangle.X,
                                 _virtualRectangle.Y - scrollValue,
                                 _virtualRectangle.Width,
                                 _virtualRectangle.Height);
        }

        public void BeginSelection(int drawX, int drawY, int scrollValue)
        {
            if (!_isUse)
            {
                return;
            }

            if (_isBegun)
            {
                throw new Exception("短形選択は開始されています。");
            }

            _drawFromPoint = new Point(drawX, drawY + scrollValue);
            _isBegun = true;
        }

        public void ChangeSelection(int drawX, int drawY, int scrollValue)
        {
            int x = Math.Min(_drawFromPoint.X, drawX);
            int y = Math.Min(_drawFromPoint.Y, drawY + scrollValue);
            int w = Math.Abs(_drawFromPoint.X - drawX);
            int h = Math.Abs(_drawFromPoint.Y - (drawY + scrollValue));
            _virtualRectangle = new Rectangle(x, y, w, h);
        }

        public void EndSelection()
        {
            _drawFromPoint = Point.Empty;
            _virtualRectangle = Rectangle.Empty;
            _isBegun = false;
        }
    }
}
