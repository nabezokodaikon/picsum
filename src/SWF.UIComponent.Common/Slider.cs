using SWF.UIComponent.Common.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.Common
{
    /// <summary>
    /// スライダーコントロール
    /// </summary>
    public class Slider : Control
    {
        #region 定数・列挙

        private const int BarHeight = 4;
        private const int BarShadowOffset = 1;

        #endregion

        #region イベント

        public event EventHandler BeginValueChange;
        public event EventHandler ValueChanging;
        public event EventHandler ValueChanged;

        #endregion

        #region インスタンス変数

        private readonly Image _button = Resources.SliderButton;
        private float _buttonPointX = Resources.SliderButton.Width / 2f;
        private int _maximumValue = 100;
        private int _minimumValue = 0;
        private int _value = 0;

        #endregion

        #region パブリックプロパティ

        public int MaximumValue
        {
            get
            {
                return _maximumValue;
            }
            set
            {
                if (value < _minimumValue)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _maximumValue = value;

                if (_value > value)
                {
                    _value = value;
                }
            }
        }

        public int MinimumValue
        {
            get
            {
                return _minimumValue;
            }
            set
            {
                if (value > _maximumValue)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _minimumValue = value;
                if (_value < value)
                {
                    _value = value;
                }
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value > _maximumValue || _minimumValue > value)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _value = value;
                setButtonPointX(_value);
                this.Invalidate();
                this.Update();

                OnValueChanged(new EventArgs());
            }
        }

        #endregion

        #region コンストラクタ

        public Slider()
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド



        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);

            _value = _minimumValue;
        }

        private PointF getCenterPoint()
        {
            return new PointF(this.Width / 2f, this.Height / 2f);
        }

        private float getButtonPointX(float x)
        {
            if (x < getMinimumButtonPointX())
            {
                return getMinimumButtonPointX();
            }
            else if (x > getMaximumButtonPointX())
            {
                return getMaximumButtonPointX();
            }
            else
            {
                return x;
            }
        }

        private bool setButtonPointX(int value)
        {
            if (value > _maximumValue || _minimumValue > value)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            if (value == _maximumValue)
            {
                float pointX = getButtonPointX(getMaximumButtonPointX());
                if (pointX != _buttonPointX)
                {
                    _buttonPointX = pointX;
                    return true;
                }
            }
            else if (value == _minimumValue)
            {
                float pointX = getButtonPointX(getMinimumButtonPointX());
                if (pointX != _buttonPointX)
                {
                    _buttonPointX = pointX;
                    return true;
                }
            }
            else
            {
                float rate = (value - _minimumValue) / (float)(_maximumValue - _minimumValue);
                float pointX = getButtonPointX(this.Width * rate);
                if (pointX != _buttonPointX)
                {
                    _buttonPointX = pointX;
                    return true;
                }
            }

            return false;
        }

        private int getValue(float x)
        {
            if (x == getMinimumButtonPointX())
            {
                return _minimumValue;
            }
            else if (x == getMaximumButtonPointX())
            {
                return _maximumValue;
            }
            else
            {
                float rate = x / getMaximumButtonPointX();
                float value = _minimumValue + ((_maximumValue - _minimumValue) * rate);
                return (int)value;
            }
        }

        private float getMaximumButtonPointX()
        {
            return this.Width - _button.Width / 2f;
        }

        private float getMinimumButtonPointX()
        {
            return _button.Width / 2f;
        }

        #region 描画メソッド

        private void drawBar(Graphics g)
        {
            PointF centerPoint = getCenterPoint();

            RectangleF shadowRect = new RectangleF(0,
                                                   centerPoint.Y - BarHeight / 2f,
                                                   this.Width - BarShadowOffset,
                                                   BarHeight);

            g.FillRectangle(Brushes.DimGray, shadowRect);

            shadowRect.Offset(BarShadowOffset, BarShadowOffset);

            g.FillRectangle(Brushes.White, shadowRect);

            RectangleF mainRect = new RectangleF(BarShadowOffset,
                                                 centerPoint.Y - BarHeight / 2f + BarShadowOffset,
                                                 this.Width - BarShadowOffset * 2,
                                                 BarHeight - BarShadowOffset);

            g.FillRectangle(Brushes.LightGray, mainRect);
        }

        private void drawButton(Graphics g)
        {
            PointF centerPoint = getCenterPoint();

            Rectangle rect = new Rectangle((int)(_buttonPointX - _button.Width / 2f),
                                           (int)(centerPoint.Y - _button.Height / 2f),
                                           _button.Width,
                                           _button.Height);

            g.DrawImage(_button, rect);
        }

        #endregion

        #endregion

        #region 継承メソッド

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (setButtonPointX(_value))
            {
                base.OnInvalidated(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            drawBar(e.Graphics);
            drawButton(e.Graphics);

            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnBeginValueChange(new EventArgs());

                float pointX = getButtonPointX(e.X);
                if (pointX != _buttonPointX)
                {
                    _buttonPointX = pointX;
                    this.Invalidate();
                    this.Update();

                    int value = getValue(pointX);
                    if (value != _value)
                    {
                        _value = value;
                        OnValueChanging(new EventArgs());
                    }
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //OnValueChanged(new EventArgs());
                Value = Value;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float pointX = getButtonPointX(e.X);
                if (pointX != _buttonPointX)
                {
                    _buttonPointX = pointX;
                    this.Invalidate();
                    this.Update();

                    int value = getValue(pointX);
                    if (value != _value)
                    {
                        _value = value;
                        OnValueChanging(new EventArgs());
                    }
                }
            }

            base.OnMouseMove(e);
        }

        protected virtual void OnBeginValueChange(EventArgs e)
        {
            if (BeginValueChange != null)
            {
                BeginValueChange(this, e);
            }
        }

        protected virtual void OnValueChanging(EventArgs e)
        {
            if (ValueChanging != null)
            {
                ValueChanging(this, e);
            }
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        #endregion
    }
}
