using SWF.Core.ResourceAccessor;
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

        private const int BAR_HEIGHT = 4;
        private const int BAR_SHADOW_OFFSET = 1;

        public event EventHandler? BeginValueChange;
        public event EventHandler? ValueChanging;
        public event EventHandler? ValueChanged;

        private readonly Image _button = ResourceFiles.SliderButtonIcon.Value;
        private float _buttonPointX = ResourceFiles.SliderButtonIcon.Value.Width / 2f;
        private int _maximumValue = 100;
        private int _minimumValue = 0;
        private int _sliderValue = 0;
        private bool _isValueChanging = false;

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
                return this._maximumValue;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, this._minimumValue, nameof(value));

                this._maximumValue = value;

                if (this._sliderValue > this._maximumValue)
                {
                    this._sliderValue = this._maximumValue;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MinimumValue
        {
            get
            {
                return this._minimumValue;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, this._maximumValue, nameof(value));

                this._minimumValue = value;
                if (this._sliderValue < value)
                {
                    this._sliderValue = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Value
        {
            get
            {
                return this._sliderValue;
            }
            set
            {
                if (value > this._maximumValue || this._minimumValue > value)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (this._sliderValue == value)
                {
                    return;
                }

                this._sliderValue = value;
                this.SetButtonPointX(this._sliderValue);
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
            this.SetStyle(
                ControlStyles.Selectable,
                false);
            this.UpdateStyles();

            this._sliderValue = this._minimumValue;
        }

        private float GetBarHeight()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            return BAR_HEIGHT * scale;
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
            if (value > this._maximumValue || this._minimumValue > value)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value == this._minimumValue)
            {
                var pointX = this.GetButtonPointX(this.GetMinimumButtonPointX());
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    return true;
                }
            }
            else if (value == this._maximumValue)
            {
                var pointX = this.GetButtonPointX(this.GetMaximumButtonPointX());
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    return true;
                }
            }
            else
            {
                var rate = (value - this._minimumValue) / (float)(this._maximumValue - this._minimumValue);
                var pointX = this.GetButtonPointX(this.Width * rate);
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    return true;
                }
            }

            return false;
        }

        private int GetValue(float x)
        {
            if (x == this.GetMinimumButtonPointX())
            {
                return this._minimumValue;
            }
            else if (x == this.GetMaximumButtonPointX())
            {
                return this._maximumValue;
            }
            else
            {
                var rate = x / this.GetMaximumButtonPointX();
                var value = this._minimumValue + ((this._maximumValue - this._minimumValue) * rate);
                return (int)value;
            }
        }

        private float GetMaximumButtonPointX()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            var scaleButtonWidth = (this._button.Width) * scale;
            return this.Width - scaleButtonWidth / 2f;
        }

        private float GetMinimumButtonPointX()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            var scaleButtonWidth = (this._button.Width) * scale;
            return scaleButtonWidth / 2f;
        }

        private void DrawBar(Graphics g)
        {
            var centerPoint = this.GetCenterPoint();

            var barHeight = this.GetBarHeight();

            var shadowRect = new RectangleF(0,
                                            centerPoint.Y - barHeight / 2f,
                                            this.Width - BAR_SHADOW_OFFSET,
                                            barHeight);

            g.FillRectangle(Brushes.DimGray, shadowRect);

            shadowRect.Offset(BAR_SHADOW_OFFSET, BAR_SHADOW_OFFSET);

            g.FillRectangle(Brushes.White, shadowRect);

            var mainRect = new RectangleF(BAR_SHADOW_OFFSET,
                                          centerPoint.Y - barHeight / 2f + BAR_SHADOW_OFFSET,
                                          this.Width - BAR_SHADOW_OFFSET * 2,
                                          barHeight - BAR_SHADOW_OFFSET);

            g.FillRectangle(Brushes.LightGray, mainRect);
        }

        private void DrawButton(Graphics g)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            var scaleButtonSize = new SizeF(this._button.Width * scale, this._button.Height * scale);
            var centerPoint = this.GetCenterPoint();

            var rect = new RectangleF(this._buttonPointX - scaleButtonSize.Width / 2f,
                                      centerPoint.Y - scaleButtonSize.Height / 2f,
                                      scaleButtonSize.Width,
                                      scaleButtonSize.Height);

            g.DrawImage(this._button, rect);
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
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    this.Invalidate();

                    var value = this.GetValue(pointX);
                    if (value != this._sliderValue)
                    {
                        this._sliderValue = value;
                        this._isValueChanging = true;
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
                this.SetButtonPointX(this._sliderValue);
                this.Invalidate();

                if (this._isValueChanging)
                {
                    this._isValueChanging = false;
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
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    this.Invalidate();
                    this.Update();

                    var value = this.GetValue(pointX);
                    if (value != this._sliderValue)
                    {
                        this._sliderValue = value;
                        this._isValueChanging = true;
                        this.OnValueChanging(EventArgs.Empty);
                    }
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            this.SetButtonPointX(this._sliderValue);
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
