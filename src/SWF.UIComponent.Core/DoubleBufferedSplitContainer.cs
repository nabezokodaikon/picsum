using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows")]
    public partial class DoubleBufferedSplitContainer
        : SplitContainer
    {
        public DoubleBufferedSplitContainer()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();
        }
    }
}
