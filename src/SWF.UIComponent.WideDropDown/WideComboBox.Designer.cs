using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Base;
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
            this._addButton = new SWF.UIComponent.Base.BaseIconButton();
            this._inputTextBox = new InputTextBox();
            this._arrowPictureBox = new BaseIconButton();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this._addButton.MouseClick += this.AddButton_MouseClick;
            // 
            // inputTextBox
            // 
            this._inputTextBox.Font = FontCacher.GetRegularGdiFont(FontCacher.Size.Medium);
            this._inputTextBox.KeyDown += this.InputTextBox_KeyDown;
            // 
            // WideComboBox
            //
            this.Controls.AddRange(
                this._inputTextBox,
                this._arrowPictureBox,
                this._addButton);
            this.Paint += this.WideComboBox_Paint;
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.Base.BaseIconButton _addButton;
        private BaseIconButton _arrowPictureBox;
        private InputTextBox _inputTextBox;

        #endregion
    }
}
