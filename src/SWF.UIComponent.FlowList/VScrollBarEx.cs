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
        public new string Name
        {
            get
            {
                return base.Name;
            }
            private set
            {
                base.Name = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool DoubleBuffered
        {
            get
            {
                return base.DoubleBuffered;
            }
            private set
            {
                base.DoubleBuffered = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            private set
            {
                base.TabIndex = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            private set
            {
                base.TabStop = value;
            }
        }

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
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
            this.SetStyle(
                ControlStyles.ContainerControl |
                ControlStyles.Selectable,
                false);
            this.UpdateStyles();

            this.DoubleBuffered = true;
        }

        private int GetMargin()
        {
            return this.LargeChange - 1;
        }
    }
}
