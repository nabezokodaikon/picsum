namespace PicSum.Main.UIComponent
{
    partial class BrowserMainPanel
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.contentsContainer = new SWF.UIComponent.TabOperation.ContentsContainer();
            this.infoPanel = new PicSum.UIComponent.InfoPanel.InfoPanel();
            this.tabSwitch = new SWF.UIComponent.TabOperation.TabSwitch();
            this.toolPanel = new System.Windows.Forms.Panel();
            this.keepToolButton = new SWF.UIComponent.Common.ToolButton();
            this.searchTagToolButton = new PicSum.UIComponent.SearchTool.SearchTagToolButton();
            this.searchRatingToolButton = new PicSum.UIComponent.SearchTool.SearchRatingToolButton();
            this.previewContentsHistoryButton = new SWF.UIComponent.Common.ToolButton();
            this.nextContentsHistoryButton = new SWF.UIComponent.Common.ToolButton();
            this.showInfoToolButton = new SWF.UIComponent.Common.ToolButton();
            this.addressBar = new PicSum.UIComponent.AddressBar.AddressBar();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 64);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.contentsContainer);
            this.splitContainer.Panel1MinSize = 320;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.infoPanel);
            this.splitContainer.Panel2Collapsed = true;
            this.splitContainer.Size = new System.Drawing.Size(746, 402);
            this.splitContainer.SplitterDistance = 320;
            this.splitContainer.SplitterWidth = 6;
            this.splitContainer.TabIndex = 3;
            // 
            // contentsContainer
            // 
            this.contentsContainer.AllowDrop = true;
            this.contentsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentsContainer.Location = new System.Drawing.Point(0, 0);
            this.contentsContainer.Name = "contentsContainer";
            this.contentsContainer.Size = new System.Drawing.Size(746, 402);
            this.contentsContainer.TabIndex = 1;
            this.contentsContainer.DragDrop += new System.Windows.Forms.DragEventHandler(this.contentsContainer_DragDrop);
            this.contentsContainer.DragEnter += new System.Windows.Forms.DragEventHandler(this.contentsContainer_DragEnter);
            // 
            // infoPanel
            // 
            this.infoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoPanel.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.infoPanel.Location = new System.Drawing.Point(0, 0);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(96, 100);
            this.infoPanel.TabIndex = 0;
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
            this.tabSwitch.TabsRightOffset = 100;
            this.tabSwitch.Text = "tabSwitch1";
            this.tabSwitch.ActiveTabChanged += new System.EventHandler(this.tabSwitch_ActiveTabChanged);
            this.tabSwitch.TabCloseButtonClick += new System.EventHandler<SWF.UIComponent.TabOperation.TabEventArgs>(this.tabSwitch_TabCloseButtonClick);
            this.tabSwitch.TabDropouted += new System.EventHandler<SWF.UIComponent.TabOperation.TabDropoutedEventArgs>(this.tabSwitch_TabDropouted);
            this.tabSwitch.BackgroundMouseDoubleLeftClick += new System.EventHandler(this.tabSwitch_BackgroundMouseDoubleLeftClick);
            this.tabSwitch.TabAreaDragOver += new System.EventHandler<System.Windows.Forms.DragEventArgs>(this.tabSwitch_TabAreaDragOver);
            this.tabSwitch.TabAreaDragDrop += new System.EventHandler<SWF.UIComponent.TabOperation.TabAreaDragEventArgs>(this.tabSwitch_TabAreaDragDrop);
            this.tabSwitch.AddTabButtonMouseClick += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.tabSwitch_AddTabButtonMouseClick);
            // 
            // toolPanel
            // 
            this.toolPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.toolPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolPanel.Controls.Add(this.keepToolButton);
            this.toolPanel.Controls.Add(this.searchTagToolButton);
            this.toolPanel.Controls.Add(this.searchRatingToolButton);
            this.toolPanel.Controls.Add(this.previewContentsHistoryButton);
            this.toolPanel.Controls.Add(this.nextContentsHistoryButton);
            this.toolPanel.Controls.Add(this.showInfoToolButton);
            this.toolPanel.Controls.Add(this.addressBar);
            this.toolPanel.Location = new System.Drawing.Point(0, 30);
            this.toolPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolPanel.MaximumSize = new System.Drawing.Size(0, 34);
            this.toolPanel.MinimumSize = new System.Drawing.Size(0, 34);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(746, 34);
            this.toolPanel.TabIndex = 5;
            // 
            // keepToolButton
            // 
            this.keepToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.keepToolButton.Image = global::PicSum.Main.Properties.Resources.KeepIcon;
            this.keepToolButton.Location = new System.Drawing.Point(670, 2);
            this.keepToolButton.Name = "keepToolButton";
            this.keepToolButton.RegionType = SWF.UIComponent.Common.ToolButton.ToolButtonRegionType.Default;
            this.keepToolButton.Size = new System.Drawing.Size(32, 28);
            this.keepToolButton.TabIndex = 0;
            this.keepToolButton.UseVisualStyleBackColor = true;
            this.keepToolButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.keepToolButton_MouseClick);
            // 
            // searchTagToolButton
            // 
            this.searchTagToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTagToolButton.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.searchTagToolButton.Image = global::PicSum.Main.Properties.Resources.TagIcon;
            this.searchTagToolButton.Location = new System.Drawing.Point(610, 2);
            this.searchTagToolButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.searchTagToolButton.Name = "searchTagToolButton";
            this.searchTagToolButton.RegionType = SWF.UIComponent.Common.ToolButton.ToolButtonRegionType.Left;
            this.searchTagToolButton.Size = new System.Drawing.Size(32, 28);
            this.searchTagToolButton.TabIndex = 0;
            this.searchTagToolButton.UseVisualStyleBackColor = true;
            this.searchTagToolButton.SelectedTag += new System.EventHandler<PicSum.UIComponent.SearchTool.SelectedTagEventArgs>(this.searchTagToolButton_SelectedTag);
            // 
            // searchRatingToolButton
            // 
            this.searchRatingToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchRatingToolButton.Image = global::PicSum.Main.Properties.Resources.ActiveRatingIcon;
            this.searchRatingToolButton.Location = new System.Drawing.Point(635, 2);
            this.searchRatingToolButton.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.searchRatingToolButton.Name = "searchRatingToolButton";
            this.searchRatingToolButton.RegionType = SWF.UIComponent.Common.ToolButton.ToolButtonRegionType.Right;
            this.searchRatingToolButton.Size = new System.Drawing.Size(32, 28);
            this.searchRatingToolButton.TabIndex = 0;
            this.searchRatingToolButton.UseVisualStyleBackColor = true;
            this.searchRatingToolButton.SelectedRating += new System.EventHandler<PicSum.UIComponent.SearchTool.SelectedRatingEventArgs>(this.searchRatingToolButton_SelectedRating);
            // 
            // previewContentsHistoryButton
            // 
            this.previewContentsHistoryButton.Enabled = false;
            this.previewContentsHistoryButton.Image = global::PicSum.Main.Properties.Resources.MiddleArrowLeft;
            this.previewContentsHistoryButton.Location = new System.Drawing.Point(6, 3);
            this.previewContentsHistoryButton.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.previewContentsHistoryButton.Name = "previewContentsHistoryButton";
            this.previewContentsHistoryButton.RegionType = SWF.UIComponent.Common.ToolButton.ToolButtonRegionType.Left;
            this.previewContentsHistoryButton.Size = new System.Drawing.Size(40, 28);
            this.previewContentsHistoryButton.TabIndex = 0;
            this.previewContentsHistoryButton.UseVisualStyleBackColor = true;
            this.previewContentsHistoryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.previewContentsHistoryButton_MouseClick);
            // 
            // nextContentsHistoryButton
            // 
            this.nextContentsHistoryButton.Enabled = false;
            this.nextContentsHistoryButton.Image = global::PicSum.Main.Properties.Resources.MiddleArrowRight;
            this.nextContentsHistoryButton.Location = new System.Drawing.Point(38, 3);
            this.nextContentsHistoryButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.nextContentsHistoryButton.Name = "nextContentsHistoryButton";
            this.nextContentsHistoryButton.RegionType = SWF.UIComponent.Common.ToolButton.ToolButtonRegionType.Right;
            this.nextContentsHistoryButton.Size = new System.Drawing.Size(40, 28);
            this.nextContentsHistoryButton.TabIndex = 5;
            this.nextContentsHistoryButton.UseVisualStyleBackColor = true;
            this.nextContentsHistoryButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.nextContentsHistoryButton_MouseClick);
            // 
            // showInfoToolButton
            // 
            this.showInfoToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showInfoToolButton.Image = global::PicSum.Main.Properties.Resources.FileInfoIcon;
            this.showInfoToolButton.Location = new System.Drawing.Point(708, 3);
            this.showInfoToolButton.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.showInfoToolButton.Name = "showInfoToolButton";
            this.showInfoToolButton.RegionType = SWF.UIComponent.Common.ToolButton.ToolButtonRegionType.Default;
            this.showInfoToolButton.Size = new System.Drawing.Size(32, 28);
            this.showInfoToolButton.TabIndex = 5;
            this.showInfoToolButton.UseVisualStyleBackColor = true;
            this.showInfoToolButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.showInfoToolButton_MouseClick);
            // 
            // addressBar
            // 
            this.addressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(138)))), ((int)(((byte)(153)))));
            this.addressBar.InnerColor = System.Drawing.Color.White;
            this.addressBar.Location = new System.Drawing.Point(84, 4);
            this.addressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.addressBar.MouseDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(48)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.addressBar.MousePointColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.addressBar.Name = "addressBar";
            this.addressBar.OutlineColor = System.Drawing.Color.Silver;
            this.addressBar.Size = new System.Drawing.Size(520, 26);
            this.addressBar.TabIndex = 0;
            this.addressBar.TextAlignment = System.Drawing.StringAlignment.Center;
            this.addressBar.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.addressBar.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.addressBar.TextTrimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.addressBar.SelectedFolder += new System.EventHandler<PicSum.UIComponent.AddressBar.SelectedFolderEventArgs>(this.addressBar_SelectedFolder);
            // 
            // BrowserMainPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.toolPanel);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.tabSwitch);
            this.Name = "BrowserMainPanel";
            this.Size = new System.Drawing.Size(746, 466);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.toolPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private SWF.UIComponent.TabOperation.ContentsContainer contentsContainer;
        private PicSum.UIComponent.InfoPanel.InfoPanel infoPanel;
        private SWF.UIComponent.TabOperation.TabSwitch tabSwitch;
        private System.Windows.Forms.Panel toolPanel;
        private SWF.UIComponent.Common.ToolButton showInfoToolButton;
        private PicSum.UIComponent.AddressBar.AddressBar addressBar;
        private SWF.UIComponent.Common.ToolButton nextContentsHistoryButton;
        private SWF.UIComponent.Common.ToolButton previewContentsHistoryButton;
        private PicSum.UIComponent.SearchTool.SearchRatingToolButton searchRatingToolButton;
        private PicSum.UIComponent.SearchTool.SearchTagToolButton searchTagToolButton;
        private SWF.UIComponent.Common.ToolButton keepToolButton;

    }
}
