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
        : DrawItemBase
    {
        private DropDownList dropDownList = null;
        private DirectoryEntity mousePointItem = null;
        private readonly List<DirectoryEntity> items = [];

        public List<DirectoryEntity> Items
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

        public new void Dispose()
        {
            if (this.dropDownList != null)
            {
                this.dropDownList.Close();
                this.dropDownList.Dispose();
                this.dropDownList = null;
            }

            base.Dispose();
        }

        private void CreateDropDownList()
        {
            this.dropDownList = new()
            {
                BackColor = Palette.INNER_COLOR,
                ItemTextTrimming = StringTrimming.EllipsisCharacter,
                ItemTextAlignment = StringAlignment.Near,
                ItemTextLineAlignment = StringAlignment.Center,
                ItemTextFormatFlags = StringFormatFlags.NoWrap
            };

            this.dropDownList.Opened += new(this.DropDownList_Opened);
            this.dropDownList.Closed += new(this.DropDownList_Closed);
            this.dropDownList.Drawitem += new(this.DropDownList_Drawitem);
            this.dropDownList.ItemMouseClick += (this.DropDownList_ItemMouseClick);
            this.dropDownList.ItemExecute += new(this.DropDownList_ItemExecute);
        }

        private DirectoryEntity GetDropDownItemFromScreenPoint()
        {
            var index = this.DropDownList.IndexFromScreenPoint();
            if (index > -1 && this.items.Count > index)
            {
                return this.items[index];
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
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.OverlapTab, item.DirectoryPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.AddTab, item.DirectoryPath));
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
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.OverlapTab, item.DirectoryPath));
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

        private void ContextMenu_ActiveTabOpen(object sender, EventArgs e)
        {
            this.DropDownList.Close();
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.OverlapTab, this.mousePointItem.DirectoryPath));
        }

        private void ContextMenu_NewTabOpen(object sender, EventArgs e)
        {
            this.DropDownList.Close();
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.AddTab, this.mousePointItem.DirectoryPath));
        }

        private void ContextMenu_NewWindowOpen(object sender, EventArgs e)
        {
            this.DropDownList.Close();
            this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.NewWindow, this.mousePointItem.DirectoryPath));
        }
    }
}
