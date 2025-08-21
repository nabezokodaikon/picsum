using SWF.UIComponent.Base;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{

    internal sealed partial class TabDropForm
        : BaseForm
    {
        public TabDropForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.Opacity = 0.75;
            this.Paint += this.TabDropForm_Paint;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

        private void TabDropForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, this.Bounds);
        }
    }
}
