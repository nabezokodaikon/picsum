using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.Storage;

namespace PicSum.Main.UIComponent
{
    partial class BrowserMainPanel
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserMainPanel));
            this.pageContainer = new SWF.UIComponent.TabOperation.PageContainer();
            this.infoPanel = new PicSum.UIComponent.InfoPanel.InfoPanel();
            this.tabSwitch = new SWF.UIComponent.TabOperation.TabSwitch();
            this.toolPanel = new System.Windows.Forms.Panel();
            this.searchBookmarkToolButton = new SWF.UIComponent.Core.ToolButton();
            this.searchClipToolButton = new SWF.UIComponent.Core.ToolButton();
            this.reloadToolButton = new SWF.UIComponent.Core.ToolButton();
            this.tagDropToolButton = new SWF.UIComponent.WideDropDown.WideDropToolButton();
            this.homeToolButton = new SWF.UIComponent.Core.ToolButton();
            this.nextPageHistoryButton = new SWF.UIComponent.Core.ToolButton();
            this.searchRatingToolButton = new SWF.UIComponent.Core.ToolButton();
            this.previewPageHistoryButton = new SWF.UIComponent.Core.ToolButton();
            this.showInfoToolButton = new SWF.UIComponent.Core.ToolButton();
            this.addressBar = new PicSum.UIComponent.AddressBar.AddressBar();
            this.toolPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainer
            // 
            this.pageContainer.AllowDrop = true;
            this.pageContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageContainer.Location = new Point(0, 0);
            this.pageContainer.Name = "pageContainer";
            this.pageContainer.Size = new Size(746, 402);
            this.pageContainer.TabIndex = 1;
            this.pageContainer.DragDrop += this.PageContainer_DragDrop;
            this.pageContainer.DragEnter += this.PageContainer_DragEnter;
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = Color.FromArgb(241, 244, 250);
            this.infoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoPanel.Location = new Point(0, 0);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new Size(96, 100);
            this.infoPanel.TabIndex = 0;
            this.infoPanel.SelectedTag += this.InfoPanel_SelectedTag;
            // 
            // tabSwitch
            // 
            this.tabSwitch.AllowDrop = true;
            this.tabSwitch.BackColor = Color.Black;
            this.tabSwitch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSwitch.Location = new Point(0, 0);
            this.tabSwitch.Name = "tabSwitch";
            this.tabSwitch.Size = new Size(746, 466);
            this.tabSwitch.TabIndex = 4;
            this.tabSwitch.Text = "tabSwitch1";
            this.tabSwitch.ActiveTabChanged += this.TabSwitch_ActiveTabChanged;
            this.tabSwitch.TabCloseButtonClick += this.TabSwitch_TabCloseButtonClick;
            this.tabSwitch.TabDropouted += this.TabSwitch_TabDropouted;
            this.tabSwitch.BackgroundMouseDoubleLeftClick += this.TabSwitch_BackgroundMouseDoubleLeftClick;
            this.tabSwitch.TabAreaDragOver += this.TabSwitch_TabAreaDragOver;
            this.tabSwitch.TabAreaDragDrop += this.TabSwitch_TabAreaDragDrop;
            this.tabSwitch.AddTabButtonMouseClick += this.TabSwitch_AddTabButtonMouseClick;
            // 
            // toolPanel
            // 
            this.toolPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.toolPanel.BackColor = Color.FromArgb(241, 244, 250);
            this.toolPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolPanel.Controls.Add(this.searchBookmarkToolButton);
            this.toolPanel.Controls.Add(this.searchClipToolButton);
            this.toolPanel.Controls.Add(this.reloadToolButton);
            this.toolPanel.Controls.Add(this.tagDropToolButton);
            this.toolPanel.Controls.Add(this.homeToolButton);
            this.toolPanel.Controls.Add(this.nextPageHistoryButton);
            this.toolPanel.Controls.Add(this.searchRatingToolButton);
            this.toolPanel.Controls.Add(this.previewPageHistoryButton);
            this.toolPanel.Controls.Add(this.showInfoToolButton);
            this.toolPanel.Controls.Add(this.addressBar);
            this.toolPanel.Location = new Point(0, 29);
            this.toolPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolPanel.MaximumSize = new Size(0, 34);
            this.toolPanel.MinimumSize = new Size(0, 34);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new Size(746, 34);
            this.toolPanel.TabIndex = 5;
            // 
            // searchBookmarkToolButton
            // 
            this.searchBookmarkToolButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.searchBookmarkToolButton.FlatAppearance.BorderSize = 0;
            this.searchBookmarkToolButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.searchBookmarkToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchBookmarkToolButton.Image = (Image)resources.GetObject("searchBookmarkToolButton.Image");
            this.searchBookmarkToolButton.Location = new Point(644, 3);
            this.searchBookmarkToolButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.searchBookmarkToolButton.Name = "searchBookmarkToolButton";
            this.searchBookmarkToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.searchBookmarkToolButton.Size = new Size(32, 28);
            this.searchBookmarkToolButton.TabIndex = 11;
            this.searchBookmarkToolButton.UseVisualStyleBackColor = true;
            this.searchBookmarkToolButton.MouseClick += this.SearchBookmarkToolButton_MouseClick;
            // 
            // clipToolButton
            // 
            this.searchClipToolButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.searchClipToolButton.FlatAppearance.BorderSize = 0;
            this.searchClipToolButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.searchClipToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchClipToolButton.Image = (Image)resources.GetObject("clipToolButton.Image");
            this.searchClipToolButton.Location = new Point(678, 3);
            this.searchClipToolButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.searchClipToolButton.Name = "clipToolButton";
            this.searchClipToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.searchClipToolButton.Size = new Size(32, 28);
            this.searchClipToolButton.TabIndex = 11;
            this.searchClipToolButton.UseVisualStyleBackColor = true;
            this.searchClipToolButton.MouseClick += this.SearchClipToolButton_MouseClick;
            // 
            // reloadToolButton
            // 
            this.reloadToolButton.FlatAppearance.BorderSize = 0;
            this.reloadToolButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.reloadToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reloadToolButton.Image = (Image)resources.GetObject("reloadToolButton.Image");
            this.reloadToolButton.Location = new Point(76, 3);
            this.reloadToolButton.Name = "reloadToolButton";
            this.reloadToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.reloadToolButton.Size = new Size(32, 28);
            this.reloadToolButton.TabIndex = 10;
            this.reloadToolButton.UseVisualStyleBackColor = true;
            this.reloadToolButton.MouseClick += this.ReloadToolButton_MouseClick;
            // 
            // tagDropToolButton
            // 
            this.tagDropToolButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.tagDropToolButton.BackColor = Color.FromArgb(241, 244, 250);
            this.tagDropToolButton.FlatAppearance.BorderSize = 0;
            this.tagDropToolButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.tagDropToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tagDropToolButton.Icon = (Image)resources.GetObject("tagDropToolButton.Icon");
            this.tagDropToolButton.Image = (Image)resources.GetObject("tagDropToolButton.Image");
            this.tagDropToolButton.Location = new Point(576, 3);
            this.tagDropToolButton.Margin = new System.Windows.Forms.Padding(2);
            this.tagDropToolButton.Name = "tagDropToolButton";
            this.tagDropToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.tagDropToolButton.SelectedItem = null;
            this.tagDropToolButton.Size = new Size(32, 28);
            this.tagDropToolButton.TabIndex = 9;
            this.tagDropToolButton.UseVisualStyleBackColor = true;
            this.tagDropToolButton.ItemMouseClick += this.TagDropToolButton_ItemMouseClick;
            this.tagDropToolButton.DropDownOpening += this.TagDropToolButton_DropDownOpening;
            // 
            // homeToolButton
            // 
            this.homeToolButton.FlatAppearance.BorderSize = 0;
            this.homeToolButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.homeToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.homeToolButton.Image = (Image)resources.GetObject("homeToolButton.Image");
            this.homeToolButton.Location = new Point(114, 3);
            this.homeToolButton.Name = "homeToolButton";
            this.homeToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.homeToolButton.Size = new Size(32, 28);
            this.homeToolButton.TabIndex = 6;
            this.homeToolButton.UseVisualStyleBackColor = true;
            this.homeToolButton.MouseClick += this.HomeToolButton_MouseClick;
            // 
            // nextPageHistoryButton
            // 
            this.nextPageHistoryButton.Enabled = false;
            this.nextPageHistoryButton.FlatAppearance.BorderSize = 0;
            this.nextPageHistoryButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.nextPageHistoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextPageHistoryButton.Image = (Image)resources.GetObject("nextPageHistoryButton.Image");
            this.nextPageHistoryButton.Location = new Point(38, 3);
            this.nextPageHistoryButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.nextPageHistoryButton.Name = "nextPageHistoryButton";
            this.nextPageHistoryButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Right;
            this.nextPageHistoryButton.Size = new Size(32, 28);
            this.nextPageHistoryButton.TabIndex = 5;
            this.nextPageHistoryButton.UseVisualStyleBackColor = true;
            this.nextPageHistoryButton.MouseClick += this.NextPageHistoryButton_MouseClick;
            // 
            // searchRatingToolButton
            // 
            this.searchRatingToolButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.searchRatingToolButton.FlatAppearance.BorderSize = 0;
            this.searchRatingToolButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.searchRatingToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchRatingToolButton.Image = (Image)resources.GetObject("searchRatingToolButton.Image");
            this.searchRatingToolButton.Location = new Point(610, 3);
            this.searchRatingToolButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.searchRatingToolButton.Name = "searchRatingToolButton";
            this.searchRatingToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.searchRatingToolButton.Size = new Size(32, 28);
            this.searchRatingToolButton.TabIndex = 8;
            this.searchRatingToolButton.UseVisualStyleBackColor = true;
            this.searchRatingToolButton.MouseClick += this.SearchRatingToolButton_MouseClick;
            // 
            // previewPageHistoryButton
            // 
            this.previewPageHistoryButton.Enabled = false;
            this.previewPageHistoryButton.FlatAppearance.BorderSize = 0;
            this.previewPageHistoryButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.previewPageHistoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewPageHistoryButton.Image = (Image)resources.GetObject("previewPageHistoryButton.Image");
            this.previewPageHistoryButton.Location = new Point(6, 3);
            this.previewPageHistoryButton.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.previewPageHistoryButton.Name = "previewPageHistoryButton";
            this.previewPageHistoryButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Left;
            this.previewPageHistoryButton.Size = new Size(32, 28);
            this.previewPageHistoryButton.TabIndex = 0;
            this.previewPageHistoryButton.UseVisualStyleBackColor = true;
            this.previewPageHistoryButton.MouseClick += this.PreviewPageHistoryButton_MouseClick;
            // 
            // showInfoToolButton
            // 
            this.showInfoToolButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.showInfoToolButton.FlatAppearance.BorderSize = 0;
            this.showInfoToolButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.showInfoToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showInfoToolButton.Image = (Image)resources.GetObject("showInfoToolButton.Image");
            this.showInfoToolButton.Location = new Point(712, 3);
            this.showInfoToolButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.showInfoToolButton.Name = "showInfoToolButton";
            this.showInfoToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.showInfoToolButton.Size = new Size(32, 28);
            this.showInfoToolButton.TabIndex = 5;
            this.showInfoToolButton.UseVisualStyleBackColor = true;
            this.showInfoToolButton.MouseClick += this.ShowInfoToolButton_MouseClick;
            // 
            // addressBar
            // 
            this.addressBar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.addressBar.BackColor = Color.FromArgb(124, 138, 153);
            this.addressBar.Location = new Point(152, 4);
            this.addressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new Size(419, 26);
            this.addressBar.TabIndex = 0;
            this.addressBar.TextAlignment = StringAlignment.Center;
            this.addressBar.TextLineAlignment = StringAlignment.Center;
            this.addressBar.TextTrimming = StringTrimming.EllipsisCharacter;
            this.addressBar.SelectedDirectory += this.AddressBar_SelectedDirectory;
            // 
            // BrowserMainPanel
            // 
            this.BackColor = Color.FromArgb(241, 244, 250);
            this.Controls.Add(this.toolPanel);
            this.Controls.Add(this.tabSwitch);
            this.Name = "BrowserMainPanel";
            this.Size = new Size(746, 466);
            this.toolPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.TabOperation.PageContainer pageContainer;
        private PicSum.UIComponent.InfoPanel.InfoPanel infoPanel;
        private SWF.UIComponent.TabOperation.TabSwitch tabSwitch;
        private System.Windows.Forms.Panel toolPanel;
        private SWF.UIComponent.Core.ToolButton showInfoToolButton;
        private PicSum.UIComponent.AddressBar.AddressBar addressBar;
        private SWF.UIComponent.Core.ToolButton nextPageHistoryButton;
        private SWF.UIComponent.Core.ToolButton previewPageHistoryButton;
        private SWF.UIComponent.Core.ToolButton homeToolButton;
        private SWF.UIComponent.Core.ToolButton searchRatingToolButton;
        private SWF.UIComponent.WideDropDown.WideDropToolButton tagDropToolButton;
        private SWF.UIComponent.Core.ToolButton reloadToolButton;
        private SWF.UIComponent.Core.ToolButton searchBookmarkToolButton;
        private SWF.UIComponent.Core.ToolButton searchClipToolButton;

        #endregion
    }
}
