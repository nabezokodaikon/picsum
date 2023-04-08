using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PicSum.Core.Base.Conf;
using PicSum.UIComponent.Common;
using SWF.UIComponent.Common;

namespace PicSum.UIComponent.SearchTool
{
    public abstract class SearchToolButtonBase<TValue> : ToolButton
    {
        #region 定数・列挙

        protected const int MAXIMUM_SHOW_DROPDOWN_ITEM_COUNT = 16;
        protected const int MINIMUM_DROPDOWN_WIDTH = 128;
        private const int DROPDOWN_ITEM_HEIGHT = 24;

        #endregion

        #region インスタンス変数

        private IList<SearchInfoEntity<TValue>> _itemList = null;
        private SearchInfoEntity<TValue> _mousePointItem = null;
        private TValue _selectedValue = default(TValue);
        private DropDownList _dropDownList = null;
        private IContainer _processContainer = null;
        #endregion

        #region プロパティ

        protected IContainer ProcessContainer
        {
            get
            {
                if (_processContainer == null)
                {
                    _processContainer = new Container();
                }

                return _processContainer;
            }
        }

        protected IList<SearchInfoEntity<TValue>> ItemList
        {
            get
            {
                return _itemList;
            }
            set
            {
                _itemList = value;
            }
        }

        protected TValue SelectedValue
        {
            get
            {
                return _selectedValue;
            }
        }

        protected DropDownList DropDownList
        {
            get
            {
                if (_dropDownList == null)
                {
                    createDropDownList();
                }

                return _dropDownList;
            }
        }

        private SearchInfoContextMenu contextMenu
        {
            get
            {
                if (_dropDownList == null)
                {
                    throw new NullReferenceException("ドロップダウンリストがNULLです。");
                }

                if (_dropDownList.ContextMenuStrip == null)
                {
                    createDropDownContextMenu();
                }

                return (SearchInfoContextMenu)_dropDownList.ContextMenuStrip;
            }
        }

        #endregion

        #region コンストラクタ

        protected SearchToolButtonBase()
        {
            initializeComponent();
        }

        #endregion

        #region メソッド

        protected abstract void OnDropDownOpening();

        protected abstract void OnDrawItem(SWF.UIComponent.FlowList.DrawItemEventArgs e);

        protected abstract void OnSelectedItem(SelectedItemEventArgs<TValue> e);

        protected override void Dispose(bool disposing)
        {
            if (disposing && _processContainer != null)
            {
                _processContainer.Dispose();
                _processContainer = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Rectangle rect = this.GetRegionBounds();
                DropDownList.Show(this.Parent, rect.Left, rect.Bottom);
            }

            base.OnMouseClick(e);
        }

        private void initializeComponent()
        {

        }

        private void createDropDownList()
        {
            if (_dropDownList != null)
            {
                throw new Exception("ドロップダウンが存在します。");
            }

            _dropDownList = new DropDownList();

            _dropDownList.BackColor = Color.White;
            _dropDownList.SelectedItemColor = Color.FromArgb(48, 48, 96, 144);
            _dropDownList.MousePointItemColor = Color.Empty;
            _dropDownList.FocusItemColor = Color.Empty;
            _dropDownList.ItemTextColor = Color.FromArgb(64, 64, 64);
            _dropDownList.ItemHeight = DROPDOWN_ITEM_HEIGHT;
            _dropDownList.ItemTextTrimming = StringTrimming.EllipsisCharacter;
            _dropDownList.ItemTextLineAlignment = StringAlignment.Center;
            _dropDownList.ItemTextAlignment = StringAlignment.Near;
            _dropDownList.ItemTextFormatFlags = StringFormatFlags.NoWrap;

            DropDownList.Opening += new CancelEventHandler(dropDownList_Opening);
            DropDownList.Closing += new ToolStripDropDownClosingEventHandler(dropDownList_Closing);
            DropDownList.Drawitem += new EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(dropDownList_DrawItem);
            DropDownList.ItemExecute += new EventHandler(dropDownList_ItemExecute);
            DropDownList.ItemMouseClick += new EventHandler<MouseEventArgs>(dropDownList_ItemMouseClick);
        }

        private void createDropDownContextMenu()
        {
            if (DropDownList.ContextMenuStrip != null)
            {
                throw new Exception("既にドロップダウンのコンテキストメニューが存在します。");
            }

            DropDownList.ContextMenuStrip = new SearchInfoContextMenu();

            contextMenu.Opening += new CancelEventHandler(contextMenu_Opening);
            contextMenu.Closing += new ToolStripDropDownClosingEventHandler(contextMenu_Closing);
            contextMenu.ActiveTabOpen += new EventHandler(contextMenu_ActiveTabOpen);
            contextMenu.NewTabOpen += new EventHandler(contextMenu_NewTabOpen);
            contextMenu.NewWindowOpen += new EventHandler(contextMenu_NewWindowOpen);
        }

        private SearchInfoEntity<TValue> itemFromPoint()
        {
            if (_itemList != null)
            {
                int index = DropDownList.IndexFromScreenPoint();
                if (index > -1 && _itemList.Count > index)
                {
                    return _itemList[index];
                }
            }

            return null;
        }

        #endregion

        #region イベント

        private void dropDownList_Opening(object sender, CancelEventArgs e)
        {
            OnDropDownOpening();
        }

        private void dropDownList_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (contextMenu.IsOpen)
            {
                e.Cancel = true;
            }
        }

        private void dropDownList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            DropDownList.Close();
            SearchInfoEntity<TValue> item = _itemList[DropDownList.GetSelectedIndexs()[0]];
            _selectedValue = item.Value;

            if (e.Button == MouseButtons.Left)
            {
                OnSelectedItem(new SelectedItemEventArgs<TValue>(ContentsOpenType.OverlapTab, item.Value));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                OnSelectedItem(new SelectedItemEventArgs<TValue>(ContentsOpenType.AddTab, item.Value));
            }
        }

        private void dropDownList_ItemExecute(object sender, EventArgs e)
        {
            DropDownList.Close();
            SearchInfoEntity<TValue> item = _itemList[DropDownList.GetSelectedIndexs()[0]];
            _selectedValue = item.Value;
            OnSelectedItem(new SelectedItemEventArgs<TValue>(ContentsOpenType.OverlapTab, item.Value));
        }

        private void dropDownList_DrawItem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (_itemList != null && _itemList.Count > e.ItemIndex && e.ItemIndex > -1)
            {
                OnDrawItem(e);
            }
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (_itemList == null)
            {
                _mousePointItem = null;
                e.Cancel = true;
            }
            else
            {
                _mousePointItem = itemFromPoint();
            }
        }

        private void contextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            DropDownList.Focus();
        }

        private void contextMenu_ActiveTabOpen(object sender, EventArgs e)
        {
            DropDownList.Close();
            _selectedValue = _mousePointItem.Value;
            OnSelectedItem(new SelectedItemEventArgs<TValue>(ContentsOpenType.OverlapTab, _mousePointItem.Value));
        }

        private void contextMenu_NewTabOpen(object sender, EventArgs e)
        {
            DropDownList.Close();
            _selectedValue = _mousePointItem.Value;
            OnSelectedItem(new SelectedItemEventArgs<TValue>(ContentsOpenType.AddTab, _mousePointItem.Value));
        }

        private void contextMenu_NewWindowOpen(object sender, EventArgs e)
        {
            DropDownList.Close();
            _selectedValue = _mousePointItem.Value;
            OnSelectedItem(new SelectedItemEventArgs<TValue>(ContentsOpenType.NewWindow, _mousePointItem.Value));
        }

        #endregion
    }
}
