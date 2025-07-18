using System.ComponentModel;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class BaseControl
        : Control
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

        public BaseControl()
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
    }
}
