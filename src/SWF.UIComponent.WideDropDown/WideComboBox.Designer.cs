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
            this.addButton = new System.Windows.Forms.Button();
            this.inputTextBox = new SWF.UIComponent.WideDropDown.InputTextBox();
            this.arrowPictureBox = new SWF.UIComponent.WideDropDown.ArrowPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.arrowPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Font = new System.Drawing.Font("Courier", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.addButton.Image = global::SWF.UIComponent.WideDropDown.Properties.Resources.TagIcon;
            this.addButton.Location = new System.Drawing.Point(598, 3);
            this.addButton.Margin = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(48, 32);
            this.addButton.TabIndex = 1;
            this.addButton.TabStop = false;
            this.addButton.Text = "+";
            this.addButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.addButton_MouseClick);
            // 
            // inputTextBox
            // 
            this.inputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.inputTextBox.Font = new System.Drawing.Font("Courier", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.inputTextBox.Location = new System.Drawing.Point(3, 3);
            this.inputTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(567, 32);
            this.inputTextBox.TabIndex = 3;
            this.inputTextBox.TabStop = false;
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyDown);
            // 
            // arrowPictureBox
            // 
            this.arrowPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.arrowPictureBox.DefaultColor = System.Drawing.Color.WhiteSmoke;
            this.arrowPictureBox.EnterColor = System.Drawing.Color.LightGray;
            this.arrowPictureBox.Image = global::SWF.UIComponent.WideDropDown.Properties.Resources.SmallArrowDown;
            this.arrowPictureBox.IsSelected = false;
            this.arrowPictureBox.Location = new System.Drawing.Point(570, 3);
            this.arrowPictureBox.Margin = new System.Windows.Forms.Padding(0, 6, 4, 6);
            this.arrowPictureBox.Name = "arrowPictureBox";
            this.arrowPictureBox.SelectedColor = System.Drawing.Color.DarkGray;
            this.arrowPictureBox.Size = new System.Drawing.Size(24, 32);
            this.arrowPictureBox.TabIndex = 2;
            this.arrowPictureBox.TabStop = false;
            this.arrowPictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.arrowPictureBox_MouseClick);
            // 
            // WideComboBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.arrowPictureBox);
            this.Controls.Add(this.addButton);
            this.Font = new System.Drawing.Font("Courier", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "WideComboBox";
            this.Size = new System.Drawing.Size(646, 38);
            ((System.ComponentModel.ISupportInitialize)(this.arrowPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button addButton;
        private ArrowPictureBox arrowPictureBox;
        private InputTextBox inputTextBox;
    }
}
