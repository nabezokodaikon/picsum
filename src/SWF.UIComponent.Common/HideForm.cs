using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.Common
{
    public class HideForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = WinApiMembers.WS_EX_TOOLWINDOW;
                cp.Style = unchecked((int)WinApiMembers.WS_POPUP) | WinApiMembers.WS_VISIBLE | WinApiMembers.WS_SYSMENU | WinApiMembers.WS_MAXIMIZEBOX;
                cp.Width = 0;
                cp.Height = 0;
                return cp;
            }
        }

    }
}
