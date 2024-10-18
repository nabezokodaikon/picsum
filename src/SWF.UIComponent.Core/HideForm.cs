using System.Runtime.Versioning;
using WinApi;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows")]
    public partial class HideForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle = WinApiMembers.WS_EX_TOOLWINDOW;
                cp.Style = unchecked((int)WinApiMembers.WS_POPUP) | WinApiMembers.WS_VISIBLE | WinApiMembers.WS_SYSMENU | WinApiMembers.WS_MAXIMIZEBOX;
                cp.Width = 0;
                cp.Height = 0;
                return cp;
            }
        }

    }
}
