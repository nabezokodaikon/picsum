namespace PicSum.UIComponent.Contents.ImageViewerContents
{
    partial class ImageViewerContents
    {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageViewerContents));
            this.leftImagePanel = new SWF.UIComponent.ImagePanel.ImagePanel();
            this.fileContextMenu = new PicSum.UIComponent.Common.FileContextMenu.FileContextMenu();
            this.rightImagePanel = new SWF.UIComponent.ImagePanel.ImagePanel();
            this.filePathToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.checkPatternPanel = new SWF.UIComponent.Common.CheckPatternPanel();
            this.viewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.singleViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftFacingViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightFacingViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.sizeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.originalSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allFitSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlyBigImageFitSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolBar = new PicSum.UIComponent.Contents.ContentsToolBar.ContentsToolBar();
            this.previewIndexToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextIndexToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.indexSlider = new SWF.UIComponent.Common.Slider();
            this.checkPatternPanel.SuspendLayout();
            this.viewContextMenuStrip.SuspendLayout();
            this.sizeContextMenuStrip.SuspendLayout();
            this.toolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // leftImagePanel
            // 
            this.leftImagePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.leftImagePanel.BackColor = System.Drawing.Color.Transparent;
            this.leftImagePanel.ContextMenuStrip = this.fileContextMenu;
            this.leftImagePanel.ImageAlign = SWF.UIComponent.ImagePanel.ImageAlign.Center;
            this.leftImagePanel.IsShowThumbnailPanel = true;
            this.leftImagePanel.Location = new System.Drawing.Point(316, 99);
            this.leftImagePanel.Margin = new System.Windows.Forms.Padding(4);
            this.leftImagePanel.Name = "leftImagePanel";
            this.leftImagePanel.Size = new System.Drawing.Size(173, 161);
            this.leftImagePanel.TabIndex = 1;
            this.leftImagePanel.Text = "imagePanel1";
            this.leftImagePanel.Visible = false;
            this.leftImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.leftImagePanel_ImageMouseClick);
            this.leftImagePanel.DragStart += new System.EventHandler(this.leftImagePanel_DragStart);
            this.leftImagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.leftImagePanel_MouseDown);
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.fileContextMenu.IsAddKeepMenuItemVisible = false;
            this.fileContextMenu.IsFileActiveTabOpenMenuItemVisible = false;
            this.fileContextMenu.IsFolderActiveTabOpenMenuItemVisible = true;
            this.fileContextMenu.IsRemoveFromListMenuItemVisible = false;
            this.fileContextMenu.Name = "fileContextMenu";
            this.fileContextMenu.Size = new System.Drawing.Size(205, 292);
            this.fileContextMenu.FileNewTabOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileNewTabOpen);
            this.fileContextMenu.FileNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileNewWindowOpen);
            this.fileContextMenu.FileOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileOpen);
            this.fileContextMenu.SaveFolderOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_SaveFolderOpen);
            this.fileContextMenu.Export += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_Export);
            this.fileContextMenu.PathCopy += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_PathCopy);
            this.fileContextMenu.NameCopy += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_NameCopy);
            this.fileContextMenu.AddKeep += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_AddKeep);
            this.fileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.fileContextMenu_Opening);
            // 
            // rightImagePanel
            // 
            this.rightImagePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rightImagePanel.BackColor = System.Drawing.Color.Transparent;
            this.rightImagePanel.ContextMenuStrip = this.fileContextMenu;
            this.rightImagePanel.ImageAlign = SWF.UIComponent.ImagePanel.ImageAlign.Center;
            this.rightImagePanel.IsShowThumbnailPanel = true;
            this.rightImagePanel.Location = new System.Drawing.Point(459, 208);
            this.rightImagePanel.Margin = new System.Windows.Forms.Padding(4);
            this.rightImagePanel.Name = "rightImagePanel";
            this.rightImagePanel.Size = new System.Drawing.Size(176, 180);
            this.rightImagePanel.TabIndex = 2;
            this.rightImagePanel.Text = "imagePanel2";
            this.rightImagePanel.Visible = false;
            this.rightImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.rightImagePanel_ImageMouseClick);
            this.rightImagePanel.DragStart += new System.EventHandler(this.rightImagePanel_DragStart);
            this.rightImagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rightImagePanel_MouseDown);
            // 
            // filePathToolTip
            // 
            this.filePathToolTip.AutoPopDelay = 5000;
            this.filePathToolTip.InitialDelay = 50;
            this.filePathToolTip.ReshowDelay = 100;
            // 
            // checkPatternPanel
            // 
            this.checkPatternPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkPatternPanel.Controls.Add(this.leftImagePanel);
            this.checkPatternPanel.Controls.Add(this.rightImagePanel);
            this.checkPatternPanel.Location = new System.Drawing.Point(0, 34);
            this.checkPatternPanel.Margin = new System.Windows.Forms.Padding(0);
            this.checkPatternPanel.Name = "checkPatternPanel";
            this.checkPatternPanel.RectangleSize = 24;
            this.checkPatternPanel.Size = new System.Drawing.Size(925, 494);
            this.checkPatternPanel.TabIndex = 3;
            // 
            // viewContextMenuStrip
            // 
            this.viewContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.viewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.singleViewToolStripMenuItem,
            this.leftFacingViewToolStripMenuItem,
            this.rightFacingViewToolStripMenuItem});
            this.viewContextMenuStrip.Name = "viewContextMenuStrip";
            this.viewContextMenuStrip.Size = new System.Drawing.Size(190, 76);
            // 
            // singleViewToolStripMenuItem
            // 
            this.singleViewToolStripMenuItem.Name = "singleViewToolStripMenuItem";
            this.singleViewToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.singleViewToolStripMenuItem.Text = "単一表示";
            this.singleViewToolStripMenuItem.Click += new System.EventHandler(this.singleViewToolStripMenuItem_Click);
            // 
            // leftFacingViewToolStripMenuItem
            // 
            this.leftFacingViewToolStripMenuItem.Name = "leftFacingViewToolStripMenuItem";
            this.leftFacingViewToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.leftFacingViewToolStripMenuItem.Text = "見開き（左送り）";
            this.leftFacingViewToolStripMenuItem.Click += new System.EventHandler(this.leftFacingViewToolStripMenuItem_Click);
            // 
            // rightFacingViewToolStripMenuItem
            // 
            this.rightFacingViewToolStripMenuItem.Name = "rightFacingViewToolStripMenuItem";
            this.rightFacingViewToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.rightFacingViewToolStripMenuItem.Text = "見開き（右送り）";
            this.rightFacingViewToolStripMenuItem.Click += new System.EventHandler(this.rightFacingViewToolStripMenuItem_Click);
            // 
            // viewToolStripSplitButton
            // 
            this.viewToolStripSplitButton.AutoToolTip = false;
            this.viewToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewToolStripSplitButton.DropDown = this.viewContextMenuStrip;
            this.viewToolStripSplitButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.viewToolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("viewToolStripSplitButton.Image")));
            this.viewToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewToolStripSplitButton.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.viewToolStripSplitButton.Name = "viewToolStripSplitButton";
            this.viewToolStripSplitButton.Size = new System.Drawing.Size(58, 24);
            this.viewToolStripSplitButton.Text = "表示";
            this.viewToolStripSplitButton.ButtonClick += new System.EventHandler(this.viewToolStripSplitButton_ButtonClick);
            // 
            // sizeContextMenuStrip
            // 
            this.sizeContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.sizeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.originalSizeToolStripMenuItem,
            this.allFitSizeToolStripMenuItem,
            this.onlyBigImageFitSizeToolStripMenuItem});
            this.sizeContextMenuStrip.Name = "sizeContextMenuStrip";
            this.sizeContextMenuStrip.OwnerItem = this.sizeToolStripSplitButton;
            this.sizeContextMenuStrip.Size = new System.Drawing.Size(316, 76);
            // 
            // originalSizeToolStripMenuItem
            // 
            this.originalSizeToolStripMenuItem.Name = "originalSizeToolStripMenuItem";
            this.originalSizeToolStripMenuItem.Size = new System.Drawing.Size(315, 24);
            this.originalSizeToolStripMenuItem.Text = "原寸大";
            this.originalSizeToolStripMenuItem.Click += new System.EventHandler(this.originalSizeToolStripMenuItem_Click);
            // 
            // allFitSizeToolStripMenuItem
            // 
            this.allFitSizeToolStripMenuItem.Name = "allFitSizeToolStripMenuItem";
            this.allFitSizeToolStripMenuItem.Size = new System.Drawing.Size(315, 24);
            this.allFitSizeToolStripMenuItem.Text = "ウィンドウにあわせる";
            this.allFitSizeToolStripMenuItem.Click += new System.EventHandler(this.allFitSizeToolStripMenuItem_Click);
            // 
            // onlyBigImageFitSizeToolStripMenuItem
            // 
            this.onlyBigImageFitSizeToolStripMenuItem.Name = "onlyBigImageFitSizeToolStripMenuItem";
            this.onlyBigImageFitSizeToolStripMenuItem.Size = new System.Drawing.Size(315, 24);
            this.onlyBigImageFitSizeToolStripMenuItem.Text = "ウィンドウにあわせる（大きい画像のみ）";
            this.onlyBigImageFitSizeToolStripMenuItem.Click += new System.EventHandler(this.onlyBigImageFitSizeToolStripMenuItem_Click);
            // 
            // sizeToolStripSplitButton
            // 
            this.sizeToolStripSplitButton.AutoToolTip = false;
            this.sizeToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sizeToolStripSplitButton.DropDown = this.sizeContextMenuStrip;
            this.sizeToolStripSplitButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.sizeToolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("sizeToolStripSplitButton.Image")));
            this.sizeToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sizeToolStripSplitButton.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.sizeToolStripSplitButton.Name = "sizeToolStripSplitButton";
            this.sizeToolStripSplitButton.Size = new System.Drawing.Size(62, 24);
            this.sizeToolStripSplitButton.Text = "サイズ";
            this.sizeToolStripSplitButton.ButtonClick += new System.EventHandler(this.sizeToolStripSplitButton_ButtonClick);
            // 
            // toolBar
            // 
            this.toolBar.CanOverflow = false;
            this.toolBar.Dock = System.Windows.Forms.DockStyle.None;
            this.toolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripSplitButton,
            this.sizeToolStripSplitButton,
            this.previewIndexToolStripButton,
            this.nextIndexToolStripButton});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(201, 27);
            this.toolBar.TabIndex = 4;
            this.toolBar.Text = "toolStrip1";
            // 
            // previewIndexToolStripButton
            // 
            this.previewIndexToolStripButton.AutoToolTip = false;
            this.previewIndexToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previewIndexToolStripButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.previewIndexToolStripButton.Image = global::PicSum.UIComponent.Contents.Properties.Resources.MiddleArrowLeft;
            this.previewIndexToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.previewIndexToolStripButton.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.previewIndexToolStripButton.Name = "previewIndexToolStripButton";
            this.previewIndexToolStripButton.Size = new System.Drawing.Size(29, 24);
            this.previewIndexToolStripButton.Text = "toolStripButton1";
            this.previewIndexToolStripButton.Click += new System.EventHandler(this.previewIndexToolStripButton_Click);
            // 
            // nextIndexToolStripButton
            // 
            this.nextIndexToolStripButton.AutoToolTip = false;
            this.nextIndexToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextIndexToolStripButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.nextIndexToolStripButton.Image = global::PicSum.UIComponent.Contents.Properties.Resources.MiddleArrowRight;
            this.nextIndexToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextIndexToolStripButton.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.nextIndexToolStripButton.Name = "nextIndexToolStripButton";
            this.nextIndexToolStripButton.Size = new System.Drawing.Size(29, 24);
            this.nextIndexToolStripButton.Text = "toolStripButton2";
            this.nextIndexToolStripButton.Click += new System.EventHandler(this.nextIndexToolStripButton_Click);
            // 
            // indexSlider
            // 
            this.indexSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.indexSlider.Location = new System.Drawing.Point(240, 4);
            this.indexSlider.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.indexSlider.MaximumValue = 100;
            this.indexSlider.MinimumValue = 0;
            this.indexSlider.Name = "indexSlider";
            this.indexSlider.Size = new System.Drawing.Size(650, 23);
            this.indexSlider.TabIndex = 5;
            this.indexSlider.Text = "slider1";
            this.indexSlider.Value = 0;
            this.indexSlider.ValueChanging += new System.EventHandler(this.indexSlider_ValueChanging);
            this.indexSlider.ValueChanged += new System.EventHandler(this.indexSlider_ValueChanged);
            // 
            // ImageViewerContents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.indexSlider);
            this.Controls.Add(this.checkPatternPanel);
            this.Controls.Add(this.toolBar);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ImageViewerContents";
            this.Size = new System.Drawing.Size(925, 528);
            this.checkPatternPanel.ResumeLayout(false);
            this.viewContextMenuStrip.ResumeLayout(false);
            this.sizeContextMenuStrip.ResumeLayout(false);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SWF.UIComponent.ImagePanel.ImagePanel leftImagePanel;
        private SWF.UIComponent.ImagePanel.ImagePanel rightImagePanel;
        private PicSum.UIComponent.Common.FileContextMenu.FileContextMenu fileContextMenu;
        private System.Windows.Forms.ToolTip filePathToolTip;
        private SWF.UIComponent.Common.CheckPatternPanel checkPatternPanel;
        private PicSum.UIComponent.Contents.ContentsToolBar.ContentsToolBar toolBar;
        private System.Windows.Forms.ToolStripSplitButton viewToolStripSplitButton;
        private System.Windows.Forms.ToolStripSplitButton sizeToolStripSplitButton;
        private System.Windows.Forms.ToolStripButton previewIndexToolStripButton;
        private System.Windows.Forms.ToolStripButton nextIndexToolStripButton;
        private System.Windows.Forms.ContextMenuStrip viewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem singleViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftFacingViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightFacingViewToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip sizeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem originalSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allFitSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onlyBigImageFitSizeToolStripMenuItem;
        private SWF.UIComponent.Common.Slider indexSlider;
    }
}
