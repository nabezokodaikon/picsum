using SWF.OperationCheck.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SWF.OperationCheck.Contorols
{
    public partial class WideComboBox : UserControl
    {
        public WideComboBox()
        {
            InitializeComponent();

        }

        private RectangleF getIconRectangle()
        {
            var x = this.inputTextBox.Right;
            var y = (this.Height - this.inputTextBox.Height) / 2f;
            var w = this.Width - this.inputTextBox.Width;
            var h = (this.Height - this.inputTextBox.Height) / 2f;
            return new RectangleF(x, y, w, h);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var icon = Resources.SmallArrowDown;
            var rect = getIconRectangle();
            var x = rect.Width - icon.Width / 2f;
            var y = rect.Height - icon.Height / 2f;
            var w = rect.Width - icon.Width / 2f;
            var h = rect.Height - icon.Height / 2f;

            e.Graphics.DrawImage(icon, rect, new RectangleF(x, y, w, h), GraphicsUnit.Pixel);

            base.OnPaint(e);
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
            }
        }
    }
}
