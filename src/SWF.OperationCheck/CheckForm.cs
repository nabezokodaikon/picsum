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
        private string selectedItem = "10050";

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
            this.wideDropToolButton1.Icon = Resources.TagIcon;
            this.wideDropToolButton1.SetItems(items);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.wideDropToolButton1.SelectedItem = "10050";
        }
    }
}
