using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ToolBar
{
    internal sealed class ContentsToolBarColorTable
        : ProfessionalColorTable
    {
        private Color gradientBeginColor = Color.FromArgb(241, 244, 250);
        private Color gradientMiddleColor = Color.FromArgb(229, 232, 242);
        private Color gradientEndColor = Color.FromArgb(217, 220, 230);
        private Color borderEndColor = Color.FromArgb(124, 138, 153);

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
