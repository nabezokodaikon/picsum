using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    partial class WideComboBox
    {
        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.addButton = new SWF.UIComponent.Core.BaseIconButton();
            this.inputTextBox = new InputTextBox();
            this.arrowPictureBox = new BaseIconButton();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.MouseClick += this.AddButton_MouseClick;
            // 
            // inputTextBox
            // 
            this.inputTextBox.Font = Fonts.GetRegularFont(Fonts.Size.Medium);
            this.inputTextBox.KeyDown += this.InputTextBox_KeyDown;
            // 
            // WideComboBox
            //
            this.Controls.AddRange(
                this.inputTextBox,
                this.arrowPictureBox,
                this.addButton);
            this.Paint += this.WideComboBox_Paint;
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.Core.BaseIconButton addButton;
        private BaseIconButton arrowPictureBox;
        private InputTextBox inputTextBox;

        #endregion
    }
}
