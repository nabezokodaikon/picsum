using PicSum.UIComponent.Contents.ContextMenu;
using SkiaSharp.Views.Desktop;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.UIComponent.SKFlowList;
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
            this._flowList = new SKFlowList();
            this._fileContextMenu = new PicSum.UIComponent.Contents.ContextMenu.FileContextMenu();
            this._toolBar = new PicSum.UIComponent.Contents.FileList.FileListToolBar();
            this._toolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowList
            // 
            this._flowList.BackgroundColor = SKFlowListResources.DARK_BACKGROUND_PAINT.Color;
            this._flowList.ContextMenuStrip = this._fileContextMenu;
            this._flowList.IsMultiSelect = true;
            this._flowList.ItemSpace = 4;
            this._flowList.MouseWheelRate = 0.4f;
            this._flowList.ItemDelete += new System.EventHandler(this.FlowList_ItemDelete);
            this._flowList.SKDrawItems += new System.EventHandler<SWF.UIComponent.SKFlowList.SKDrawItemsEventArgs>(this.FlowList_Drawitems);
            this._flowList.DrawItemChanged += new System.EventHandler<SWF.UIComponent.SKFlowList.SKDrawItemChangedEventArgs>(this.FlowList_DrawItemChanged);
            this._flowList.DragStart += new System.EventHandler(this.FlowList_DragStart);
            this._flowList.SelectedItemChanged += new System.EventHandler(this.FlowList_SelectedItemChange);
            this._flowList.ItemExecute += new System.EventHandler(this.FlowList_ItemExecute);
            this._flowList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FlowList_MouseClick);
            this._flowList.ItemMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.FlowList_ItemMouseClick);
            this._flowList.ItemMouseDoubleClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.FlowList_ItemMouseDoubleClick);
            // 
            // fileContextMenu
            // 
            this._fileContextMenu.DirectoryActiveTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_DirectoryActiveTabOpen);
            this._fileContextMenu.DirectoryNewTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_DirectoryNewTabOpen);
            this._fileContextMenu.SelectApplication += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SelectApplication);
            this._fileContextMenu.DirectoryNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_DirectoryNewWindowOpen);
            this._fileContextMenu.FileNewTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewTabOpen);
            this._fileContextMenu.SaveDirectoryOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_SaveDirectoryOpen);
            this._fileContextMenu.FileActiveTabOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileActiveTabOpen);
            this._fileContextMenu.Bookmark += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_Bookmark);
            this._fileContextMenu.NameCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_NameCopy);
            this._fileContextMenu.ExplorerOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_ExplorerOpen);
            this._fileContextMenu.FileNewWindowOpen += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileEventArgs>(this.FileContextMenu_FileNewWindowOpen);
            this._fileContextMenu.PathCopy += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_PathCopy);
            this._fileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FileContextMenu_Opening);
            this._fileContextMenu.RemoveFromList += new System.EventHandler<PicSum.UIComponent.Contents.ContextMenu.ExecuteFileListEventArgs>(this.FileContextMenu_RemoveFromList);
            // 
            // toolBar
            // 
            this._toolBar.ThumbnailSizeSliderMaximumValue = ThumbnailUtil.THUMBNAIL_MAXIMUM_SIZE;
            this._toolBar.ThumbnailSizeSliderMinimumValue = ThumbnailUtil.THUMBNAIL_MINIMUM_SIZE;
            this._toolBar.ThumbnailSizeSliderValue = 200;
            this._toolBar.TakenDateSortButtonEnabled = false;
            this._toolBar.NameSortButtonClick += new System.EventHandler(this.ToolBar_NameSortButtonClick);
            this._toolBar.PathSortButtonClick += new System.EventHandler(this.ToolBar_PathSortButtonClick);
            this._toolBar.CreateDateSortButtonClick += new System.EventHandler(this.ToolBar_CreateDateSortButtonClick);
            this._toolBar.UpdateDateSortButtonClick += new System.EventHandler(this.ToolBar_UpdateDateSortButtonClick);
            this._toolBar.TakenDateSortButtonClick += new System.EventHandler(this.ToolBar_TakenDateSortButtonClick);
            this._toolBar.AddDateSortButtonClick += new System.EventHandler(this.ToolBar_AddDateSortButtonClick);
            this._toolBar.ThumbnailSizeSliderBeginValueChange += new System.EventHandler(this.ToolBar_ThumbnailSizeSliderBeginValueChange);
            this._toolBar.ThumbnailSizeSliderValueChanging += new System.EventHandler(this.ToolBar_ThumbnailSizeSliderValueChanging);
            this._toolBar.ThumbnailSizeSliderValueChanged += new System.EventHandler(this.ToolBar_ThumbnailSizeSliderValueChanged);
            this._toolBar.MovePreviewButtonClick += new System.EventHandler(this.ToolBar_MovePreviewButtonClick);
            this._toolBar.MoveNextButtonClick += new System.EventHandler(this.ToolBar_MoveNextButtonClick);
            this._toolBar.DirectoryMenuItemClick += new System.EventHandler(this.ToolBar_DirectoryMenuItemClick);
            this._toolBar.ImageFileMenuItemClick += new System.EventHandler(this.ToolBar_ImageFileMenuItemClick);
            this._toolBar.OtherFileMenuItemClick += new System.EventHandler(this.ToolBar_OtherFileMenuItemClick);
            this._toolBar.FileNameMenuItemClick += new System.EventHandler(this.ToolBar_FileNameMenuItemClick);
            // 
            // FileListPageBase
            //
            this.Size = new System.Drawing.Size(short.MaxValue, short.MaxValue);
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.Controls.AddRange(
                this._flowList,
                this._toolBar);
            this.Loaded += this.AbstractFileListPage_Loaded;
            this._toolBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.SKFlowList.SKFlowList _flowList;
        protected PicSum.UIComponent.Contents.ContextMenu.FileContextMenu _fileContextMenu;
        protected PicSum.UIComponent.Contents.FileList.FileListToolBar _toolBar;

        #endregion
    }
}
