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
            this.addButton = new SWF.UIComponent.Core.ToolButton();
            this.inputTextBox = new InputTextBox();
            this.arrowPictureBox = new ArrowPictureBox();
            ((System.ComponentModel.ISupportInitialize)this.arrowPictureBox).BeginInit();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.addButton.Font = new Font("Yu Gothic UI", 7.8F);
            this.addButton.Location = new Point(594, 0);
            this.addButton.Margin = new System.Windows.Forms.Padding(0);
            this.addButton.Name = "addButton";
            this.addButton.Size = new Size(52, 38);
            this.addButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.addButton.MouseClick += this.AddButton_MouseClick;
            // 
            // inputTextBox
            // 
            this.inputTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.inputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.inputTextBox.Font = new Font("Yu Gothic UI", 10F);
            this.inputTextBox.Location = new Point(0, 1);
            this.inputTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new Size(569, 36);
            this.inputTextBox.TabIndex = 3;
            this.inputTextBox.TabStop = false;
            this.inputTextBox.KeyDown += this.InputTextBox_KeyDown;
            // 
            // arrowPictureBox
            // 
            this.arrowPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.arrowPictureBox.Location = new Point(570, 1);
            this.arrowPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.arrowPictureBox.Name = "arrowPictureBox";
            this.arrowPictureBox.Size = new Size(22, 36);
            this.arrowPictureBox.TabIndex = 2;
            this.arrowPictureBox.TabStop = false;
            this.arrowPictureBox.MouseClick += this.ArrowPictureBox_MouseClick;
            // 
            // WideComboBox
            // 
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.arrowPictureBox);
            this.Controls.Add(this.addButton);
            this.Font = new Font("Yu Gothic UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "WideComboBox";
            this.Size = new Size(646, 38);
            ((System.ComponentModel.ISupportInitialize)this.arrowPictureBox).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private SWF.UIComponent.Core.ToolButton addButton;
        private ArrowPictureBox arrowPictureBox;
        private InputTextBox inputTextBox;

        #endregion
    }
}
