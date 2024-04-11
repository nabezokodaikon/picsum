using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows")]
    public partial class WideComboBox
        : UserControl
    {
        public event EventHandler<DropDownOpeningEventArgs> DropDownOpening;
        public event EventHandler<AddItemEventArgs> AddItem;

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

        public WideComboBox()
        {
            this.InitializeComponent();

            this.dropDownList.IsClickAndClose = true;
            this.dropDownList.Closed += this.DropDownList_Closed;
            this.dropDownList.ItemMouseClick += this.DropDownList_ItemMouseClick;
        }

        public void SetItems(List<string> items)
        {
            ArgumentNullException.ThrowIfNull(nameof(items));

            this.dropDownList.SetItems(items);
        }

        public void AddItems(IList<string> itemList)
        {
            ArgumentNullException.ThrowIfNull(nameof(itemList));

            this.dropDownList.AddItems(itemList.Where(item => !string.IsNullOrEmpty(item)).ToList());
        }

        public void SelectItem()
        {
            var item = this.inputTextBox.Text;
            if (string.IsNullOrEmpty(item))
            {
                return;
            }

            this.dropDownList.SelectItem(item);
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;

            if (string.IsNullOrEmpty(this.inputTextBox.Text))
            {
                return;
            }

            if (this.AddItem == null)
            {
                return;
            }

            var item = this.inputTextBox.Text.Trim();
            var args = new AddItemEventArgs(item);
            this.AddItem(this, args);
        }

        private void AddButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.inputTextBox.Text))
            {
                return;
            }

            if (this.AddItem == null)
            {
                return;
            }

            var item = this.inputTextBox.Text.Trim();
            var args = new AddItemEventArgs(item);
            this.AddItem(this, args);
        }

        private void ArrowPictureBox_MouseClick(object sender, MouseEventArgs e)
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
        }

        private void DropDownList_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.arrowPictureBox.IsSelected = false;
        }

        private void DropDownList_ItemMouseClick(object sender, ItemMouseClickEventArgs e)
        {
            this.inputTextBox.Text = e.Item;
        }
    }
}
