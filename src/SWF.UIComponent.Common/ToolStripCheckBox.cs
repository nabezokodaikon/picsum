using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace SWF.UIComponent.Common
{
    /// <summary>
    /// ツールストリップチェックボックス
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
    public class ToolStripCheckBox : ToolStripControlHost
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        public event EventHandler CheckedChanged;

        #endregion

        #region インスタンス変数

        #endregion

        #region パブリックプロパティ

        [Browsable(true)]
        public new string Text
        {
            get
            {
                return checkBox.Text;
            }
            set
            {
                checkBox.Text = value;
            }
        }

        [Browsable(true)]
        public bool Checked
        {
            get
            {
                return checkBox.Checked;
            }
            set
            {
                checkBox.Checked = value;
            }
        }

        [Browsable(true)]
        public new ContentAlignment TextAlign
        {
            get
            {
                return checkBox.TextAlign;
            }
            set
            {
                checkBox.TextAlign = value;
            }
        }

        #endregion

        #region プライベートプロパティ

        private CheckBox checkBox
        {
            get
            {
                return (CheckBox)this.Control;
            }
        }

        #endregion

        #region コンストラクタ

        public ToolStripCheckBox()
            : base(new CheckBox())
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
           
        }

        #endregion

        #region 継承メソッド

        protected override void OnSubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            CheckBox checkBox = (CheckBox)control;
            checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            CheckBox checkBox = (CheckBox)control;
            checkBox.CheckedChanged -= new EventHandler(checkBox_CheckedChanged);
        }

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            if (CheckedChanged != null)
            {
                CheckedChanged(this, e);
            }
        }

        #endregion

        #region ホストしているコントロールのイベント

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            OnCheckedChanged(new EventArgs());
        }

        #endregion
    }
}
