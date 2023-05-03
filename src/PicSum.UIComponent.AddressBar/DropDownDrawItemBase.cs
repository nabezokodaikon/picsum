using PicSum.Core.Base.Conf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    abstract class DropDownDrawItemBase
        : DrawItemBase
    {
        private DropDownList dropDownList = null;
        private DirectoryEntity mousePointItem = null;
        private IList<DirectoryEntity> items = new List<DirectoryEntity>();

        public IList<DirectoryEntity> Items
        {
            get
            {
                return this.items;
            }
        }

        protected bool IsDropDown
        {
            get
            {
                return this.dropDownList != null && this.dropDownList.Visible;
            }
        }

        protected DropDownList DropDownList
        {
            get
            {
                if (this.dropDownList == null)
                {
                    this.CreateDropDownList();
                }

                return this.dropDownList;
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

        protected abstract void DrawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e);

        protected override void Dispose()
        {
            if (this.dropDownList != null)
            {
                this.dropDownList.Close();
                this.dropDownList.Dispose();
            }

            base.Dispose();
        }

        private void CreateDropDownList()
        {
            this.dropDownList = new DropDownList();
            this.dropDownList.BackColor = base.Palette.InnerColor;
            this.dropDownList.ItemHeight = DROPDOWN_ITEM_HEIGHT;
            this.dropDownList.ItemTextTrimming = StringTrimming.EllipsisCharacter;
            this.dropDownList.ItemTextAlignment = StringAlignment.Near;
            this.dropDownList.ItemTextLineAlignment = StringAlignment.Center;
            this.dropDownList.ItemTextFormatFlags = StringFormatFlags.NoWrap;

            this.dropDownList.Opened += new EventHandler(this.DropDownList_Opened);
            this.dropDownList.Closed += new ToolStripDropDownClosedEventHandler(this.DropDownList_Closed);
            this.dropDownList.Drawitem += new EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(this.DropDownList_Drawitem);
            this.dropDownList.ItemMouseClick += new EventHandler<MouseEventArgs>(this.DropDownList_ItemMouseClick);
            this.dropDownList.ItemExecute += new EventHandler(this.DropDownList_ItemExecute);
        }

        private DirectoryEntity GetDropDownItemFromScreenPoint()
        {
            var index = this.DropDownList.IndexFromScreenPoint();
            if (index > -1 && items.Count > index)
            {
                return items[index];
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
            if (this.items.Count > 0)
            {
                this.DrawDropDownItem(e);
            }
        }

        private void DropDownList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            this.DropDownList.Close();

            var index = this.DropDownList.GetSelectedIndexs()[0];
            if (index < 0 || this.items.Count - 1 < index)
            {
                return;
            }

            var item = this.items[index];
            if (e.Button == MouseButtons.Left)
            {
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.OverlapTab, item.DirectoryPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.AddTab, item.DirectoryPath));
            }
        }

        private void DropDownList_ItemExecute(object sender, EventArgs e)
        {
            this.DropDownList.Close();

            var index = this.DropDownList.GetSelectedIndexs()[0];
            if (index < 0 || this.items.Count - 1 < index)
            {
                return;
            }

            var item = this.items[index];
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.OverlapTab, item.DirectoryPath));
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (this.items == null)
            {
                this.mousePointItem = null;
                e.Cancel = true;
            }
            else
            {
                this.mousePointItem = this.GetDropDownItemFromScreenPoint();
            }
        }

        private void ContextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            this.DropDownList.Focus();
        }

        private void ContextMenu_ActiveTabOpen(object sender, EventArgs e)
        {
            this.DropDownList.Close();
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.OverlapTab, this.mousePointItem.DirectoryPath));
        }

        private void ContextMenu_NewTabOpen(object sender, EventArgs e)
        {
            this.DropDownList.Close();
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.AddTab, this.mousePointItem.DirectoryPath));
        }

        private void ContextMenu_NewWindowOpen(object sender, EventArgs e)
        {
            this.DropDownList.Close();
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(ContentsOpenType.NewWindow, this.mousePointItem.DirectoryPath));
        }
    }
}
