using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    public partial class WideComboBox
        : UserControl
    {
        public event EventHandler<DropDownOpeningEventArgs> DropDownOpening;
        public event EventHandler<AddItemEventArgs> AddItem;

        private readonly WideDropDownList dropDownList = new WideDropDownList();

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

        public WideComboBox()
        {
            InitializeComponent();
            this.dropDownList.Font = this.Font;
            this.dropDownList.Size = new Size(420, 200);
            this.dropDownList.Closed += dropDownList_Closed;
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

        public void AddItems(IList<string> itemList)
        {
            if (itemList == null)
            {
                throw new ArgumentNullException(nameof(itemList));
            }

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

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
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

        private void addButton_MouseClick(object sender, MouseEventArgs e)
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

        private void arrowPictureBox_MouseClick(object sender, MouseEventArgs e)
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

        private void dropDownList_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.arrowPictureBox.IsSelected = false;
        }

        private void dropDownList_ItemMouseClick(object sender, ItemMouseClickEventArgs e)
        {
            this.inputTextBox.Text = e.Item;
        }
    }
}
