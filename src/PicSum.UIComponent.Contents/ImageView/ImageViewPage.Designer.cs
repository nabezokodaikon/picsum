using PicSum.UIComponent.Contents.ContextMenu;
using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.TabOperation;
using System;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ImageView
{
    partial class ImageViewPage
    {
        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.leftImagePanel = new PicSum.UIComponent.Contents.ImageView.ImagePanel();
            this.fileContextMenu = new PicSum.UIComponent.Contents.ContextMenu.FileContextMenu();
            this.rightImagePanel = new PicSum.UIComponent.Contents.ImageView.ImagePanel();
            this.checkPatternPanel = new SWF.UIComponent.Core.CheckPatternPanel();
            this.toolBar = new PicSum.UIComponent.Contents.ImageView.ImageViewToolBar();
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
            this.leftImagePanel.Visible = false;
            this.leftImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.LeftImagePanel_ImageMouseClick);
            this.leftImagePanel.DragStart += new System.EventHandler(this.LeftImagePanel_DragStart);
            this.leftImagePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LeftImagePanel_MouseUp);
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.fileContextMenu.VisibleRemoveFromListMenuItem = false;
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
            this.rightImagePanel.Visible = false;
            this.rightImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.RightImagePanel_ImageMouseClick);
            this.rightImagePanel.DragStart += new System.EventHandler(this.RightImagePanel_DragStart);
            this.rightImagePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightImagePanel_MouseUp);
            // 
            // checkPatternPanel
            // 
            this.checkPatternPanel.Controls.AddRange(
                this.leftImagePanel,
                this.rightImagePanel);
            this.checkPatternPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // toolBar
            // 
            this.toolBar.IsDrawBottomBorderLine = true;
            this.toolBar.Visible = true;
            this.toolBar.DoublePreviewButtonClick += new System.EventHandler(this.ToolBar_DoublePreviewButtonClick);
            this.toolBar.DoubleNextButtonClick += new System.EventHandler(this.ToolBar_DoubleNextButtonClick);
            this.toolBar.SinglePreviewButtonClick += new System.EventHandler(this.ToolBar_SinglePreviewButtonClick);
            this.toolBar.SingleNextButtonClick += new System.EventHandler(this.ToolBar_SingleNextButtonClick);
            this.toolBar.SingleViewMenuItemClick += new System.EventHandler(this.ToolBar_SingleViewMenuItemClick);
            this.toolBar.SpreadLeftFeedMenuItemClick += new System.EventHandler(this.ToolBar_SpreadLeftFeedMenuItemClick);
            this.toolBar.SpreadRightFeedMenuItemClick += new System.EventHandler(this.ToolBar_SpreadRightFeedMenuItemClick);
            this.toolBar.OriginalSizeMenuItemClick += new System.EventHandler(this.ToolBar_OriginalSizeMenuItemClick);
            this.toolBar.FitWindowMenuItemClick += new System.EventHandler(this.ToolBar_FitWindowMenuItemClick);
            this.toolBar.FitWindowLargeOnlyMenuItemClick += new System.EventHandler(this.ToolBar_FitWindowLargeOnlyMenuItemClick);
            this.toolBar.ZoomMenuItemClick += new EventHandler<ZoomMenuItemClickEventArgs>(this.ToolBar_ZoomMenuItemClick);
            this.toolBar.IndexSliderValueChanging += new System.EventHandler(this.ToolBar_IndexSliderValueChanging);
            this.toolBar.IndexSliderValueChanged += new System.EventHandler(this.ToolBar_IndexSliderValueChanged);
            // 
            // ImageViewPage
            //
            this.Width = 800;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.Controls.AddRange(
                this.checkPatternPanel,
                this.toolBar);
            this.HandleCreated += this.ImageViewPage_HandleCreated;
            this.MouseWheel += this.ImageViewPage_MouseWheel;
            this.DrawTabPage += this.ImageViewPage_DrawTabPage;
            this.checkPatternPanel.ResumeLayout(false);
            this.toolBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private PicSum.UIComponent.Contents.ImageView.ImagePanel leftImagePanel;
        private PicSum.UIComponent.Contents.ImageView.ImagePanel rightImagePanel;
        private PicSum.UIComponent.Contents.ContextMenu.FileContextMenu fileContextMenu;
        private SWF.UIComponent.Core.CheckPatternPanel checkPatternPanel;
        private PicSum.UIComponent.Contents.ImageView.ImageViewToolBar toolBar;

        #endregion
    }
}
