using System;
using System.Drawing;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 項目描画イベントクラス
    /// </summary>
    public class DrawItemEventArgs : EventArgs
    {
        private readonly Graphics _graphics;
        private readonly int _itemIndex;
        private readonly Rectangle _itemRectangle;
        private readonly bool _isSelected = false;
        private readonly bool _isMousePoint = false;
        private readonly bool _isFocus = false;

        /// <summary>
        /// 描画オブジェクト
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                return _graphics;
            }
        }

        /// <summary>
        /// 項目のインデックス
        /// </summary>
        public int ItemIndex
        {
            get
            {
                return _itemIndex;
            }
        }

        /// <summary>
        /// 項目の描画領域
        /// </summary>
        public Rectangle ItemRectangle
        {
            get
            {
                return _itemRectangle;
            }
        }

        /// <summary>
        /// 項目が選択されているか、されていないかを表します。
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
        }

        /// <summary>
        /// 項目がマウスポイントされているか、されていないかを表します。
        /// </summary>
        public bool IsMousePoint
        {
            get
            {
                return _isMousePoint;
            }
        }

        /// <summary>
        /// 項目がフォーカスされているか、されていないかを表します。
        /// </summary>
        public bool IsFocus
        {
            get
            {
                return _isFocus;
            }
        }

        public DrawItemEventArgs(Graphics graphics,
                                 int itemIndex, Rectangle itemRectangle,
                                 bool isSelected, bool isMousePoint, bool isFocus)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            if (itemIndex < 0)
            {
                throw new ArgumentException("項目のインデックスが0未満です。", "itemIndex");
            }

            _graphics = graphics;
            _itemIndex = itemIndex;
            _itemRectangle = itemRectangle;
            _isSelected = isSelected;
            _isMousePoint = isMousePoint;
            _isFocus = isFocus;
        }
    }
}
