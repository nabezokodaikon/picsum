using System;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 描画パラメータ
    /// </summary>
    internal sealed class DrawParameter
    {
        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColCount { get; private set; }

        /// <summary>
        /// 描画する最初の行
        /// </summary>
        public int DrawFirstRow { get; private set; }

        /// <summary>
        /// 描画する最後の行
        /// </summary>
        public int DrawLastRow { get; private set; }

        /// <summary>
        /// 描画する最初の項目インデックス
        /// </summary>
        public int DrawFirstItemIndex { get; private set; }

        /// <summary>
        /// 描画する最後の項目インデックス
        /// </summary>
        public int DrawLastItemIndex { get; private set; }

        /// <summary>
        /// 項目間の横スペース
        /// </summary>
        public int ItemSideSpace { get; private set; }

        /// <summary>
        /// スクロールバー最大値
        /// </summary>
        public int ScrollBarMaximum { get; private set; }

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
                throw new ArgumentException("行数が1未満です。", nameof(rowCount));
            }

            if (colCount < 1)
            {
                throw new ArgumentException("列数が1未満です。", nameof(colCount));
            }

            if (drawFirstRow < 0)
            {
                throw new ArgumentException("描画する最初の行が0未満です。", nameof(drawFirstRow));
            }

            if (drawLastRow < 0)
            {
                throw new ArgumentException("描画する最後の行が0未満です。", nameof(drawLastRow));
            }

            if (drawFirstItemIndex < 0)
            {
                throw new ArgumentException("描画する最初の項目インデックスが0未満です。", nameof(drawFirstItemIndex));
            }

            if (drawLastItemIndex < 0)
            {
                throw new ArgumentException("描画する最後の項目インデックスが0未満です。", nameof(drawLastItemIndex));
            }

            if (itemSideSpace < 0)
            {
                throw new ArgumentException("項目間の横スペースが0未満です。", nameof(itemSideSpace));
            }

            this.RowCount = rowCount;
            this.ColCount = colCount;
            this.DrawFirstRow = drawFirstRow;
            this.DrawLastRow = drawLastRow;
            this.DrawFirstItemIndex = drawFirstItemIndex;
            this.DrawLastItemIndex = drawLastItemIndex;
            this.ItemSideSpace = itemSideSpace;
            this.ScrollBarMaximum = scrollBarMaximum;
        }
    }
}
