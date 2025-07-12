using SWF.Core.Base;
using System.Drawing;

namespace PicSum.UIComponent.Contents.FileList
{
    partial class FileListToolBar
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.viewButton = new SWF.UIComponent.Core.ToolTextButton();
            this.nameSortButton = new SWF.UIComponent.Core.ToolTextButton();
            this.pathSortButton = new SWF.UIComponent.Core.ToolTextButton();
            this.timestampSortButton = new SWF.UIComponent.Core.ToolTextButton();
            this.registrationSortButton = new SWF.UIComponent.Core.ToolTextButton();
            this.thumbnailSizeSlider = new SWF.UIComponent.Core.Slider();
            this.viewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.directoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otherFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileNameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movePreviewButton = new SWF.UIComponent.Core.ToolTextButton();
            this.moveNextButton = new SWF.UIComponent.Core.ToolTextButton();
            this.viewMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewButton
            // 
            this.viewButton.MouseClick += this.ViewButton_MouseClick;
            // 
            // nameSortButton
            // 
            this.nameSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.nameSortButton.MouseClick += this.NameSortButton_MouseClick;
            // 
            // pathSortButton
            // 
            this.pathSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.pathSortButton.MouseClick += this.PathSortButton_MouseClick;
            // 
            // timestampSortButton
            // 
            this.timestampSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.timestampSortButton.MouseClick += this.TimestampSortButton_MouseClick;
            // 
            // registrationSortButton
            // 
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
            this.viewMenu.Font = AppConstants.UI_FONT_14_REGULAR;
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
            this.movePreviewButton.MouseClick += this.MovePreviewButton_MouseClick;
            // 
            // moveNextButton
            // 
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
            this.DoubleBuffered = true;
            this.IsDrawBottomBorderLine = true;
            this.viewMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private SWF.UIComponent.Core.ToolTextButton viewButton;
        private SWF.UIComponent.Core.Slider thumbnailSizeSlider;
        private System.Windows.Forms.ContextMenuStrip viewMenu;
        private System.Windows.Forms.ToolStripMenuItem directoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otherFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileNameMenuItem;
        private SWF.UIComponent.Core.ToolTextButton movePreviewButton;
        private SWF.UIComponent.Core.ToolTextButton moveNextButton;
        private SWF.UIComponent.Core.ToolTextButton nameSortButton;
        private SWF.UIComponent.Core.ToolTextButton pathSortButton;
        private SWF.UIComponent.Core.ToolTextButton timestampSortButton;
        private SWF.UIComponent.Core.ToolTextButton registrationSortButton;
    }
}
