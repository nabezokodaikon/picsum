namespace SWF.OperationCheck
{
    partial class CheckForm
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.wideDropToolButton1 = new SWF.OperationCheck.Contorols.WideDropToolButton();
            this.wideComboBox1 = new SWF.OperationCheck.Contorols.WideComboBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(18, 23);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 44);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(391, 36);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 31);
            this.comboBox1.TabIndex = 3;
            // 
            // wideDropToolButton1
            // 
            this.wideDropToolButton1.DropDownListSize = new System.Drawing.Size(468, 248);
            this.wideDropToolButton1.Location = new System.Drawing.Point(1070, 23);
            this.wideDropToolButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.wideDropToolButton1.Name = "wideDropToolButton1";
            this.wideDropToolButton1.RegionType = SWF.UIComponent.Common.ToolButton.ToolButtonRegionType.Default;
            this.wideDropToolButton1.SelectedItem = null;
            this.wideDropToolButton1.Size = new System.Drawing.Size(112, 44);
            this.wideDropToolButton1.TabIndex = 0;
            this.wideDropToolButton1.Text = "wideDropToolButton1";
            this.wideDropToolButton1.UseVisualStyleBackColor = true;
            // 
            // wideComboBox1
            // 
            this.wideComboBox1.DropDownListSize = new System.Drawing.Size(428, 208);
            this.wideComboBox1.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.wideComboBox1.Location = new System.Drawing.Point(458, 253);
            this.wideComboBox1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.wideComboBox1.Name = "wideComboBox1";
            this.wideComboBox1.Size = new System.Drawing.Size(415, 38);
            this.wideComboBox1.TabIndex = 4;
            this.wideComboBox1.DropDownOpening += new System.EventHandler<SWF.OperationCheck.Contorols.DropDownOpeningEventArgs>(this.wideComboBox1_DropDownOpening);
            this.wideComboBox1.AddItem += new System.EventHandler<SWF.OperationCheck.Contorols.AddItemEventArgs>(this.wideComboBox1_AddItem);
            // 
            // CheckForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1275, 483);
            this.Controls.Add(this.wideComboBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.wideDropToolButton1);
            this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "CheckForm";
            this.Text = "CheckForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Contorols.WideDropToolButton wideDropToolButton1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private Contorols.WideComboBox wideComboBox1;
    }
}

