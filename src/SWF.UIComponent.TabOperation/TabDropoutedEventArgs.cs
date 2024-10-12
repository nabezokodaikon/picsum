using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブをドロップアウトしたイベント
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class TabDropoutedEventArgs
        : TabEventArgs
    {
        public bool ToOtherOwner { get; private set; }
        public Point WindowLocation { get; private set; }
        public Size WindowSize { get; private set; }
        public FormWindowState WindowState { get; private set; }

        public TabDropoutedEventArgs(TabInfo tab, Point windowLocation, Size windowSize, FormWindowState windowState)
            : base(tab)
        {
            this.ToOtherOwner = false;
            this.WindowLocation = windowLocation;
            this.WindowSize = windowSize;
            this.WindowState = windowState;
        }

        public TabDropoutedEventArgs(TabInfo tab)
            : base(tab)
        {
            this.ToOtherOwner = true;
            this.WindowLocation = Point.Empty;
            this.WindowSize = Size.Empty;
            this.WindowState = FormWindowState.Normal;
        }
    }
}
