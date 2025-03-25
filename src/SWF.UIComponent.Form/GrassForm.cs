using SWF.Core.Base;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.Form
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class GrassForm
        : System.Windows.Forms.Form
    {
        private const int TOP_OFFSET = 41;
        private const int RESIZE_MARGIN = 8;

        private static Version GetWindowsVersion()
        {
            var osVersionInfo = new WinApiMembers.OSVERSIONINFOEX();
            osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(WinApiMembers.OSVERSIONINFOEX));
            var _ = WinApiMembers.RtlGetVersion(ref osVersionInfo);
            return new Version(osVersionInfo.dwMajorVersion, osVersionInfo.dwMinorVersion, osVersionInfo.dwBuildNumber);
        }

        private WinApiMembers.MARGINS glassMargins = null;
        private bool isInit = true;
        private Size initSize = Size.Empty;
        private FormWindowState initWindowState = FormWindowState.Normal;
        private FormWindowState currentWindowState = FormWindowState.Normal;
        private Rectangle currentWindowBounds = Rectangle.Empty;
        private readonly Version osVersion = GetWindowsVersion();
        private readonly Color activeWindowColor = Color.FromArgb(34, 38, 41);
        private readonly Color deactivateWindowColor = Color.FromArgb(97, 99, 101);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size Size
        {
            get
            {
                if (this.isInit)
                {
                    return this.initSize;
                }
                else
                {
                    return base.Size;
                }
            }
            set
            {
                if (this.isInit)
                {
                    this.initSize = value;
                }
                else
                {
                    base.Size = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FormWindowState WindowState
        {
            get
            {
                if (this.isInit)
                {
                    return this.initWindowState;
                }
                else
                {
                    return base.WindowState;
                }
            }
            set
            {
                if (this.isInit)
                {
                    this.initWindowState = value;
                }
                else
                {
                    base.WindowState = value;
                }
            }
        }

        public GrassForm()
        {
            this.glassMargins = new()
            {
                cyTopHeight = TOP_OFFSET,
                cxLeftWidth = 0,
                cxRightWidth = 0,
                cyBottomHeight = 0,
            };

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ContainerControl |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.SetWindowColor(this.activeWindowColor);
        }

        public void MouseLeftDoubleClickProcess()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        public void RestoreWindowState()
        {
            this.Bounds = this.currentWindowBounds;
            this.WindowState = this.currentWindowState;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApiMembers.WM_NCCALCSIZE)
            {
                var nccsp = (WinApiMembers.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(
                    m.LParam, typeof(WinApiMembers.NCCALCSIZE_PARAMS));
                nccsp.rgrc0.left += 8;
                //nccsp.rgrc0.top += 8;
                nccsp.rgrc0.right -= 8;
                nccsp.rgrc0.bottom -= 8;
                Marshal.StructureToPtr(nccsp, m.LParam, false);
            }
            else
            {
                var dwmHandled = WinApiMembers.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, out var result);
                if (dwmHandled == 1)
                {
                    m.Result = result;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            this.SetWindowColor(this.activeWindowColor);
            base.OnActivated(e);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            this.SetWindowColor(this.deactivateWindowColor);
            base.OnDeactivate(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {

            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.currentWindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.currentWindowState = FormWindowState.Normal;
                this.currentWindowBounds = this.Bounds;
            }

            base.OnResize(e);

            this.SetControlRegion();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {

            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.currentWindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.currentWindowState = FormWindowState.Normal;
                this.currentWindowBounds = this.Bounds;
            }

            base.OnLocationChanged(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (this.isInit)
            {
                base.Size = this.initSize;
                base.WindowState = this.initWindowState;
                this.isInit = false;
            }

            base.OnLoad(e);
        }

        protected void SetGrass()
        {
            WinApiMembers.DwmExtendFrameIntoClientArea(this.Handle, this.glassMargins);
        }

        protected void SetControlRegion()
        {
            var w = AppConstants.GetControlBoxWidth();
            var h = AppConstants.GetControlBoxHeight();
            var p = new Point(this.Right - w, this.Top);
            var captionButtonRect = new Rectangle(p.X, p.Y, w, h);
            foreach (Control ctl in this.Controls)
            {
                ctl.Region = this.GetControlRegion(ctl, captionButtonRect);
            }
        }

        protected void AttachResizeEvents(Control current)
        {
            current.MouseMove += this.ChildMouseMoveHandler;
            current.MouseDown += this.ChildMouseDownHandler;

            foreach (Control child in current.Controls)
            {
                this.AttachResizeEvents(child);
            }
        }

        protected void DetachResizeEvents(Control current)
        {
            current.MouseMove -= this.ChildMouseMoveHandler;
            current.MouseDown -= this.ChildMouseDownHandler;

            foreach (Control child in current.Controls)
            {
                this.DetachResizeEvents(child);
            }
        }

        private void SetWindowColor(Color color)
        {
            if (this.osVersion.Major >= 10 && this.osVersion.Build >= 22000)
            {
                var colorValue = color.R | (color.G << 8) | (color.B << 16);
                var _ = WinApiMembers.DwmSetWindowAttribute(
                    this.Handle, WinApiMembers.DWMWA_CAPTION_COLOR, ref colorValue, sizeof(int));
                _ = WinApiMembers.DwmSetWindowAttribute(
                    this.Handle, WinApiMembers.DWMWA_BORDER_COLOR, ref colorValue, Marshal.SizeOf<int>());
            }
        }

        private Region GetControlRegion(Control ctl, Rectangle captionButtonRect)
        {
            var ctlScreenRect = new Rectangle(this.PointToScreen(ctl.Location), ctl.Size);
            if (ctlScreenRect.IntersectsWith(captionButtonRect))
            {
                var crossScreenRect = Rectangle.Intersect(ctlScreenRect, captionButtonRect);
                var crossControlPoint = ctl.PointToClient(crossScreenRect.Location);
                using (var path = new GraphicsPath())
                {
                    path.AddRectangle(new Rectangle(0, 0, ctl.Width, ctl.Height));
                    path.AddRectangle(new Rectangle(crossControlPoint, crossScreenRect.Size));
                    return new Region(path);
                }
            }
            else
            {
                return new Region(new Rectangle(0, 0, ctl.Width, ctl.Height));
            }
        }

        private void ChildMouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (this.WindowState != FormWindowState.Normal)
            {
                return;
            }

            var control = sender as Control;
            this.Cursor = Cursors.Default;
            control.Cursor = Cursors.Default;

            var point = this.PointToClient(control.PointToScreen(e.Location));

            if (point.X <= RESIZE_MARGIN
                && point.Y <= RESIZE_MARGIN)
            {
                // 左上
                control.Cursor = Cursors.SizeNWSE;
            }
            //else if (point.X <= RESIZE_MARGIN &&
            //     point.Y >= this.ClientSize.Height - RESIZE_MARGIN)
            //{
            //    // 左下
            //    control.Cursor = Cursors.SizeNESW;
            //}
            else if (point.X >= this.ClientSize.Width - RESIZE_MARGIN
                && point.Y <= RESIZE_MARGIN)
            {
                // 右上
                control.Cursor = Cursors.SizeNESW;
            }
            //else if (point.X >= this.ClientSize.Width - RESIZE_MARGIN
            //    && point.Y >= this.ClientSize.Height - RESIZE_MARGIN)
            //{
            //    // 右下
            //    control.Cursor = Cursors.SizeNWSE;
            //}
            //else if (point.X <= RESIZE_MARGIN)
            //{
            //    // 左
            //    control.Cursor = Cursors.SizeWE;
            //}
            else if (point.Y <= RESIZE_MARGIN)
            {
                // 上
                control.Cursor = Cursors.SizeNS;
            }
            //else if (point.X >= this.ClientSize.Width - RESIZE_MARGIN)
            //{
            //    // 右
            //    control.Cursor = Cursors.SizeWE;
            //}
            //else if (point.Y >= this.ClientSize.Height - RESIZE_MARGIN)
            //{
            //    // 下
            //    control.Cursor = Cursors.SizeNS;
            //}
        }

        private void ChildMouseDownHandler(object sender, MouseEventArgs e)
        {
            if (this.WindowState != FormWindowState.Normal)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var control = sender as Control;
            var point = this.PointToClient(control.PointToScreen(e.Location));
            var hitTest = 0;

            if (point.X <= RESIZE_MARGIN
                && point.Y <= RESIZE_MARGIN)
            {
                // 左上
                hitTest = WinApiMembers.HTTOPLEFT;
            }
            //else if (point.X <= RESIZE_MARGIN &&
            //     point.Y >= this.ClientSize.Height - RESIZE_MARGIN)
            //{
            //    // 左下
            //    hitTest = HTBOTTOMLEFT;
            //}
            else if (point.X >= this.ClientSize.Width - RESIZE_MARGIN
                && point.Y <= RESIZE_MARGIN)
            {
                // 右上
                hitTest = WinApiMembers.HTTOPRIGHT;
            }
            //else if (point.X >= this.ClientSize.Width - RESIZE_MARGIN
            //    && point.Y >= this.ClientSize.Height - RESIZE_MARGIN)
            //{
            //    // 右下
            //    hitTest = HTBOTTOMRIGHT;
            //}
            //else if (point.X <= RESIZE_MARGIN)
            //{
            //    // 左
            //    hitTest = HTLEFT;
            //}
            else if (point.Y <= RESIZE_MARGIN)
            {
                // 上
                hitTest = WinApiMembers.HTTOP;
            }
            //else if (point.X >= this.ClientSize.Width - RESIZE_MARGIN)
            //{
            //    // 右
            //    hitTest = HTRIGHT;
            //}
            //else if (point.Y >= this.ClientSize.Height - RESIZE_MARGIN)
            //{
            //    // 下
            //    hitTest = HTBOTTOM;
            //}

            if (hitTest != 0)
            {
                WinApi.WinApiMembers.ReleaseCapture();
                WinApi.WinApiMembers.SendMessage(this.Handle, 0xA1, hitTest, 0);
            }
        }
    }
}
