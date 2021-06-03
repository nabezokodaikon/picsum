using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SWF.UIComponent.Common
{
    /// <summary>
    /// ツールストリップスライダー
    /// </summary>
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
                return slider.MaximumValue;
            }
            set
            {
                slider.MaximumValue = value;
            }
        }

        public int MinimumValue
        {
            get
            {
                return slider.MinimumValue;
            }
            set
            {
                slider.MinimumValue = value;
            }
        }

        public int Value
        {
            get
            {
                return slider.Value;
            }
            set
            {
                slider.Value = value;
            }
        }

        #endregion

        #region プライベートプロパティ

        private Slider slider
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
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            slider.Size = new Size(96, 24);
            slider.BackColor = Color.Transparent;
        }

        #endregion

        #region 継承メソッド

        protected override void OnSubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            Slider slider = (Slider)control;
            slider.BeginValueChange += new EventHandler(slider_BeginValueChange);
            slider.ValueChanging += new EventHandler(slider_ValueChanging);
            slider.ValueChanged += new EventHandler(slider_ValueChanged);
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            Slider slider = (Slider)control;
            slider.BeginValueChange -= new EventHandler(slider_BeginValueChange);
            slider.ValueChanging -= new EventHandler(slider_ValueChanging);
            slider.ValueChanged -= new EventHandler(slider_ValueChanged);
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

        #region ホストしているコントロールのイベント

        private void slider_BeginValueChange(object sender, EventArgs e)
        {
            OnBeginValueChange(e);
        }

        private void slider_ValueChanging(object sender, EventArgs e)
        {
            OnValueChanging(e);
        }

        private void slider_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged(e);
        }

        #endregion
    }
}
