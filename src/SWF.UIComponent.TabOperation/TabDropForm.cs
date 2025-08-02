using SWF.UIComponent.Core;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class TabDropForm
        : BaseForm
    {
        private static readonly Color TRANSPARENT_COLOR = Color.FromArgb(0, 0, 0, 0);

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
