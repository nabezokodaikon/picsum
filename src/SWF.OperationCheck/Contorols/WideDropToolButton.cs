using PicSum.UIComponent.Common;
using SWF.UIComponent.Common;
using SWF.UIComponent.FlowList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SWF.OperationCheck.Contorols
{
    public class WideDropToolButton
        : ToolButton
    {
        public event EventHandler<ItemMouseClickEventArgs> ItemMouseClick;
        public event EventHandler<DropDownOpeningEventArgs> DropDownOpening;

        private readonly WideDropDownList dropDownList = new WideDropDownList();
        
        public string SelectedItem { get; set; }

        public Size DropDownListSize
        {
            get
            {
                return this.dropDownList.Size;
            }
            set
            {
                this.dropDownList.Size = value;
            }
        }

        public WideDropToolButton()
        {
            this.dropDownList.Size = new Size(420, 200);
            this.dropDownList.ItemMouseClick += dropDownList_ItemMouseClick;
        }

        public void SetItemSize(int width, int height)
        {
            this.dropDownList.SetItemSize(width, height);
        }

        public void SetItems(List<string> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.dropDownList.SetItems(items);
        }

        public void SelectItem(string item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.dropDownList.SelectItem(item);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (this.DropDownOpening != null)
            {
                var args = new DropDownOpeningEventArgs();
                this.DropDownOpening(this, args);
            }

            this.dropDownList.Show(
                this, new Point(this.Width - this.dropDownList.Size.Width, this.Height));

            if (!string.IsNullOrEmpty(this.SelectedItem))
            {
                this.dropDownList.SelectItem(this.SelectedItem);
            }

            base.OnMouseClick(e);
        }

        private void dropDownList_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine();
        }

        private void dropDownList_ItemMouseClick(object sender, ItemMouseClickEventArgs e)
        {
            this.SelectedItem = e.Item;

            if (this.ItemMouseClick != null)
            {
                this.ItemMouseClick(this, e);
            }
        }
    }
}
