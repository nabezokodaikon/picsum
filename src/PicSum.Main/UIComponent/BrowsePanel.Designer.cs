using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.Storage;

namespace PicSum.Main.UIComponent
{
    partial class BrowsePanel
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
            this.pageContainer.DragDrop += this.PageContainer_DragDrop;
            this.pageContainer.DragEnter += this.PageContainer_DragEnter;
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = Color.FromArgb(250, 250, 250);
            this.infoPanel.Font = new Font("Yu Gothic UI", 9F);
            this.infoPanel.SelectedTag += this.InfoPanel_SelectedTag;
            // 
            // tabSwitch
            // 
            this.tabSwitch.AllowDrop = true;
            this.tabSwitch.BackColor = Color.Black;
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
            this.toolPanel.Controls.AddRange(
                this.reloadToolButton,
                this.nextPageHistoryButton,
                this.previewPageHistoryButton,
                this.showInfoToolButton,
                this.addressBar);
            // 
            // reloadToolButton
            // 
            this.reloadToolButton.MouseClick += this.ReloadToolButton_MouseClick;
            // 
            // nextPageHistoryButton
            // 
            this.nextPageHistoryButton.Enabled = false;
            this.nextPageHistoryButton.MouseClick += this.NextPageHistoryButton_MouseClick;
            // 
            // previewPageHistoryButton
            // 
            this.previewPageHistoryButton.Enabled = false;
            this.previewPageHistoryButton.MouseClick += this.PreviewPageHistoryButton_MouseClick;
            // 
            // showInfoToolButton
            // 
            this.showInfoToolButton.MouseClick += this.ShowInfoToolButton_MouseClick;
            // 
            // addressBar
            //
            this.addressBar.BackColor = Color.FromArgb(124, 138, 153);
            this.addressBar.SelectedDirectory += this.AddressBar_SelectedDirectory;
            // 
            // searchBookmarkToolButton
            // 
            this.searchBookmarkToolButton.MouseClick += this.SearchBookmarkToolButton_MouseClick;
            // 
            // tagDropToolButton
            // 
            this.tagDropToolButton.ItemMouseClick += this.TagDropToolButton_ItemMouseClick;
            this.tagDropToolButton.DropDownOpening += this.TagDropToolButton_DropDownOpening;
            // 
            // homeToolButton
            // 
            this.homeToolButton.MouseClick += this.HomeToolButton_MouseClick;
            // 
            // searchRatingToolButton
            // 
            this.searchRatingToolButton.MouseClick += this.SearchRatingToolButton_MouseClick;
            // 
            // toolPanel2
            // 
            this.toolPanel2.BackColor = Color.FromArgb(250, 250, 250);
            this.toolPanel2.Controls.AddRange(
                this.searchBookmarkToolButton,
                this.homeToolButton,
                this.tagDropToolButton,
                this.searchRatingToolButton);
            this.toolPanel2.IsDrawRightBorderLine = true;
            this.toolPanel2.VerticalTopMargin = 28;
            // 
            // BrowsePanel
            // 
            this.BackColor = Color.FromArgb(64, 68, 71);
            this.Load += this.BrowsePanel_Load;
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
