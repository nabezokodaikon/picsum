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
            this.checkPatternPanel = new SWF.UIComponent.Core.CheckPatternPanel();
            this.toolBar = new PicSum.UIComponent.Contents.ImageViewer.ImageViewerToolBar();
            this.checkPatternPanel.SuspendLayout();
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
            this.leftImagePanel.Name = "leftImagePanel";
            this.leftImagePanel.Size = new System.Drawing.Size(0, 0);
            this.leftImagePanel.Text = "imagePanel1";
            this.leftImagePanel.Visible = false;
            this.leftImagePanel.Font = new System.Drawing.Font("Yu Gothic UI", 18F);
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
            this.fileContextMenu.SelectApplication += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SelectApplication);
            this.fileContextMenu.SaveDirectoryOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SaveDirectoryOpen);
            this.fileContextMenu.PathCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_PathCopy);
            this.fileContextMenu.NameCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_NameCopy);
            this.fileContextMenu.Bookmark += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_Bookmark);
            this.fileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FileContextMenu_Opening);
            // 
            // rightImagePanel
            // 
            this.rightImagePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rightImagePanel.BackColor = System.Drawing.Color.Transparent;
            this.rightImagePanel.ContextMenuStrip = this.fileContextMenu;
            this.rightImagePanel.ImageAlign = SWF.Core.Base.ImageAlign.Center;
            this.rightImagePanel.IsShowThumbnailPanel = true;
            this.rightImagePanel.Location = new System.Drawing.Point(0, 0);
            this.rightImagePanel.Name = "rightImagePanel";
            this.rightImagePanel.Size = new System.Drawing.Size(0, 0);
            this.rightImagePanel.Text = "imagePanel2";
            this.rightImagePanel.Visible = false;
            this.rightImagePanel.Font = new System.Drawing.Font("Yu Gothic UI", 18F);
            this.rightImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.RightImagePanel_ImageMouseClick);
            this.rightImagePanel.DragStart += new System.EventHandler(this.RightImagePanel_DragStart);
            this.rightImagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RightImagePanel_MouseDown);
            this.rightImagePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightImagePanel_MouseUp);
            // 
            // checkPatternPanel
            // 
            this.checkPatternPanel.Controls.Add(this.leftImagePanel);
            this.checkPatternPanel.Controls.Add(this.rightImagePanel);
            this.checkPatternPanel.Name = "checkPatternPanel";
            this.checkPatternPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // toolBar
            // 
            this.toolBar.IsDrawBottomBorderLine = true;
            this.toolBar.Visible = false;
            this.toolBar.DoublePreviewButtonClick += new System.EventHandler(this.DoublePreviewIndexToolStripButton_Click);
            this.toolBar.DoubleNextButtonClick += new System.EventHandler(this.DoubleNextIndexToolStripButton_Click);
            this.toolBar.SinglePreviewButtonClick += new System.EventHandler(this.SinglePreviewIndexToolStripButton_Click);
            this.toolBar.SingleNextButtonClick += new System.EventHandler(this.SingleNextIndexToolStripButton_Click);
            this.toolBar.SingleViewMenuItemClick += new System.EventHandler(this.SingleViewToolStripMenuItem_Click);
            this.toolBar.SpreadLeftFeedMenuItemClick += new System.EventHandler(this.LeftFacingViewToolStripMenuItem_Click);
            this.toolBar.SpreadRightFeedMenuItemClick += new System.EventHandler(this.RightFacingViewToolStripMenuItem_Click);
            this.toolBar.OriginalSizeMenuItemClick += new System.EventHandler(this.OriginalSizeToolStripMenuItem_Click);
            this.toolBar.FitWindowMenuItemClick += new System.EventHandler(this.AllFitSizeToolStripMenuItem_Click);
            this.toolBar.FitWindowLargeOnlyMenuItemClick += new System.EventHandler(this.OnlyBigImageFitSizeToolStripMenuItem_Click);
            this.toolBar.IndexSliderValueChanging += new System.EventHandler(this.IndexSlider_ValueChanging);
            this.toolBar.IndexSliderValueChanged += new System.EventHandler(this.IndexSlider_ValueChanged);
            // 
            // ImageViewerPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.Controls.Add(this.checkPatternPanel);
            this.Controls.Add(this.toolBar);
            this.Name = "ImageViewerPage";
            this.Size = new System.Drawing.Size(925, 528);
            this.checkPatternPanel.ResumeLayout(false);
            this.toolBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.ImagePanel.ImagePanel leftImagePanel;
        private SWF.UIComponent.ImagePanel.ImagePanel rightImagePanel;
        private PicSum.UIComponent.Contents.ContextMenu.FileContextMenu fileContextMenu;
        private SWF.UIComponent.Core.CheckPatternPanel checkPatternPanel;
        private PicSum.UIComponent.Contents.ImageViewer.ImageViewerToolBar toolBar;

        #endregion
    }
}
