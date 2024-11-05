using System.Runtime.Versioning;
using System.Windows.Forms.Design;

namespace SWF.UIComponent.Core
{
    /// <summary>
    /// ツールストリップスライダー
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
    public partial class ToolStripSlider : ToolStripControlHost
    {

        public event EventHandler? BeginValueChange;
        public event EventHandler? ValueChanging;
        public event EventHandler? ValueChanged;

        public int MaximumValue
        {
            get
            {
                return this.Slider.MaximumValue;
            }
            set
            {
                this.Slider.MaximumValue = value;
            }
        }

        public int MinimumValue
        {
            get
            {
                return this.Slider.MinimumValue;
            }
            set
            {
                this.Slider.MinimumValue = value;
            }
        }

        public int Value
        {
            get
            {
                return this.Slider.Value;
            }
            set
            {
                this.Slider.Value = value;
            }
        }

        private Slider Slider
        {
            get
            {
                return (Slider)this.Control;
            }
        }

        public ToolStripSlider()
            : base(new Slider())
        {
            this.Slider.Size = new(96, 24);
            this.Slider.BackColor = Color.Transparent;
        }

        protected override void OnSubscribeControlEvents(Control? control)
        {
            ArgumentNullException.ThrowIfNull(control, nameof(control));

            base.OnSubscribeControlEvents(control);
            var slider = (Slider)control;
            slider.BeginValueChange += new(this.Slider_BeginValueChange);
            slider.ValueChanging += new(this.Slider_ValueChanging);
            slider.ValueChanged += new(this.Slider_ValueChanged);
        }

        protected override void OnUnsubscribeControlEvents(Control? control)
        {
            ArgumentNullException.ThrowIfNull(control, nameof(control));

            base.OnSubscribeControlEvents(control);
            var slider = (Slider)control;
            slider.BeginValueChange -= new(this.Slider_BeginValueChange);
            slider.ValueChanging -= new(this.Slider_ValueChanging);
            slider.ValueChanged -= new(this.Slider_ValueChanged);
        }

        protected virtual void OnBeginValueChange(EventArgs e)
        {
            BeginValueChange?.Invoke(this, e);
        }

        protected virtual void OnValueChanging(EventArgs e)
        {
            ValueChanging?.Invoke(this, e);
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        private void Slider_BeginValueChange(object? sender, EventArgs e)
        {
            this.OnBeginValueChange(e);
        }

        private void Slider_ValueChanging(object? sender, EventArgs e)
        {
            this.OnValueChanging(e);
        }

        private void Slider_ValueChanged(object? sender, EventArgs e)
        {
            this.OnValueChanged(e);
        }

    }
}
