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

        private bool _isShowingDropDown = false;
        private readonly WideDropDownList _dropDownList;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image Icon
        {
            get
            {
                return this._dropDownList.Icon;
            }
            set
            {
                this._dropDownList.Icon = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedItem { get; set; }

        public WideDropToolButton()
        {
            this._dropDownList = new(this);
            this._dropDownList.IsClickAndClose = true;
            this._dropDownList.ItemMouseClick += this.DropDownList_ItemMouseClick;

            this.MouseClick += this.WideDropToolButton_MouseClick;
            this.LostFocus += this.WideDropToolButton_LostFocus;
        }

        public void SetItems(string[] items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));

            this._dropDownList.SetItems(items);
        }

        public void SelectItem(string item)
        {
            ArgumentException.ThrowIfNullOrEmpty(item, nameof(item));

            this._dropDownList.SelectItem(item);
        }

        private void WideDropToolButton_LostFocus(object sender, EventArgs e)
        {
            this._isShowingDropDown = false;
        }

        private void WideDropToolButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (this._isShowingDropDown)
            {
                this._isShowingDropDown = false;
                this._dropDownList.Close();
            }
            else
            {
                this._isShowingDropDown = true;

                if (this.DropDownOpening != null)
                {
                    var args = new DropDownOpeningEventArgs();
                    this.DropDownOpening(this, args);
                }

                this._dropDownList.Show(
                    this, new Point(this.Width, 0));

                if (!string.IsNullOrEmpty(this.SelectedItem))
                {
                    this._dropDownList.SelectItem(this.SelectedItem);
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
