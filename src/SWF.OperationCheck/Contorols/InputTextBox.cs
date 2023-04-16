using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApi;
using static WinApi.WinApiMembers;

namespace SWF.OperationCheck.Contorols
{
    public class InputTextBox
        : TextBox
    {
        public InputTextBox()
        {
            this.Multiline = true;
            this.verticalAlignment();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //this.Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            this.verticalAlignment();
            base.OnResize(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            this.verticalAlignment();
            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            this.verticalAlignment();
            base.OnGotFocus(e);
        }

        private void verticalAlignment()
        {
            SizeF textSize;
            float twipsPerPixelX;
            float twipsPerPixelY;
            using (var g = Graphics.FromHwnd(this.Handle))
            {
                textSize = g.MeasureString("1", this.Font);
                twipsPerPixelX = 1440f / g.DpiX;
                twipsPerPixelY = 1440f / g.DpiY;
            }

            var textMargin = textSize.Height / 4;
            var height = textSize.Height + textMargin;
            var top = ((this.Height - height)) / 2f;
            var rect = new Rectangle(0, (int)top, this.Width, this.Height);

            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf<RectangleF>());

            try
            {
                Marshal.StructureToPtr(rect, ptr, false);
                WinApiMembers.SendMessage(this.Handle, WinApiMembers.EM_SETRECT, 0, ptr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }
    }
}
