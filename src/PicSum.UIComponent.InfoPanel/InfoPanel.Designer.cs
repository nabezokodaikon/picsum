namespace PicSum.UIComponent.InfoPanel
{
    partial class InfoPanel
    {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.thumbnailPictureBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fileCreateDateLabel = new System.Windows.Forms.Label();
            this.fileUpdatedateLabel = new System.Windows.Forms.Label();
            this.fileSizeLabel = new System.Windows.Forms.Label();
            this.fileTypeLabel = new System.Windows.Forms.Label();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.tagContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tagDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagToAllEntryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ratingBar = new SWF.UIComponent.Common.RatingBar();
            this.tagFlowList = new SWF.UIComponent.FlowList.FlowList();
            this.wideComboBox = new SWF.UIComponent.WideDropDown.WideComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.tagContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // thumbnailPictureBox
            // 
            this.thumbnailPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.thumbnailPictureBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.thumbnailPictureBox.Location = new System.Drawing.Point(0, 0);
            this.thumbnailPictureBox.MinimumSize = new System.Drawing.Size(128, 128);
            this.thumbnailPictureBox.Name = "thumbnailPictureBox";
            this.thumbnailPictureBox.Size = new System.Drawing.Size(512, 256);
            this.thumbnailPictureBox.TabIndex = 0;
            this.thumbnailPictureBox.TabStop = false;
            this.thumbnailPictureBox.Text = "thumbnailPictureBox1";
            this.thumbnailPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.thumbnailPictureBox_Paint);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.fileCreateDateLabel);
            this.panel1.Controls.Add(this.fileUpdatedateLabel);
            this.panel1.Controls.Add(this.fileSizeLabel);
            this.panel1.Controls.Add(this.fileTypeLabel);
            this.panel1.Controls.Add(this.fileNameLabel);
            this.panel1.Location = new System.Drawing.Point(0, 262);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 148);
            this.panel1.TabIndex = 17;
            // 
            // fileCreateDateLabel
            // 
            this.fileCreateDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileCreateDateLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileCreateDateLabel.Location = new System.Drawing.Point(4, 100);
            this.fileCreateDateLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileCreateDateLabel.Name = "fileCreateDateLabel";
            this.fileCreateDateLabel.Size = new System.Drawing.Size(504, 18);
            this.fileCreateDateLabel.TabIndex = 26;
            this.fileCreateDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileUpdatedateLabel
            // 
            this.fileUpdatedateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileUpdatedateLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileUpdatedateLabel.Location = new System.Drawing.Point(4, 126);
            this.fileUpdatedateLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileUpdatedateLabel.Name = "fileUpdatedateLabel";
            this.fileUpdatedateLabel.Size = new System.Drawing.Size(504, 18);
            this.fileUpdatedateLabel.TabIndex = 25;
            this.fileUpdatedateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileSizeLabel
            // 
            this.fileSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSizeLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileSizeLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileSizeLabel.Location = new System.Drawing.Point(4, 74);
            this.fileSizeLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileSizeLabel.Name = "fileSizeLabel";
            this.fileSizeLabel.Size = new System.Drawing.Size(504, 18);
            this.fileSizeLabel.TabIndex = 24;
            // 
            // fileTypeLabel
            // 
            this.fileTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTypeLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileTypeLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileTypeLabel.Location = new System.Drawing.Point(4, 48);
            this.fileTypeLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileTypeLabel.Name = "fileTypeLabel";
            this.fileTypeLabel.Size = new System.Drawing.Size(504, 18);
            this.fileTypeLabel.TabIndex = 23;
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileNameLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileNameLabel.Location = new System.Drawing.Point(4, 4);
            this.fileNameLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(504, 36);
            this.fileNameLabel.TabIndex = 22;
            // 
            // tagContextMenuStrip
            // 
            this.tagContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tagContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tagDeleteMenuItem,
            this.tagToAllEntryMenuItem});
            this.tagContextMenuStrip.Name = "tagContextMenuStrip";
            this.tagContextMenuStrip.Size = new System.Drawing.Size(181, 52);
            this.tagContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.tagContextMenuStrip_Opening);
            // 
            // tagDeleteMenuItem
            // 
            this.tagDeleteMenuItem.Name = "tagDeleteMenuItem";
            this.tagDeleteMenuItem.Size = new System.Drawing.Size(180, 24);
            this.tagDeleteMenuItem.Text = "タグを削除";
            this.tagDeleteMenuItem.Click += new System.EventHandler(this.tagDeleteMenuItem_Click);
            // 
            // tagToAllEntryMenuItem
            // 
            this.tagToAllEntryMenuItem.Name = "tagToAllEntryMenuItem";
            this.tagToAllEntryMenuItem.Size = new System.Drawing.Size(180, 24);
            this.tagToAllEntryMenuItem.Text = "タグを全てに適用";
            this.tagToAllEntryMenuItem.Click += new System.EventHandler(this.tagToAllEntryMenuItem_Click);
            // 
            // ratingBar
            // 
            this.ratingBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ratingBar.BackColor = System.Drawing.Color.Transparent;
            this.ratingBar.Location = new System.Drawing.Point(0, 413);
            this.ratingBar.Margin = new System.Windows.Forms.Padding(0);
            this.ratingBar.MaximumValue = 1;
            this.ratingBar.Name = "ratingBar";
            this.ratingBar.Size = new System.Drawing.Size(512, 58);
            this.ratingBar.TabIndex = 15;
            this.ratingBar.RatingButtonMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.ratingBar_RatingButtonMouseClick);
            // 
            // tagFlowList
            // 
            this.tagFlowList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagFlowList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.tagFlowList.ContextMenuStrip = this.tagContextMenuStrip;
            this.tagFlowList.FocusItemColor = System.Drawing.Color.Empty;
            this.tagFlowList.IsLileList = true;
            this.tagFlowList.ItemHeight = 24;
            this.tagFlowList.ItemTextAlignment = System.Drawing.StringAlignment.Near;
            this.tagFlowList.ItemTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tagFlowList.ItemTextFormatFlags = System.Drawing.StringFormatFlags.NoWrap;
            this.tagFlowList.ItemTextLineAlignment = System.Drawing.StringAlignment.Center;
            this.tagFlowList.ItemTextTrimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.tagFlowList.Location = new System.Drawing.Point(0, 524);
            this.tagFlowList.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.tagFlowList.MousePointItemColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(48)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tagFlowList.Name = "tagFlowList";
            this.tagFlowList.RectangleSelectionColor = System.Drawing.Color.Empty;
            this.tagFlowList.SelectedItemColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tagFlowList.Size = new System.Drawing.Size(512, 191);
            this.tagFlowList.TabIndex = 16;
            this.tagFlowList.Text = "flowList1";
            this.tagFlowList.DrawItem += new System.EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(this.tagFlowList_DrawItem);
            this.tagFlowList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tagFlowList_MouseClick);
            this.tagFlowList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tagFlowList_MouseDoubleClick);
            // 
            // wideComboBox
            // 
            this.wideComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wideComboBox.Location = new System.Drawing.Point(0, 477);
            this.wideComboBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.wideComboBox.Name = "wideComboBox";
            this.wideComboBox.Size = new System.Drawing.Size(512, 38);
            this.wideComboBox.TabIndex = 27;
            this.wideComboBox.DropDownOpening += new System.EventHandler<SWF.UIComponent.WideDropDown.DropDownOpeningEventArgs>(this.wideComboBox_DropDownOpening);
            this.wideComboBox.AddItem += new System.EventHandler<SWF.UIComponent.WideDropDown.AddItemEventArgs>(this.wideComboBox_AddItem);
            // 
            // InfoPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.wideComboBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.thumbnailPictureBox);
            this.Controls.Add(this.ratingBar);
            this.Controls.Add(this.tagFlowList);
            this.Name = "InfoPanel";
            this.Size = new System.Drawing.Size(512, 715);
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tagContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox thumbnailPictureBox;
        private SWF.UIComponent.Common.RatingBar ratingBar;
        private SWF.UIComponent.FlowList.FlowList tagFlowList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label fileCreateDateLabel;
        private System.Windows.Forms.Label fileUpdatedateLabel;
        private System.Windows.Forms.Label fileSizeLabel;
        private System.Windows.Forms.Label fileTypeLabel;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.ContextMenuStrip tagContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tagDeleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagToAllEntryMenuItem;
        private SWF.UIComponent.WideDropDown.WideComboBox wideComboBox;
    }
}
