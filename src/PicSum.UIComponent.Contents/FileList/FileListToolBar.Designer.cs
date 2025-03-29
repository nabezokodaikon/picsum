using System.Drawing;

namespace PicSum.UIComponent.Contents.FileList
{
    partial class FileListToolBar
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.viewButton = new SWF.UIComponent.Core.ToolButton();
            this.nameSortButton = new SWF.UIComponent.Core.ToolButton();
            this.pathSortButton = new SWF.UIComponent.Core.ToolButton();
            this.timestampSortButton = new SWF.UIComponent.Core.ToolButton();
            this.registrationSortButton = new SWF.UIComponent.Core.ToolButton();
            this.thumbnailSizeSlider = new SWF.UIComponent.Core.Slider();
            this.viewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.folderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otherFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileNameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movePreviewButton = new SWF.UIComponent.Core.ToolButton();
            this.moveNextButton = new SWF.UIComponent.Core.ToolButton();
            this.viewMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewButton
            // 
            this.viewButton.Name = "viewButton";
            this.viewButton.MouseClick += this.ViewButton_MouseClick;
            // 
            // nameSortButton
            // 
            this.nameSortButton.Name = "nameSortButton";
            this.nameSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.nameSortButton.MouseClick += this.NameSortButton_MouseClick;
            // 
            // pathSortButton
            // 
            this.pathSortButton.Name = "pathSortButton";
            this.pathSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.pathSortButton.MouseClick += this.PathSortButton_MouseClick;
            // 
            // timestampSortButton
            // 
            this.timestampSortButton.Name = "timestampSortButton";
            this.timestampSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.timestampSortButton.MouseClick += this.TimestampSortButton_MouseClick;
            // 
            // registrationSortButton
            // 
            this.registrationSortButton.Name = "registrationSortButton";
            this.registrationSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.registrationSortButton.MouseClick += this.RegistrationSortButton_MouseClick;
            // 
            // thumbnailSizeSlider
            // 
            this.thumbnailSizeSlider.Name = "thumbnailSizeSlider";
            this.thumbnailSizeSlider.Text = "slider1";
            this.thumbnailSizeSlider.BeginValueChange += this.ThumbnailSizeSlider_BeginValueChange;
            this.thumbnailSizeSlider.ValueChanging += this.ThumbnailSizeSlider_ValueChanging;
            this.thumbnailSizeSlider.ValueChanged += this.ThumbnailSizeSlider_ValueChanged;
            // 
            // viewMenu
            // 
            this.viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.folderMenuItem, this.imageFileMenuItem, this.otherFileMenuItem, this.toolStripSeparator1, this.fileNameMenuItem });
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new Size(181, 128);
            // 
            // folderMenuItem
            // 
            this.folderMenuItem.Name = "folderMenuItem";
            this.folderMenuItem.Size = new Size(180, 24);
            this.folderMenuItem.Text = "Folder";
            this.folderMenuItem.Click += this.FolderMenuItem_Click;
            // 
            // imageFileMenuItem
            // 
            this.imageFileMenuItem.Name = "imageFileMenuItem";
            this.imageFileMenuItem.Size = new Size(180, 24);
            this.imageFileMenuItem.Text = "Image File";
            this.imageFileMenuItem.Click += this.ImageFileMenuItem_Click;
            // 
            // otherFileMenuItem
            // 
            this.otherFileMenuItem.Name = "otherFileMenuItem";
            this.otherFileMenuItem.Size = new Size(180, 24);
            this.otherFileMenuItem.Text = "Other File";
            this.otherFileMenuItem.Click += this.OtherFileMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(177, 6);
            // 
            // fileNameMenuItem
            // 
            this.fileNameMenuItem.Name = "fileNameMenuItem";
            this.fileNameMenuItem.Size = new Size(180, 24);
            this.fileNameMenuItem.Text = "FileName";
            this.fileNameMenuItem.Click += this.FileNameMenuItem_Click;
            // 
            // movePreviewButton
            // 
            this.movePreviewButton.Name = "movePreviewButton";
            this.movePreviewButton.MouseClick += this.MovePreviewButton_MouseClick;
            // 
            // moveNextButton
            // 
            this.moveNextButton.Name = "moveNextButton";
            this.moveNextButton.MouseClick += this.MoveNextButton_MouseClick;
            // 
            // FileListToolBar
            // 
            this.Controls.Add(this.moveNextButton);
            this.Controls.Add(this.movePreviewButton);
            this.Controls.Add(this.thumbnailSizeSlider);
            this.Controls.Add(this.registrationSortButton);
            this.Controls.Add(this.timestampSortButton);
            this.Controls.Add(this.pathSortButton);
            this.Controls.Add(this.nameSortButton);
            this.Controls.Add(this.viewButton);
            this.DoubleBuffered = true;
            this.Name = "FileListToolBar";
            this.Size = new Size(767, 29);
            this.IsDrawBottomBorderLine = true;
            this.viewMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private SWF.UIComponent.Core.ToolButton viewButton;
        private SWF.UIComponent.Core.Slider thumbnailSizeSlider;
        private System.Windows.Forms.ContextMenuStrip viewMenu;
        private System.Windows.Forms.ToolStripMenuItem folderMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otherFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileNameMenuItem;
        private SWF.UIComponent.Core.ToolButton movePreviewButton;
        private SWF.UIComponent.Core.ToolButton moveNextButton;
        private SWF.UIComponent.Core.ToolButton nameSortButton;
        private SWF.UIComponent.Core.ToolButton pathSortButton;
        private SWF.UIComponent.Core.ToolButton timestampSortButton;
        private SWF.UIComponent.Core.ToolButton registrationSortButton;
    }
}
