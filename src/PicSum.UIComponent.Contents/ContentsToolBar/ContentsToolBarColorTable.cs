using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ContentsToolBar
{
    class ContentsToolBarColorTable : ProfessionalColorTable
    {
        private Color _gradientBeginColor = Color.FromArgb(241, 244, 250);
        private Color _gradientMiddleColor = Color.FromArgb(229, 232, 242);
        private Color _gradientEndColor = Color.FromArgb(217, 220, 230);
        private Color _borderEndColor = Color.FromArgb(124, 138, 153);

        public override Color ToolStripGradientBegin
        {
            get
            {
                return _gradientBeginColor;
            }
        }

        public override Color ToolStripGradientMiddle
        {
            get
            {
                return _gradientBeginColor;
            }
        }

        public override Color ToolStripGradientEnd
        {
            get
            {
                return _gradientBeginColor;
            }
        }

        public override Color ToolStripBorder
        {
            get
            {
                return _gradientBeginColor;
            }
        }
    }
}
