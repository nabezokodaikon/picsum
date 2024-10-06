using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ToolBar
{
    internal sealed class PageToolBarColorTable
        : ProfessionalColorTable
    {
        private readonly Color gradientBeginColor = Color.FromArgb(240, 240, 240);

        public override Color ToolStripGradientBegin
        {
            get
            {
                return this.gradientBeginColor;
            }
        }

        public override Color ToolStripGradientMiddle
        {
            get
            {
                return this.gradientBeginColor;
            }
        }

        public override Color ToolStripGradientEnd
        {
            get
            {
                return this.gradientBeginColor;
            }
        }

        public override Color ToolStripBorder
        {
            get
            {
                return this.gradientBeginColor;
            }
        }
    }
}
