using SWF.Core.Base;
using SWF.Core.Resource;
using System.ComponentModel;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    /// <summary>
    /// スライダーコントロール
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class Slider : Control
    {

        private const int BarHeight = 4;
        private const int BarShadowOffset = 1;

        public event EventHandler? BeginValueChange;
        public event EventHandler? ValueChanging;
        public event EventHandler? ValueChanged;

        private readonly Image button = ResourceFiles.SliderButtonIcon.Value;
        private float buttonPointX = ResourceFiles.SliderButtonIcon.Value.Width / 2f;
        private int maximumValue = 100;
        private int minimumValue = 0;
        private int sliderValue = 0;
        private bool isValueChanging = false;

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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaximumValue
        {
            get
            {
                return this.maximumValue;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, this.minimumValue, nameof(value));

                this.maximumValue = value;

                if (this.sliderValue > this.maximumValue)
                {
                    this.sliderValue = this.maximumValue;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MinimumValue
        {
            get
            {
                return this.minimumValue;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, this.maximumValue, nameof(value));

                this.minimumValue = value;
                if (this.sliderValue < value)
                {
                    this.sliderValue = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Value
        {
            get
            {
                return this.sliderValue;
            }
            set
            {
                if (value > this.maximumValue || this.minimumValue > value)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (this.sliderValue == value)
                {
                    return;
                }

                this.sliderValue = value;
                this.SetButtonPointX(this.sliderValue);
                this.Invalidate();

                this.OnValueChanged(EventArgs.Empty);
            }
        }

        public Slider()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.sliderValue = this.minimumValue;
        }

        private float GetBarHeight()
        {
            var scale = AppConstants.GetCurrentWindowScale(this);
            return BarHeight * scale;
        }

        private PointF GetCenterPoint()
        {
            return new PointF(this.Width / 2f, this.Height / 2f);
        }

        private float GetButtonPointX(float x)
        {
            if (x < this.GetMinimumButtonPointX())
            {
                return this.GetMinimumButtonPointX();
            }
            else if (x > this.GetMaximumButtonPointX())
            {
                return this.GetMaximumButtonPointX();
            }
            else
            {
                return x;
            }
        }

        private bool SetButtonPointX(int value)
        {
            if (value > this.maximumValue || this.minimumValue > value)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value == this.minimumValue)
            {
                var pointX = this.GetButtonPointX(this.GetMinimumButtonPointX());
                if (pointX != this.buttonPointX)
                {
                    this.buttonPointX = pointX;
                    return true;
                }
            }
            else if (value == this.maximumValue)
            {
                var pointX = this.GetButtonPointX(this.GetMaximumButtonPointX());
                if (pointX != this.buttonPointX)
                {
                    this.buttonPointX = pointX;
                    return true;
                }
            }
            else
            {
                var rate = (value - this.minimumValue) / (float)(this.maximumValue - this.minimumValue);
                var pointX = this.GetButtonPointX(this.Width * rate);
                if (pointX != this.buttonPointX)
                {
                    this.buttonPointX = pointX;
                    return true;
                }
            }

            return false;
        }

        private int GetValue(float x)
        {
            if (x == this.GetMinimumButtonPointX())
            {
                return this.minimumValue;
            }
            else if (x == this.GetMaximumButtonPointX())
            {
                return this.maximumValue;
            }
            else
            {
                var rate = x / this.GetMaximumButtonPointX();
                var value = this.minimumValue + ((this.maximumValue - this.minimumValue) * rate);
                return (int)value;
            }
        }

        private float GetMaximumButtonPointX()
        {
            var scale = AppConstants.GetCurrentWindowScale(this);
            var scaleButtonWidth = (this.button.Width) * scale;
            return this.Width - scaleButtonWidth / 2f;
        }

        private float GetMinimumButtonPointX()
        {
            var scale = AppConstants.GetCurrentWindowScale(this);
            var scaleButtonWidth = (this.button.Width) * scale;
            return scaleButtonWidth / 2f;
        }

        private void DrawBar(Graphics g)
        {
            var centerPoint = this.GetCenterPoint();

            var barHeight = this.GetBarHeight();

            var shadowRect = new RectangleF(0,
                                            centerPoint.Y - barHeight / 2f,
                                            this.Width - BarShadowOffset,
                                            barHeight);

            g.FillRectangle(Brushes.DimGray, shadowRect);

            shadowRect.Offset(BarShadowOffset, BarShadowOffset);

            g.FillRectangle(Brushes.White, shadowRect);

            var mainRect = new RectangleF(BarShadowOffset,
                                          centerPoint.Y - barHeight / 2f + BarShadowOffset,
                                          this.Width - BarShadowOffset * 2,
                                          barHeight - BarShadowOffset);

            g.FillRectangle(Brushes.LightGray, mainRect);
        }

        private void DrawButton(Graphics g)
        {
            var scale = AppConstants.GetCurrentWindowScale(this);
            var scaleButtonSize = new SizeF(this.button.Width * scale, this.button.Height * scale);
            var centerPoint = this.GetCenterPoint();

            var rect = new RectangleF(this.buttonPointX - scaleButtonSize.Width / 2f,
                                      centerPoint.Y - scaleButtonSize.Height / 2f,
                                      scaleButtonSize.Width,
                                      scaleButtonSize.Height);

            g.DrawImage(this.button, rect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.DrawBar(e.Graphics);
            this.DrawButton(e.Graphics);

            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.MaximumValue < 2)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                this.OnBeginValueChange(EventArgs.Empty);

                var pointX = this.GetButtonPointX(e.X);
                if (pointX != this.buttonPointX)
                {
                    this.buttonPointX = pointX;
                    this.Invalidate();

                    var value = this.GetValue(pointX);
                    if (value != this.sliderValue)
                    {
                        this.sliderValue = value;
                        this.isValueChanging = true;
                        this.OnValueChanging(EventArgs.Empty);
                    }
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.MaximumValue < 2)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                this.SetButtonPointX(this.sliderValue);
                this.Invalidate();

                if (this.isValueChanging)
                {
                    this.isValueChanging = false;
                }
                else
                {
                    this.OnValueChanged(EventArgs.Empty);
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.MaximumValue < 2)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                var pointX = this.GetButtonPointX(e.X);
                if (pointX != this.buttonPointX)
                {
                    this.buttonPointX = pointX;
                    this.Invalidate();
                    this.Update();

                    var value = this.GetValue(pointX);
                    if (value != this.sliderValue)
                    {
                        this.sliderValue = value;
                        this.isValueChanging = true;
                        this.OnValueChanging(EventArgs.Empty);
                    }
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            this.SetButtonPointX(this.sliderValue);
            this.Invalidate();
            base.OnResize(e);
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
    }
}
