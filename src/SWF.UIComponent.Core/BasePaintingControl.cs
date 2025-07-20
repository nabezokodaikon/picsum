using System.ComponentModel;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class BasePaintingControl
        : Control
    {
        public event EventHandler? Loaded = null;

        private readonly new bool IsHandleCreated = false;
        private bool _isHandleCreated = false;
        private bool _isParentChanged = false;

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
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
            this.SetStyle(
                ControlStyles.ContainerControl,
                false);
            this.UpdateStyles();

            this.DoubleBuffered = true;

            this.HandleCreated += this.BasePaintingControl_HandleCreated;
            this.ParentChanged += this.BasePaintingControl_ParentChanged;
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

            this._isHandleCreated = true;

            if (this._isParentChanged)
            {
                this.IsLoaded = true;
                this.OnLoaded(EventArgs.Empty);
            }
        }

        private void BasePaintingControl_ParentChanged(object? sender, EventArgs e)
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
