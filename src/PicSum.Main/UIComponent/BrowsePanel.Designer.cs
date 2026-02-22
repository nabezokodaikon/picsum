using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
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
            this._pageContainer.AllowDrop = true;
            this._pageContainer.BackColor = Color.FromArgb(255, 64, 68, 71);
            this._pageContainer.DragDrop += this.PageContainer_DragDrop;
            this._pageContainer.DragEnter += this.PageContainer_DragEnter;
            // 
            // infoPanel
            // 
            this._infoPanel.BackColor = Color.FromArgb(250, 250, 250);
            this._infoPanel.SelectedTag += this.InfoPanel_SelectedTag;
            // 
            // tabSwitch
            // 
            this._tabSwitch.AllowDrop = true;
            this._tabSwitch.BackColor = Color.Black;
            this._tabSwitch.ActiveTabChanged += this.TabSwitch_ActiveTabChanged;
            this._tabSwitch.TabCloseButtonClick += this.TabSwitch_TabCloseButtonClick;
            this._tabSwitch.TabDropouted += this.TabSwitch_TabDropouted;
            this._tabSwitch.BackgroundMouseDoubleLeftClick += this.TabSwitch_BackgroundMouseDoubleLeftClick;
            this._tabSwitch.TabAreaDragOver += this.TabSwitch_TabAreaDragOver;
            this._tabSwitch.TabAreaDragDrop += this.TabSwitch_TabAreaDragDrop;
            this._tabSwitch.AddTabButtonMouseClick += this.TabSwitch_AddTabButtonMouseClick;
            // 
            // toolPanel
            // 
            this._toolPanel.BackColor = Color.FromArgb(250, 250, 250);
            this._toolPanel.Controls.AddRange(
                this._reloadToolButton,
                this._nextPageHistoryButton,
                this._previewPageHistoryButton,
                this._showInfoToolButton,
                this._addressBar);
            // 
            // reloadToolButton
            // 
            this._reloadToolButton.MouseClick += this.ReloadToolButton_MouseClick;
            // 
            // nextPageHistoryButton
            // 
            this._nextPageHistoryButton.Enabled = false;
            this._nextPageHistoryButton.MouseClick += this.NextPageHistoryButton_MouseClick;
            // 
            // previewPageHistoryButton
            // 
            this._previewPageHistoryButton.Enabled = false;
            this._previewPageHistoryButton.MouseClick += this.PreviewPageHistoryButton_MouseClick;
            // 
            // showInfoToolButton
            // 
            this._showInfoToolButton.MouseClick += this.ShowInfoToolButton_MouseClick;
            // 
            // addressBar
            //
            this._addressBar.BackColor = Color.FromArgb(124, 138, 153);
            this._addressBar.SelectedDirectory += this.AddressBar_SelectedDirectory;
            // 
            // searchBookmarkToolButton
            // 
            this._searchBookmarkToolButton.MouseClick += this.SearchBookmarkToolButton_MouseClick;
            // 
            // tagDropToolButton
            //
            this._tagDropToolButton.ItemMouseClick += this.TagDropToolButton_ItemMouseClick;
            this._tagDropToolButton.DropDownOpening += this.TagDropToolButton_DropDownOpening;
            // 
            // homeToolButton
            // 
            this._homeToolButton.MouseClick += this.HomeToolButton_MouseClick;
            // 
            // searchRatingToolButton
            // 
            this._searchRatingToolButton.MouseClick += this.SearchRatingToolButton_MouseClick;
            // 
            // historyToolButton
            // 
            this._historyToolButton.MouseClick += this.HistoryToolButton_MouseClick;
            // 
            // toolPanel2
            // 
            this._toolPanel2.BackColor = Color.FromArgb(250, 250, 250);
            this._toolPanel2.Controls.AddRange(
                this._searchBookmarkToolButton,
                this._homeToolButton,
                this._tagDropToolButton,
                this._searchRatingToolButton,
                this._historyToolButton);
            this._toolPanel2.IsDrawRightBorderLine = true;
            this._toolPanel2.VerticalTopMargin = 28;
            // 
            // BrowsePanel
            // 
            this.Loaded += this.BrowsePanel_Loaded;
        }

        private SWF.UIComponent.TabOperation.PageContainer _pageContainer;
        private PicSum.UIComponent.InfoPanel.InfoPanel _infoPanel;
        private SWF.UIComponent.TabOperation.TabSwitch _tabSwitch;
        private System.Windows.Forms.Control _toolPanel;
        private SWF.UIComponent.Base.ToolPanel _toolPanel2;
        private SWF.UIComponent.Base.BaseIconButton _showInfoToolButton;
        private PicSum.UIComponent.AddressBar.AddressBar _addressBar;
        private SWF.UIComponent.Base.BaseIconButton _nextPageHistoryButton;
        private SWF.UIComponent.Base.BaseIconButton _previewPageHistoryButton;
        private SWF.UIComponent.Base.BaseIconButton _homeToolButton;
        private SWF.UIComponent.Base.BaseIconButton _searchRatingToolButton;
        private SWF.UIComponent.WideDropDown.WideDropToolButton _tagDropToolButton;
        private SWF.UIComponent.Base.BaseIconButton _reloadToolButton;
        private SWF.UIComponent.Base.BaseIconButton _searchBookmarkToolButton;
        private SWF.UIComponent.Base.BaseIconButton _historyToolButton;

        #endregion
    }
}
