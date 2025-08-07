using SWF.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    abstract class DropDownDrawItemBase
        : DrawItemBase, IDisposable
    {
        private DropDownList _dropDownList = null;
        private DirectoryEntity _mousePointItem = null;
        private readonly List<DirectoryEntity> _items = [];

        public List<DirectoryEntity> Items
        {
            get
            {
                return this._items;
            }
        }

        protected bool IsDropDown
        {
            get
            {
                return this._dropDownList != null && this._dropDownList.Visible;
            }
        }

        protected DropDownList DropDownList
        {
            get
            {
                if (this._dropDownList == null)
                {
                    this.CreateDropDownList();
                }

                return this._dropDownList;
            }
        }

        public override void Draw(Graphics g)
        {
            throw new NotImplementedException();
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected abstract void DrawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e);

        public new void Dispose()
        {
            if (this._dropDownList != null)
            {
                this._dropDownList.Close();
                this._dropDownList.Dispose();
                this._dropDownList = null;
            }

            base.Dispose();
            this._disposed = true;
            GC.SuppressFinalize(this);
        }

        private void CreateDropDownList()
        {
            this._dropDownList = new()
            {
                BackColor = Palette.INNER_COLOR,
                ItemTextTrimming = StringTrimming.EllipsisCharacter,
                ItemTextAlignment = StringAlignment.Near,
                ItemTextLineAlignment = StringAlignment.Center,
                ItemTextFormatFlags = StringFormatFlags.NoWrap
            };

            this._dropDownList.Opened += new(this.DropDownList_Opened);
            this._dropDownList.Closed += new(this.DropDownList_Closed);
            this._dropDownList.Drawitem += new(this.DropDownList_Drawitem);
            this._dropDownList.ItemMouseClick += (this.DropDownList_ItemMouseClick);
            this._dropDownList.ItemExecute += new(this.DropDownList_ItemExecute);
        }

        private DirectoryEntity GetDropDownItemFromScreenPoint()
        {
            var index = this.DropDownList.IndexFromScreenPoint();
            if (index > -1 && this._items.Count > index)
            {
                return this._items[index];
            }

            return null;
        }

        private void DropDownList_Opened(object sender, EventArgs e)
        {
            this.OnDropDownOpened(e);
        }

        private void DropDownList_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.OnDropDownClosed(e);
        }

        private void DropDownList_Drawitem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this._disposed)
            {
                return;
            }

            if (this._items.Count > 0)
            {
                this.DrawDropDownItem(e);
            }
        }

        private void DropDownList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            this.DropDownList.Close();

            var index = this.DropDownList.GetSelectedIndexs()[0];
            if (index < 0 || this._items.Count - 1 < index)
            {
                return;
            }

            var item = this._items[index];
            if (e.Button == MouseButtons.Left)
            {
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenMode.OverlapTab, item.DirectoryPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenMode.AddTab, item.DirectoryPath));
            }
        }

        private void DropDownList_ItemExecute(object sender, EventArgs e)
        {
            this.DropDownList.Close();

            var index = this.DropDownList.GetSelectedIndexs()[0];
            if (index < 0 || this._items.Count - 1 < index)
            {
                return;
            }

            var item = this._items[index];
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenMode.OverlapTab, item.DirectoryPath));
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (this._items == null)
            {
                this._mousePointItem = null;
                e.Cancel = true;
            }
            else
            {
                this._mousePointItem = this.GetDropDownItemFromScreenPoint();
            }
        }

        private void ContextMenu_ActiveTabOpen(object sender, EventArgs e)
        {
            this.DropDownList.Close();
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenMode.OverlapTab, this._mousePointItem.DirectoryPath));
        }

        private void ContextMenu_NewTabOpen(object sender, EventArgs e)
        {
            this.DropDownList.Close();
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenMode.AddTab, this._mousePointItem.DirectoryPath));
        }

        private void ContextMenu_NewWindowOpen(object sender, EventArgs e)
        {
            this.DropDownList.Close();
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenMode.NewWindow, this._mousePointItem.DirectoryPath));
        }
    }
}
