using SWF.UIComponent.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows")]
    public class WideDropToolButton
        : ToolButton
    {
        public event EventHandler<ItemMouseClickEventArgs> ItemMouseClick;
        public event EventHandler<DropDownOpeningEventArgs> DropDownOpening;

        private readonly WideDropDownList dropDownList = new();

        public Image Icon
        {
            get
            {
                return this.dropDownList.Icon;
            }
            set
            {
                this.dropDownList.Icon = value;
            }
        }

        public string SelectedItem { get; set; }

        public WideDropToolButton()
        {
            this.dropDownList.IsClickAndClose = false;
            this.dropDownList.ItemMouseClick += this.DropDownList_ItemMouseClick;
        }

        public void SetItems(List<string> items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));

            this.dropDownList.SetItems(items);
        }

        public void SelectItem(string item)
        {
            ArgumentException.ThrowIfNullOrEmpty(item, nameof(item));

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

        private void DropDownList_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void DropDownList_ItemMouseClick(object sender, ItemMouseClickEventArgs e)
        {
            this.SelectedItem = e.Item;
            this.ItemMouseClick?.Invoke(this, e);
        }
    }
}
