using SWF.UIComponent.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class WideDropToolButton
        : ToolIconButton
    {
        public event EventHandler<ItemMouseClickEventArgs> ItemMouseClick;
        public event EventHandler<DropDownOpeningEventArgs> DropDownOpening;

        private bool isShowingDropDown = false;
        private readonly WideDropDownList dropDownList;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedItem { get; set; }

        public WideDropToolButton()
        {
            this.dropDownList = new(this);
            this.dropDownList.IsClickAndClose = true;
            this.dropDownList.ItemMouseClick += this.DropDownList_ItemMouseClick;

            this.MouseClick += this.WideDropToolButton_MouseClick;
        }

        public void SetItems(string[] items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));

            this.dropDownList.SetItems(items);
        }

        public void SelectItem(string item)
        {
            ArgumentException.ThrowIfNullOrEmpty(item, nameof(item));

            this.dropDownList.SelectItem(item);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.isShowingDropDown = false;
        }

        private void WideDropToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (this.isShowingDropDown)
            {
                this.isShowingDropDown = false;
                this.dropDownList.Close();
            }
            else
            {
                this.isShowingDropDown = true;

                if (this.DropDownOpening != null)
                {
                    var args = new DropDownOpeningEventArgs();
                    this.DropDownOpening(this, args);
                }

                this.dropDownList.Show(
                    this, new Point(this.Width, 0));

                if (!string.IsNullOrEmpty(this.SelectedItem))
                {
                    this.dropDownList.SelectItem(this.SelectedItem);
                }
            }
        }

        private void DropDownList_ItemMouseClick(object sender, ItemMouseClickEventArgs e)
        {
            this.SelectedItem = e.Item;
            this.ItemMouseClick?.Invoke(this, e);
        }
    }
}
