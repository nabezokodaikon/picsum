namespace PicSum.UIComponent.InfoPanel
{
    partial class InfoPanel
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
            this.thumbnailPictureBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tagContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tagDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagToAllEntryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ratingBar = new SWF.UIComponent.Common.RatingBar();
            this.tagFlowList = new SWF.UIComponent.FlowList.FlowList();
            this.wideComboBox = new SWF.UIComponent.WideDropDown.WideComboBox();
            this.fileInfoLabel = new FileInfoLabel();
            ((System.ComponentModel.ISupportInitialize)this.thumbnailPictureBox).BeginInit();
            this.panel1.SuspendLayout();
            this.tagContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.tagFlowList).BeginInit();
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
            this.thumbnailPictureBox.Paint += this.ThumbnailPictureBox_Paint;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.fileInfoLabel);
            this.panel1.Location = new System.Drawing.Point(0, 262);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 125);
            this.panel1.TabIndex = 17;
            // 
            // tagContextMenuStrip
            // 
            this.tagContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tagContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.tagDeleteMenuItem, this.tagToAllEntryMenuItem });
            this.tagContextMenuStrip.Name = "tagContextMenuStrip";
            this.tagContextMenuStrip.Size = new System.Drawing.Size(158, 48);
            this.tagContextMenuStrip.Opening += this.TagContextMenuStrip_Opening;
            // 
            // tagDeleteMenuItem
            // 
            this.tagDeleteMenuItem.Name = "tagDeleteMenuItem";
            this.tagDeleteMenuItem.Size = new System.Drawing.Size(157, 22);
            this.tagDeleteMenuItem.Text = "Remove Tag";
            this.tagDeleteMenuItem.Click += this.TagDeleteMenuItem_Click;
            // 
            // tagToAllEntryMenuItem
            // 
            this.tagToAllEntryMenuItem.Name = "tagToAllEntryMenuItem";
            this.tagToAllEntryMenuItem.Size = new System.Drawing.Size(157, 22);
            this.tagToAllEntryMenuItem.Text = "Apply Tag to All";
            this.tagToAllEntryMenuItem.Click += this.TagToAllEntryMenuItem_Click;
            // 
            // ratingBar
            // 
            this.ratingBar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.ratingBar.BackColor = System.Drawing.Color.Transparent;
            this.ratingBar.Location = new System.Drawing.Point(0, 390);
            this.ratingBar.Margin = new System.Windows.Forms.Padding(0);
            this.ratingBar.MaximumValue = 1;
            this.ratingBar.Name = "ratingBar";
            this.ratingBar.Size = new System.Drawing.Size(512, 56);
            this.ratingBar.TabIndex = 15;
            this.ratingBar.RatingButtonMouseClick += this.RatingBar_RatingButtonMouseClick;
            // 
            // tagFlowList
            // 
            this.tagFlowList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.tagFlowList.BackColor = System.Drawing.Color.FromArgb(241, 244, 250);
            this.tagFlowList.CanKeyDown = true;
            this.tagFlowList.ContextMenuStrip = this.tagContextMenuStrip;
            this.tagFlowList.IsLileList = true;
            this.tagFlowList.ItemHeight = 24;
            this.tagFlowList.ItemTextAlignment = System.Drawing.StringAlignment.Near;
            this.tagFlowList.ItemTextFormatFlags = System.Drawing.StringFormatFlags.NoWrap;
            this.tagFlowList.ItemTextLineAlignment = System.Drawing.StringAlignment.Center;
            this.tagFlowList.ItemTextTrimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.tagFlowList.Location = new System.Drawing.Point(0, 499);
            this.tagFlowList.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.tagFlowList.Name = "tagFlowList";
            this.tagFlowList.Size = new System.Drawing.Size(512, 216);
            this.tagFlowList.TabIndex = 16;
            this.tagFlowList.TabStop = false;
            this.tagFlowList.Text = "flowList1";
            this.tagFlowList.DrawItem += this.TagFlowList_DrawItem;
            this.tagFlowList.MouseClick += this.TagFlowList_MouseClick;
            this.tagFlowList.MouseDoubleClick += this.TagFlowList_MouseDoubleClick;
            // 
            // wideComboBox
            // 
            this.wideComboBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.wideComboBox.Font = new System.Drawing.Font("Yu Gothic UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            this.wideComboBox.Icon = Properties.Resources.TagIcon;
            this.wideComboBox.Location = new System.Drawing.Point(0, 452);
            this.wideComboBox.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.wideComboBox.Name = "wideComboBox";
            this.wideComboBox.Size = new System.Drawing.Size(512, 38);
            this.wideComboBox.TabIndex = 27;
            this.wideComboBox.DropDownOpening += this.WideComboBox_DropDownOpening;
            this.wideComboBox.AddItem += this.WideComboBox_AddItem;
            // 
            // fileInfoLabel
            // 
            this.fileInfoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileInfoLabel.FileName = "";
            this.fileInfoLabel.FileSize = "";
            this.fileInfoLabel.FileType = "";
            this.fileInfoLabel.Location = new System.Drawing.Point(0, 0);
            this.fileInfoLabel.Name = "fileInfoLabel";
            this.fileInfoLabel.Size = new System.Drawing.Size(512, 125);
            this.fileInfoLabel.TabIndex = 0;
            this.fileInfoLabel.Text = "fileInfoLabel1";
            this.fileInfoLabel.Timestamp = "";
            // 
            // InfoPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.wideComboBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.thumbnailPictureBox);
            this.Controls.Add(this.ratingBar);
            this.Controls.Add(this.tagFlowList);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            this.Name = "InfoPanel";
            this.Size = new System.Drawing.Size(512, 715);
            ((System.ComponentModel.ISupportInitialize)this.thumbnailPictureBox).EndInit();
            this.panel1.ResumeLayout(false);
            this.tagContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.tagFlowList).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox thumbnailPictureBox;
        private SWF.UIComponent.Common.RatingBar ratingBar;
        private SWF.UIComponent.FlowList.FlowList tagFlowList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip tagContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tagDeleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagToAllEntryMenuItem;
        private SWF.UIComponent.WideDropDown.WideComboBox wideComboBox;
        private FileInfoLabel fileInfoLabel;
    }
}
