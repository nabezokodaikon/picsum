using PicSum.UIComponent.Contents.ContextMenu;
using SWF.Core.Base;
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
            this.toolBar = new PicSum.UIComponent.Contents.FileList.FileListToolBar();
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
            this.flowList.Margin = new System.Windows.Forms.Padding(0);
            this.flowList.Name = "flowList";
            this.flowList.TabIndex = 1;
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
            this.fileContextMenu.SelectApplication += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SelectApplication);
            this.fileContextMenu.DirectoryNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_DirectoryNewWindowOpen);
            this.fileContextMenu.FileNewTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewTabOpen);
            this.fileContextMenu.SaveDirectoryOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SaveDirectoryOpen);
            this.fileContextMenu.FileActiveTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileActiveTabOpen);
            this.fileContextMenu.Bookmark += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_Bookmark);
            this.fileContextMenu.NameCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_NameCopy);
            this.fileContextMenu.ExplorerOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_ExplorerOpen);
            this.fileContextMenu.FileNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewWindowOpen);
            this.fileContextMenu.PathCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_PathCopy);
            this.fileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FileContextMenu_Opening);
            this.fileContextMenu.RemoveFromList += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_RemoveFromList);
            // 
            // toolBar
            // 
            this.toolBar.TabStop = false;
            this.toolBar.ThumbnailSizeSliderMaximumValue = AppConstants.THUMBNAIL_MAXIMUM_SIZE;
            this.toolBar.ThumbnailSizeSliderMinimumValue = AppConstants.THUMBNAIL_MINIMUM_SIZE;
            this.toolBar.ThumbnailSizeSliderValue = 96;
            this.toolBar.NameSortButtonClick += new System.EventHandler(this.SortFileNameToolStripButton_Click);
            this.toolBar.PathSortButtonClick += new System.EventHandler(this.SortFilePathToolStripButton_Click);
            this.toolBar.TimestampSortButtonClick += new System.EventHandler(this.SortFileUpdateDateToolStripButton_Click);
            this.toolBar.RegistrationSortButtonClick += new System.EventHandler(this.SortFilerRgistrationDateToolStripButton_Click);
            this.toolBar.ThumbnailSizeSliderBeginValueChange += new System.EventHandler(this.ThumbnailSizeToolStripSlider_BeginValueChange);
            this.toolBar.ThumbnailSizeSliderValueChanging += new System.EventHandler(this.ThumbnailSizeToolStripSlider_ValueChanging);
            this.toolBar.ThumbnailSizeSliderValueChanged += new System.EventHandler(this.ThumbnailSizeToolStripSlider_ValueChanged);
            this.toolBar.MovePreviewButtonClick += new System.EventHandler(this.MovePreviewToolStripButton_Click);
            this.toolBar.MoveNextButtonClick += new System.EventHandler(this.MoveNextToolStripButton_Click);
            this.toolBar.FolderMenuItemClick += new System.EventHandler(this.ShowDirectoryToolStripMenuItem_Click);
            this.toolBar.ImageFileMenuItemClick += new System.EventHandler(this.ShowImageFileToolStripMenuItem_Click);
            this.toolBar.OtherFileMenuItemClick += new System.EventHandler(this.ShowOtherFileToolStripMenuItem_Click);
            this.toolBar.FileNameMenuItemClick += new System.EventHandler(this.ShowFileNameToolStripMenuItem_Click);
            // 
            // FileListPageBase
            //
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.Controls.Add(this.flowList);
            this.Controls.Add(this.toolBar);
            this.Name = "FileListPageBase";
            this.Size = new System.Drawing.Size(630, 393);
            this.toolBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.FlowList.FlowList flowList;
        protected PicSum.UIComponent.Contents.ContextMenu.FileContextMenu fileContextMenu;
        protected PicSum.UIComponent.Contents.FileList.FileListToolBar toolBar;

        #endregion
    }
}
