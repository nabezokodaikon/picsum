using SWF.UIComponent.Core.Properties;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
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

        private readonly Image button = Resources.SliderButton;
        private float buttonPointX = Resources.SliderButton.Width / 2f;
        private int maximumValue = 100;
        private int minimumValue = 0;
        private int value = 0;

        #endregion

        #region パブリックプロパティ

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

                if (this.value > value)
                {
                    this.value = value;
                }
            }
        }

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
                if (this.value < value)
                {
                    this.value = value;
                }
            }
        }

        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (value > this.maximumValue || this.minimumValue > value)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.value = value;
                this.SetButtonPointX(this.value);
                this.Invalidate();
                this.Update();

                this.OnValueChanged(EventArgs.Empty);
            }
        }

        #endregion

        #region コンストラクタ

        public Slider()
        {
            this.InitializeComponent();
        }

        #endregion

        #region パブリックメソッド



        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.value = this.minimumValue;
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

            if (value == this.maximumValue)
            {
                float pointX = this.GetButtonPointX(this.GetMaximumButtonPointX());
                if (pointX != this.buttonPointX)
                {
                    this.buttonPointX = pointX;
                    return true;
                }
            }
            else if (value == this.minimumValue)
            {
                float pointX = this.GetButtonPointX(this.GetMinimumButtonPointX());
                if (pointX != this.buttonPointX)
                {
                    this.buttonPointX = pointX;
                    return true;
                }
            }
            else
            {
                float rate = (value - this.minimumValue) / (float)(this.maximumValue - this.minimumValue);
                float pointX = this.GetButtonPointX(this.Width * rate);
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
                float rate = x / this.GetMaximumButtonPointX();
                float value = this.minimumValue + ((this.maximumValue - this.minimumValue) * rate);
                return (int)value;
            }
        }

        private float GetMaximumButtonPointX()
        {
            return this.Width - this.button.Width / 2f;
        }

        private float GetMinimumButtonPointX()
        {
            return this.button.Width / 2f;
        }

        #region 描画メソッド

        private void DrawBar(Graphics g)
        {
            PointF centerPoint = this.GetCenterPoint();

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

        private void DrawButton(Graphics g)
        {
            PointF centerPoint = this.GetCenterPoint();

            Rectangle rect = new Rectangle((int)(this.buttonPointX - this.button.Width / 2f),
                                           (int)(centerPoint.Y - this.button.Height / 2f),
                                           this.button.Width,
                                           this.button.Height);

            g.DrawImage(this.button, rect);
        }

        #endregion

        #endregion

        #region 継承メソッド

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (this.SetButtonPointX(this.value))
            {
                base.OnInvalidated(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.DrawBar(e.Graphics);
            this.DrawButton(e.Graphics);

            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OnBeginValueChange(EventArgs.Empty);

                float pointX = this.GetButtonPointX(e.X);
                if (pointX != this.buttonPointX)
                {
                    this.buttonPointX = pointX;
                    this.Invalidate();
                    this.Update();

                    int value = this.GetValue(pointX);
                    if (value != this.value)
                    {
                        this.value = value;
                        this.OnValueChanging(EventArgs.Empty);
                    }
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Value = this.value;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float pointX = this.GetButtonPointX(e.X);
                if (pointX != this.buttonPointX)
                {
                    this.buttonPointX = pointX;
                    this.Invalidate();
                    this.Update();

                    int value = this.GetValue(pointX);
                    if (value != this.value)
                    {
                        this.value = value;
                        this.OnValueChanging(EventArgs.Empty);
                    }
                }
            }

            base.OnMouseMove(e);
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

        #endregion
    }
}
