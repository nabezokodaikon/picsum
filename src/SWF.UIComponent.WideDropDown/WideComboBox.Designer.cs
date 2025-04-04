using SWF.UIComponent.Core;
using System.Drawing;

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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(WideComboBox));
            this.addButton = new SWF.UIComponent.Core.ToolIconButton();
            this.inputTextBox = new InputTextBox();
            this.arrowPictureBox = new ToolIconButton();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Font = new Font("Yu Gothic UI", 7.8F);
            this.addButton.Name = "addButton";
            this.addButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.addButton.MouseClick += this.AddButton_MouseClick;
            // 
            // inputTextBox
            // 
            this.inputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.inputTextBox.Font = new Font("Yu Gothic UI", 10F);
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.TabStop = false;
            this.inputTextBox.KeyDown += this.InputTextBox_KeyDown;
            // 
            // arrowPictureBox
            // 
            this.arrowPictureBox.Name = "arrowPictureBox";
            // 
            // WideComboBox
            // 
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.arrowPictureBox);
            this.Controls.Add(this.addButton);
            this.Font = new Font("Yu Gothic UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "WideComboBox";
            this.ResumeLayout(false);
        }


        private SWF.UIComponent.Core.ToolIconButton addButton;
        private ToolIconButton arrowPictureBox;
        private InputTextBox inputTextBox;

        #endregion
    }
}
