using System.ComponentModel;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 垂直スクロールバー拡張
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class VScrollBarEx
        : VScrollBar
    {
        /// <summary>
        /// 最大値
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        public VScrollBarEx()
        {
            this.SetStyle(
                ControlStyles.Selectable,
                false);
        }

        private int GetMargin()
        {
            return this.LargeChange - 1;
        }
    }
}
