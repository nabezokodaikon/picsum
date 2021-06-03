using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブをドロップアウトしたイベント
    /// </summary>
    public class TabDropoutedEventArgs : TabEventArgs
    {
        private bool _toOtherOwner = false;
        private Point _windowLocation = Point.Empty;
        private Size _windowSize = Size.Empty;
        private FormWindowState _windowState = FormWindowState.Normal;

        public bool ToOtherOwner
        {
            get
            {
                return _toOtherOwner;
            }
        }

        public Point WindowLocation
        {
            get
            {
                return _windowLocation;
            }
        }

        public Size WindowSize
        {
            get
            {
                return _windowSize;
            }
        }

        public FormWindowState WindowState
        {
            get
            {
                return _windowState;
            }
        }

        public TabDropoutedEventArgs(TabInfo tab, Point windowLocation, Size windowSize, FormWindowState windowState)
            : base(tab)
        {
            _toOtherOwner = false;
            _windowLocation = windowLocation;
            _windowSize = windowSize;
            _windowState = windowState;
        }

        public TabDropoutedEventArgs(TabInfo tab)
            : base(tab)
        {
            _toOtherOwner = true;
        }
    }
}
