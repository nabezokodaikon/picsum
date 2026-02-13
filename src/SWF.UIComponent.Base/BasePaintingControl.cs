using System.ComponentModel;

namespace SWF.UIComponent.Base
{

    public class BasePaintingControl
        : Control
    {
        public event EventHandler? Loaded = null;

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

        public bool IsLoaded { get; private set; } = false;

        public BasePaintingControl()
        {
            this.AutoSize = false;
            this.DoubleBuffered = false;

            this.HandleCreated += this.BasePaintingControl_HandleCreated;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint,
                true);
            this.SetStyle(
                ControlStyles.ContainerControl,
                true);
            this.UpdateStyles();
        }

        protected virtual void OnLoaded(EventArgs e)
        {
            this.Loaded?.Invoke(this, e);
        }

        private void BasePaintingControl_HandleCreated(object? sender, EventArgs e)
        {
            if (this.IsLoaded)
            {
                return;
            }

            this.IsLoaded = true;
            this.OnLoaded(EventArgs.Empty);
        }
    }
}
