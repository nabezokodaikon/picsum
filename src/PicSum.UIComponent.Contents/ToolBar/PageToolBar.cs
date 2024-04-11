using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ToolBar
{
    [SupportedOSPlatform("windows")]
    public sealed class PageToolBar
        : ToolStrip
    {
        public PageToolBar()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.DoubleBuffered = true;
            this.CanOverflow = false;
            this.GripStyle = ToolStripGripStyle.Hidden;
            this.Renderer = this.GetRenderer();
        }

        private ToolStripProfessionalRenderer GetRenderer()
        {
            var renderer = new ToolStripProfessionalRenderer(new PageToolBarColorTable())
            {
                RoundedEdges = false
            };
            return renderer;
        }
    }
}
