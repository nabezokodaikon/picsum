using SWF.UIComponent.Common.Properties;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.Common
{
    /// <summary>
    /// スライダーコントロール
    /// </summary>
    [SupportedOSPlatform("windows")]
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
                return this._maximumValue;
            }
            set
            {
                if (value < this._minimumValue)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this._maximumValue = value;

                if (this._value > value)
                {
                    this._value = value;
                }
            }
        }

        public int MinimumValue
        {
            get
            {
                return this._minimumValue;
            }
            set
            {
                if (value > this._maximumValue)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this._minimumValue = value;
                if (this._value < value)
                {
                    this._value = value;
                }
            }
        }

        public int Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (value > this._maximumValue || this._minimumValue > value)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this._value = value;
                this.setButtonPointX(this._value);
                this.Invalidate();
                this.Update();

                this.OnValueChanged(new EventArgs());
            }
        }

        #endregion

        #region コンストラクタ

        public Slider()
        {
            this.initializeComponent();
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

            this._value = this._minimumValue;
        }

        private PointF getCenterPoint()
        {
            return new PointF(this.Width / 2f, this.Height / 2f);
        }

        private float getButtonPointX(float x)
        {
            if (x < this.getMinimumButtonPointX())
            {
                return this.getMinimumButtonPointX();
            }
            else if (x > this.getMaximumButtonPointX())
            {
                return this.getMaximumButtonPointX();
            }
            else
            {
                return x;
            }
        }

        private bool setButtonPointX(int value)
        {
            if (value > this._maximumValue || this._minimumValue > value)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            if (value == this._maximumValue)
            {
                float pointX = this.getButtonPointX(this.getMaximumButtonPointX());
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    return true;
                }
            }
            else if (value == this._minimumValue)
            {
                float pointX = this.getButtonPointX(this.getMinimumButtonPointX());
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    return true;
                }
            }
            else
            {
                float rate = (value - this._minimumValue) / (float)(this._maximumValue - this._minimumValue);
                float pointX = this.getButtonPointX(this.Width * rate);
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    return true;
                }
            }

            return false;
        }

        private int getValue(float x)
        {
            if (x == this.getMinimumButtonPointX())
            {
                return this._minimumValue;
            }
            else if (x == this.getMaximumButtonPointX())
            {
                return this._maximumValue;
            }
            else
            {
                float rate = x / this.getMaximumButtonPointX();
                float value = this._minimumValue + ((this._maximumValue - this._minimumValue) * rate);
                return (int)value;
            }
        }

        private float getMaximumButtonPointX()
        {
            return this.Width - this._button.Width / 2f;
        }

        private float getMinimumButtonPointX()
        {
            return this._button.Width / 2f;
        }

        #region 描画メソッド

        private void drawBar(Graphics g)
        {
            PointF centerPoint = this.getCenterPoint();

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
            PointF centerPoint = this.getCenterPoint();

            Rectangle rect = new Rectangle((int)(this._buttonPointX - this._button.Width / 2f),
                                           (int)(centerPoint.Y - this._button.Height / 2f),
                                           this._button.Width,
                                           this._button.Height);

            g.DrawImage(this._button, rect);
        }

        #endregion

        #endregion

        #region 継承メソッド

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (this.setButtonPointX(this._value))
            {
                base.OnInvalidated(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.drawBar(e.Graphics);
            this.drawButton(e.Graphics);

            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OnBeginValueChange(new EventArgs());

                float pointX = this.getButtonPointX(e.X);
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    this.Invalidate();
                    this.Update();

                    int value = this.getValue(pointX);
                    if (value != this._value)
                    {
                        this._value = value;
                        this.OnValueChanging(new EventArgs());
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
                this.Value = this.Value;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float pointX = this.getButtonPointX(e.X);
                if (pointX != this._buttonPointX)
                {
                    this._buttonPointX = pointX;
                    this.Invalidate();
                    this.Update();

                    int value = this.getValue(pointX);
                    if (value != this._value)
                    {
                        this._value = value;
                        this.OnValueChanging(new EventArgs());
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
