using SWF.Core.Base;
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
        private static readonly Rectangle INPUT_TEXT_BOX_DEFAULT_BOUNDS
            = new(0, 1, 569, 36);
        private static readonly Rectangle ARROW_PICTURE_BOX_DEFAULT_BOUNDS
            = new(570, 1, 22, 36);
        private static readonly Rectangle ADD_BUTTON_DEFAULT_BOUNDS
            = new(601, 0, 38, 38);

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

            this.addButton.Image = ResourceFiles.TagIcon.Value;
            this.arrowPictureBox.Image = ResourceFiles.SmallArrowDownIcon.Value;

            this.dropDownList.IsClickAndClose = true;
            this.dropDownList.Closed += this.DropDownList_Closed;
            this.dropDownList.ItemMouseClick += this.DropDownList_ItemMouseClick;
        }

        public void SetControlsBounds(float scale)
        {
            this.SuspendLayout();

            this.inputTextBox.SetBounds(
                INPUT_TEXT_BOX_DEFAULT_BOUNDS.X,
                INPUT_TEXT_BOX_DEFAULT_BOUNDS.Y,
                (int)(this.Width - (ARROW_PICTURE_BOX_DEFAULT_BOUNDS.Width + ADD_BUTTON_DEFAULT_BOUNDS.Width + 1) * scale),
                this.Height - INPUT_TEXT_BOX_DEFAULT_BOUNDS.Y * 2);

            this.arrowPictureBox.SetBounds(
                (int)(this.inputTextBox.Right),
                this.inputTextBox.Top,
                (int)(ARROW_PICTURE_BOX_DEFAULT_BOUNDS.Width * scale),
                this.inputTextBox.Height);

            this.addButton.SetBounds(
                this.arrowPictureBox.Right + (int)(1 * scale),
                ADD_BUTTON_DEFAULT_BOUNDS.Top,
                (int)(ADD_BUTTON_DEFAULT_BOUNDS.Width * scale),
                this.Height);

            this.inputTextBox.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.arrowPictureBox.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Right;

            this.addButton.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Right;

            this.ResumeLayout(false);
            this.PerformLayout();
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
            var w = this.arrowPictureBox.Right;
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
