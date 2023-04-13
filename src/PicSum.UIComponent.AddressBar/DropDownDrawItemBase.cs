using System;
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
        private DirectoryEntity _mousePointItem = null;
        private IList<DirectoryEntity> _items = new List<DirectoryEntity>();

        public IList<DirectoryEntity> Items
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
            
            _dropDownList.Opened += new EventHandler(dropDownList_Opened);
            _dropDownList.Closed += new ToolStripDropDownClosedEventHandler(dropDownList_Closed);
            _dropDownList.Drawitem += new EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(dropDownList_Drawitem);
            _dropDownList.ItemMouseClick += new EventHandler<MouseEventArgs>(dropDownList_ItemMouseClick);
            _dropDownList.ItemExecute += new EventHandler(dropDownList_ItemExecute);
        }

        private DirectoryEntity getDropDownItemFromScreenPoint()
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

            DirectoryEntity item = _items[index];
            if (e.Button == MouseButtons.Left)
            {
                OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.OverlapTab, item.DirectoryPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.AddTab, item.DirectoryPath));
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
            
            DirectoryEntity item = _items[index];
            OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.OverlapTab, item.DirectoryPath));
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
            OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.OverlapTab, _mousePointItem.DirectoryPath));
        }

        private void contextMenu_NewTabOpen(object sender, EventArgs e)
        {
            DropDownList.Close();
            OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.AddTab, _mousePointItem.DirectoryPath));
        }

        private void contextMenu_NewWindowOpen(object sender, EventArgs e)
        {
            DropDownList.Close();
            OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.NewWindow, _mousePointItem.DirectoryPath));
        }
    }
}
