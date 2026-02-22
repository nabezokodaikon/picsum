using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using System;
using System.Drawing;

namespace PicSum.UIComponent.InfoPanel
{
    partial class InfoPanel
    {
        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this._thumbnailPictureBox = new SKControl();
            this._tagContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this._tagDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._tagToAllEntryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._ratingBar = new SWF.UIComponent.Base.RatingBar();
            this._tagFlowList = new SWF.UIComponent.SKFlowList.SKFlowList();
            this._wideComboBox = new SWF.UIComponent.WideDropDown.WideComboBox();
            this._fileInfoLabel = new InfoLabel();
            this._tagContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // thumbnailPictureBox
            // 
            this._thumbnailPictureBox.BackColor = Color.FromArgb(250, 250, 250);
            this._thumbnailPictureBox.PaintSurface += this.ThumbnailPictureBox_PaintSurface;
            // 
            // tagContextMenuStrip
            //
            this._tagContextMenuStrip.ShowImageMargin = false;
            this._tagContextMenuStrip.Font = FontCacher.GetRegularGdiFont(FontCacher.Size.Medium);
            this._tagContextMenuStrip.ImageScalingSize = new Size(20, 20);
            this._tagContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._tagDeleteMenuItem, this._tagToAllEntryMenuItem });
            this._tagContextMenuStrip.Opening += this.TagContextMenuStrip_Opening;
            // 
            // tagDeleteMenuItem
            // 
            this._tagDeleteMenuItem.Text = "Remove Tag";
            this._tagDeleteMenuItem.Click += this.TagDeleteMenuItem_Click;
            // 
            // tagToAllEntryMenuItem
            // 
            this._tagToAllEntryMenuItem.Text = "Apply Tag to All";
            this._tagToAllEntryMenuItem.Click += this.TagToAllEntryMenuItem_Click;
            // 
            // ratingBar
            // 
            this._ratingBar.BackColor = Color.FromArgb(250, 250, 250);
            this._ratingBar.MaximumValue = 1;
            this._ratingBar.RatingButtonMouseClick += this.RatingBar_RatingButtonMouseClick;
            // 
            // tagFlowList
            // 
            this._tagFlowList.BackgroundColor = new SKColor(250, 250, 250);
            this._tagFlowList.ContextMenuStrip = this._tagContextMenuStrip;
            this._tagFlowList.IsLileList = true;
            this._tagFlowList.ItemHeight = 32;
            this._tagFlowList.MouseWheelRate = 2.5f;
            this._tagFlowList.SKDrawItem += this.TagFlowList_DrawItem;
            this._tagFlowList.MouseClick += this.TagFlowList_MouseClick;
            this._tagFlowList.MouseDoubleClick += this.TagFlowList_MouseDoubleClick;
            // 
            // wideComboBox
            // 
            this._wideComboBox.BackColor = Color.FromArgb(250, 250, 250);
            this._wideComboBox.Icon = SkiaUtil.ToSKImage(SWF.Core.ResourceAccessor.ResourceFiles.TagIcon.Value);
            this._wideComboBox.DropDownOpening += this.WideComboBox_DropDownOpening;
            this._wideComboBox.AddItem += this.WideComboBox_AddItem;
            // 
            // fileInfoLabel
            // 
            this._fileInfoLabel.BackColor = Color.FromArgb(250, 250, 250);
            // 
            // InfoPanel
            // 
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.Controls.AddRange(
                this._wideComboBox,
                this._thumbnailPictureBox,
                this._ratingBar,
                this._tagFlowList,
                this._fileInfoLabel);
            this.IsDrawLeftBorderLine = true;
            this.VerticalTopMargin = 28;
            this._tagContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private SKControl _thumbnailPictureBox;
        private SWF.UIComponent.Base.RatingBar _ratingBar;
        private SWF.UIComponent.SKFlowList.SKFlowList _tagFlowList;
        private System.Windows.Forms.ContextMenuStrip _tagContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _tagDeleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _tagToAllEntryMenuItem;
        private SWF.UIComponent.WideDropDown.WideComboBox _wideComboBox;
        private InfoLabel _fileInfoLabel;

        #endregion
    }
}
