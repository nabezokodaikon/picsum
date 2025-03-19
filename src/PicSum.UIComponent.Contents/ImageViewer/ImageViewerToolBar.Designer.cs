namespace PicSum.UIComponent.Contents.ImageViewer
{
    partial class ImageViewerToolBar
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
            this.components = new System.ComponentModel.Container();
            this.viewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.singleViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spreadLeftFeedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spreadRightFeedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doublePreviewButton = new SWF.UIComponent.Core.ToolButton();
            this.singlePreviewButton = new SWF.UIComponent.Core.ToolButton();
            this.doubleNextButton = new SWF.UIComponent.Core.ToolButton();
            this.singleNextButton = new SWF.UIComponent.Core.ToolButton();
            this.indexSlider = new SWF.UIComponent.Core.Slider();
            this.viewButton = new SWF.UIComponent.Core.ToolButton();
            this.sizeButton = new SWF.UIComponent.Core.ToolButton();
            this.sizeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.originalSizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitWindowLargeOnlyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filePathToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.viewMenu.SuspendLayout();
            this.sizeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewMenu
            // 
            this.viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.singleViewMenuItem, this.spreadLeftFeedMenuItem, this.spreadRightFeedMenuItem });
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(178, 70);
            // 
            // singleViewMenuItem
            // 
            this.singleViewMenuItem.Name = "singleViewMenuItem";
            this.singleViewMenuItem.Size = new System.Drawing.Size(177, 22);
            this.singleViewMenuItem.Text = "Single View";
            this.singleViewMenuItem.Click += this.SingleViewMenuItem_Click;
            // 
            // spreadLeftFeedMenuItem
            // 
            this.spreadLeftFeedMenuItem.Name = "spreadLeftFeedMenuItem";
            this.spreadLeftFeedMenuItem.Size = new System.Drawing.Size(177, 22);
            this.spreadLeftFeedMenuItem.Text = "Spread (Left Feed)";
            this.spreadLeftFeedMenuItem.Click += this.SpreadLeftFeedMenuItem_Click;
            // 
            // spreadRightFeedMenuItem
            // 
            this.spreadRightFeedMenuItem.Name = "spreadRightFeedMenuItem";
            this.spreadRightFeedMenuItem.Size = new System.Drawing.Size(177, 22);
            this.spreadRightFeedMenuItem.Text = "Spread (Right Feed)";
            this.spreadRightFeedMenuItem.Click += this.SpreadRightFeedMenuItem_Click;
            // 
            // doublePreviewButton
            // 
            this.doublePreviewButton.BackColor = System.Drawing.Color.FromArgb(241, 244, 250);
            this.doublePreviewButton.FlatAppearance.BorderSize = 0;
            this.doublePreviewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.doublePreviewButton.Location = new System.Drawing.Point(143, 3);
            this.doublePreviewButton.Name = "doublePreviewButton";
            this.doublePreviewButton.Size = new System.Drawing.Size(64, 23);
            this.doublePreviewButton.TabIndex = 1;
            this.doublePreviewButton.TabStop = false;
            this.doublePreviewButton.Text = "<<-";
            this.doublePreviewButton.UseVisualStyleBackColor = false;
            this.doublePreviewButton.MouseClick += this.DoublePreviewButton_MouseClick;
            // 
            // singlePreviewButton
            // 
            this.singlePreviewButton.BackColor = System.Drawing.Color.FromArgb(241, 244, 250);
            this.singlePreviewButton.FlatAppearance.BorderSize = 0;
            this.singlePreviewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.singlePreviewButton.Location = new System.Drawing.Point(213, 3);
            this.singlePreviewButton.Name = "singlePreviewButton";
            this.singlePreviewButton.Size = new System.Drawing.Size(64, 23);
            this.singlePreviewButton.TabIndex = 2;
            this.singlePreviewButton.TabStop = false;
            this.singlePreviewButton.Text = "<-";
            this.singlePreviewButton.UseVisualStyleBackColor = false;
            this.singlePreviewButton.MouseClick += this.SinglePreviewButton_MouseClick;
            // 
            // doubleNextButton
            // 
            this.doubleNextButton.BackColor = System.Drawing.Color.FromArgb(241, 244, 250);
            this.doubleNextButton.FlatAppearance.BorderSize = 0;
            this.doubleNextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.doubleNextButton.Location = new System.Drawing.Point(353, 3);
            this.doubleNextButton.Name = "doubleNextButton";
            this.doubleNextButton.Size = new System.Drawing.Size(64, 23);
            this.doubleNextButton.TabIndex = 3;
            this.doubleNextButton.TabStop = false;
            this.doubleNextButton.Text = "->>";
            this.doubleNextButton.UseVisualStyleBackColor = false;
            this.doubleNextButton.MouseClick += this.DoubleNextButton_MouseClick;
            // 
            // singleNextButton
            // 
            this.singleNextButton.BackColor = System.Drawing.Color.FromArgb(241, 244, 250);
            this.singleNextButton.FlatAppearance.BorderSize = 0;
            this.singleNextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.singleNextButton.Location = new System.Drawing.Point(283, 3);
            this.singleNextButton.Name = "singleNextButton";
            this.singleNextButton.Size = new System.Drawing.Size(64, 23);
            this.singleNextButton.TabIndex = 4;
            this.singleNextButton.TabStop = false;
            this.singleNextButton.Text = "->";
            this.singleNextButton.UseVisualStyleBackColor = false;
            this.singleNextButton.MouseClick += this.SingleNextButton_MouseClick;
            // 
            // indexSlider
            // 
            this.indexSlider.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.indexSlider.Location = new System.Drawing.Point(423, 3);
            this.indexSlider.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
            this.indexSlider.Name = "indexSlider";
            this.indexSlider.Size = new System.Drawing.Size(252, 23);
            this.indexSlider.TabIndex = 5;
            this.indexSlider.TabStop = false;
            this.indexSlider.BeginValueChange += this.IndexSlider_BeginValueChange;
            this.indexSlider.ValueChanging += this.IndexSlider_ValueChanging;
            this.indexSlider.ValueChanged += this.IndexSlider_ValueChanged;
            // 
            // viewButton
            // 
            this.viewButton.BackColor = System.Drawing.Color.FromArgb(241, 244, 250);
            this.viewButton.FlatAppearance.BorderSize = 0;
            this.viewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.viewButton.Location = new System.Drawing.Point(3, 3);
            this.viewButton.Name = "viewButton";
            this.viewButton.Size = new System.Drawing.Size(64, 23);
            this.viewButton.TabIndex = 6;
            this.viewButton.TabStop = false;
            this.viewButton.Text = "View";
            this.viewButton.UseVisualStyleBackColor = false;
            this.viewButton.MouseClick += this.ViewButton_MouseClick;
            // 
            // sizeButton
            // 
            this.sizeButton.BackColor = System.Drawing.Color.FromArgb(241, 244, 250);
            this.sizeButton.FlatAppearance.BorderSize = 0;
            this.sizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sizeButton.Location = new System.Drawing.Point(73, 3);
            this.sizeButton.Name = "sizeButton";
            this.sizeButton.Size = new System.Drawing.Size(64, 23);
            this.sizeButton.TabIndex = 7;
            this.sizeButton.TabStop = false;
            this.sizeButton.Text = "Size";
            this.sizeButton.UseVisualStyleBackColor = false;
            this.sizeButton.MouseClick += this.SizeButton_MouseClick;
            // 
            // sizeMenu
            // 
            this.sizeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.originalSizeMenuItem, this.fitWindowMenuItem, this.fitWindowLargeOnlyMenuItem });
            this.sizeMenu.Name = "viewMenu";
            this.sizeMenu.Size = new System.Drawing.Size(253, 70);
            // 
            // originalSizeMenuItem
            // 
            this.originalSizeMenuItem.Name = "originalSizeMenuItem";
            this.originalSizeMenuItem.Size = new System.Drawing.Size(252, 22);
            this.originalSizeMenuItem.Text = "Original Size";
            this.originalSizeMenuItem.Click += this.OriginalSizeMenuItem_Click;
            // 
            // fitWindowMenuItem
            // 
            this.fitWindowMenuItem.Name = "fitWindowMenuItem";
            this.fitWindowMenuItem.Size = new System.Drawing.Size(252, 22);
            this.fitWindowMenuItem.Text = "Fit To Window";
            this.fitWindowMenuItem.Click += this.FitWindowMenuItem_Click;
            // 
            // fitWindowLargeOnlyMenuItem
            // 
            this.fitWindowLargeOnlyMenuItem.Name = "fitWindowLargeOnlyMenuItem";
            this.fitWindowLargeOnlyMenuItem.Size = new System.Drawing.Size(252, 22);
            this.fitWindowLargeOnlyMenuItem.Text = "Fit To Window (Large Image Only)";
            this.fitWindowLargeOnlyMenuItem.Click += this.FitWindowLargeOnlyMenuItem_Click;
            // 
            // filePathToolTip
            // 
            this.filePathToolTip.AutoPopDelay = 5000;
            this.filePathToolTip.InitialDelay = 50;
            this.filePathToolTip.ReshowDelay = 100;
            // 
            // ImageViewerToolBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sizeButton);
            this.Controls.Add(this.viewButton);
            this.Controls.Add(this.indexSlider);
            this.Controls.Add(this.singleNextButton);
            this.Controls.Add(this.doubleNextButton);
            this.Controls.Add(this.singlePreviewButton);
            this.Controls.Add(this.doublePreviewButton);
            this.Name = "ImageViewerToolBar";
            this.Size = new System.Drawing.Size(691, 29);
            this.viewMenu.ResumeLayout(false);
            this.sizeMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip viewMenu;
        private SWF.UIComponent.Core.ToolButton doublePreviewButton;
        private SWF.UIComponent.Core.ToolButton singlePreviewButton;
        private SWF.UIComponent.Core.ToolButton doubleNextButton;
        private SWF.UIComponent.Core.ToolButton singleNextButton;
        private SWF.UIComponent.Core.Slider indexSlider;
        private SWF.UIComponent.Core.ToolButton viewButton;
        private System.Windows.Forms.ToolStripMenuItem singleViewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spreadLeftFeedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spreadRightFeedMenuItem;
        private SWF.UIComponent.Core.ToolButton sizeButton;
        private System.Windows.Forms.ContextMenuStrip sizeMenu;
        private System.Windows.Forms.ToolStripMenuItem originalSizeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitWindowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitWindowLargeOnlyMenuItem;
        private System.Windows.Forms.ToolTip filePathToolTip;
    }
}
