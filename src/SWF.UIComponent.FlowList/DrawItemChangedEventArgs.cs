using System;

namespace SWF.UIComponent.FlowList
{
    public class DrawItemChangedEventArgs : EventArgs
    {
        private int _drawFirstItemIndex = -1;
        private int _drawLastItemIndex = -1;

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
        /// コンストラクタ
        /// </summary>
        /// <param name="drawFirstItemIndex"></param>
        /// <param name="drawLastItemIndex"></param>
        public DrawItemChangedEventArgs(int drawFirstItemIndex, int drawLastItemIndex)
        {
            _drawFirstItemIndex = drawFirstItemIndex;
            _drawLastItemIndex = drawLastItemIndex;
        }
    }
}
