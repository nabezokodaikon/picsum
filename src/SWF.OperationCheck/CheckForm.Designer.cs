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
            this.wideDropToolButton1 = new SWF.OperationCheck.Contorols.WideDropToolButton();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // wideDropToolButton1
            // 
            this.wideDropToolButton1.DropDownListSize = new System.Drawing.Size(428, 208);
            this.wideDropToolButton1.Icon = null;
            this.wideDropToolButton1.Location = new System.Drawing.Point(713, 12);
            this.wideDropToolButton1.Name = "wideDropToolButton1";
            this.wideDropToolButton1.RegionType = SWF.UIComponent.Common.ToolButton.ToolButtonRegionType.Default;
            this.wideDropToolButton1.Size = new System.Drawing.Size(75, 23);
            this.wideDropToolButton1.TabIndex = 0;
            this.wideDropToolButton1.Text = "wideDropToolButton1";
            this.wideDropToolButton1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CheckForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.wideDropToolButton1);
            this.Name = "CheckForm";
            this.Text = "CheckForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Contorols.WideDropToolButton wideDropToolButton1;
        private System.Windows.Forms.Button button1;
    }
}

