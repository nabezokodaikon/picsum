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
        public Size WindowSize { get; private set; }
        public FormWindowState WindowState { get; private set; }

        public TabDropoutedEventArgs(
            TabInfo tab,
            Size windowSize,
            FormWindowState windowState)
            : base(tab)
        {
            this.ToOtherOwner = false;
            this.WindowSize = windowSize;
            this.WindowState = windowState;
        }

        public TabDropoutedEventArgs(TabInfo tab)
            : base(tab)
        {
            this.ToOtherOwner = true;
            this.WindowSize = Size.Empty;
            this.WindowState = FormWindowState.Normal;
        }
    }
}
