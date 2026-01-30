using System.ComponentModel;

namespace SWF.UIComponent.Base
{

    public class BaseForm
        : Form
    {
        public bool IsLoaded { get; private set; } = false;

        public new bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            private set
            {
                base.AutoSize = value;
            }
        }

        public new AutoScaleMode AutoScaleMode
        {
            get
            {
                return base.AutoScaleMode;
            }
            private set
            {
                base.AutoScaleMode = value;
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

        public BaseForm()
        {
            this.DoubleBuffered = false;
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoSize = false;

            this.Load += this.BaseForm_Load;

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

        private void BaseForm_Load(object? sender, EventArgs e)
        {
            this.IsLoaded = true;
        }
    }
}
