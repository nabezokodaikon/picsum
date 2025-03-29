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
            this.viewMenu.Font = new System.Drawing.Font("Yu Gothic UI", 10F);
            this.viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.singleViewMenuItem, this.spreadLeftFeedMenuItem, this.spreadRightFeedMenuItem });
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(197, 76);
            // 
            // singleViewMenuItem
            // 
            this.singleViewMenuItem.Name = "singleViewMenuItem";
            this.singleViewMenuItem.Size = new System.Drawing.Size(196, 24);
            this.singleViewMenuItem.Text = "Single View";
            this.singleViewMenuItem.Click += this.SingleViewMenuItem_Click;
            // 
            // spreadLeftFeedMenuItem
            // 
            this.spreadLeftFeedMenuItem.Name = "spreadLeftFeedMenuItem";
            this.spreadLeftFeedMenuItem.Size = new System.Drawing.Size(196, 24);
            this.spreadLeftFeedMenuItem.Text = "Spread (Left Feed)";
            this.spreadLeftFeedMenuItem.Click += this.SpreadLeftFeedMenuItem_Click;
            // 
            // spreadRightFeedMenuItem
            // 
            this.spreadRightFeedMenuItem.Name = "spreadRightFeedMenuItem";
            this.spreadRightFeedMenuItem.Size = new System.Drawing.Size(196, 24);
            this.spreadRightFeedMenuItem.Text = "Spread (Right Feed)";
            this.spreadRightFeedMenuItem.Click += this.SpreadRightFeedMenuItem_Click;
            // 
            // doublePreviewButton
            // 
            this.doublePreviewButton.Name = "doublePreviewButton";
            this.doublePreviewButton.MouseClick += this.DoublePreviewButton_MouseClick;
            // 
            // singlePreviewButton
            // 
            this.singlePreviewButton.Name = "singlePreviewButton";
            this.singlePreviewButton.MouseClick += this.SinglePreviewButton_MouseClick;
            // 
            // doubleNextButton
            // 
            this.doubleNextButton.Name = "doubleNextButton";
            this.doubleNextButton.MouseClick += this.DoubleNextButton_MouseClick;
            // 
            // singleNextButton
            // 
            this.singleNextButton.Name = "singleNextButton";
            this.singleNextButton.MouseClick += this.SingleNextButton_MouseClick;
            // 
            // indexSlider
            // 
            this.indexSlider.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
            this.indexSlider.Name = "indexSlider";
            this.indexSlider.TabIndex = 5;
            this.indexSlider.TabStop = false;
            this.indexSlider.BeginValueChange += this.IndexSlider_BeginValueChange;
            this.indexSlider.ValueChanging += this.IndexSlider_ValueChanging;
            this.indexSlider.ValueChanged += this.IndexSlider_ValueChanged;
            // 
            // viewButton
            // 
            this.viewButton.Name = "viewButton";
            this.viewButton.MouseClick += this.ViewButton_MouseClick;
            // 
            // sizeButton
            // 
            this.sizeButton.Name = "sizeButton";
            this.sizeButton.MouseClick += this.SizeButton_MouseClick;
            // 
            // sizeMenu
            // 
            this.sizeMenu.Font = new System.Drawing.Font("Yu Gothic UI", 10F);
            this.sizeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.originalSizeMenuItem, this.fitWindowMenuItem, this.fitWindowLargeOnlyMenuItem });
            this.sizeMenu.Name = "viewMenu";
            this.sizeMenu.Size = new System.Drawing.Size(287, 76);
            // 
            // originalSizeMenuItem
            // 
            this.originalSizeMenuItem.Name = "originalSizeMenuItem";
            this.originalSizeMenuItem.Size = new System.Drawing.Size(286, 24);
            this.originalSizeMenuItem.Text = "Original Size";
            this.originalSizeMenuItem.Click += this.OriginalSizeMenuItem_Click;
            // 
            // fitWindowMenuItem
            // 
            this.fitWindowMenuItem.Name = "fitWindowMenuItem";
            this.fitWindowMenuItem.Size = new System.Drawing.Size(286, 24);
            this.fitWindowMenuItem.Text = "Fit To Window";
            this.fitWindowMenuItem.Click += this.FitWindowMenuItem_Click;
            // 
            // fitWindowLargeOnlyMenuItem
            // 
            this.fitWindowLargeOnlyMenuItem.Name = "fitWindowLargeOnlyMenuItem";
            this.fitWindowLargeOnlyMenuItem.Size = new System.Drawing.Size(286, 24);
            this.fitWindowLargeOnlyMenuItem.Text = "Fit To Window (Large Image Only)";
            this.fitWindowLargeOnlyMenuItem.Click += this.FitWindowLargeOnlyMenuItem_Click;
            // 
            // filePathToolTip
            // 
            this.filePathToolTip.AutoPopDelay = 5000;
            this.filePathToolTip.BackColor = System.Drawing.SystemColors.Window;
            this.filePathToolTip.InitialDelay = 50;
            this.filePathToolTip.ReshowDelay = 100;
            // 
            // ImageViewerToolBar
            // 
            this.Controls.Add(this.sizeButton);
            this.Controls.Add(this.viewButton);
            this.Controls.Add(this.indexSlider);
            this.Controls.Add(this.singleNextButton);
            this.Controls.Add(this.doubleNextButton);
            this.Controls.Add(this.singlePreviewButton);
            this.Controls.Add(this.doublePreviewButton);
            this.DoubleBuffered = true;
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
