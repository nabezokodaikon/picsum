using SWF.UIComponent.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    partial class WideComboBox
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.addButton = new SWF.UIComponent.Core.ToolIconButton();
            this.inputTextBox = new InputTextBox();
            this.arrowPictureBox = new ToolIconButton();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Name = "addButton";
            this.addButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.addButton.MouseClick += this.AddButton_MouseClick;
            // 
            // inputTextBox
            // 
            this.inputTextBox.Font = new Font("Yu Gothic UI", 10F);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.KeyDown += this.InputTextBox_KeyDown;
            // 
            // arrowPictureBox
            // 
            this.arrowPictureBox.Name = "arrowPictureBox";
            // 
            // WideComboBox
            //
            this.Controls.AddRange(
                this.inputTextBox,
                this.arrowPictureBox,
                this.addButton);
            this.Name = "WideComboBox";
            this.Paint += this.WideComboBox_Paint;
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.Core.ToolIconButton addButton;
        private ToolIconButton arrowPictureBox;
        private InputTextBox inputTextBox;

        #endregion
    }
}
