using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using System.Drawing;

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
            this.thumbnailPictureBox = new SWF.UIComponent.Core.PictureBoxEx();
            this.tagContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tagDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagToAllEntryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ratingBar = new SWF.UIComponent.Core.RatingBar();
            this.tagFlowList = new SWF.UIComponent.FlowList.FlowList();
            this.wideComboBox = new SWF.UIComponent.WideDropDown.WideComboBox();
            this.fileInfoLabel = new FileInfoLabel();
            this.tagContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // thumbnailPictureBox
            // 
            this.thumbnailPictureBox.BackColor = Color.FromArgb(250, 250, 250);
            this.thumbnailPictureBox.Font = Fonts.UI_FONT_18;
            this.thumbnailPictureBox.Paint += this.ThumbnailPictureBox_Paint;
            // 
            // tagContextMenuStrip
            // 
            this.tagContextMenuStrip.Font = Fonts.UI_FONT_14_REGULAR;
            this.tagContextMenuStrip.ImageScalingSize = new Size(20, 20);
            this.tagContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.tagDeleteMenuItem, this.tagToAllEntryMenuItem });
            this.tagContextMenuStrip.Opening += this.TagContextMenuStrip_Opening;
            // 
            // tagDeleteMenuItem
            // 
            this.tagDeleteMenuItem.Text = "Remove Tag";
            this.tagDeleteMenuItem.Click += this.TagDeleteMenuItem_Click;
            // 
            // tagToAllEntryMenuItem
            // 
            this.tagToAllEntryMenuItem.Text = "Apply Tag to All";
            this.tagToAllEntryMenuItem.Click += this.TagToAllEntryMenuItem_Click;
            // 
            // ratingBar
            // 
            this.ratingBar.BackColor = Color.FromArgb(250, 250, 250);
            this.ratingBar.MaximumValue = 1;
            this.ratingBar.RatingButtonMouseClick += this.RatingBar_RatingButtonMouseClick;
            // 
            // tagFlowList
            // 
            this.tagFlowList.BackColor = Color.FromArgb(250, 250, 250);
            this.tagFlowList.ContextMenuStrip = this.tagContextMenuStrip;
            this.tagFlowList.IsLileList = true;
            this.tagFlowList.ItemHeight = 32;
            this.tagFlowList.DrawItem += this.TagFlowList_DrawItem;
            this.tagFlowList.MouseClick += this.TagFlowList_MouseClick;
            this.tagFlowList.MouseDoubleClick += this.TagFlowList_MouseDoubleClick;
            // 
            // wideComboBox
            // 
            this.wideComboBox.BackColor = Color.FromArgb(250, 250, 250);
            this.wideComboBox.Icon = SWF.Core.ResourceAccessor.ResourceFiles.TagIcon.Value;
            this.wideComboBox.DropDownOpening += this.WideComboBox_DropDownOpening;
            this.wideComboBox.AddItem += this.WideComboBox_AddItem;
            // 
            // fileInfoLabel
            // 
            this.fileInfoLabel.BackColor = Color.FromArgb(250, 250, 250);
            this.fileInfoLabel.Font = Fonts.UI_FONT_14_REGULAR;
            // 
            // InfoPanel
            // 
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.Controls.AddRange(
                this.wideComboBox,
                this.thumbnailPictureBox,
                this.ratingBar,
                this.tagFlowList,
                this.fileInfoLabel);
            this.IsDrawLeftBorderLine = true;
            this.VerticalTopMargin = 28;
            this.tagContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.Core.PictureBoxEx thumbnailPictureBox;
        private SWF.UIComponent.Core.RatingBar ratingBar;
        private SWF.UIComponent.FlowList.FlowList tagFlowList;
        private System.Windows.Forms.ContextMenuStrip tagContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tagDeleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagToAllEntryMenuItem;
        private SWF.UIComponent.WideDropDown.WideComboBox wideComboBox;
        private FileInfoLabel fileInfoLabel;

        #endregion
    }
}
