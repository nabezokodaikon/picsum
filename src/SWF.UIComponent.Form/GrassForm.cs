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
            var dwmHandled = WinApiMembers.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, out var result);
            if (dwmHandled == 1)
            {
                m.Result = result;
                return;
            }

            if (m.Msg == WinApiMembers.WM_NCCALCSIZE && (int)m.WParam == 1)
            {
                if (this.glassMargins == null)
                {
                    var nccsp = (WinApiMembers.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(WinApiMembers.NCCALCSIZE_PARAMS));

                    this.glassMargins = new()
                    {
                        cyTopHeight = TOP_OFFSET,
                        cxLeftWidth = nccsp.rgrc2.left - nccsp.rgrc1.left,
                        cxRightWidth = nccsp.rgrc1.right - nccsp.rgrc2.right,
                        cyBottomHeight = nccsp.rgrc1.bottom - nccsp.rgrc2.bottom
                    };

                    nccsp.rgrc0.top -= 1;

                    Marshal.StructureToPtr(nccsp, m.LParam, true);
                }

                m.Result = IntPtr.Zero;
            }
            else if (m.Msg == WinApiMembers.WM_NCHITTEST && (int)m.Result == 0)
            {
                m.Result = this.HitTestNCA(m.LParam);
            }
            else
            {
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

            this.SettingtControlRegion();
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
            this.SettingtControlRegion();
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

        private void SettingtControlRegion()
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

        private IntPtr HitTestNCA(IntPtr lparam)
        {
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            Point p;
            checked
            {
                p = new Point(WinApiMembers.LoWord((int)lparam), WinApiMembers.HiWord((int)lparam));
            }

            var topleft = this.RectangleToScreen(new Rectangle(0, 0, this.glassMargins.cxLeftWidth, this.glassMargins.cxLeftWidth));
            if (topleft.Contains(p))
            {
                return new IntPtr(HTTOPLEFT);
            }

            var topright = this.RectangleToScreen(new Rectangle(this.Width - this.glassMargins.cxRightWidth, 0, this.glassMargins.cxRightWidth, this.glassMargins.cxRightWidth));
            if (topright.Contains(p))
            {
                return new IntPtr(HTTOPRIGHT);
            }

            var botleft = this.RectangleToScreen(new Rectangle(0, this.Height - this.glassMargins.cyBottomHeight, this.glassMargins.cxLeftWidth, this.glassMargins.cyBottomHeight));
            if (botleft.Contains(p))
            {
                return new IntPtr(HTBOTTOMLEFT);
            }

            var botright = this.RectangleToScreen(new Rectangle(this.Width - this.glassMargins.cxRightWidth, this.Height - this.glassMargins.cyBottomHeight, this.glassMargins.cxRightWidth, this.glassMargins.cyBottomHeight));
            if (botright.Contains(p))
            {
                return new IntPtr(HTBOTTOMRIGHT);
            }

            var top = this.RectangleToScreen(new Rectangle(0, 0, this.Width, this.glassMargins.cxLeftWidth));
            if (top.Contains(p))
            {
                return new IntPtr(HTTOP);
            }

            var left = this.RectangleToScreen(new Rectangle(0, 0, this.glassMargins.cxLeftWidth, this.Height));
            if (left.Contains(p))
            {
                return new IntPtr(HTLEFT);
            }

            var right = this.RectangleToScreen(new Rectangle(this.Width - this.glassMargins.cxRightWidth, 0, this.glassMargins.cxRightWidth, this.Height));
            if (right.Contains(p))
            {
                return new IntPtr(HTRIGHT);
            }

            var bottom = this.RectangleToScreen(new Rectangle(0, this.Height - this.glassMargins.cyBottomHeight, this.Width, this.glassMargins.cyBottomHeight));
            if (bottom.Contains(p))
            {
                return new IntPtr(HTBOTTOM);
            }

            var cap = this.RectangleToScreen(new Rectangle(0, this.glassMargins.cxLeftWidth, this.Width, this.glassMargins.cyTopHeight - this.glassMargins.cxLeftWidth));
            if (cap.Contains(p))
            {
                return new IntPtr(HTCAPTION);
            }

            return new IntPtr(HTCLIENT);
        }

    }
}
