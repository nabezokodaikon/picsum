using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 垂直スクロールバー拡張
    /// </summary>

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

        private new int Minimum
        {
            get
            {
                return base.Minimum;
            }
        }

        /// <summary>
        /// 最大値
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = Math.Min(value, base.Maximum);
            }
        }

        public VScrollBarEx()
        {
            this.DoubleBuffered = false;
            base.Minimum = 0;

            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick,
                true);
            this.SetStyle(
                ControlStyles.ContainerControl |
                ControlStyles.Selectable,
                false);
            this.UpdateStyles();
        }

        private int GetMargin()
        {
            return this.LargeChange - 1;
        }
    }
}
