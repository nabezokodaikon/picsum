﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PicSum.Core.Base.Conf;
using PicSum.UIComponent.Common;

namespace PicSum.UIComponent.AddressBar
{
    abstract class DropDownDrawItemBase : DrawItemBase
    {
        private DropDownList _dropDownList = null;
        private FolderEntity _mousePointItem = null;
        private IList<FolderEntity> _items = new List<FolderEntity>();

        public IList<FolderEntity> Items
        {
            get
            {
                return _items;
            }
        }

        protected bool IsDropDown
        {
            get
            {
                return _dropDownList != null && _dropDownList.Visible;
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

        private FolderContextMenu contextMenu
        {
            get
            {
                if (_dropDownList == null ||
                    _dropDownList.ContextMenuStrip == null ||
                    _dropDownList.ContextMenuStrip.GetType() != typeof(FolderContextMenu))
                {
                    throw new NullReferenceException("ドロップダウンリストにコンテキストメニューがセットされていません。");
                }

                return (FolderContextMenu)_dropDownList.ContextMenuStrip;
            }
        }

        public override void Draw(System.Drawing.Graphics g)
        {
            throw new NotImplementedException();
        }

        public override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected abstract void drawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e);

        protected override void Dispose()
        {
            if (_dropDownList != null)
            {
                _dropDownList.ContextMenuStrip.Close();
                _dropDownList.Close();
                _dropDownList.Dispose();
            }

            base.Dispose();
        }

        private void createDropDownList()
        {
            _dropDownList = new DropDownList();
            _dropDownList.BackColor = base.Palette.InnerColor;
            _dropDownList.SelectedItemColor = base.Palette.MousePointColor;
            _dropDownList.MousePointItemColor = Color.Empty;
            _dropDownList.FocusItemColor = Color.Empty;
            _dropDownList.ItemTextColor = base.Palette.TextColor;
            _dropDownList.ItemHeight = DROPDOWN_ITEM_HEIGHT;
            _dropDownList.ItemTextTrimming = StringTrimming.EllipsisCharacter;
            _dropDownList.ItemTextAlignment = StringAlignment.Near;
            _dropDownList.ItemTextLineAlignment = StringAlignment.Center;
            _dropDownList.ItemTextFormatFlags = StringFormatFlags.NoWrap;
            _dropDownList.ContextMenuStrip = new FolderContextMenu();

            _dropDownList.Opened += new EventHandler(dropDownList_Opened);
            _dropDownList.Closing += new ToolStripDropDownClosingEventHandler(dropDownList_Closing);
            _dropDownList.Closed += new ToolStripDropDownClosedEventHandler(dropDownList_Closed);
            _dropDownList.Drawitem += new EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(dropDownList_Drawitem);
            _dropDownList.ItemMouseClick += new EventHandler<MouseEventArgs>(dropDownList_ItemMouseClick);
            _dropDownList.ItemExecute += new EventHandler(dropDownList_ItemExecute);

            contextMenu.Opening += new CancelEventHandler(contextMenu_Opening);
            contextMenu.Closing += new ToolStripDropDownClosingEventHandler(contextMenu_Closing);
            contextMenu.ActiveTabOpen += new EventHandler(contextMenu_ActiveTabOpen);
            contextMenu.NewTabOpen += new EventHandler(contextMenu_NewTabOpen);
            contextMenu.OtherWindowOpen += new EventHandler(contextMenu_OtherWindowOpen);
            contextMenu.NewWindowOpen += new EventHandler(contextMenu_NewWindowOpen);
        }

        private FolderEntity getDropDownItemFromScreenPoint()
        {
            int index = DropDownList.IndexFromScreenPoint();
            if (index > -1 && _items.Count > index)
            {
                return _items[index];
            }

            return null;
        }

        private void dropDownList_Opened(object sender, EventArgs e)
        {
            OnDropDownOpened(e);
        }

        private void dropDownList_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (contextMenu.IsOpen)
            {
                e.Cancel = true;
            }
        }

        private void dropDownList_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            OnDropDownClosed(e);
        }

        private void dropDownList_Drawitem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (_items.Count > 0)
            {
                drawDropDownItem(e);
            }
        }

        private void dropDownList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            DropDownList.Close();

            int index = DropDownList.GetSelectedIndexs()[0];
            if (index < 0 || _items.Count - 1 < index)
            {
                return;
            }

            FolderEntity item = _items[index];
            if (e.Button == MouseButtons.Left)
            {
                OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.OverlapTab, item.FolderPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.AddTab, item.FolderPath));
            }
        }

        private void dropDownList_ItemExecute(object sender, EventArgs e)
        {
            DropDownList.Close();

            int index = DropDownList.GetSelectedIndexs()[0];
            if (index < 0 || _items.Count - 1 < index)
            {
                return;
            }
            
            FolderEntity item = _items[index];
            OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.OverlapTab, item.FolderPath));
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (_items == null)
            {
                _mousePointItem = null;
                e.Cancel = true;
            }
            else
            {
                _mousePointItem = getDropDownItemFromScreenPoint();
            }
        }

        private void contextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            DropDownList.Focus();
        }

        private void contextMenu_ActiveTabOpen(object sender, EventArgs e)
        {
            DropDownList.Close();
            OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.OverlapTab, _mousePointItem.FolderPath));
        }

        private void contextMenu_NewTabOpen(object sender, EventArgs e)
        {
            DropDownList.Close();
            OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.AddTab, _mousePointItem.FolderPath));
        }

        private void contextMenu_OtherWindowOpen(object sender, EventArgs e)
        {
            DropDownList.Close();
            OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.OtherWindow, _mousePointItem.FolderPath));
        }

        private void contextMenu_NewWindowOpen(object sender, EventArgs e)
        {
            DropDownList.Close();
            OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.NewWindow, _mousePointItem.FolderPath));
        }
    }
}
