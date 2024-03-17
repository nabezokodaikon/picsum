using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 垂直スクロールバー拡張
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class VScrollBarEx
        : VScrollBar
    {
        /// <summary>
        /// 最大値
        /// </summary>
        public new int Maximum
        {
            get
            {
                return base.Maximum - this.GetMargin();
            }
            set
            {
                base.Maximum = value + this.GetMargin();
            }
        }

        private int GetMargin()
        {
            return this.LargeChange - 1;
        }
    }
}
