using System;
using System.Drawing;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 座標情報
    /// </summary>
    internal class HitTestInfo
    {
        private int _itemIndex = -1;
        private int _row = -1;
        private int _col = -1;
        private Rectangle _virtualRectangle = Rectangle.Empty;
        private Rectangle _drawRectangle = Rectangle.Empty;
        private bool _isItem = false;
        private bool _isSelected = false;
        private bool _isMousePoint = false;
        private bool _isFocus = false;

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
        /// 項目の行
        /// </summary>
        public int Row
        {
            get
            {
                return _row;
            }
        }

        /// <summary>
        /// 項目の列ス
        /// </summary>
        public int Col
        {
            get
            {
                return _col;
            }
        }

        /// <summary>
        /// 項目の仮想領域
        /// </summary>
        public Rectangle VirtualRectangle
        {
            get
            {
                return _virtualRectangle;
            }
        }

        /// <summary>
        /// 項目の描画領域
        /// </summary>
        public Rectangle DrawRectangle
        {
            get
            {
                return _drawRectangle;
            }
        }

        /// <summary>
        /// 項目であるか、そうでないかを表します。
        /// </summary>
        public bool IsItem
        {
            get
            {
                return _isItem;
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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HitTestInfo() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="itemIndex">項目のインデックス</param>
        /// <param name="row">項目の行</param>
        /// <param name="col">項目の列</param>
        /// <param name="virtualRectangle">項目の仮想領域</param>
        /// <param name="drawRectangle">項目の描画領域</param>
        /// <param name="isSelected">項目が選択されているか、されていないかを表します。</param>
        /// <param name="isMousePoint">項目がマウスポイントされているか、されていないかを表します。</param>
        /// <param name="isFocus">項目がフォーカスされているか、されていないかを表します。</param>
        public HitTestInfo(int itemIndex, int row, int col,
                           Rectangle virtualRectangle, Rectangle drawRectangle,
                           bool isItem, bool isSelected, bool isMousePoint, bool isFocus)
        {
            if (itemIndex < 0)
            {
                throw new ArgumentException("項目のインデックスが0未満です。", "itemIndex");
            }

            if (row < 0)
            {
                throw new ArgumentException("項目の行が0未満です。", "row");
            }

            if (col < 0)
            {
                throw new ArgumentException("項目の列が0未満です。", "col");
            }

            _itemIndex = itemIndex;
            _row = row;
            _col = col;
            _virtualRectangle = virtualRectangle;
            _drawRectangle = drawRectangle;
            _isItem = isItem;
            _isSelected = isSelected;
            _isMousePoint = isMousePoint;
            _isFocus = isFocus;
        }
    }
}
