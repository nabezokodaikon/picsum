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
            this.AcceptsReturn = false;
            this.verticalAlignment();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Return)
            {
                this.OnKeyDown(new KeyEventArgs(keyData));
            }
            
            return base.ProcessDialogKey(keyData);
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
            using (var g = Graphics.FromHwnd(this.Handle))
            {
                textSize = g.MeasureString("1", this.Font);
            }

            var top = (this.Height - textSize.Height) / 2f;
            var left = 4;
            var right = this.Width;
            var bottom = this.Height;
            var rect = Rectangle.FromLTRB(left, (int)top, right, bottom);
            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf<Rectangle>());

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
