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
            this.toolPanel = new Panel();
            this.reloadToolButton = new SWF.UIComponent.Core.ToolIconButton();
            this.nextPageHistoryButton = new SWF.UIComponent.Core.ToolIconButton();
            this.previewPageHistoryButton = new SWF.UIComponent.Core.ToolIconButton();
            this.showInfoToolButton = new SWF.UIComponent.Core.ToolIconButton();
            this.addressBar = new PicSum.UIComponent.AddressBar.AddressBar();
            this.searchBookmarkToolButton = new SWF.UIComponent.Core.ToolIconButton();
            this.tagDropToolButton = new SWF.UIComponent.WideDropDown.WideDropToolButton();
            this.homeToolButton = new SWF.UIComponent.Core.ToolIconButton();
            this.searchRatingToolButton = new SWF.UIComponent.Core.ToolIconButton();
            this.toolPanel2 = new SWF.UIComponent.Core.ToolPanel();
            this.toolPanel.SuspendLayout();
            this.toolPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainer
            // 
            this.pageContainer.AllowDrop = true;
            this.pageContainer.BackColor = Color.Transparent;
            this.pageContainer.Location = new Point(500, 0);
            this.pageContainer.Name = "pageContainer";
            this.pageContainer.Size = new Size(746, 402);
            this.pageContainer.TabIndex = 1;
            this.pageContainer.DragDrop += this.PageContainer_DragDrop;
            this.pageContainer.DragEnter += this.PageContainer_DragEnter;
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = Color.Transparent;
            this.infoPanel.Font = new Font("Yu Gothic UI", 9F);
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
            this.tabSwitch.Location = new Point(0, 0);
            this.tabSwitch.Name = "tabSwitch";
            this.tabSwitch.Size = new Size(746, 29);
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
            this.toolPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.toolPanel.BackColor = Color.Transparent;
            this.toolPanel.BackgroundImageLayout = ImageLayout.Stretch;
            this.toolPanel.Controls.Add(this.reloadToolButton);
            this.toolPanel.Controls.Add(this.nextPageHistoryButton);
            this.toolPanel.Controls.Add(this.previewPageHistoryButton);
            this.toolPanel.Controls.Add(this.showInfoToolButton);
            this.toolPanel.Controls.Add(this.addressBar);
            this.toolPanel.Location = new Point(0, 29);
            this.toolPanel.Margin = new Padding(0);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new Size(746, 34);
            this.toolPanel.TabIndex = 5;
            // 
            // reloadToolButton
            // 
            this.reloadToolButton.Location = new Point(73, 3);
            this.reloadToolButton.Margin = new Padding(0);
            this.reloadToolButton.Name = "reloadToolButton";
            this.reloadToolButton.Size = new Size(32, 28);
            this.reloadToolButton.MouseClick += this.ReloadToolButton_MouseClick;
            // 
            // nextPageHistoryButton
            // 
            this.nextPageHistoryButton.Enabled = false;
            this.nextPageHistoryButton.Location = new Point(38, 3);
            this.nextPageHistoryButton.Margin = new Padding(0);
            this.nextPageHistoryButton.Name = "nextPageHistoryButton";
            this.nextPageHistoryButton.Size = new Size(32, 28);
            this.nextPageHistoryButton.MouseClick += this.NextPageHistoryButton_MouseClick;
            // 
            // previewPageHistoryButton
            // 
            this.previewPageHistoryButton.Enabled = false;
            this.previewPageHistoryButton.Location = new Point(3, 3);
            this.previewPageHistoryButton.Margin = new Padding(0);
            this.previewPageHistoryButton.Name = "previewPageHistoryButton";
            this.previewPageHistoryButton.Size = new Size(32, 28);
            this.previewPageHistoryButton.MouseClick += this.PreviewPageHistoryButton_MouseClick;
            // 
            // showInfoToolButton
            // 
            this.showInfoToolButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.showInfoToolButton.Location = new Point(712, 3);
            this.showInfoToolButton.Margin = new Padding(0);
            this.showInfoToolButton.Name = "showInfoToolButton";
            this.showInfoToolButton.Size = new Size(32, 28);
            this.showInfoToolButton.MouseClick += this.ShowInfoToolButton_MouseClick;
            // 
            // addressBar
            // 
            this.addressBar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.addressBar.BackColor = Color.FromArgb(124, 138, 153);
            this.addressBar.Location = new Point(108, 4);
            this.addressBar.Margin = new Padding(0);
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new Size(601, 26);
            this.addressBar.TabIndex = 0;
            this.addressBar.SelectedDirectory += this.AddressBar_SelectedDirectory;
            // 
            // searchBookmarkToolButton
            // 
            this.searchBookmarkToolButton.Location = new Point(3, 98);
            this.searchBookmarkToolButton.Margin = new Padding(3, 0, 3, 3);
            this.searchBookmarkToolButton.Name = "searchBookmarkToolButton";
            this.searchBookmarkToolButton.Size = new Size(32, 28);
            this.searchBookmarkToolButton.MouseClick += this.SearchBookmarkToolButton_MouseClick;
            // 
            // tagDropToolButton
            // 
            this.tagDropToolButton.Location = new Point(3, 36);
            this.tagDropToolButton.Margin = new Padding(3, 0, 3, 3);
            this.tagDropToolButton.Name = "tagDropToolButton";
            this.tagDropToolButton.Size = new Size(32, 28);
            this.tagDropToolButton.ItemMouseClick += this.TagDropToolButton_ItemMouseClick;
            this.tagDropToolButton.DropDownOpening += this.TagDropToolButton_DropDownOpening;
            // 
            // homeToolButton
            // 
            this.homeToolButton.Location = new Point(3, 5);
            this.homeToolButton.Name = "homeToolButton";
            this.homeToolButton.Size = new Size(32, 28);
            this.homeToolButton.MouseClick += this.HomeToolButton_MouseClick;
            // 
            // searchRatingToolButton
            // 
            this.searchRatingToolButton.Location = new Point(3, 67);
            this.searchRatingToolButton.Margin = new Padding(3, 0, 3, 3);
            this.searchRatingToolButton.Name = "searchRatingToolButton";
            this.searchRatingToolButton.Size = new Size(32, 28);
            this.searchRatingToolButton.MouseClick += this.SearchRatingToolButton_MouseClick;
            // 
            // toolPanel2
            // 
            this.toolPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.toolPanel2.BackColor = Color.Transparent;
            this.toolPanel2.Controls.Add(this.searchBookmarkToolButton);
            this.toolPanel2.Controls.Add(this.homeToolButton);
            this.toolPanel2.Controls.Add(this.tagDropToolButton);
            this.toolPanel2.Controls.Add(this.searchRatingToolButton);
            this.toolPanel2.Location = new Point(0, 63);
            this.toolPanel2.Margin = new Padding(0);
            this.toolPanel2.Name = "toolPanel2";
            this.toolPanel2.Size = new Size(38, 403);
            this.toolPanel2.TabIndex = 6;
            this.toolPanel2.IsDrawRightBorderLine = true;
            this.toolPanel2.VerticalTopMargin = 28;
            // 
            // BrowserMainPanel
            // 
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.Controls.Add(this.toolPanel2);
            this.Controls.Add(this.toolPanel);
            this.Controls.Add(this.tabSwitch);
            this.Name = "BrowserMainPanel";
            this.Size = new Size(746, 466);
            this.toolPanel.ResumeLayout(false);
            this.toolPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private SWF.UIComponent.TabOperation.PageContainer pageContainer;
        private PicSum.UIComponent.InfoPanel.InfoPanel infoPanel;
        private SWF.UIComponent.TabOperation.TabSwitch tabSwitch;
        private System.Windows.Forms.Panel toolPanel;
        private SWF.UIComponent.Core.ToolPanel toolPanel2;
        private SWF.UIComponent.Core.ToolIconButton showInfoToolButton;
        private PicSum.UIComponent.AddressBar.AddressBar addressBar;
        private SWF.UIComponent.Core.ToolIconButton nextPageHistoryButton;
        private SWF.UIComponent.Core.ToolIconButton previewPageHistoryButton;
        private SWF.UIComponent.Core.ToolIconButton homeToolButton;
        private SWF.UIComponent.Core.ToolIconButton searchRatingToolButton;
        private SWF.UIComponent.WideDropDown.WideDropToolButton tagDropToolButton;
        private SWF.UIComponent.Core.ToolIconButton reloadToolButton;
        private SWF.UIComponent.Core.ToolIconButton searchBookmarkToolButton;

        #endregion
    }
}
