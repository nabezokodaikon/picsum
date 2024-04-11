using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SWF.UIComponent.Common
{
    /// <summary>
    /// ツールストリップスライダー
    /// </summary>
    [SupportedOSPlatform("windows")]
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
    public class ToolStripSlider : ToolStripControlHost
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        public event EventHandler BeginValueChange;
        public event EventHandler ValueChanging;
        public event EventHandler ValueChanged;

        #endregion

        #region インスタンス変数

        #endregion

        #region パブリックプロパティ

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

        #endregion

        #region プライベートプロパティ

        private Slider Slider
        {
            get
            {
                return (Slider)this.Control;
            }
        }

        #endregion

        #region コンストラクタ

        public ToolStripSlider()
            : base(new Slider())
        {
            this.InitializeComponent();
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.Slider.Size = new(96, 24);
            this.Slider.BackColor = Color.Transparent;
        }

        #endregion

        #region 継承メソッド

        protected override void OnSubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            Slider slider = (Slider)control;
            slider.BeginValueChange += new(this.Slider_BeginValueChange);
            slider.ValueChanging += new(this.Slider_ValueChanging);
            slider.ValueChanged += new(this.Slider_ValueChanged);
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            Slider slider = (Slider)control;
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

        #endregion

        #region ホストしているコントロールのイベント

        private void Slider_BeginValueChange(object sender, EventArgs e)
        {
            this.OnBeginValueChange(e);
        }

        private void Slider_ValueChanging(object sender, EventArgs e)
        {
            this.OnValueChanging(e);
        }

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            this.OnValueChanged(e);
        }

        #endregion
    }
}
