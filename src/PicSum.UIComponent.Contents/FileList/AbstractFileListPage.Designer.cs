using PicSum.UIComponent.Contents.ContextMenu;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using System;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    partial class AbstractFileListPage
    {
        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
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
            this.flowList.ItemDelete += new System.EventHandler(this.FlowList_ItemDelete);
            this.flowList.DrawItem += new System.EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(this.FlowList_Drawitem);
            this.flowList.DrawItemChanged += new System.EventHandler<SWF.UIComponent.FlowList.DrawItemChangedEventArgs>(this.FlowList_DrawItemChanged);
            this.flowList.DragStart += new System.EventHandler(this.FlowList_DragStart);
            this.flowList.SelectedItemChanged += new System.EventHandler(this.FlowList_SelectedItemChange);
            this.flowList.ItemExecute += new System.EventHandler(this.FlowList_ItemExecute);
            this.flowList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FlowList_MouseClick);
            this.flowList.ItemMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.FlowList_ItemMouseClick);
            this.flowList.ItemMouseDoubleClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.FlowList_ItemMouseDoubleClick);
            // 
            // fileContextMenu
            // 
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
            this.toolBar.ThumbnailSizeSliderMaximumValue = ThumbnailUtil.THUMBNAIL_MAXIMUM_SIZE;
            this.toolBar.ThumbnailSizeSliderMinimumValue = ThumbnailUtil.THUMBNAIL_MINIMUM_SIZE;
            this.toolBar.ThumbnailSizeSliderValue = 96;
            this.toolBar.TakenDateSortButtonEnabled = false;
            this.toolBar.NameSortButtonClick += new System.EventHandler(this.ToolBar_NameSortButtonClick);
            this.toolBar.PathSortButtonClick += new System.EventHandler(this.ToolBar_PathSortButtonClick);
            this.toolBar.CreateDateSortButtonClick += new System.EventHandler(this.ToolBar_CreateDateSortButtonClick);
            this.toolBar.UpdateDateSortButtonClick += new System.EventHandler(this.ToolBar_UpdateDateSortButtonClick);
            this.toolBar.TakenDateSortButtonClick += new System.EventHandler(this.ToolBar_TakenDateSortButtonClick);
            this.toolBar.AddDateSortButtonClick += new System.EventHandler(this.ToolBar_AddDateSortButtonClick);
            this.toolBar.ThumbnailSizeSliderBeginValueChange += new System.EventHandler(this.ToolBar_ThumbnailSizeSliderBeginValueChange);
            this.toolBar.ThumbnailSizeSliderValueChanging += new System.EventHandler(this.ToolBar_ThumbnailSizeSliderValueChanging);
            this.toolBar.ThumbnailSizeSliderValueChanged += new System.EventHandler(this.ToolBar_ThumbnailSizeSliderValueChanged);
            this.toolBar.MovePreviewButtonClick += new System.EventHandler(this.ToolBar_MovePreviewButtonClick);
            this.toolBar.MoveNextButtonClick += new System.EventHandler(this.ToolBar_MoveNextButtonClick);
            this.toolBar.DirectoryMenuItemClick += new System.EventHandler(this.ToolBar_DirectoryMenuItemClick);
            this.toolBar.ImageFileMenuItemClick += new System.EventHandler(this.ToolBar_ImageFileMenuItemClick);
            this.toolBar.OtherFileMenuItemClick += new System.EventHandler(this.ToolBar_OtherFileMenuItemClick);
            this.toolBar.FileNameMenuItemClick += new System.EventHandler(this.ToolBar_FileNameMenuItemClick);
            // 
            // FileListPageBase
            //
            this.Size = new System.Drawing.Size(short.MaxValue, short.MaxValue);
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.Controls.AddRange(
                this.flowList,
                this.toolBar);
            this.Loaded += this.AbstractFileListPage_Loaded;
            this.toolBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.FlowList.FlowList flowList;
        protected PicSum.UIComponent.Contents.ContextMenu.FileContextMenu fileContextMenu;
        protected PicSum.UIComponent.Contents.FileList.FileListToolBar toolBar;

        #endregion
    }
}
