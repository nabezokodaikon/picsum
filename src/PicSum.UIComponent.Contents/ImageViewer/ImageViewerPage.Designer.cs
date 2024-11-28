using PicSum.UIComponent.Contents.ContextMenu;
using System;

namespace PicSum.UIComponent.Contents.ImageViewer
{
    partial class ImageViewerPage
    {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageViewerPage));
            this.leftImagePanel = new SWF.UIComponent.ImagePanel.ImagePanel();
            this.fileContextMenu = new PicSum.UIComponent.Contents.ContextMenu.FileContextMenu();
            this.rightImagePanel = new SWF.UIComponent.ImagePanel.ImagePanel();
            this.filePathToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.checkPatternPanel = new SWF.UIComponent.Core.CheckPatternPanel();
            this.viewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.singleViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftFacingViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightFacingViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.sizeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.originalSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allFitSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlyBigImageFitSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolBar = new PicSum.UIComponent.Contents.ToolBar.PageToolBar();
            this.doublePreviewIndexToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.doubleNextIndexToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.singlePreviewIndexToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.singleNextIndexToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.indexSlider = new SWF.UIComponent.Core.Slider();
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
            this.leftImagePanel.ImageAlign = SWF.Core.Base.ImageAlign.Center;
            this.leftImagePanel.IsShowThumbnailPanel = true;
            this.leftImagePanel.Location = new System.Drawing.Point(0, 0);
            this.leftImagePanel.Margin = new System.Windows.Forms.Padding(4);
            this.leftImagePanel.Name = "leftImagePanel";
            this.leftImagePanel.Size = new System.Drawing.Size(0, 0);
            this.leftImagePanel.TabIndex = 1;
            this.leftImagePanel.Text = "imagePanel1";
            this.leftImagePanel.Visible = false;
            this.leftImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.LeftImagePanel_ImageMouseClick);
            this.leftImagePanel.DragStart += new System.EventHandler(this.LeftImagePanel_DragStart);
            this.leftImagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LeftImagePanel_MouseDown);
            this.leftImagePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LeftImagePanel_MouseUp);
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.fileContextMenu.VisibleRemoveFromListMenuItem = false;
            this.fileContextMenu.Name = "fileContextMenu";
            this.fileContextMenu.Size = new System.Drawing.Size(205, 292);
            this.fileContextMenu.FileNewTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewTabOpen);
            this.fileContextMenu.FileNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewWindowOpen);
            this.fileContextMenu.FileOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileOpen);
            this.fileContextMenu.SelectApplication += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SelectApplication);
            this.fileContextMenu.SaveDirectoryOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SaveDirectoryOpen);
            this.fileContextMenu.Export += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_Export);
            this.fileContextMenu.PathCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_PathCopy);
            this.fileContextMenu.NameCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_NameCopy);
            this.fileContextMenu.Bookmark += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_Bookmark);
            this.fileContextMenu.Clip += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_Clip);
            this.fileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FileContextMenu_Opening);
            this.fileContextMenu.ConvertToAvif += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToAvif);
            this.fileContextMenu.ConvertToBitmap += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToBitmap);
            this.fileContextMenu.ConvertToHeif += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToHeif);
            this.fileContextMenu.ConvertToIcon += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToIcon);
            this.fileContextMenu.ConvertToJpeg += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToJpeg);
            this.fileContextMenu.ConvertToPng += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToPng);
            this.fileContextMenu.ConvertToSvg += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToSvg);
            this.fileContextMenu.ConvertToWebp += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToWebp);
            // 
            // rightImagePanel
            // 
            this.rightImagePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rightImagePanel.BackColor = System.Drawing.Color.Transparent;
            this.rightImagePanel.ContextMenuStrip = this.fileContextMenu;
            this.rightImagePanel.ImageAlign = SWF.Core.Base.ImageAlign.Center;
            this.rightImagePanel.IsShowThumbnailPanel = true;
            this.rightImagePanel.Location = new System.Drawing.Point(0, 0);
            this.rightImagePanel.Margin = new System.Windows.Forms.Padding(4);
            this.rightImagePanel.Name = "rightImagePanel";
            this.rightImagePanel.Size = new System.Drawing.Size(0, 0);
            this.rightImagePanel.TabIndex = 2;
            this.rightImagePanel.Text = "imagePanel2";
            this.rightImagePanel.Visible = false;
            this.rightImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.RightImagePanel_ImageMouseClick);
            this.rightImagePanel.DragStart += new System.EventHandler(this.RightImagePanel_DragStart);
            this.rightImagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RightImagePanel_MouseDown);
            this.rightImagePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightImagePanel_MouseUp);
            // 
            // filePathToolTip
            // 
            this.filePathToolTip.AutoPopDelay = 5000;
            this.filePathToolTip.InitialDelay = 50;
            this.filePathToolTip.ReshowDelay = 100;
            // 
            // checkPatternPanel
            // 
            this.checkPatternPanel.Controls.Add(this.leftImagePanel);
            this.checkPatternPanel.Controls.Add(this.rightImagePanel);
            this.checkPatternPanel.Margin = new System.Windows.Forms.Padding(0);
            this.checkPatternPanel.Name = "checkPatternPanel";
            this.checkPatternPanel.RectangleSize = 24;
            this.checkPatternPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkPatternPanel.TabIndex = 3;
            this.checkPatternPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // viewContextMenuStrip
            // 
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
            this.singleViewToolStripMenuItem.Text = "Single View";
            this.singleViewToolStripMenuItem.Click += new System.EventHandler(this.SingleViewToolStripMenuItem_Click);
            // 
            // leftFacingViewToolStripMenuItem
            // 
            this.leftFacingViewToolStripMenuItem.Name = "leftFacingViewToolStripMenuItem";
            this.leftFacingViewToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.leftFacingViewToolStripMenuItem.Text = "Spread (Left Feed)";
            this.leftFacingViewToolStripMenuItem.Click += new System.EventHandler(this.LeftFacingViewToolStripMenuItem_Click);
            // 
            // rightFacingViewToolStripMenuItem
            // 
            this.rightFacingViewToolStripMenuItem.Name = "rightFacingViewToolStripMenuItem";
            this.rightFacingViewToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.rightFacingViewToolStripMenuItem.Text = "Spread (Right Feed)";
            this.rightFacingViewToolStripMenuItem.Click += new System.EventHandler(this.RightFacingViewToolStripMenuItem_Click);
            // 
            // viewToolStripSplitButton
            // 
            this.viewToolStripDropDownButton.AutoToolTip = false;
            this.viewToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewToolStripDropDownButton.DropDown = this.viewContextMenuStrip;
            this.viewToolStripDropDownButton.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.viewToolStripDropDownButton.Name = "viewToolStripSplitButton";
            this.viewToolStripDropDownButton.Text = "View";
            // 
            // sizeContextMenuStrip
            // 
            this.sizeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.originalSizeToolStripMenuItem,
            this.allFitSizeToolStripMenuItem,
            this.onlyBigImageFitSizeToolStripMenuItem});
            this.sizeContextMenuStrip.Name = "sizeContextMenuStrip";
            this.sizeContextMenuStrip.OwnerItem = this.sizeToolStripDropDownButton;
            this.sizeContextMenuStrip.Size = new System.Drawing.Size(316, 76);
            // 
            // originalSizeToolStripMenuItem
            // 
            this.originalSizeToolStripMenuItem.Name = "originalSizeToolStripMenuItem";
            this.originalSizeToolStripMenuItem.Size = new System.Drawing.Size(315, 24);
            this.originalSizeToolStripMenuItem.Text = "Original Size";
            this.originalSizeToolStripMenuItem.Click += new System.EventHandler(this.OriginalSizeToolStripMenuItem_Click);
            // 
            // allFitSizeToolStripMenuItem
            // 
            this.allFitSizeToolStripMenuItem.Name = "allFitSizeToolStripMenuItem";
            this.allFitSizeToolStripMenuItem.Size = new System.Drawing.Size(315, 24);
            this.allFitSizeToolStripMenuItem.Text = "Fit To Window";
            this.allFitSizeToolStripMenuItem.Click += new System.EventHandler(this.AllFitSizeToolStripMenuItem_Click);
            // 
            // onlyBigImageFitSizeToolStripMenuItem
            // 
            this.onlyBigImageFitSizeToolStripMenuItem.Name = "onlyBigImageFitSizeToolStripMenuItem";
            this.onlyBigImageFitSizeToolStripMenuItem.Size = new System.Drawing.Size(315, 24);
            this.onlyBigImageFitSizeToolStripMenuItem.Text = "Fit To Window (Large Image Only)";
            this.onlyBigImageFitSizeToolStripMenuItem.Click += new System.EventHandler(this.OnlyBigImageFitSizeToolStripMenuItem_Click);
            // 
            // sizeToolStripSplitButton
            // 
            this.sizeToolStripDropDownButton.AutoToolTip = false;
            this.sizeToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sizeToolStripDropDownButton.DropDown = this.sizeContextMenuStrip;
            this.sizeToolStripDropDownButton.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.sizeToolStripDropDownButton.Name = "sizeToolStripSplitButton";
            this.sizeToolStripDropDownButton.Text = "Size";
            // 
            // toolBar
            // 
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripDropDownButton,
            this.sizeToolStripDropDownButton,
            this.doublePreviewIndexToolStripButton,
            this.singlePreviewIndexToolStripButton,
            this.singleNextIndexToolStripButton,
            this.doubleNextIndexToolStripButton});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(201, 27);
            this.toolBar.TabIndex = 4;
            this.toolBar.Text = "toolStrip1";
            // 
            // previewIndexToolStripButton
            //
            this.doublePreviewIndexToolStripButton.AutoSize = false;
            this.doublePreviewIndexToolStripButton.AutoToolTip = false;
            this.doublePreviewIndexToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.doublePreviewIndexToolStripButton.Name = "previewIndexToolStripButton";
            this.doublePreviewIndexToolStripButton.Size = new System.Drawing.Size(64, 24);
            this.doublePreviewIndexToolStripButton.Text = "<<-";
            this.doublePreviewIndexToolStripButton.Click += new System.EventHandler(this.DoublePreviewIndexToolStripButton_Click);
            // 
            // nextIndexToolStripButton
            //
            this.doubleNextIndexToolStripButton.AutoSize = false;
            this.doubleNextIndexToolStripButton.AutoToolTip = false;
            this.doubleNextIndexToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.doubleNextIndexToolStripButton.Name = "nextIndexToolStripButton";
            this.doubleNextIndexToolStripButton.Size = new System.Drawing.Size(64, 24);
            this.doubleNextIndexToolStripButton.Text = "->>";
            this.doubleNextIndexToolStripButton.Click += new System.EventHandler(this.DoubleNextIndexToolStripButton_Click);
            // 
            // singlePreviewIndexToolStripButton
            //
            this.singlePreviewIndexToolStripButton.AutoSize = false;
            this.singlePreviewIndexToolStripButton.AutoToolTip = false;
            this.singlePreviewIndexToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.singlePreviewIndexToolStripButton.Name = "singlePreviewIndexToolStripButton";
            this.singlePreviewIndexToolStripButton.Size = new System.Drawing.Size(64, 24);
            this.singlePreviewIndexToolStripButton.Text = "<-";
            this.singlePreviewIndexToolStripButton.Click += new System.EventHandler(this.SinglePreviewIndexToolStripButton_Click);
            // 
            // singleNextIndexToolStripButton
            //
            this.singleNextIndexToolStripButton.AutoSize = false;
            this.singleNextIndexToolStripButton.AutoToolTip = false;
            this.singleNextIndexToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.singleNextIndexToolStripButton.Name = "singleNextIndexToolStripButton";
            this.singleNextIndexToolStripButton.Size = new System.Drawing.Size(64, 24);
            this.singleNextIndexToolStripButton.Text = "->";
            this.singleNextIndexToolStripButton.Click += new System.EventHandler(this.SingleNextIndexToolStripButton_Click);
            // 
            // indexSlider
            // 
            this.indexSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.indexSlider.BackColor = System.Drawing.Color.Transparent;
            this.indexSlider.Location = new System.Drawing.Point(430, 2);
            this.indexSlider.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.indexSlider.MaximumValue = 100;
            this.indexSlider.MinimumValue = 0;
            this.indexSlider.Name = "indexSlider";
            this.indexSlider.Size = new System.Drawing.Size(475, 23);
            this.indexSlider.TabIndex = 5;
            this.indexSlider.Text = "slider1";
            this.indexSlider.Value = 0;
            this.indexSlider.ValueChanging += new System.EventHandler(this.IndexSlider_ValueChanging);
            this.indexSlider.ValueChanged += new System.EventHandler(this.IndexSlider_ValueChanged);
            // 
            // ImageViewerPage
            //
            this.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.ImageViewerPage_GiveFeedback);
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.indexSlider);
            this.Controls.Add(this.checkPatternPanel);
            this.Controls.Add(this.toolBar);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ImageViewerPage";
            this.Size = new System.Drawing.Size(925, 528);
            this.checkPatternPanel.ResumeLayout(false);
            this.viewContextMenuStrip.ResumeLayout(false);
            this.sizeContextMenuStrip.ResumeLayout(false);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private SWF.UIComponent.ImagePanel.ImagePanel leftImagePanel;
        private SWF.UIComponent.ImagePanel.ImagePanel rightImagePanel;
        private PicSum.UIComponent.Contents.ContextMenu.FileContextMenu fileContextMenu;
        private System.Windows.Forms.ToolTip filePathToolTip;
        private SWF.UIComponent.Core.CheckPatternPanel checkPatternPanel;
        private PicSum.UIComponent.Contents.ToolBar.PageToolBar toolBar;
        private System.Windows.Forms.ToolStripDropDownButton viewToolStripDropDownButton;
        private System.Windows.Forms.ToolStripDropDownButton sizeToolStripDropDownButton;
        private System.Windows.Forms.ToolStripButton doublePreviewIndexToolStripButton;
        private System.Windows.Forms.ToolStripButton doubleNextIndexToolStripButton;
        private System.Windows.Forms.ToolStripButton singlePreviewIndexToolStripButton;
        private System.Windows.Forms.ToolStripButton singleNextIndexToolStripButton;
        private System.Windows.Forms.ContextMenuStrip viewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem singleViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftFacingViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightFacingViewToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip sizeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem originalSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allFitSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onlyBigImageFitSizeToolStripMenuItem;
        private SWF.UIComponent.Core.Slider indexSlider;

        #endregion
    }
}
