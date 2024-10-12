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
            this.components = new System.ComponentModel.Container();
            this.pageContainer = new SWF.UIComponent.TabOperation.PageContainer();
            this.infoPanel = new PicSum.UIComponent.InfoPanel.InfoPanel();
            this.tabSwitch = new SWF.UIComponent.TabOperation.TabSwitch();
            this.toolPanel = new System.Windows.Forms.Panel();
            this.searchBookmarkToolButton = new SWF.UIComponent.Core.ToolButton();
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
            this.pageContainer.Location = new System.Drawing.Point(0, 0);
            this.pageContainer.Name = "pageContainer";
            this.pageContainer.Size = new System.Drawing.Size(746, 402);
            this.pageContainer.TabIndex = 1;
            this.pageContainer.DragDrop += new System.Windows.Forms.DragEventHandler(this.PageContainer_DragDrop);
            this.pageContainer.DragEnter += new System.Windows.Forms.DragEventHandler(this.PageContainer_DragEnter);
            // 
            // infoPanel
            // 
            this.infoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoPanel.Font = new System.Drawing.Font("Yu Gothic UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.infoPanel.Location = new System.Drawing.Point(0, 0);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(96, 100);
            this.infoPanel.TabIndex = 0;
            this.infoPanel.SelectedTag += new System.EventHandler<PicSum.UIComponent.InfoPanel.SelectedTagEventArgs>(this.InfoPanel_SelectedTag);
            // 
            // tabSwitch
            // 
            this.tabSwitch.AllowDrop = true;
            this.tabSwitch.BackColor = System.Drawing.Color.Black;
            this.tabSwitch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSwitch.Location = new System.Drawing.Point(0, 0);
            this.tabSwitch.Name = "tabSwitch";
            this.tabSwitch.Size = new System.Drawing.Size(746, 466);
            this.tabSwitch.TabIndex = 4;
            this.tabSwitch.TabsRightOffset = 144;
            this.tabSwitch.Text = "tabSwitch1";
            this.tabSwitch.ActiveTabChanged += new System.EventHandler(this.TabSwitch_ActiveTabChanged);
            this.tabSwitch.TabCloseButtonClick += new System.EventHandler<SWF.UIComponent.TabOperation.TabEventArgs>(this.TabSwitch_TabCloseButtonClick);
            this.tabSwitch.TabDropouted += new System.EventHandler<SWF.UIComponent.TabOperation.TabDropoutedEventArgs>(this.TabSwitch_TabDropouted);
            this.tabSwitch.BackgroundMouseDoubleLeftClick += new System.EventHandler(this.TabSwitch_BackgroundMouseDoubleLeftClick);
            this.tabSwitch.TabAreaDragOver += new System.EventHandler<System.Windows.Forms.DragEventArgs>(this.TabSwitch_TabAreaDragOver);
            this.tabSwitch.TabAreaDragDrop += new System.EventHandler<SWF.UIComponent.TabOperation.TabAreaDragEventArgs>(this.TabSwitch_TabAreaDragDrop);
            this.tabSwitch.AddTabButtonMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.TabSwitch_AddTabButtonMouseClick);
            // 
            // toolPanel
            // 
            this.toolPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.toolPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolPanel.Controls.Add(this.searchBookmarkToolButton);
            this.toolPanel.Controls.Add(this.reloadToolButton);
            this.toolPanel.Controls.Add(this.tagDropToolButton);
            this.toolPanel.Controls.Add(this.homeToolButton);
            this.toolPanel.Controls.Add(this.nextPageHistoryButton);
            this.toolPanel.Controls.Add(this.searchRatingToolButton);
            this.toolPanel.Controls.Add(this.previewPageHistoryButton);
            this.toolPanel.Controls.Add(this.showInfoToolButton);
            this.toolPanel.Controls.Add(this.addressBar);
            this.toolPanel.Location = new System.Drawing.Point(0, 29);
            this.toolPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolPanel.MaximumSize = new System.Drawing.Size(0, 34);
            this.toolPanel.MinimumSize = new System.Drawing.Size(0, 34);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(746, 34);
            this.toolPanel.TabIndex = 5;
            // 
            // searchBookmarkToolButton
            // 
            this.searchBookmarkToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBookmarkToolButton.FlatAppearance.BorderSize = 0;
            this.searchBookmarkToolButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.searchBookmarkToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchBookmarkToolButton.Image = global::PicSum.Main.Properties.Resources.BookmarkIcon;
            this.searchBookmarkToolButton.Location = new System.Drawing.Point(670, 3);
            this.searchBookmarkToolButton.Name = "searchBookmarkToolButton";
            this.searchBookmarkToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.searchBookmarkToolButton.Size = new System.Drawing.Size(32, 28);
            this.searchBookmarkToolButton.TabIndex = 11;
            this.searchBookmarkToolButton.UseVisualStyleBackColor = true;
            this.searchBookmarkToolButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SearchBookmarkToolButton_MouseClick);
            // 
            // reloadToolButton
            // 
            this.reloadToolButton.FlatAppearance.BorderSize = 0;
            this.reloadToolButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.reloadToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reloadToolButton.Image = global::PicSum.Main.Properties.Resources.ReloadIcon;
            this.reloadToolButton.Location = new System.Drawing.Point(76, 3);
            this.reloadToolButton.Name = "reloadToolButton";
            this.reloadToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.reloadToolButton.Size = new System.Drawing.Size(32, 28);
            this.reloadToolButton.TabIndex = 10;
            this.reloadToolButton.UseVisualStyleBackColor = true;
            this.reloadToolButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ReloadToolButton_MouseClick);
            // 
            // tagDropToolButton
            // 
            this.tagDropToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tagDropToolButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.tagDropToolButton.FlatAppearance.BorderSize = 0;
            this.tagDropToolButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.tagDropToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tagDropToolButton.Icon = global::PicSum.Main.Properties.Resources.TagIcon;
            this.tagDropToolButton.Image = global::PicSum.Main.Properties.Resources.TagIcon;
            this.tagDropToolButton.Location = new System.Drawing.Point(594, 3);
            this.tagDropToolButton.Name = "tagDropToolButton";
            this.tagDropToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.tagDropToolButton.SelectedItem = null;
            this.tagDropToolButton.Size = new System.Drawing.Size(32, 28);
            this.tagDropToolButton.TabIndex = 9;
            this.tagDropToolButton.UseVisualStyleBackColor = true;
            this.tagDropToolButton.ItemMouseClick += new System.EventHandler<SWF.UIComponent.WideDropDown.ItemMouseClickEventArgs>(this.TagDropToolButton_ItemMouseClick);
            this.tagDropToolButton.DropDownOpening += new System.EventHandler<SWF.UIComponent.WideDropDown.DropDownOpeningEventArgs>(this.TagDropToolButton_DropDownOpening);
            // 
            // homeToolButton
            // 
            this.homeToolButton.FlatAppearance.BorderSize = 0;
            this.homeToolButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.homeToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.homeToolButton.Image = global::PicSum.Main.Properties.Resources.HomeIcon;
            this.homeToolButton.Location = new System.Drawing.Point(114, 3);
            this.homeToolButton.Name = "homeToolButton";
            this.homeToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.homeToolButton.Size = new System.Drawing.Size(32, 28);
            this.homeToolButton.TabIndex = 6;
            this.homeToolButton.UseVisualStyleBackColor = true;
            this.homeToolButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HomeToolButton_MouseClick);
            // 
            // nextPageHistoryButton
            // 
            this.nextPageHistoryButton.Enabled = false;
            this.nextPageHistoryButton.FlatAppearance.BorderSize = 0;
            this.nextPageHistoryButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.nextPageHistoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextPageHistoryButton.Image = global::PicSum.Main.Properties.Resources.GoNextIcon;
            this.nextPageHistoryButton.Location = new System.Drawing.Point(38, 3);
            this.nextPageHistoryButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.nextPageHistoryButton.Name = "nextPageHistoryButton";
            this.nextPageHistoryButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Right;
            this.nextPageHistoryButton.Size = new System.Drawing.Size(32, 28);
            this.nextPageHistoryButton.TabIndex = 5;
            this.nextPageHistoryButton.UseVisualStyleBackColor = true;
            this.nextPageHistoryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NextPageHistoryButton_MouseClick);
            // 
            // searchRatingToolButton
            // 
            this.searchRatingToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchRatingToolButton.FlatAppearance.BorderSize = 0;
            this.searchRatingToolButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.searchRatingToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchRatingToolButton.Image = global::PicSum.Main.Properties.Resources.ActiveRatingIcon;
            this.searchRatingToolButton.Location = new System.Drawing.Point(632, 3);
            this.searchRatingToolButton.Name = "searchRatingToolButton";
            this.searchRatingToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.searchRatingToolButton.Size = new System.Drawing.Size(32, 28);
            this.searchRatingToolButton.TabIndex = 8;
            this.searchRatingToolButton.UseVisualStyleBackColor = true;
            this.searchRatingToolButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SearchRatingToolButton_MouseClick);
            // 
            // previewPageHistoryButton
            // 
            this.previewPageHistoryButton.Enabled = false;
            this.previewPageHistoryButton.FlatAppearance.BorderSize = 0;
            this.previewPageHistoryButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.previewPageHistoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewPageHistoryButton.Image = global::PicSum.Main.Properties.Resources.GoBackIcon;
            this.previewPageHistoryButton.Location = new System.Drawing.Point(6, 3);
            this.previewPageHistoryButton.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.previewPageHistoryButton.Name = "previewPageHistoryButton";
            this.previewPageHistoryButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Left;
            this.previewPageHistoryButton.Size = new System.Drawing.Size(32, 28);
            this.previewPageHistoryButton.TabIndex = 0;
            this.previewPageHistoryButton.UseVisualStyleBackColor = true;
            this.previewPageHistoryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PreviewPageHistoryButton_MouseClick);
            // 
            // showInfoToolButton
            // 
            this.showInfoToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showInfoToolButton.FlatAppearance.BorderSize = 0;
            this.showInfoToolButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.showInfoToolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showInfoToolButton.Image = global::PicSum.Main.Properties.Resources.InfoIcon;
            this.showInfoToolButton.Location = new System.Drawing.Point(708, 3);
            this.showInfoToolButton.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.showInfoToolButton.Name = "showInfoToolButton";
            this.showInfoToolButton.RegionType = SWF.UIComponent.Core.ToolButton.ToolButtonRegionType.Default;
            this.showInfoToolButton.Size = new System.Drawing.Size(32, 28);
            this.showInfoToolButton.TabIndex = 5;
            this.showInfoToolButton.UseVisualStyleBackColor = true;
            this.showInfoToolButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ShowInfoToolButton_MouseClick);
            // 
            // addressBar
            // 
            this.addressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(138)))), ((int)(((byte)(153)))));
            this.addressBar.Location = new System.Drawing.Point(152, 4);
            this.addressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new System.Drawing.Size(436, 26);
            this.addressBar.TabIndex = 0;
            this.addressBar.TextAlignment = System.Drawing.StringAlignment.Center;
            this.addressBar.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.addressBar.TextTrimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.addressBar.SelectedDirectory += new System.EventHandler<PicSum.UIComponent.AddressBar.SelectedDirectoryEventArgs>(this.AddressBar_SelectedDirectory);
            // 
            // BrowserMainPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.toolPanel);
            this.Controls.Add(this.tabSwitch);
            this.Name = "BrowserMainPanel";
            this.Size = new System.Drawing.Size(746, 466);
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

        #endregion
    }
}
