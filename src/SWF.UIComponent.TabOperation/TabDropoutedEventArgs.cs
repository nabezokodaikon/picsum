using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブをドロップアウトしたイベント
    /// </summary>

    public sealed class TabDropoutedEventArgs
        : TabEventArgs
    {
        public bool ToOtherOwner { get; private set; }
        public RectangleF TabsRectange { get; private set; }
        public Point TabSwitchMouseLocation { get; private set; }
        public Size WindowSize { get; private set; }
        public FormWindowState WindowState { get; private set; }

        public TabDropoutedEventArgs(
            TabInfo tab,
            RectangleF tabsRectange,
            Point tabSwitchMouseLocation,
            Size windowSize,
            FormWindowState windowState)
            : base(tab)
        {
            this.ToOtherOwner = false;
            this.TabsRectange = tabsRectange;
            this.TabSwitchMouseLocation = tabSwitchMouseLocation;
            this.WindowSize = windowSize;
            this.WindowState = windowState;
        }

        public TabDropoutedEventArgs(TabInfo tab)
            : base(tab)
        {
            this.ToOtherOwner = true;
            this.TabsRectange = Rectangle.Empty;
            this.TabSwitchMouseLocation = Point.Empty;
            this.WindowSize = Size.Empty;
            this.WindowState = FormWindowState.Normal;
        }
    }
}
