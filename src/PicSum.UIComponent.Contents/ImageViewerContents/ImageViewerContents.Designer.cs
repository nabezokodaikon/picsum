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
            this.indexToolStripSlider = new SWF.UIComponent.Common.ToolStripSlider();
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
            this.leftImagePanel.Location = new System.Drawing.Point(237, 79);
            this.leftImagePanel.Name = "leftImagePanel";
            this.leftImagePanel.Size = new System.Drawing.Size(130, 129);
            this.leftImagePanel.TabIndex = 1;
            this.leftImagePanel.Text = "imagePanel1";
            this.leftImagePanel.Visible = false;
            this.leftImagePanel.DragStart += new System.EventHandler(this.leftImagePanel_DragStart);
            this.leftImagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.leftImagePanel_MouseDown);
            this.leftImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.leftImagePanel_ImageMouseClick);
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.IsAddKeepMenuItemVisible = false;
            this.fileContextMenu.IsFileActiveTabOpenMenuItemVisible = false;
            this.fileContextMenu.IsFolderActiveTabOpenMenuItemVisible = true;
            this.fileContextMenu.IsRemoveFromListMenuItemVisible = false;
            this.fileContextMenu.Name = "fileContextMenu";
            this.fileContextMenu.Size = new System.Drawing.Size(209, 312);
            this.fileContextMenu.FileOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileOpen);
            this.fileContextMenu.FileNewTabOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileNewTabOpen);
            this.fileContextMenu.SaveFolderOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_SaveFolderOpen);
            this.fileContextMenu.FileOtherWindowOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileOtherWindowOpen);
            this.fileContextMenu.AddKeep += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_AddKeep);
            this.fileContextMenu.NameCopy += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_NameCopy);
            this.fileContextMenu.Export += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_Export);
            this.fileContextMenu.FileNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileNewWindowOpen);
            this.fileContextMenu.PathCopy += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_PathCopy);
            this.fileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.fileContextMenu_Opening);
            // 
            // rightImagePanel
            // 
            this.rightImagePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rightImagePanel.BackColor = System.Drawing.Color.Transparent;
            this.rightImagePanel.ContextMenuStrip = this.fileContextMenu;
            this.rightImagePanel.ImageAlign = SWF.UIComponent.ImagePanel.ImageAlign.Center;
            this.rightImagePanel.IsShowThumbnailPanel = true;
            this.rightImagePanel.Location = new System.Drawing.Point(344, 166);
            this.rightImagePanel.Name = "rightImagePanel";
            this.rightImagePanel.Size = new System.Drawing.Size(132, 144);
            this.rightImagePanel.TabIndex = 2;
            this.rightImagePanel.Text = "imagePanel2";
            this.rightImagePanel.Visible = false;
            this.rightImagePanel.DragStart += new System.EventHandler(this.rightImagePanel_DragStart);
            this.rightImagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rightImagePanel_MouseDown);
            this.rightImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.rightImagePanel_ImageMouseClick);
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
            this.checkPatternPanel.Location = new System.Drawing.Point(0, 27);
            this.checkPatternPanel.Margin = new System.Windows.Forms.Padding(0);
            this.checkPatternPanel.Name = "checkPatternPanel";
            this.checkPatternPanel.RectangleSize = 24;
            this.checkPatternPanel.Size = new System.Drawing.Size(694, 395);
            this.checkPatternPanel.TabIndex = 3;
            // 
            // viewContextMenuStrip
            // 
            this.viewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.singleViewToolStripMenuItem,
            this.leftFacingViewToolStripMenuItem,
            this.rightFacingViewToolStripMenuItem});
            this.viewContextMenuStrip.Name = "viewContextMenuStrip";
            this.viewContextMenuStrip.Size = new System.Drawing.Size(173, 70);
            // 
            // singleViewToolStripMenuItem
            // 
            this.singleViewToolStripMenuItem.Name = "singleViewToolStripMenuItem";
            this.singleViewToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.singleViewToolStripMenuItem.Text = "単一表示";
            this.singleViewToolStripMenuItem.Click += new System.EventHandler(this.singleViewToolStripMenuItem_Click);
            // 
            // leftFacingViewToolStripMenuItem
            // 
            this.leftFacingViewToolStripMenuItem.Name = "leftFacingViewToolStripMenuItem";
            this.leftFacingViewToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.leftFacingViewToolStripMenuItem.Text = "見開き（左送り）";
            this.leftFacingViewToolStripMenuItem.Click += new System.EventHandler(this.leftFacingViewToolStripMenuItem_Click);
            // 
            // rightFacingViewToolStripMenuItem
            // 
            this.rightFacingViewToolStripMenuItem.Name = "rightFacingViewToolStripMenuItem";
            this.rightFacingViewToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.rightFacingViewToolStripMenuItem.Text = "見開き（右送り）";
            this.rightFacingViewToolStripMenuItem.Click += new System.EventHandler(this.rightFacingViewToolStripMenuItem_Click);
            // 
            // viewToolStripSplitButton
            // 
            this.viewToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewToolStripSplitButton.DropDown = this.viewContextMenuStrip;
            this.viewToolStripSplitButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.viewToolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("viewToolStripSplitButton.Image")));
            this.viewToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewToolStripSplitButton.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.viewToolStripSplitButton.Name = "viewToolStripSplitButton";
            this.viewToolStripSplitButton.Size = new System.Drawing.Size(48, 24);
            this.viewToolStripSplitButton.Text = "表示";
            this.viewToolStripSplitButton.ButtonClick += new System.EventHandler(this.viewToolStripSplitButton_ButtonClick);
            // 
            // sizeContextMenuStrip
            // 
            this.sizeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.originalSizeToolStripMenuItem,
            this.allFitSizeToolStripMenuItem,
            this.onlyBigImageFitSizeToolStripMenuItem});
            this.sizeContextMenuStrip.Name = "sizeContextMenuStrip";
            this.sizeContextMenuStrip.OwnerItem = this.sizeToolStripSplitButton;
            this.sizeContextMenuStrip.Size = new System.Drawing.Size(305, 70);
            // 
            // originalSizeToolStripMenuItem
            // 
            this.originalSizeToolStripMenuItem.Name = "originalSizeToolStripMenuItem";
            this.originalSizeToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.originalSizeToolStripMenuItem.Text = "原寸大";
            this.originalSizeToolStripMenuItem.Click += new System.EventHandler(this.originalSizeToolStripMenuItem_Click);
            // 
            // allFitSizeToolStripMenuItem
            // 
            this.allFitSizeToolStripMenuItem.Name = "allFitSizeToolStripMenuItem";
            this.allFitSizeToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.allFitSizeToolStripMenuItem.Text = "ウィンドウにあわせる";
            this.allFitSizeToolStripMenuItem.Click += new System.EventHandler(this.allFitSizeToolStripMenuItem_Click);
            // 
            // onlyBigImageFitSizeToolStripMenuItem
            // 
            this.onlyBigImageFitSizeToolStripMenuItem.Name = "onlyBigImageFitSizeToolStripMenuItem";
            this.onlyBigImageFitSizeToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.onlyBigImageFitSizeToolStripMenuItem.Text = "ウィンドウにあわせる（大きい画像のみ）";
            this.onlyBigImageFitSizeToolStripMenuItem.Click += new System.EventHandler(this.onlyBigImageFitSizeToolStripMenuItem_Click);
            // 
            // sizeToolStripSplitButton
            // 
            this.sizeToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sizeToolStripSplitButton.DropDown = this.sizeContextMenuStrip;
            this.sizeToolStripSplitButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.sizeToolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("sizeToolStripSplitButton.Image")));
            this.sizeToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sizeToolStripSplitButton.Name = "sizeToolStripSplitButton";
            this.sizeToolStripSplitButton.Size = new System.Drawing.Size(60, 24);
            this.sizeToolStripSplitButton.Text = "サイズ";
            this.sizeToolStripSplitButton.ButtonClick += new System.EventHandler(this.sizeToolStripSplitButton_ButtonClick);
            // 
            // toolStrip
            // 
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripSplitButton,
            this.sizeToolStripSplitButton,
            this.previewIndexToolStripButton,
            this.nextIndexToolStripButton,
            this.indexToolStripSlider});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolStrip";
            this.toolBar.Size = new System.Drawing.Size(694, 27);
            this.toolBar.TabIndex = 4;
            this.toolBar.Text = "toolStrip1";
            // 
            // previewIndexToolStripButton
            // 
            this.previewIndexToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previewIndexToolStripButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.previewIndexToolStripButton.Image = global::PicSum.UIComponent.Contents.Properties.Resources.MiddleArrowLeft;
            this.previewIndexToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.previewIndexToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.previewIndexToolStripButton.Name = "previewIndexToolStripButton";
            this.previewIndexToolStripButton.Size = new System.Drawing.Size(23, 24);
            this.previewIndexToolStripButton.Text = "toolStripButton1";
            this.previewIndexToolStripButton.Click += new System.EventHandler(this.previewIndexToolStripButton_Click);
            // 
            // nextIndexToolStripButton
            // 
            this.nextIndexToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextIndexToolStripButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.nextIndexToolStripButton.Image = global::PicSum.UIComponent.Contents.Properties.Resources.MiddleArrowRight;
            this.nextIndexToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextIndexToolStripButton.Name = "nextIndexToolStripButton";
            this.nextIndexToolStripButton.Size = new System.Drawing.Size(23, 24);
            this.nextIndexToolStripButton.Text = "toolStripButton2";
            this.nextIndexToolStripButton.Click += new System.EventHandler(this.nextIndexToolStripButton_Click);
            // 
            // indexToolStripSlider
            // 
            this.indexToolStripSlider.BackColor = System.Drawing.Color.Transparent;
            this.indexToolStripSlider.MaximumValue = 100;
            this.indexToolStripSlider.MinimumValue = 0;
            this.indexToolStripSlider.Name = "indexToolStripSlider";
            this.indexToolStripSlider.Size = new System.Drawing.Size(96, 24);
            this.indexToolStripSlider.Text = "toolStripSlider1";
            this.indexToolStripSlider.Value = 0;
            this.indexToolStripSlider.ValueChanging += new System.EventHandler(this.indexToolStripSlider_ValueChanging);
            this.indexToolStripSlider.ValueChanged += new System.EventHandler(this.indexToolStripSlider_ValueChanged);
            // 
            // ImageViewerContents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkPatternPanel);
            this.Controls.Add(this.toolBar);
            this.Name = "ImageViewerContents";
            this.Size = new System.Drawing.Size(694, 422);
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
        private SWF.UIComponent.Common.ToolStripSlider indexToolStripSlider;
        private System.Windows.Forms.ContextMenuStrip viewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem singleViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftFacingViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightFacingViewToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip sizeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem originalSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allFitSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onlyBigImageFitSizeToolStripMenuItem;
    }
}
