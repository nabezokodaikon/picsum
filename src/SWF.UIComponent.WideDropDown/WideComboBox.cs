using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class WideComboBox
        : UserControl
    {
        public event EventHandler<DropDownOpeningEventArgs> DropDownOpening;
        public event EventHandler<AddItemEventArgs> AddItem;

        private readonly WideDropDownList dropDownList = new();

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

        public WideComboBox()
        {
            this.InitializeComponent();

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.dropDownList.IsClickAndClose = true;
            this.dropDownList.Closed += this.DropDownList_Closed;
            this.dropDownList.ItemMouseClick += this.DropDownList_ItemMouseClick;
        }

        public void SetItems(string[] items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));

            this.dropDownList.SetItems(items);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            var w = this.Width - this.addButton.Width - 1f;
            var h = this.Height - 1f;

            e.Graphics.DrawRectangle(Pens.LightGray, 0, 0, w, h);

            base.OnPaint(e);
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.W)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

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
                this, new Point(this.arrowPictureBox.Right - this.dropDownList.Size.Width, this.arrowPictureBox.Bottom));
        }

        private void DropDownList_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.arrowPictureBox.IsSelected = false;
        }

        private void DropDownList_ItemMouseClick(object sender, ItemMouseClickEventArgs e)
        {
            this.inputTextBox.Text = e.Item;
            this.inputTextBox.SelectionStart = this.inputTextBox.Text.Length;
            this.inputTextBox.Focus();
        }
    }
}
