namespace PicSum.UIComponent.Contents.FileListContents
{
    partial class FileListContentsBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileListContentsBase));
            this.flowList = new SWF.UIComponent.FlowList.FlowList();
            this.fileContextMenu = new PicSum.UIComponent.Common.FileContextMenu.FileContextMenu();
            this.viewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showImageFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOtherFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolBar = new PicSum.UIComponent.Contents.ContentsToolBar.ContentsToolBar();
            this.sortFileNameToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sortFilePathToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sortFileUpdateDateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sortFileCreateDateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.thumbnailSizeToolStripSlider = new SWF.UIComponent.Common.ToolStripSlider();
            this.movePreviewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.moveNextToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.viewContextMenuStrip.SuspendLayout();
            this.toolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowList
            // 
            this.flowList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowList.BackColor = System.Drawing.Color.White;
            this.flowList.ContextMenuStrip = this.fileContextMenu;
            this.flowList.FocusItemColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.flowList.IsMultiSelect = true;
            this.flowList.ItemSpace = 4;
            this.flowList.ItemTextAlignment = System.Drawing.StringAlignment.Center;
            this.flowList.ItemTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.flowList.ItemTextLineAlignment = System.Drawing.StringAlignment.Center;
            this.flowList.ItemTextTrimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.flowList.Location = new System.Drawing.Point(0, 27);
            this.flowList.Margin = new System.Windows.Forms.Padding(0);
            this.flowList.MousePointItemColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.flowList.Name = "flowList";
            this.flowList.RectangleSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.flowList.SelectedItemColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(48)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.flowList.Size = new System.Drawing.Size(630, 366);
            this.flowList.TabIndex = 1;
            this.flowList.Text = "flowList1";
            this.flowList.ItemDelete += new System.EventHandler(this.flowList_ItemDelete);
            this.flowList.DrawItem += new System.EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(this.flowList_Drawitem);
            this.flowList.DrawItemChanged += new System.EventHandler<SWF.UIComponent.FlowList.DrawItemChangedEventArgs>(this.flowList_DrawItemChanged);
            this.flowList.DragStart += new System.EventHandler(this.flowList_DragStart);
            this.flowList.SelectedItemChanged += new System.EventHandler(this.flowList_SelectedItemChange);
            this.flowList.ItemExecute += new System.EventHandler(this.flowList_ItemExecute);
            this.flowList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.flowList_MouseDown);
            this.flowList.ItemMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.flowList_ItemMouseClick);
            this.flowList.ItemMouseDoubleClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.flowList_ItemMouseDoubleClick);
            this.flowList.BackgroundMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.flowLilst_BackgroundMouseClick);
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.IsAddKeepMenuItemVisible = false;
            this.fileContextMenu.IsFileActiveTabOpenMenuItemVisible = true;
            this.fileContextMenu.IsDirectoryActiveTabOpenMenuItemVisible = true;
            this.fileContextMenu.IsRemoveFromListMenuItemVisible = false;
            this.fileContextMenu.Name = "fileContextMenu";
            this.fileContextMenu.Size = new System.Drawing.Size(209, 312);
            this.fileContextMenu.DirectoryActiveTabOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_DirectoryActiveTabOpen);
            this.fileContextMenu.DirectoryNewTabOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_DirectoryNewTabOpen);
            this.fileContextMenu.FileOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileOpen);
            this.fileContextMenu.DirectoryNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_DirectoryNewWindowOpen);
            this.fileContextMenu.FileNewTabOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileNewTabOpen);
            this.fileContextMenu.SaveDirectoryOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_SaveDirectoryOpen);
            this.fileContextMenu.FileActiveTabOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileActiveTabOpen);
            this.fileContextMenu.AddKeep += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_AddKeep);
            this.fileContextMenu.NameCopy += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_NameCopy);
            this.fileContextMenu.Export += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_Export);
            this.fileContextMenu.ExplorerOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_ExplorerOpen);
            this.fileContextMenu.FileNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileEventArgs>(this.fileContextMenu_FileNewWindowOpen);
            this.fileContextMenu.PathCopy += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_PathCopy);
            this.fileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FileContextMenu_Opening);
            this.fileContextMenu.RemoveFromList += new System.EventHandler<PicSum.UIComponent.Common.FileContextMenu.ExecuteFileListEventArgs>(this.fileContextMenu_RemoveFromList);
            // 
            // viewContextMenuStrip
            // 
            this.viewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showDirectoryToolStripMenuItem,
            this.showImageFileToolStripMenuItem,
            this.showOtherFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.showFileNameToolStripMenuItem});
            this.viewContextMenuStrip.Name = "viewContextMenuStrip";
            this.viewContextMenuStrip.OwnerItem = this.viewToolStripDropDownButton;
            this.viewContextMenuStrip.Size = new System.Drawing.Size(173, 98);
            // 
            // showDirectoryToolStripMenuItem
            // 
            this.showDirectoryToolStripMenuItem.Name = "showDirectoryToolStripMenuItem";
            this.showDirectoryToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.showDirectoryToolStripMenuItem.Text = "Folder";
            this.showDirectoryToolStripMenuItem.Click += new System.EventHandler(this.showDirectoryToolStripMenuItem_Click);
            // 
            // showImageFileToolStripMenuItem
            // 
            this.showImageFileToolStripMenuItem.Name = "showImageFileToolStripMenuItem";
            this.showImageFileToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.showImageFileToolStripMenuItem.Text = "Image File";
            this.showImageFileToolStripMenuItem.Click += new System.EventHandler(this.showImageFileToolStripMenuItem_Click);
            // 
            // showOtherFileToolStripMenuItem
            // 
            this.showOtherFileToolStripMenuItem.Name = "showOtherFileToolStripMenuItem";
            this.showOtherFileToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.showOtherFileToolStripMenuItem.Text = "Other File";
            this.showOtherFileToolStripMenuItem.Click += new System.EventHandler(this.showOtherFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(169, 6);
            // 
            // showFileNameToolStripMenuItem
            // 
            this.showFileNameToolStripMenuItem.Name = "showFileNameToolStripMenuItem";
            this.showFileNameToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.showFileNameToolStripMenuItem.Text = "File Name";
            this.showFileNameToolStripMenuItem.Click += new System.EventHandler(this.showFileNameToolStripMenuItem_Click);
            // 
            // viewToolStripDropDownButton
            // 
            this.viewToolStripDropDownButton.AutoToolTip = false;
            this.viewToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewToolStripDropDownButton.DropDown = this.viewContextMenuStrip;
            this.viewToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("viewToolStripDropDownButton.Image")));
            this.viewToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewToolStripDropDownButton.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.viewToolStripDropDownButton.Name = "viewToolStripDropDownButton";
            this.viewToolStripDropDownButton.Size = new System.Drawing.Size(45, 24);
            this.viewToolStripDropDownButton.Text = "Display";
            // 
            // toolBar
            // 
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripDropDownButton,
            this.sortFileNameToolStripButton,
            this.sortFilePathToolStripButton,
            this.sortFileCreateDateToolStripButton,
            this.sortFileUpdateDateToolStripButton,
            this.thumbnailSizeToolStripSlider,
            this.movePreviewToolStripButton,
            this.moveNextToolStripButton});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(630, 27);
            this.toolBar.TabIndex = 2;
            this.toolBar.Text = "toolStrip1";
            // 
            // sortFileNameToolStripButton
            // 
            this.sortFileNameToolStripButton.AutoSize = false;
            this.sortFileNameToolStripButton.AutoToolTip = false;
            this.sortFileNameToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortFileNameToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.sortFileNameToolStripButton.Name = "sortFileNameToolStripButton";
            this.sortFileNameToolStripButton.Size = new System.Drawing.Size(120, 24);
            this.sortFileNameToolStripButton.Text = "Name";
            this.sortFileNameToolStripButton.Click += new System.EventHandler(this.sortFileNameToolStripButton_Click);
            // 
            // sortFilePathToolStripButton
            // 
            this.sortFilePathToolStripButton.AutoSize = false;
            this.sortFilePathToolStripButton.AutoToolTip = false;
            this.sortFilePathToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortFilePathToolStripButton.Name = "sortFilePathToolStripButton";
            this.sortFilePathToolStripButton.Size = new System.Drawing.Size(120, 24);
            this.sortFilePathToolStripButton.Text = "Path";
            this.sortFilePathToolStripButton.Click += new System.EventHandler(this.sortFilePathToolStripButton_Click);
            // 
            // sortFileCreateDateToolStripButton
            // 
            this.sortFileCreateDateToolStripButton.AutoSize = false;
            this.sortFileCreateDateToolStripButton.AutoToolTip = false;
            this.sortFileCreateDateToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortFileCreateDateToolStripButton.Name = "sortFileCreateDateToolStripButton";
            this.sortFileCreateDateToolStripButton.Size = new System.Drawing.Size(120, 24);
            this.sortFileCreateDateToolStripButton.Text = "Creation Date";
            this.sortFileCreateDateToolStripButton.Click += new System.EventHandler(this.sortFileCreateDateToolStripButton_Click);
            // 
            // sortFileUpdateDateToolStripButton
            // 
            this.sortFileUpdateDateToolStripButton.AutoSize = false;
            this.sortFileUpdateDateToolStripButton.AutoToolTip = false;
            this.sortFileUpdateDateToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortFileUpdateDateToolStripButton.Name = "sortFileUpdateDateToolStripButton";
            this.sortFileUpdateDateToolStripButton.Size = new System.Drawing.Size(120, 24);
            this.sortFileUpdateDateToolStripButton.Text = "Update Date";
            this.sortFileUpdateDateToolStripButton.Click += new System.EventHandler(this.sortFileUpdateDateToolStripButton_Click);
            // 
            // thumbnailSizeToolStripSlider
            // 
            this.thumbnailSizeToolStripSlider.BackColor = System.Drawing.Color.Transparent;
            this.thumbnailSizeToolStripSlider.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.thumbnailSizeToolStripSlider.MaximumValue = 256;
            this.thumbnailSizeToolStripSlider.MinimumValue = 96;
            this.thumbnailSizeToolStripSlider.Name = "thumbnailSizeToolStripSlider";
            this.thumbnailSizeToolStripSlider.Size = new System.Drawing.Size(96, 24);
            this.thumbnailSizeToolStripSlider.Text = "toolStripSlider1";
            this.thumbnailSizeToolStripSlider.Value = 96;
            this.thumbnailSizeToolStripSlider.ValueChanging += new System.EventHandler(this.thumbnailSizeToolStripSlider_ValueChanging);
            this.thumbnailSizeToolStripSlider.BeginValueChange += new System.EventHandler(this.thumbnailSizeToolStripSlider_BeginValueChange);
            this.thumbnailSizeToolStripSlider.ValueChanged += new System.EventHandler(this.thumbnailSizeToolStripSlider_ValueChanged);
            // 
            // movePreviewToolStripButton
            // 
            this.movePreviewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.movePreviewToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("movePreviewToolStripButton.Image")));
            this.movePreviewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.movePreviewToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.movePreviewToolStripButton.Name = "movePreviewToolStripButton";
            this.movePreviewToolStripButton.Size = new System.Drawing.Size(23, 24);
            this.movePreviewToolStripButton.Click += new System.EventHandler(this.movePreviewToolStripButton_Click);
            // 
            // moveNextToolStripButton
            // 
            this.moveNextToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveNextToolStripButton.Image = global::PicSum.UIComponent.Contents.Properties.Resources.MiddleArrowRight;
            this.moveNextToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveNextToolStripButton.Name = "moveNextToolStripButton";
            this.moveNextToolStripButton.Size = new System.Drawing.Size(23, 24);
            this.moveNextToolStripButton.Click += new System.EventHandler(this.moveNextToolStripButton_Click);
            // 
            // FileListContentsBase
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowList);
            this.Controls.Add(this.toolBar);
            this.Name = "FileListContentsBase";
            this.Size = new System.Drawing.Size(630, 393);
            this.viewContextMenuStrip.ResumeLayout(false);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SWF.UIComponent.FlowList.FlowList flowList;
        private PicSum.UIComponent.Common.FileContextMenu.FileContextMenu fileContextMenu;
        private PicSum.UIComponent.Contents.ContentsToolBar.ContentsToolBar toolBar;
        private System.Windows.Forms.ToolStripDropDownButton viewToolStripDropDownButton;
        private System.Windows.Forms.ToolStripButton sortFileNameToolStripButton;
        private System.Windows.Forms.ToolStripButton sortFilePathToolStripButton;
        private System.Windows.Forms.ToolStripButton sortFileUpdateDateToolStripButton;
        private System.Windows.Forms.ToolStripButton sortFileCreateDateToolStripButton;
        private SWF.UIComponent.Common.ToolStripSlider thumbnailSizeToolStripSlider;
        private System.Windows.Forms.ToolStripButton movePreviewToolStripButton;
        private System.Windows.Forms.ToolStripButton moveNextToolStripButton;
        private System.Windows.Forms.ContextMenuStrip viewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showImageFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOtherFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showFileNameToolStripMenuItem;
    }
}
