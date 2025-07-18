using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Core;
using System;
using System.ComponentModel;
using System.Drawing;

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
            this.viewButton = new SWF.UIComponent.Core.BaseTextButton();
            this.nameSortButton = new SWF.UIComponent.Core.BaseTextButton();
            this.pathSortButton = new SWF.UIComponent.Core.BaseTextButton();
            this.timestampSortButton = new SWF.UIComponent.Core.BaseTextButton();
            this.registrationSortButton = new SWF.UIComponent.Core.BaseTextButton();
            this.thumbnailSizeSlider = new SWF.UIComponent.Core.Slider();
            this.viewMenu = new System.Windows.Forms.ContextMenuStrip();
            this.directoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otherFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileNameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movePreviewButton = new SWF.UIComponent.Core.BaseTextButton();
            this.moveNextButton = new SWF.UIComponent.Core.BaseTextButton();
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
            this.nameSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.nameSortButton.MouseClick += this.NameSortButton_MouseClick;
            // 
            // pathSortButton
            //
            this.pathSortButton.Text = "Path";
            this.pathSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.pathSortButton.MouseClick += this.PathSortButton_MouseClick;
            // 
            // timestampSortButton
            //
            this.timestampSortButton.Text = "Time stamp";
            this.timestampSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.timestampSortButton.MouseClick += this.TimestampSortButton_MouseClick;
            // 
            // registrationSortButton
            //
            this.registrationSortButton.Text = "Registration";
            this.registrationSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.registrationSortButton.MouseClick += this.RegistrationSortButton_MouseClick;
            // 
            // thumbnailSizeSlider
            // 
            this.thumbnailSizeSlider.BeginValueChange += this.ThumbnailSizeSlider_BeginValueChange;
            this.thumbnailSizeSlider.ValueChanging += this.ThumbnailSizeSlider_ValueChanging;
            this.thumbnailSizeSlider.ValueChanged += this.ThumbnailSizeSlider_ValueChanged;
            // 
            // viewMenu
            //
            this.viewMenu.Renderer = new CustomDropDownRenderer(this.viewMenu);
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
                this.registrationSortButton,
                this.timestampSortButton,
                this.pathSortButton,
                this.nameSortButton,
                this.viewButton);
            this.IsDrawBottomBorderLine = true;
            this.viewMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private SWF.UIComponent.Core.BaseTextButton viewButton;
        private SWF.UIComponent.Core.Slider thumbnailSizeSlider;
        private System.Windows.Forms.ToolStripDropDown viewMenu;
        private System.Windows.Forms.ToolStripMenuItem directoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otherFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileNameMenuItem;
        private SWF.UIComponent.Core.BaseTextButton movePreviewButton;
        private SWF.UIComponent.Core.BaseTextButton moveNextButton;
        private SWF.UIComponent.Core.BaseTextButton nameSortButton;
        private SWF.UIComponent.Core.BaseTextButton pathSortButton;
        private SWF.UIComponent.Core.BaseTextButton timestampSortButton;
        private SWF.UIComponent.Core.BaseTextButton registrationSortButton;
    }
}
