using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.Common
{
    [SupportedOSPlatform("windows")]
    public class DoubleBufferedSplitContainer
        : SplitContainer
    {
        public DoubleBufferedSplitContainer()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();
        }
    }
}
