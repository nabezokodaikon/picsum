using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.Storage;

namespace PicSum.Main.UIComponent
{
    partial class BrowseMainPanel
    {
        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // pageContainer
            // 
            this.pageContainer.AllowDrop = true;
            this.pageContainer.BackColor = Color.FromArgb(250, 250, 250);
            this.pageContainer.Location = new Point(500, 0);
            this.pageContainer.Name = "pageContainer";
            this.pageContainer.Size = new Size(746, 402);
            this.pageContainer.DragDrop += this.PageContainer_DragDrop;
            this.pageContainer.DragEnter += this.PageContainer_DragEnter;
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = Color.FromArgb(250, 250, 250);
            this.infoPanel.Font = new Font("Yu Gothic UI", 9F);
            this.infoPanel.Location = new Point(0, 0);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new Size(96, 100);
            this.infoPanel.SelectedTag += this.InfoPanel_SelectedTag;
            // 
            // tabSwitch
            // 
            this.tabSwitch.AllowDrop = true;
            this.tabSwitch.BackColor = Color.Black;
            this.tabSwitch.Location = new Point(0, 0);
            this.tabSwitch.Name = "tabSwitch";
            this.tabSwitch.Size = new Size(746, 29);
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
            this.toolPanel.BackColor = Color.FromArgb(250, 250, 250);
            this.toolPanel.Controls.Add(this.reloadToolButton);
            this.toolPanel.Controls.Add(this.nextPageHistoryButton);
            this.toolPanel.Controls.Add(this.previewPageHistoryButton);
            this.toolPanel.Controls.Add(this.showInfoToolButton);
            this.toolPanel.Controls.Add(this.addressBar);
            this.toolPanel.Location = new Point(0, 29);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new Size(746, 34);
            // 
            // reloadToolButton
            // 
            this.reloadToolButton.Location = new Point(73, 3);
            this.reloadToolButton.Name = "reloadToolButton";
            this.reloadToolButton.Size = new Size(32, 28);
            this.reloadToolButton.MouseClick += this.ReloadToolButton_MouseClick;
            // 
            // nextPageHistoryButton
            // 
            this.nextPageHistoryButton.Enabled = false;
            this.nextPageHistoryButton.Location = new Point(38, 3);
            this.nextPageHistoryButton.Name = "nextPageHistoryButton";
            this.nextPageHistoryButton.Size = new Size(32, 28);
            this.nextPageHistoryButton.MouseClick += this.NextPageHistoryButton_MouseClick;
            // 
            // previewPageHistoryButton
            // 
            this.previewPageHistoryButton.Enabled = false;
            this.previewPageHistoryButton.Location = new Point(3, 3);
            this.previewPageHistoryButton.Name = "previewPageHistoryButton";
            this.previewPageHistoryButton.Size = new Size(32, 28);
            this.previewPageHistoryButton.MouseClick += this.PreviewPageHistoryButton_MouseClick;
            // 
            // showInfoToolButton
            // 
            this.showInfoToolButton.Location = new Point(712, 3);
            this.showInfoToolButton.Name = "showInfoToolButton";
            this.showInfoToolButton.Size = new Size(32, 28);
            this.showInfoToolButton.MouseClick += this.ShowInfoToolButton_MouseClick;
            // 
            // addressBar
            //
            this.addressBar.BackColor = Color.FromArgb(124, 138, 153);
            this.addressBar.Location = new Point(108, 4);
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new Size(601, 26);
            this.addressBar.SelectedDirectory += this.AddressBar_SelectedDirectory;
            // 
            // searchBookmarkToolButton
            // 
            this.searchBookmarkToolButton.Location = new Point(3, 98);
            this.searchBookmarkToolButton.Name = "searchBookmarkToolButton";
            this.searchBookmarkToolButton.Size = new Size(32, 28);
            this.searchBookmarkToolButton.MouseClick += this.SearchBookmarkToolButton_MouseClick;
            // 
            // tagDropToolButton
            // 
            this.tagDropToolButton.Location = new Point(3, 36);
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
            this.searchRatingToolButton.Name = "searchRatingToolButton";
            this.searchRatingToolButton.Size = new Size(32, 28);
            this.searchRatingToolButton.MouseClick += this.SearchRatingToolButton_MouseClick;
            // 
            // toolPanel2
            // 
            this.toolPanel2.BackColor = Color.FromArgb(250, 250, 250);
            this.toolPanel2.Controls.Add(this.searchBookmarkToolButton);
            this.toolPanel2.Controls.Add(this.homeToolButton);
            this.toolPanel2.Controls.Add(this.tagDropToolButton);
            this.toolPanel2.Controls.Add(this.searchRatingToolButton);
            this.toolPanel2.Location = new Point(0, 63);
            this.toolPanel2.Name = "toolPanel2";
            this.toolPanel2.Size = new Size(38, 403);
            this.toolPanel2.IsDrawRightBorderLine = true;
            this.toolPanel2.VerticalTopMargin = 28;
            // 
            // BrowseMainPanel
            // 
            this.BackColor = Color.FromArgb(64, 68, 71);
            this.Name = "BrowseMainPanel";
            this.Size = new Size(746, 466);
            this.Load += this.BrowseMainPanel_Load;
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
