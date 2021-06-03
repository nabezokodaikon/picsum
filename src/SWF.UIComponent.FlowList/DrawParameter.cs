using System;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 描画パラメータ
    /// </summary>
    internal class DrawParameter
    {
        private int _rowCount = 0;
        private int _colCount = 0;
        private int _drawFirstRow = -1;
        private int _drawLastRow = -1;
        private int _drawFirstItemIndex = -1;
        private int _drawLastItemIndex = -1;
        private int _itemSideSpace = 0;
        private int _scrollBarMaximum = 0;

        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount
        {
            get
            {
                return _rowCount;
            }
        }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColCount
        {
            get
            {
                return _colCount;
            }
        }

        /// <summary>
        /// 描画する最初の行
        /// </summary>
        public int DrawFirstRow
        {
            get
            {
                return _drawFirstRow;
            }
        }

        /// <summary>
        /// 描画する最後の行
        /// </summary>
        public int DrawLastRow
        {
            get
            {
                return _drawLastRow;
            }
        }

        /// <summary>
        /// 描画する最初の項目インデックス
        /// </summary>
        public int DrawFirstItemIndex
        {
            get
            {
                return _drawFirstItemIndex;
            }
        }

        /// <summary>
        /// 描画する最後の項目インデックス
        /// </summary>
        public int DrawLastItemIndex
        {
            get
            {
                return _drawLastItemIndex;
            }
        }

        /// <summary>
        /// 項目間の横スペース
        /// </summary>
        public int ItemSideSpace
        {
            get
            {
                return _itemSideSpace;
            }
        }

        /// <summary>
        /// スクロールバー最大値
        /// </summary>
        public int ScrollBarMaximum
        {
            get
            {
                return _scrollBarMaximum;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DrawParameter() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <param name="drawFirstRow"></param>
        /// <param name="drawLastRow"></param>
        /// <param name="drawFirstItemIndex"></param>
        /// <param name="drawLastItemIndex"></param>
        /// <param name="itemSideSpace"></param>
        /// <param name="scrollBarMaximum"></param>
        /// <param name="virtualRectangle"></param>
        /// <param name="drawRectangle"></param>
        public DrawParameter(int rowCount, int colCount,
                             int drawFirstRow, int drawLastRow,
                             int drawFirstItemIndex, int drawLastItemIndex,
                             int itemSideSpace, int scrollBarMaximum)
        {
            if (rowCount < 1)
            {
                throw new ArgumentException("行数が1未満です。", "rowCount");
            }

            if (colCount < 1)
            {
                throw new ArgumentException("列数が1未満です。", "colCount");
            }

            if (drawFirstRow < 0)
            {
                throw new ArgumentException("描画する最初の行が0未満です。", "drawFirstRow");
            }

            if (drawLastRow < 0)
            {
                throw new ArgumentException("描画する最後の行が0未満です。", "drawLastRow");
            }

            if (drawFirstItemIndex < 0)
            {
                throw new ArgumentException("描画する最初の項目インデックスが0未満です。", "drawFirstItemIndex");
            }

            if (drawLastItemIndex < 0)
            {
                throw new ArgumentException("描画する最後の項目インデックスが0未満です。", "drawLastItemIndex");
            }

            if (itemSideSpace < 0)
            {
                throw new ArgumentException("項目間の横スペースが0未満です。", "itemSideSpace");
            }

            _rowCount = rowCount;
            _colCount = colCount;
            _drawFirstRow = drawFirstRow;
            _drawLastRow = drawLastRow;
            _drawFirstItemIndex = drawFirstItemIndex;
            _drawLastItemIndex = drawLastItemIndex;
            _itemSideSpace = itemSideSpace;
            _scrollBarMaximum = scrollBarMaximum;
        }
    }
}
