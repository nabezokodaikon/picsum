using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.Form
{
    [SupportedOSPlatform("windows")]
    public class GrassForm
        : System.Windows.Forms.Form
    {
        #region クラスメンバ

        private const int TOP_OFFSET = 41;

        #endregion

        #region インスタンス変数

        private WinApiMembers.MARGINS glassMargins = null;
        private bool isInit = true;
        private Size initSize = Size.Empty;
        private FormWindowState initWindowState = FormWindowState.Normal;

        #endregion

        #region パブリックプロパティ

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

        #endregion

        #region コンストラクタ

        public GrassForm()
        {
            this.InitializeComponent();
        }

        #endregion

        #region パブリックメソッド

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

        #endregion

        #region 継承メソッド

        protected override void WndProc(ref Message m)
        {
            int dwmHandled = WinApiMembers.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, out var result);
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.SettingtControlRegion();
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

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ContainerControl |
                ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();
        }

        private void SettingtControlRegion()
        {
            var frameWidth = WinApiMembers.GetSystemMetrics(WinApiMembers.SM.CXSIZEFRAME);
            var buttonWidth = WinApiMembers.GetSystemMetrics(WinApiMembers.SM.CXSIZE) * 3;

            // TODO: コントロールボックスのサイズ。
            // ハードコーディングでなく、WinApiで取得できるようにする。
            var w = frameWidth + buttonWidth + 40;
            var h = WinApiMembers.GetSystemMetrics(WinApiMembers.SM.CYCAPTION) + 16;

            if (this.WindowState == FormWindowState.Maximized)
            {
                var screen = Screen.FromControl(this);
                w -= (screen.Bounds.X - this.Location.X);
                h -= (screen.Bounds.Y - this.Location.Y);
            }

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

            var p = new Point(WinApiMembers.LoWord((int)lparam), WinApiMembers.HiWord((int)lparam));

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

        #endregion
    }
}
