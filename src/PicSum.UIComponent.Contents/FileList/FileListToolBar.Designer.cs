using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Base;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    partial class FileListToolBar
    {
        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this._viewButton = new SWF.UIComponent.Base.BaseTextButton();
            this._nameSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this._pathSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this._createDateSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this._updateDateSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this._takenDateSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this._addDateSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this._thumbnailSizeSlider = new SWF.UIComponent.Base.Slider();
            this._viewMenu = new System.Windows.Forms.ContextMenuStrip();
            this._directoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._imageFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._otherFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._fileNameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._movePreviewButton = new SWF.UIComponent.Base.BaseTextButton();
            this._moveNextButton = new SWF.UIComponent.Base.BaseTextButton();
            this._viewMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewButton
            //
            this._viewButton.Text = "View";
            this._viewButton.MouseClick += this.ViewButton_MouseClick;
            // 
            // nameSortButton
            //
            this._nameSortButton.Text = "Name";
            this._nameSortButton.MouseClick += this.NameSortButton_MouseClick;
            // 
            // pathSortButton
            //
            this._pathSortButton.Text = "Path";
            this._pathSortButton.MouseClick += this.PathSortButton_MouseClick;
            // 
            // createDateSortButton
            //
            this._createDateSortButton.Text = "Created";
            this._createDateSortButton.MouseClick += this.CreateDateSortButton_MouseClick;
            // 
            // timestampSortButton
            //
            this._updateDateSortButton.Text = "Updated";
            this._updateDateSortButton.MouseClick += this.UpdateDateSortButton_MouseClick;
            // 
            // timestampSortButton
            //
            this._takenDateSortButton.Text = "Taken";
            this._takenDateSortButton.MouseClick += this.TakenDateSortButton_MouseClick;
            // 
            // addDateSortButton
            //
            this._addDateSortButton.Text = "Added";
            this._addDateSortButton.MouseClick += this.AddDateSortButton_MouseClick;
            // 
            // thumbnailSizeSlider
            // 
            this._thumbnailSizeSlider.BeginValueChange += this.ThumbnailSizeSlider_BeginValueChange;
            this._thumbnailSizeSlider.ValueChanging += this.ThumbnailSizeSlider_ValueChanging;
            this._thumbnailSizeSlider.ValueChanged += this.ThumbnailSizeSlider_ValueChanged;
            // 
            // viewMenu
            //
            this._viewMenu.Font = FontCacher.GetRegularGdiFont(FontCacher.Size.Medium);
            this._viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._directoryMenuItem, this._imageFileMenuItem, this._otherFileMenuItem, this._toolStripSeparator1, this._fileNameMenuItem });
            // 
            // directoryMenuItem
            // 
            this._directoryMenuItem.Text = "Folder";
            this._directoryMenuItem.Click += this.DirectoryMenuItem_Click;
            // 
            // imageFileMenuItem
            // 
            this._imageFileMenuItem.Text = "Image File";
            this._imageFileMenuItem.Click += this.ImageFileMenuItem_Click;
            // 
            // otherFileMenuItem
            // 
            this._otherFileMenuItem.Text = "Other File";
            this._otherFileMenuItem.Click += this.OtherFileMenuItem_Click;
            // 
            // fileNameMenuItem
            // 
            this._fileNameMenuItem.Text = "FileName";
            this._fileNameMenuItem.Click += this.FileNameMenuItem_Click;
            // 
            // movePreviewButton
            //
            this._movePreviewButton.Text = "<-";
            this._movePreviewButton.MouseClick += this.MovePreviewButton_MouseClick;
            // 
            // moveNextButton
            //
            this._moveNextButton.Text = "->";
            this._moveNextButton.MouseClick += this.MoveNextButton_MouseClick;
            // 
            // FileListToolBar
            //
            this.Controls.AddRange(
                this._moveNextButton,
                this._movePreviewButton,
                this._thumbnailSizeSlider,
                this._addDateSortButton,
                this._createDateSortButton,
                this._updateDateSortButton,
                this._takenDateSortButton,
                this._pathSortButton,
                this._nameSortButton,
                this._viewButton);
            this.IsDrawBottomBorderLine = true;
            this._viewMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private SWF.UIComponent.Base.BaseTextButton _viewButton;
        private SWF.UIComponent.Base.Slider _thumbnailSizeSlider;
        private System.Windows.Forms.ToolStripDropDown _viewMenu;
        private System.Windows.Forms.ToolStripMenuItem _directoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _imageFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _otherFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator _toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _fileNameMenuItem;
        private SWF.UIComponent.Base.BaseTextButton _movePreviewButton;
        private SWF.UIComponent.Base.BaseTextButton _moveNextButton;
        private SWF.UIComponent.Base.BaseTextButton _nameSortButton;
        private SWF.UIComponent.Base.BaseTextButton _pathSortButton;
        private SWF.UIComponent.Base.BaseTextButton _createDateSortButton;
        private SWF.UIComponent.Base.BaseTextButton _updateDateSortButton;
        private SWF.UIComponent.Base.BaseTextButton _takenDateSortButton;
        private SWF.UIComponent.Base.BaseTextButton _addDateSortButton;
    }
}
