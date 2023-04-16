using SWF.OperationCheck.Contorols;
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

namespace SWF.OperationCheck
{
    public partial class CheckForm : Form
    {
        public CheckForm()
        {
            InitializeComponent();

            var items = new List<string>();
            for (var i = 10000; i < 10100; i++)
            {
                items.Add(i.ToString());
            }

            var icon = Resources.TagIcon;

            this.wideDropToolButton1.SetItemSize(128, 32);
            this.wideDropToolButton1.SetItems(items);

            this.wideComboBox1.SetItemSize(128, 32);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.wideDropToolButton1.SelectedItem = "10050";
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Back)
            {
                e.Handled = true;
                return;
            }
        }

        private void wideComboBox1_AddItem(object sender, AddItemEventArgs e)
        {
            this.wideComboBox1.AddItems(new List<string> { e.Item });
        }

        private void wideComboBox1_DropDownOpening(object sender, DropDownOpeningEventArgs e)
        {
            var items = new List<string>();
            for (var i = 10000; i < 10100; i++)
            {
                items.Add(i.ToString());
            }

            this.wideComboBox1.AddItems(items);
        }
    }
}
