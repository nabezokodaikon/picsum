using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ContentsToolBar
{
    public class ContentsToolBar : ToolStrip
    {
        public ContentsToolBar()
        {
            initializeComponent();
        }

        private void initializeComponent()
        {
            this.DoubleBuffered = true;
            this.CanOverflow = false;
            this.GripStyle = ToolStripGripStyle.Hidden;
            this.Renderer = getRenderer();
        }

        private ToolStripRenderer getRenderer()
        {
            ToolStripProfessionalRenderer renderer = new ToolStripProfessionalRenderer(new ContentsToolBarColorTable());
            renderer.RoundedEdges = false;
            return renderer;
        }
    }
}
