using System.ComponentModel;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class BaseControl
        : Control
    {
        public event EventHandler? Loaded = null;

        private bool _isHandleCreated = false;
        private bool _isParentChanged = false;

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

        public BaseControl()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick,
                true);
            this.SetStyle(
                ControlStyles.ContainerControl,
                false);
            this.UpdateStyles();

            this.AutoSize = false;
            this.DoubleBuffered = true;

            this.HandleCreated += this.BaseControl_HandleCreated;
            this.ParentChanged += this.BaseControl_ParentChanged;
        }

        protected virtual void OnLoaded(EventArgs e)
        {
            this.Loaded?.Invoke(this, e);
        }

        private void BaseControl_HandleCreated(object? sender, EventArgs e)
        {
            if (this.IsLoaded)
            {
                return;
            }

            this._isHandleCreated = true;

            if (this._isParentChanged)
            {
                this.IsLoaded = true;
                this.OnLoaded(EventArgs.Empty);
            }
        }

        private void BaseControl_ParentChanged(object? sender, EventArgs e)
        {
            if (this.IsLoaded)
            {
                return;
            }

            this._isParentChanged = true;

            if (this._isHandleCreated)
            {
                this.IsLoaded = true;
                this.OnLoaded(EventArgs.Empty);
            }
        }
    }
}
