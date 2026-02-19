using System.ComponentModel;
using WinApi;

namespace SWF.UIComponent.Base
{

    public class BaseForm
        : Form
    {
        public event EventHandler? Moving;
        public event EventHandler? Moved;

        private FormWindowState _currentWindowState = FormWindowState.Normal;
        private Rectangle _currentWindowBounds = Rectangle.Empty;

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

        public Rectangle CurrentWindowBounds
        {
            get
            {
                return this._currentWindowBounds;
            }
        }

        public BaseForm()
        {
            this.DoubleBuffered = false;
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoSize = false;

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

            this.Load += this.BaseForm_Load;
            this.LocationChanged += this.BaseForm_LocationChanged;
            this.Resize += this.BaseForm_Resize;
        }

        public void MouseLeftDoubleClickProcess()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        public void RestoreWindowState()
        {
            this.Bounds = this._currentWindowBounds;
            this.WindowState = this._currentWindowState;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApiMembers.WM_MOVING)
            {
                // キャプチャ時のドラッグ中
                this.OnMoving(EventArgs.Empty);
            }
            else if (m.Msg == WinApiMembers.WM_EXITSIZEMOVE)
            {
                // キャプチャ時のドラッグ終了
                this.OnMoved(EventArgs.Empty);
            }

            base.WndProc(ref m);
        }

        protected void OnMoving(EventArgs arg)
        {
            this.Moving?.Invoke(this, arg);
        }

        protected void OnMoved(EventArgs arg)
        {
            this.Moved?.Invoke(this, arg);
        }

        private void BaseForm_Load(object? sender, EventArgs e)
        {
            this.IsLoaded = true;
        }

        private void BaseForm_LocationChanged(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {

            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this._currentWindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this._currentWindowState = FormWindowState.Normal;
                this._currentWindowBounds = this.Bounds;
            }
        }

        private void BaseForm_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {

            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this._currentWindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this._currentWindowState = FormWindowState.Normal;
                this._currentWindowBounds = this.Bounds;
            }
        }
    }
}
