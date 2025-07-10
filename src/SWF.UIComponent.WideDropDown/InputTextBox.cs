using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class InputTextBox
        : TextBox
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new BorderStyle BorderStyle
        {
            get
            {
                return base.BorderStyle;
            }
            private set
            {
                base.BorderStyle = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Multiline
        {
            get
            {
                return base.Multiline;
            }
            private set
            {
                base.Multiline = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            private set
            {
                base.TabStop = value;
            }
        }

        public InputTextBox()
        {
            this.BorderStyle = BorderStyle.None;
            this.Multiline = true;
            this.AcceptsReturn = false;

            this.Resize += this.InputTextBox_Resize;
            this.LostFocus += this.InputTextBox_LostFocus;
            this.GotFocus += this.InputTextBox_GotFocus;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Return)
            {
                var args = new KeyEventArgs(keyData);
                this.OnKeyDown(args);
                return args.Handled;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void InputTextBox_Resize(object sender, EventArgs e)
        {
            this.VerticalAlignment();
        }

        private void InputTextBox_LostFocus(object sender, EventArgs e)
        {
            this.VerticalAlignment();
        }

        private void InputTextBox_GotFocus(object sender, EventArgs e)
        {
            this.VerticalAlignment();
        }

        private void VerticalAlignment()
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
                _ = WinApiMembers.SendMessage(this.Handle, WinApiMembers.EM_SETRECT, 0, ptr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }
    }
}
