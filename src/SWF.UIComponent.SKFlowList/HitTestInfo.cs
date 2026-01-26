using SkiaSharp;
using System;

namespace SWF.UIComponent.SKFlowList
{
    /// <summary>
    /// 座標情報
    /// </summary>
    internal sealed class HitTestInfo
    {
        /// <summary>
        /// 項目のインデックス
        /// </summary>
        public int ItemIndex { get; private set; }

        /// <summary>
        /// 項目の行
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// 項目の列ス
        /// </summary>
        public int Col { get; private set; }

        /// <summary>
        /// 項目の仮想領域
        /// </summary>
        public SKRect VirtualRectangle { get; private set; }

        /// <summary>
        /// 項目の描画領域
        /// </summary>
        public SKRect DrawRectangle { get; private set; }

        /// <summary>
        /// 項目であるか、そうでないかを表します。
        /// </summary>
        public bool IsItem { get; private set; }

        /// <summary>
        /// 項目が選択されているか、されていないかを表します。
        /// </summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// 項目がマウスポイントされているか、されていないかを表します。
        /// </summary>
        public bool IsMousePoint { get; private set; }

        /// <summary>
        /// 項目がフォーカスされているか、されていないかを表します。
        /// </summary>
        public bool IsFocus { get; private set; }

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
                           SKRect virtualRectangle, SKRect drawRectangle,
                           bool isItem, bool isSelected, bool isMousePoint, bool isFocus)
        {
            if (itemIndex < 0)
            {
                throw new ArgumentException("項目のインデックスが0未満です。", nameof(itemIndex));
            }

            if (row < 0)
            {
                throw new ArgumentException("項目の行が0未満です。", nameof(row));
            }

            if (col < 0)
            {
                throw new ArgumentException("項目の列が0未満です。", nameof(col));
            }

            this.ItemIndex = itemIndex;
            this.Row = row;
            this.Col = col;
            this.VirtualRectangle = virtualRectangle;
            this.DrawRectangle = drawRectangle;
            this.IsItem = isItem;
            this.IsSelected = isSelected;
            this.IsMousePoint = isMousePoint;
            this.IsFocus = isFocus;
        }
    }
}
