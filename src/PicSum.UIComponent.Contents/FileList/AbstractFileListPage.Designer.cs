using PicSum.UIComponent.Contents.ContextMenu;
using System;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    partial class AbstractFileListPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AbstractFileListPage));
            this.flowList = new SWF.UIComponent.FlowList.FlowList();
            this.fileContextMenu = new PicSum.UIComponent.Contents.ContextMenu.FileContextMenu();
            this.viewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showImageFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOtherFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolBar = new PicSum.UIComponent.Contents.ToolBar.PageToolBar();
            this.sortFileNameToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sortFilePathToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sortFileUpdateDateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sortFileRgistrationDateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.thumbnailSizeToolStripSlider = new SWF.UIComponent.Core.ToolStripSlider();
            this.movePreviewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.moveNextToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.viewContextMenuStrip.SuspendLayout();
            this.toolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowList
            // 
            this.flowList.BackColor = System.Drawing.Color.White;
            this.flowList.ContextMenuStrip = this.fileContextMenu;
            this.flowList.IsMultiSelect = true;
            this.flowList.ItemSpace = 4;
            this.flowList.ItemTextAlignment = System.Drawing.StringAlignment.Center;
            this.flowList.ItemTextLineAlignment = System.Drawing.StringAlignment.Center;
            this.flowList.ItemTextTrimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.flowList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowList.Margin = new System.Windows.Forms.Padding(0);
            this.flowList.Name = "flowList";
            this.flowList.TabIndex = 1;
            this.flowList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowList.Text = "flowList1";
            this.flowList.ItemDelete += new System.EventHandler(this.FlowList_ItemDelete);
            this.flowList.DrawItem += new System.EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(this.FlowList_Drawitem);
            this.flowList.DrawItemChanged += new System.EventHandler<SWF.UIComponent.FlowList.DrawItemChangedEventArgs>(this.FlowList_DrawItemChanged);
            this.flowList.DragStart += new System.EventHandler(this.FlowList_DragStart);
            this.flowList.SelectedItemChanged += new System.EventHandler(this.FlowList_SelectedItemChange);
            this.flowList.ItemExecute += new System.EventHandler(this.FlowList_ItemExecute);
            this.flowList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FlowList_MouseClick);
            this.flowList.ItemMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.FlowList_ItemMouseClick);
            this.flowList.ItemMouseDoubleClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.FlowList_ItemMouseDoubleClick);
            this.flowList.BackgroundMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.FlowLilst_BackgroundMouseClick);
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.Name = "fileContextMenu";
            this.fileContextMenu.Size = new System.Drawing.Size(209, 312);
            this.fileContextMenu.DirectoryActiveTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_DirectoryActiveTabOpen);
            this.fileContextMenu.DirectoryNewTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_DirectoryNewTabOpen);
            this.fileContextMenu.FileOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileOpen);
            this.fileContextMenu.SelectApplication += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SelectApplication);
            this.fileContextMenu.DirectoryNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_DirectoryNewWindowOpen);
            this.fileContextMenu.FileNewTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewTabOpen);
            this.fileContextMenu.SaveDirectoryOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SaveDirectoryOpen);
            this.fileContextMenu.FileActiveTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileActiveTabOpen);
            this.fileContextMenu.Bookmark += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_Bookmark);
            this.fileContextMenu.NameCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_NameCopy);
            this.fileContextMenu.Export += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_Export);
            this.fileContextMenu.ExplorerOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_ExplorerOpen);
            this.fileContextMenu.FileNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewWindowOpen);
            this.fileContextMenu.PathCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_PathCopy);
            this.fileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FileContextMenu_Opening);
            this.fileContextMenu.RemoveFromList += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_RemoveFromList);
            this.fileContextMenu.Clip += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_Clip);
            this.fileContextMenu.ConvertToAvif += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToAvif);
            this.fileContextMenu.ConvertToBitmap += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToBitmap);
            this.fileContextMenu.ConvertToHeif += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToHeif);
            this.fileContextMenu.ConvertToIcon += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToIcon);
            this.fileContextMenu.ConvertToJpeg += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToJpeg);
            this.fileContextMenu.ConvertToPng += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToPng);
            this.fileContextMenu.ConvertToSvg += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToSvg);
            this.fileContextMenu.ConvertToWebp += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_ConvertToWebp);
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
            this.showDirectoryToolStripMenuItem.Text = "Folder";
            this.showDirectoryToolStripMenuItem.Click += new System.EventHandler(this.ShowDirectoryToolStripMenuItem_Click);
            // 
            // showImageFileToolStripMenuItem
            // 
            this.showImageFileToolStripMenuItem.Name = "showImageFileToolStripMenuItem";
            this.showImageFileToolStripMenuItem.Text = "Image File";
            this.showImageFileToolStripMenuItem.Click += new System.EventHandler(this.ShowImageFileToolStripMenuItem_Click);
            // 
            // showOtherFileToolStripMenuItem
            // 
            this.showOtherFileToolStripMenuItem.Name = "showOtherFileToolStripMenuItem";
            this.showOtherFileToolStripMenuItem.Text = "Other File";
            this.showOtherFileToolStripMenuItem.Click += new System.EventHandler(this.ShowOtherFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // showFileNameToolStripMenuItem
            // 
            this.showFileNameToolStripMenuItem.Name = "showFileNameToolStripMenuItem";
            this.showFileNameToolStripMenuItem.Text = "File Name";
            this.showFileNameToolStripMenuItem.Click += new System.EventHandler(this.ShowFileNameToolStripMenuItem_Click);
            // 
            // viewToolStripDropDownButton
            // 
            this.viewToolStripDropDownButton.AutoToolTip = false;
            this.viewToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewToolStripDropDownButton.DropDown = this.viewContextMenuStrip;
            this.viewToolStripDropDownButton.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.viewToolStripDropDownButton.Name = "viewToolStripDropDownButton";
            this.viewToolStripDropDownButton.Text = "View";
            // 
            // toolBar
            // 
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripDropDownButton,
            this.sortFileNameToolStripButton,
            this.sortFilePathToolStripButton,
            this.sortFileUpdateDateToolStripButton,
            this.sortFileRgistrationDateToolStripButton,
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
            this.sortFileNameToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.sortFileNameToolStripButton.Name = "sortFileNameToolStripButton";
            this.sortFileNameToolStripButton.Size = new System.Drawing.Size(120, 24);
            this.sortFileNameToolStripButton.Text = "Name";
            this.sortFileNameToolStripButton.Click += new System.EventHandler(this.SortFileNameToolStripButton_Click);
            // 
            // sortFilePathToolStripButton
            // 
            this.sortFilePathToolStripButton.AutoSize = false;
            this.sortFilePathToolStripButton.AutoToolTip = false;
            this.sortFilePathToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortFilePathToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.sortFilePathToolStripButton.Name = "sortFilePathToolStripButton";
            this.sortFilePathToolStripButton.Size = new System.Drawing.Size(120, 24);
            this.sortFilePathToolStripButton.Text = "Path";
            this.sortFilePathToolStripButton.Click += new System.EventHandler(this.SortFilePathToolStripButton_Click);
            // 
            // sortFileUpdateDateToolStripButton
            // 
            this.sortFileUpdateDateToolStripButton.AutoSize = false;
            this.sortFileUpdateDateToolStripButton.AutoToolTip = false;
            this.sortFileUpdateDateToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortFileUpdateDateToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.sortFileUpdateDateToolStripButton.Name = "sortFileUpdateDateToolStripButton";
            this.sortFileUpdateDateToolStripButton.Size = new System.Drawing.Size(120, 24);
            this.sortFileUpdateDateToolStripButton.Text = "Time stamp";
            this.sortFileUpdateDateToolStripButton.Click += new System.EventHandler(this.SortFileUpdateDateToolStripButton_Click);
            // 
            // sortFilerRgistrationDateToolStripButton
            // 
            this.sortFileRgistrationDateToolStripButton.AutoSize = false;
            this.sortFileRgistrationDateToolStripButton.AutoToolTip = false;
            this.sortFileRgistrationDateToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortFileRgistrationDateToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.sortFileRgistrationDateToolStripButton.Name = "sortFilerRgistrationDateToolStripButton";
            this.sortFileRgistrationDateToolStripButton.Size = new System.Drawing.Size(120, 24);
            this.sortFileRgistrationDateToolStripButton.Text = "Registration Date";
            this.sortFileRgistrationDateToolStripButton.Click += new System.EventHandler(this.SortFilerRgistrationDateToolStripButton_Click);
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
            this.thumbnailSizeToolStripSlider.ValueChanging += new System.EventHandler(this.ThumbnailSizeToolStripSlider_ValueChanging);
            this.thumbnailSizeToolStripSlider.BeginValueChange += new System.EventHandler(this.ThumbnailSizeToolStripSlider_BeginValueChange);
            this.thumbnailSizeToolStripSlider.ValueChanged += new System.EventHandler(this.ThumbnailSizeToolStripSlider_ValueChanged);
            // 
            // movePreviewToolStripButton
            //
            this.movePreviewToolStripButton.AutoSize = false;
            this.movePreviewToolStripButton.AutoToolTip = false;
            this.movePreviewToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.movePreviewToolStripButton.Name = "movePreviewToolStripButton";
            this.movePreviewToolStripButton.Size = new System.Drawing.Size(64, 24);
            this.movePreviewToolStripButton.Text = "<-";
            this.movePreviewToolStripButton.Click += new System.EventHandler(this.MovePreviewToolStripButton_Click);
            // 
            // moveNextToolStripButton
            //
            this.moveNextToolStripButton.AutoSize = false;
            this.moveNextToolStripButton.AutoToolTip = false;
            this.moveNextToolStripButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.moveNextToolStripButton.Name = "moveNextToolStripButton";
            this.moveNextToolStripButton.Size = new System.Drawing.Size(64, 24);
            this.moveNextToolStripButton.Text = "->";
            this.moveNextToolStripButton.Click += new System.EventHandler(this.MoveNextToolStripButton_Click);
            // 
            // FileListPageBase
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowList);
            this.Controls.Add(this.toolBar);
            this.Name = "FileListPageBase";
            this.Size = new System.Drawing.Size(630, 393);
            this.viewContextMenuStrip.ResumeLayout(false);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private SWF.UIComponent.FlowList.FlowList flowList;
        protected PicSum.UIComponent.Contents.ContextMenu.FileContextMenu fileContextMenu;
        private PicSum.UIComponent.Contents.ToolBar.PageToolBar toolBar;
        private System.Windows.Forms.ToolStripDropDownButton viewToolStripDropDownButton;
        private System.Windows.Forms.ToolStripButton sortFileNameToolStripButton;
        private System.Windows.Forms.ToolStripButton sortFilePathToolStripButton;
        private System.Windows.Forms.ToolStripButton sortFileUpdateDateToolStripButton;
        protected System.Windows.Forms.ToolStripButton sortFileRgistrationDateToolStripButton;
        private SWF.UIComponent.Core.ToolStripSlider thumbnailSizeToolStripSlider;
        private System.Windows.Forms.ToolStripButton movePreviewToolStripButton;
        private System.Windows.Forms.ToolStripButton moveNextToolStripButton;
        private System.Windows.Forms.ContextMenuStrip viewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showImageFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOtherFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showFileNameToolStripMenuItem;

        #endregion
    }
}
