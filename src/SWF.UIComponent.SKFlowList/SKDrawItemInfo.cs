using SkiaSharp;
using System;

namespace SWF.UIComponent.SKFlowList
{
    /// <summary>
    /// 項目描画イベントクラス
    /// </summary>
    public sealed class SKDrawItemInfo
    {
        /// <summary>
        /// 項目のインデックス
        /// </summary>
        public int ItemIndex { get; private set; }

        /// <summary>
        /// 項目の描画領域
        /// </summary>
        public SKRectI ItemRectangle { get; private set; }

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

        public SKDrawItemInfo(
            int itemIndex,
            SKRectI itemRectangle,
            bool isSelected,
            bool isMousePoint,
            bool isFocus)
        {
            if (itemIndex < 0)
            {
                throw new ArgumentException("項目のインデックスが0未満です。", nameof(itemIndex));
            }

            this.ItemIndex = itemIndex;
            this.ItemRectangle = itemRectangle;
            this.IsSelected = isSelected;
            this.IsMousePoint = isMousePoint;
            this.IsFocus = isFocus;
        }
    }
}
