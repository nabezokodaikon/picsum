using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 垂直スクロールバー拡張
    /// </summary>
    internal class VScrollBarEx : VScrollBar
    {
        /// <summary>
        /// 最大値
        /// </summary>
        public new int Maximum
        {
            get
            {
                return base.Maximum - getMargin();
            }
            set
            {
                base.Maximum = value + getMargin();
            }
        }

        private int getMargin()
        {
            return this.LargeChange - 1;
        }
    }
}
