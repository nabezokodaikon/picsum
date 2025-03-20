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
            this.viewButton.BackColor = Color.FromArgb(241, 244, 250);
            this.viewButton.FlatAppearance.BorderSize = 0;
            this.viewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.viewButton.Location = new Point(3, 3);
            this.viewButton.Name = "viewButton";
            this.viewButton.Size = new Size(64, 23);
            this.viewButton.TabIndex = 0;
            this.viewButton.Text = "View";
            this.viewButton.UseVisualStyleBackColor = false;
            this.viewButton.MouseClick += this.ViewButton_MouseClick;
            // 
            // nameSortButton
            // 
            this.nameSortButton.BackColor = Color.FromArgb(241, 244, 250);
            this.nameSortButton.FlatAppearance.BorderSize = 0;
            this.nameSortButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nameSortButton.Location = new Point(73, 3);
            this.nameSortButton.Name = "nameSortButton";
            this.nameSortButton.Size = new Size(96, 23);
            this.nameSortButton.TabIndex = 1;
            this.nameSortButton.Text = "Name";
            this.nameSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.nameSortButton.UseVisualStyleBackColor = false;
            this.nameSortButton.MouseClick += this.NameSortButton_MouseClick;
            // 
            // pathSortButton
            // 
            this.pathSortButton.BackColor = Color.FromArgb(241, 244, 250);
            this.pathSortButton.FlatAppearance.BorderSize = 0;
            this.pathSortButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pathSortButton.Location = new Point(175, 3);
            this.pathSortButton.Name = "pathSortButton";
            this.pathSortButton.Size = new Size(96, 23);
            this.pathSortButton.TabIndex = 2;
            this.pathSortButton.Text = "Path";
            this.pathSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.pathSortButton.UseVisualStyleBackColor = false;
            this.pathSortButton.MouseClick += this.PathSortButton_MouseClick;
            // 
            // timestampSortButton
            // 
            this.timestampSortButton.BackColor = Color.FromArgb(241, 244, 250);
            this.timestampSortButton.FlatAppearance.BorderSize = 0;
            this.timestampSortButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.timestampSortButton.Location = new Point(277, 3);
            this.timestampSortButton.Name = "timestampSortButton";
            this.timestampSortButton.Size = new Size(96, 23);
            this.timestampSortButton.TabIndex = 3;
            this.timestampSortButton.Text = "Time stamp";
            this.timestampSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.timestampSortButton.UseVisualStyleBackColor = false;
            this.timestampSortButton.MouseClick += this.TimestampSortButton_MouseClick;
            // 
            // registrationSortButton
            // 
            this.registrationSortButton.BackColor = Color.FromArgb(241, 244, 250);
            this.registrationSortButton.FlatAppearance.BorderSize = 0;
            this.registrationSortButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registrationSortButton.Location = new Point(379, 3);
            this.registrationSortButton.Name = "registrationSortButton";
            this.registrationSortButton.Size = new Size(96, 23);
            this.registrationSortButton.TabIndex = 4;
            this.registrationSortButton.Text = "Registration";
            this.registrationSortButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.registrationSortButton.UseVisualStyleBackColor = false;
            this.registrationSortButton.MouseClick += this.RegistrationSortButton_MouseClick;
            // 
            // thumbnailSizeSlider
            // 
            this.thumbnailSizeSlider.Location = new Point(481, 3);
            this.thumbnailSizeSlider.Name = "thumbnailSizeSlider";
            this.thumbnailSizeSlider.Size = new Size(108, 23);
            this.thumbnailSizeSlider.TabIndex = 5;
            this.thumbnailSizeSlider.Text = "slider1";
            this.thumbnailSizeSlider.BeginValueChange += this.ThumbnailSizeSlider_BeginValueChange;
            this.thumbnailSizeSlider.ValueChanging += this.ThumbnailSizeSlider_ValueChanging;
            this.thumbnailSizeSlider.ValueChanged += this.ThumbnailSizeSlider_ValueChanged;
            // 
            // viewMenu
            // 
            this.viewMenu.Font = new Font("Yu Gothic UI", 10F);
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
            this.movePreviewButton.BackColor = Color.FromArgb(241, 244, 250);
            this.movePreviewButton.FlatAppearance.BorderSize = 0;
            this.movePreviewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.movePreviewButton.Location = new Point(595, 3);
            this.movePreviewButton.Name = "movePreviewButton";
            this.movePreviewButton.Size = new Size(64, 23);
            this.movePreviewButton.TabIndex = 6;
            this.movePreviewButton.Text = "<-";
            this.movePreviewButton.UseVisualStyleBackColor = false;
            this.movePreviewButton.MouseClick += this.MovePreviewButton_MouseClick;
            // 
            // moveNextButton
            // 
            this.moveNextButton.BackColor = Color.FromArgb(241, 244, 250);
            this.moveNextButton.FlatAppearance.BorderSize = 0;
            this.moveNextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moveNextButton.Location = new Point(665, 3);
            this.moveNextButton.Name = "moveNextButton";
            this.moveNextButton.Size = new Size(64, 23);
            this.moveNextButton.TabIndex = 7;
            this.moveNextButton.Text = "->";
            this.moveNextButton.UseVisualStyleBackColor = false;
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
