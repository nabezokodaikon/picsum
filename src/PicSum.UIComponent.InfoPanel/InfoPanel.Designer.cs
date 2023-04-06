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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.addTagButton = new SWF.UIComponent.Common.ToolButton();
            this.tagComboBox = new System.Windows.Forms.ComboBox();
            this.tagFlowList = new SWF.UIComponent.FlowList.FlowList();
            this.tagContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tagDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagToAllEntryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ratingBar = new SWF.UIComponent.Common.RatingBar();
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tagContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // thumbnailPictureBox
            // 
            this.thumbnailPictureBox.BackColor = System.Drawing.SystemColors.ControlDark;
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
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.fileCreateDateLabel);
            this.panel1.Controls.Add(this.fileUpdatedateLabel);
            this.panel1.Controls.Add(this.fileSizeLabel);
            this.panel1.Controls.Add(this.fileTypeLabel);
            this.panel1.Controls.Add(this.fileNameLabel);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(0, 262);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 148);
            this.panel1.TabIndex = 17;
            // 
            // fileCreateDateLabel
            // 
            this.fileCreateDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileCreateDateLabel.BackColor = System.Drawing.SystemColors.Control;
            this.fileCreateDateLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileCreateDateLabel.Location = new System.Drawing.Point(73, 100);
            this.fileCreateDateLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileCreateDateLabel.Name = "fileCreateDateLabel";
            this.fileCreateDateLabel.Size = new System.Drawing.Size(435, 18);
            this.fileCreateDateLabel.TabIndex = 26;
            // 
            // fileUpdatedateLabel
            // 
            this.fileUpdatedateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileUpdatedateLabel.BackColor = System.Drawing.SystemColors.Control;
            this.fileUpdatedateLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileUpdatedateLabel.Location = new System.Drawing.Point(73, 126);
            this.fileUpdatedateLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileUpdatedateLabel.Name = "fileUpdatedateLabel";
            this.fileUpdatedateLabel.Size = new System.Drawing.Size(435, 18);
            this.fileUpdatedateLabel.TabIndex = 25;
            // 
            // fileSizeLabel
            // 
            this.fileSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSizeLabel.BackColor = System.Drawing.SystemColors.Control;
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
            this.fileTypeLabel.BackColor = System.Drawing.SystemColors.Control;
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
            this.fileNameLabel.BackColor = System.Drawing.SystemColors.Control;
            this.fileNameLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileNameLabel.Location = new System.Drawing.Point(4, 4);
            this.fileNameLabel.Margin = new System.Windows.Forms.Padding(4);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(504, 36);
            this.fileNameLabel.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(4, 126);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 18);
            this.label5.TabIndex = 19;
            this.label5.Text = "更新日時：";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(4, 100);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 18);
            this.label4.TabIndex = 18;
            this.label4.Text = "作成日時：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Controls.Add(this.addTagButton);
            this.panel2.Controls.Add(this.tagComboBox);
            this.panel2.Location = new System.Drawing.Point(0, 458);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(512, 39);
            this.panel2.TabIndex = 18;
            // 
            // addTagButton
            // 
            this.addTagButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addTagButton.Image = global::PicSum.UIComponent.InfoPanel.Properties.Resources.TagIcon;
            this.addTagButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addTagButton.Location = new System.Drawing.Point(468, 4);
            this.addTagButton.Margin = new System.Windows.Forms.Padding(0, 4, 4, 4);
            this.addTagButton.Name = "addTagButton";
            this.addTagButton.RegionType = SWF.UIComponent.Common.ToolButton.ToolButtonRegionType.Default;
            this.addTagButton.Size = new System.Drawing.Size(40, 31);
            this.addTagButton.TabIndex = 1;
            this.addTagButton.Text = "+";
            this.addTagButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addTagButton.UseVisualStyleBackColor = true;
            this.addTagButton.Click += new System.EventHandler(this.addTagButton_Click);
            // 
            // tagComboBox
            // 
            this.tagComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagComboBox.FormattingEnabled = true;
            this.tagComboBox.Location = new System.Drawing.Point(3, 6);
            this.tagComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.tagComboBox.Name = "tagComboBox";
            this.tagComboBox.Size = new System.Drawing.Size(461, 23);
            this.tagComboBox.TabIndex = 0;
            this.tagComboBox.DropDown += new System.EventHandler(this.tagComboBox_DropDown);
            this.tagComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tagComboBox_KeyPress);
            // 
            // tagFlowList
            // 
            this.tagFlowList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagFlowList.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tagFlowList.ContextMenuStrip = this.tagContextMenuStrip;
            this.tagFlowList.FocusItemColor = System.Drawing.Color.Empty;
            this.tagFlowList.IsLileList = true;
            this.tagFlowList.ItemHeight = 24;
            this.tagFlowList.ItemTextAlignment = System.Drawing.StringAlignment.Near;
            this.tagFlowList.ItemTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tagFlowList.ItemTextFormatFlags = System.Drawing.StringFormatFlags.NoWrap;
            this.tagFlowList.ItemTextLineAlignment = System.Drawing.StringAlignment.Center;
            this.tagFlowList.ItemTextTrimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.tagFlowList.Location = new System.Drawing.Point(0, 503);
            this.tagFlowList.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.tagFlowList.MousePointItemColor = System.Drawing.Color.Empty;
            this.tagFlowList.Name = "tagFlowList";
            this.tagFlowList.RectangleSelectionColor = System.Drawing.Color.Empty;
            this.tagFlowList.SelectedItemColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tagFlowList.Size = new System.Drawing.Size(512, 212);
            this.tagFlowList.TabIndex = 16;
            this.tagFlowList.Text = "flowList1";
            this.tagFlowList.DrawItem += new System.EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(this.tagFlowList_DrawItem);
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
            this.ratingBar.Location = new System.Drawing.Point(0, 422);
            this.ratingBar.Margin = new System.Windows.Forms.Padding(3, 9, 3, 9);
            this.ratingBar.MaximumValue = 5;
            this.ratingBar.Name = "ratingBar";
            this.ratingBar.Size = new System.Drawing.Size(512, 24);
            this.ratingBar.TabIndex = 15;
            this.ratingBar.RatingButtonMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.ratingBar_RatingButtonMouseClick);
            // 
            // InfoPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.thumbnailPictureBox);
            this.Controls.Add(this.ratingBar);
            this.Controls.Add(this.tagFlowList);
            this.Name = "InfoPanel";
            this.Size = new System.Drawing.Size(512, 715);
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private SWF.UIComponent.Common.ToolButton addTagButton;
        private System.Windows.Forms.ComboBox tagComboBox;
        private System.Windows.Forms.ContextMenuStrip tagContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tagDeleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagToAllEntryMenuItem;


    }
}
