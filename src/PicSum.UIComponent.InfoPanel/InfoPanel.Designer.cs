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
            this.panel1.Controls.Add(this.fileUpdatedateLabel);
            this.panel1.Controls.Add(this.fileSizeLabel);
            this.panel1.Controls.Add(this.fileTypeLabel);
            this.panel1.Controls.Add(this.fileNameLabel);
            this.panel1.Location = new System.Drawing.Point(0, 262);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 125);
            this.panel1.TabIndex = 17;
            // 
            // fileUpdatedateLabel
            // 
            this.fileUpdatedateLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.fileUpdatedateLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileUpdatedateLabel.Location = new System.Drawing.Point(4, 74);
            this.fileUpdatedateLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileUpdatedateLabel.Name = "fileUpdatedateLabel";
            this.fileUpdatedateLabel.Size = new System.Drawing.Size(504, 18);
            this.fileUpdatedateLabel.TabIndex = 25;
            this.fileUpdatedateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileSizeLabel
            // 
            this.fileSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.fileSizeLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileSizeLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileSizeLabel.Location = new System.Drawing.Point(4, 100);
            this.fileSizeLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileSizeLabel.Name = "fileSizeLabel";
            this.fileSizeLabel.Size = new System.Drawing.Size(504, 18);
            this.fileSizeLabel.TabIndex = 24;
            this.fileSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileTypeLabel
            // 
            this.fileTypeLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.fileTypeLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileTypeLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileTypeLabel.Location = new System.Drawing.Point(4, 48);
            this.fileTypeLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileTypeLabel.Name = "fileTypeLabel";
            this.fileTypeLabel.Size = new System.Drawing.Size(504, 18);
            this.fileTypeLabel.TabIndex = 23;
            this.fileTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.fileNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.fileNameLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileNameLabel.Location = new System.Drawing.Point(4, 4);
            this.fileNameLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(504, 36);
            this.fileNameLabel.TabIndex = 22;
            this.fileNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // InfoPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
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
