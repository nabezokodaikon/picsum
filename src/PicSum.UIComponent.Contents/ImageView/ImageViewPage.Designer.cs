using PicSum.UIComponent.Contents.ContextMenu;
using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Base;
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
            this._leftImagePanel = new PicSum.UIComponent.Contents.ImageView.SKImagePanel();
            this._fileContextMenu = new PicSum.UIComponent.Contents.ContextMenu.FileContextMenu();
            this._rightImagePanel = new PicSum.UIComponent.Contents.ImageView.SKImagePanel();
            this._checkPatternPanel = new SWF.UIComponent.Base.CheckPatternPanel();
            this._toolBar = new PicSum.UIComponent.Contents.ImageView.ImageViewToolBar();
            this._checkPatternPanel.SuspendLayout();
            this._toolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // leftImagePanel
            // 
            this._leftImagePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._leftImagePanel.ContextMenuStrip = this._fileContextMenu;
            this._leftImagePanel.Align = SWF.Core.Base.ImageAlign.Center;
            this._leftImagePanel.IsShowThumbnailPanel = true;
            this._leftImagePanel.Hide();
            this._leftImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.LeftImagePanel_ImageMouseClick);
            this._leftImagePanel.DragStart += new System.EventHandler(this.LeftImagePanel_DragStart);
            this._leftImagePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LeftImagePanel_MouseUp);
            // 
            // fileContextMenu
            // 
            this._fileContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._fileContextMenu.VisibleRemoveFromListMenuItem = false;
            this._fileContextMenu.FileNewTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewTabOpen);
            this._fileContextMenu.FileNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewWindowOpen);
            this._fileContextMenu.SelectApplication += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SelectApplication);
            this._fileContextMenu.SaveDirectoryOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SaveDirectoryOpen);
            this._fileContextMenu.PathCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_PathCopy);
            this._fileContextMenu.NameCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_NameCopy);
            this._fileContextMenu.Bookmark += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_Bookmark);
            this._fileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FileContextMenu_Opening);
            // 
            // rightImagePanel
            // 
            this._rightImagePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._rightImagePanel.ContextMenuStrip = this._fileContextMenu;
            this._rightImagePanel.Align = SWF.Core.Base.ImageAlign.Center;
            this._rightImagePanel.IsShowThumbnailPanel = true;
            this._rightImagePanel.Hide();
            this._rightImagePanel.ImageMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.RightImagePanel_ImageMouseClick);
            this._rightImagePanel.DragStart += new System.EventHandler(this.RightImagePanel_DragStart);
            this._rightImagePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RightImagePanel_MouseUp);
            // 
            // checkPatternPanel
            // 
            this._checkPatternPanel.Controls.AddRange(
                this._leftImagePanel,
                this._rightImagePanel);
            // 
            // toolBar
            // 
            this._toolBar.IsDrawBottomBorderLine = true;
            this._toolBar.Visible = true;
            this._toolBar.DoublePreviewButtonClick += new System.EventHandler(this.ToolBar_DoublePreviewButtonClick);
            this._toolBar.DoubleNextButtonClick += new System.EventHandler(this.ToolBar_DoubleNextButtonClick);
            this._toolBar.SinglePreviewButtonClick += new System.EventHandler(this.ToolBar_SinglePreviewButtonClick);
            this._toolBar.SingleNextButtonClick += new System.EventHandler(this.ToolBar_SingleNextButtonClick);
            this._toolBar.SingleViewMenuItemClick += new System.EventHandler(this.ToolBar_SingleViewMenuItemClick);
            this._toolBar.SpreadLeftFeedMenuItemClick += new System.EventHandler(this.ToolBar_SpreadLeftFeedMenuItemClick);
            this._toolBar.SpreadRightFeedMenuItemClick += new System.EventHandler(this.ToolBar_SpreadRightFeedMenuItemClick);
            this._toolBar.OriginalSizeMenuItemClick += new System.EventHandler(this.ToolBar_OriginalSizeMenuItemClick);
            this._toolBar.FitWindowMenuItemClick += new System.EventHandler(this.ToolBar_FitWindowMenuItemClick);
            this._toolBar.FitWindowLargeOnlyMenuItemClick += new System.EventHandler(this.ToolBar_FitWindowLargeOnlyMenuItemClick);
            this._toolBar.ZoomMenuItemClick += new EventHandler<ZoomMenuItemClickEventArgs>(this.ToolBar_ZoomMenuItemClick);
            this._toolBar.IndexSliderValueChanging += new System.EventHandler(this.ToolBar_IndexSliderValueChanging);
            this._toolBar.IndexSliderValueChanged += new System.EventHandler(this.ToolBar_IndexSliderValueChanged);
            // 
            // ImageViewPage
            //
            this.Size = new System.Drawing.Size(short.MaxValue, short.MaxValue);
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.Controls.AddRange(
                this._checkPatternPanel,
                this._toolBar);
            this.Loaded += this.ImageViewPage_Loaded;
            this.MouseWheel += this.ImageViewPage_MouseWheel;
            this.DrawTabPage += this.ImageViewPage_DrawTabPage;
            this._checkPatternPanel.ResumeLayout(false);
            this._toolBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private PicSum.UIComponent.Contents.ImageView.SKImagePanel _leftImagePanel;
        private PicSum.UIComponent.Contents.ImageView.SKImagePanel _rightImagePanel;
        private PicSum.UIComponent.Contents.ContextMenu.FileContextMenu _fileContextMenu;
        private SWF.UIComponent.Base.CheckPatternPanel _checkPatternPanel;
        private PicSum.UIComponent.Contents.ImageView.ImageViewToolBar _toolBar;

        #endregion
    }
}
