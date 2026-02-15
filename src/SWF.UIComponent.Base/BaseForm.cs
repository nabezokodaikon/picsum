using System.ComponentModel;
using WinApi;

namespace SWF.UIComponent.Base
{

    public class BaseForm
        : Form
    {
        public event EventHandler? Moving;
        public event EventHandler? Moved;

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
    }
}
