using System.Runtime.Versioning;
using WinApi;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class HideForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle = WinApiMembers.WS_EX_TOOLWINDOW | WinApiMembers.WS_EX_NOACTIVATE;
                cp.Style = unchecked((int)WinApiMembers.WS_POPUP);
                cp.Width = 0;
                cp.Height = 0;
                return cp;
            }
        }

        public HideForm()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, false);

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
        }
    }
}
