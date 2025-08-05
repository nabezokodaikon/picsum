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
            this.viewButton = new SWF.UIComponent.Base.BaseTextButton();
            this.nameSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this.pathSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this.createDateSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this.updateDateSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this.takenDateSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this.addDateSortButton = new SWF.UIComponent.Base.BaseTextButton();
            this.thumbnailSizeSlider = new SWF.UIComponent.Base.Slider();
            this.viewMenu = new System.Windows.Forms.ContextMenuStrip();
            this.directoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otherFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileNameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movePreviewButton = new SWF.UIComponent.Base.BaseTextButton();
            this.moveNextButton = new SWF.UIComponent.Base.BaseTextButton();
            this.viewMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewButton
            //
            this.viewButton.Text = "View";
            this.viewButton.MouseClick += this.ViewButton_MouseClick;
            // 
            // nameSortButton
            //
            this.nameSortButton.Text = "Name";
            this.nameSortButton.MouseClick += this.NameSortButton_MouseClick;
            // 
            // pathSortButton
            //
            this.pathSortButton.Text = "Path";
            this.pathSortButton.MouseClick += this.PathSortButton_MouseClick;
            // 
            // createDateSortButton
            //
            this.createDateSortButton.Text = "Created";
            this.createDateSortButton.MouseClick += this.CreateDateSortButton_MouseClick;
            // 
            // timestampSortButton
            //
            this.updateDateSortButton.Text = "Updated";
            this.updateDateSortButton.MouseClick += this.UpdateDateSortButton_MouseClick;
            // 
            // timestampSortButton
            //
            this.takenDateSortButton.Text = "Taken";
            this.takenDateSortButton.MouseClick += this.TakenDateSortButton_MouseClick;
            // 
            // addDateSortButton
            //
            this.addDateSortButton.Text = "Added";
            this.addDateSortButton.MouseClick += this.AddDateSortButton_MouseClick;
            // 
            // thumbnailSizeSlider
            // 
            this.thumbnailSizeSlider.BeginValueChange += this.ThumbnailSizeSlider_BeginValueChange;
            this.thumbnailSizeSlider.ValueChanging += this.ThumbnailSizeSlider_ValueChanging;
            this.thumbnailSizeSlider.ValueChanged += this.ThumbnailSizeSlider_ValueChanged;
            // 
            // viewMenu
            //
            this.viewMenu.Font = Fonts.GetRegularFont(Fonts.Size.Medium);
            this.viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.directoryMenuItem, this.imageFileMenuItem, this.otherFileMenuItem, this.toolStripSeparator1, this.fileNameMenuItem });
            // 
            // directoryMenuItem
            // 
            this.directoryMenuItem.Text = "Folder";
            this.directoryMenuItem.Click += this.DirectoryMenuItem_Click;
            // 
            // imageFileMenuItem
            // 
            this.imageFileMenuItem.Text = "Image File";
            this.imageFileMenuItem.Click += this.ImageFileMenuItem_Click;
            // 
            // otherFileMenuItem
            // 
            this.otherFileMenuItem.Text = "Other File";
            this.otherFileMenuItem.Click += this.OtherFileMenuItem_Click;
            // 
            // fileNameMenuItem
            // 
            this.fileNameMenuItem.Text = "FileName";
            this.fileNameMenuItem.Click += this.FileNameMenuItem_Click;
            // 
            // movePreviewButton
            //
            this.movePreviewButton.Text = "<-";
            this.movePreviewButton.MouseClick += this.MovePreviewButton_MouseClick;
            // 
            // moveNextButton
            //
            this.moveNextButton.Text = "->";
            this.moveNextButton.MouseClick += this.MoveNextButton_MouseClick;
            // 
            // FileListToolBar
            //
            this.Controls.AddRange(
                this.moveNextButton,
                this.movePreviewButton,
                this.thumbnailSizeSlider,
                this.addDateSortButton,
                this.createDateSortButton,
                this.updateDateSortButton,
                this.takenDateSortButton,
                this.pathSortButton,
                this.nameSortButton,
                this.viewButton);
            this.IsDrawBottomBorderLine = true;
            this.viewMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private SWF.UIComponent.Base.BaseTextButton viewButton;
        private SWF.UIComponent.Base.Slider thumbnailSizeSlider;
        private System.Windows.Forms.ToolStripDropDown viewMenu;
        private System.Windows.Forms.ToolStripMenuItem directoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otherFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileNameMenuItem;
        private SWF.UIComponent.Base.BaseTextButton movePreviewButton;
        private SWF.UIComponent.Base.BaseTextButton moveNextButton;
        private SWF.UIComponent.Base.BaseTextButton nameSortButton;
        private SWF.UIComponent.Base.BaseTextButton pathSortButton;
        private SWF.UIComponent.Base.BaseTextButton createDateSortButton;
        private SWF.UIComponent.Base.BaseTextButton updateDateSortButton;
        private SWF.UIComponent.Base.BaseTextButton takenDateSortButton;
        private SWF.UIComponent.Base.BaseTextButton addDateSortButton;
    }
}
