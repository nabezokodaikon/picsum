using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ToolBar
{
    [SupportedOSPlatform("windows")]
    public sealed class ContentsToolBar
        : ToolStrip
    {
        public ContentsToolBar()
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

        private ToolStripRenderer GetRenderer()
        {
            var renderer = new ToolStripProfessionalRenderer(new ContentsToolBarColorTable());
            renderer.RoundedEdges = false;
            return renderer;
        }
    }
}
