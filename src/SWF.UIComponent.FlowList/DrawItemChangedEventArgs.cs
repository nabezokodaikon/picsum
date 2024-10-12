using System;

namespace SWF.UIComponent.FlowList
{
    public sealed class DrawItemChangedEventArgs
        : EventArgs
    {
        /// <summary>
        /// 描画する最初の項目インデックス
        /// </summary>
        public int DrawFirstItemIndex { get; private set; }

        /// <summary>
        /// 描画する最後の項目インデックス
        /// </summary>
        public int DrawLastItemIndex { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="drawFirstItemIndex"></param>
        /// <param name="drawLastItemIndex"></param>
        public DrawItemChangedEventArgs(int drawFirstItemIndex, int drawLastItemIndex)
        {
            this.DrawFirstItemIndex = drawFirstItemIndex;
            this.DrawLastItemIndex = drawLastItemIndex;
        }
    }
}
