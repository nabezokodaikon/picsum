using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.Form
{
    public class GrassForm : System.Windows.Forms.Form
    {
        #region 定数・列挙

        private const int DEFAULT_GRASS_MARGIN = 8;

        #endregion

        #region イベント・デリゲート

        public event EventHandler DwmCompositionChanged;

        #endregion

        #region インスタンス変数

        private readonly bool _isGlassWindowsVersion = (6 <= Environment.OSVersion.Version.Major);
        private WinApiMembers.MARGINS _glassMargins = null;
        private Point _mouseDownCursorPoint = Point.Empty;
        private Rectangle _mouseDownFormRectangle = Rectangle.Empty;
        private int _topOffset = 31;
        private bool _isInit = true;
        private Size _initSize = Size.Empty;
        private FormWindowState _initWindowState = FormWindowState.Normal;
        private bool _isMaximum = false;
        private bool _isSizeRestored = false;
        private Size _restoredSize = Size.Empty;

        #endregion

        #region パブリックプロパティ

        public bool IsGrassEnabled
        {
            get
            {
                return isGrassEnabled;
            }
        }

        public int TopOffset
        {
            get
            {
                return _topOffset;
            }
            set
            {
                _topOffset = value;
            }
        }

        public new Size Size
        {
            get
            {
                if (isGrassEnabled)
                {
                    if (_isInit)
                    {
                        return _initSize;
                    }
                    else
                    {
                        return base.Size;
                    }
                }
                else
                {
                    return base.Size;
                }
            }
            set
            {
                if (isGrassEnabled)
                {
                    if (_isInit)
                    {
                        _initSize = value;
                    }
                    else
                    {
                        base.Size = value;
                    }
                }
                else
                {
                    _initSize = value;
                    base.Size = value;
                }
            }
        }

        public new FormWindowState WindowState
        {
            get
            {
                if (isGrassEnabled)
                {
                    if (_isInit)
                    {
                        return _initWindowState;
                    }
                    else
                    {
                        return base.WindowState;
                    }
                }
                else
                {
                    return base.WindowState;
                }
            }
            set
            {
                if (isGrassEnabled)
                {
                    if (_isInit)
                    {
                        _initWindowState = value;
                    }
                    else
                    {
                        base.WindowState = value;
                    }
                }
                else
                {
                    _initWindowState = value;
                    base.WindowState = value;
                }
            }
        }

        #endregion

        #region 継承プロパティ

        protected bool IsInit
        {
            get
            {
                return _isInit;
            }
        }

        #endregion

        #region プライベートプロパティ

        private bool isSizeRestored
        {
            get
            {
                return _isMaximum && _isSizeRestored;
            }
            set
            {
                _isMaximum = false;
                _isSizeRestored = false;
            }
        }

        private bool isGrassEnabled
        {
            get
            {
                return _isGlassWindowsVersion && WinApiMembers.DwmIsCompositionEnabled();
            }
        }

        #endregion

        #region コンストラクタ

        public GrassForm()
        {
            initializeComponent();
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
            if (m.Msg == WinApiMembers.WM_DWMCOMPOSITIONCHANGED)
            {
                _glassMargins = null;
                isSizeRestored = false;
                setWindowPos();
                OnDwmCompositionChanged(new EventArgs());
                base.WndProc(ref m);
            }

            if (isGrassEnabled)
            {
                grassWndProc(ref m);
            }
            else
            {
                classicWndProc(ref m);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (isGrassEnabled)
            {
                setControlRegion();
            }
            else
            {
                reSetControlRegion();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
        }

        protected override void OnShown(EventArgs e)
        {
            if (_isInit)
            {
                base.Size = _initSize;
                base.WindowState = _initWindowState;
                _isInit = false;
            }

            base.OnShown(e);
        }

        protected void SetGrass()
        {
            if (isGrassEnabled)
            {
                WinApiMembers.DwmExtendFrameIntoClientArea(this.Handle, _glassMargins);
            }
        }

        protected void ResetGrass()
        {
            if (isGrassEnabled)
            {
                WinApiMembers.DWM_BLURBEHIND bbhOff = new WinApiMembers.DWM_BLURBEHIND();
                bbhOff.dwFlags = WinApiMembers.DWM_BLURBEHIND.DWM_BB_ENABLE | WinApiMembers.DWM_BLURBEHIND.DWM_BB_BLURREGION;
                bbhOff.fEnable = false;
                bbhOff.hRegionBlur = IntPtr.Zero;
                WinApiMembers.DwmEnableBlurBehindWindow(this.Handle, bbhOff);
            }
        }

        protected void SetControlRegion()
        {
            if (isGrassEnabled)
            {
                setControlRegion();
            }
            else
            {
                reSetControlRegion();
            }
        }

        protected virtual void OnDwmCompositionChanged(EventArgs e)
        {
            if (DwmCompositionChanged != null)
            {
                DwmCompositionChanged(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true);
        }

        private void grassWndProc(ref Message m)
        {
            IntPtr result;
            int dwmHandled = WinApiMembers.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, out result);
            if (dwmHandled == 1)
            {
                m.Result = result;
                return;
            }

            if (m.Msg == WinApiMembers.WM_WINDOWPOSCHANGING)
            {
                if (isSizeRestored)
                {
                    WinApiMembers.WINDOWPOS wp = (WinApiMembers.WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(WinApiMembers.WINDOWPOS));
                    wp.cx = _restoredSize.Width;
                    wp.cy = _restoredSize.Height;
                    Marshal.StructureToPtr(wp, m.LParam, true);
                    isSizeRestored = false;
                }

                base.WndProc(ref m);
            }
            else if (m.Msg == WinApiMembers.WM_SIZE)
            {
                int wParam = (int)m.WParam;
                if (wParam == WinApiMembers.SIZE_RESTORED)
                {
                    if (_isMaximum)
                    {
                        _isSizeRestored = true;
                    }

                    if (!isSizeRestored)
                    {
                        int lParam = (int)m.LParam;
                        int w = WinApiMembers.LoWord(lParam);
                        int h = WinApiMembers.HiWord(lParam);
                        _restoredSize = new Size(w, h);
                    }
                }
                else if (wParam == WinApiMembers.SIZE_MINIMIZED)
                {
                    _isMaximum = true;
                }
                else if (wParam == WinApiMembers.SIZE_MAXIMIZED)
                {
                    _isMaximum = true;
                }

                base.WndProc(ref m);
            }
            else if (m.Msg == WinApiMembers.WM_ACTIVATE)
            {
                int wParam = ((int)m.WParam) & 0xFFFF;
                if (wParam == WinApiMembers.WA_ACTIVE)
                {
                    setWindowPos();
                }

                base.WndProc(ref m);
            }
            else if (m.Msg == WinApiMembers.WM_NCCALCSIZE && (int)m.WParam == 1)
            {
                if (_glassMargins == null)
                {
                    WinApiMembers.NCCALCSIZE_PARAMS nccsp = (WinApiMembers.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(WinApiMembers.NCCALCSIZE_PARAMS));

                    _glassMargins = new WinApiMembers.MARGINS();

                    _glassMargins.cyTopHeight = _topOffset;

                    _glassMargins.cxLeftWidth = nccsp.rgrc2.left - nccsp.rgrc1.left;
                    if (_glassMargins.cxLeftWidth <= 0)
                    {
                        _glassMargins.cxLeftWidth = DEFAULT_GRASS_MARGIN;
                    }

                    _glassMargins.cxRightWidth = nccsp.rgrc1.right - nccsp.rgrc2.right;
                    if (_glassMargins.cxRightWidth <= 0)
                    {
                        _glassMargins.cxRightWidth = DEFAULT_GRASS_MARGIN;
                    }

                    _glassMargins.cyBottomHeight = nccsp.rgrc1.bottom - nccsp.rgrc2.bottom;
                    if (_glassMargins.cyBottomHeight <= 0)
                    {
                        _glassMargins.cyBottomHeight = DEFAULT_GRASS_MARGIN;
                    }

                    nccsp.rgrc0.top -= 1;

                    Marshal.StructureToPtr(nccsp, m.LParam, true);
                }

                m.Result = IntPtr.Zero;
            }
            else if (m.Msg == WinApiMembers.WM_NCHITTEST && (int)m.Result == 0)
            {
                m.Result = hitTestNCA(m.HWnd, m.WParam, m.LParam);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void classicWndProc(ref Message m)
        {
            if (m.Msg == WinApiMembers.WM_NCCALCSIZE && (int)m.WParam == 1)
            {
                if (_glassMargins == null)
                {
                    _glassMargins = new WinApiMembers.MARGINS();
                }
            }

            base.WndProc(ref m);
        }

        private void setWindowPos()
        {
            WinApiMembers.RECT rect;
            WinApiMembers.GetWindowRect(this.Handle, out rect);
            WinApiMembers.SetWindowPos(this.Handle,
                                       WinApiMembers.HWND.HWND_TOP,
                                       rect.left,
                                       rect.top,
                                       WinApiMembers.RECTWIDTH(rect),
                                       WinApiMembers.RECTHEIGHT(rect),
                                       WinApiMembers.SWP_FRAMECHANGED);
        }

        private void moveAction(Point p)
        {
            int moveWidth = p.X - _mouseDownCursorPoint.X;
            int moveHeight = p.Y - _mouseDownCursorPoint.Y;
            Rectangle rect = _mouseDownFormRectangle;
            this.Left = rect.Left + moveWidth;
            this.Top = rect.Top + moveHeight;
        }

        private void reSetControlRegion()
        {
            foreach (Control ctl in this.Controls)
            {
                ctl.Region = new Region(new Rectangle(0, 0, ctl.Width, ctl.Height));
            }
        }

        private void setControlRegion()
        {
            int frameWidth = WinApiMembers.GetSystemMetrics(WinApiMembers.SM.CXSIZEFRAME);
            int buttonWidth = WinApiMembers.GetSystemMetrics(WinApiMembers.SM.CXSIZE) * 3;

            // TODO: コントロールボックスのサイズ。
            // ハードコーディングでなく、WinApiで取得できるようにする。
            int w = frameWidth + buttonWidth + 40;
            int h = WinApiMembers.GetSystemMetrics(WinApiMembers.SM.CYCAPTION) + 18;

            if (this.WindowState == FormWindowState.Maximized)
            {
                Screen screen = Screen.FromControl(this);
                w += (screen.Bounds.X - this.Location.X);
                h += (screen.Bounds.Y - this.Location.Y);
            }

            Point p = new Point(this.Right - w, this.Top);
            Rectangle captionButtonRect = new Rectangle(p.X, p.Y, w, h);
            foreach (Control ctl in this.Controls)
            {
                ctl.Region = getControlRegion(ctl, captionButtonRect);
            }
        }

        private Region getControlRegion(Control ctl, Rectangle captionButtonRect)
        {
            Rectangle ctlScreenRect = new Rectangle(PointToScreen(ctl.Location), ctl.Size);
            if (ctlScreenRect.IntersectsWith(captionButtonRect))
            {
                Rectangle crossScreenRect = Rectangle.Intersect(ctlScreenRect, captionButtonRect);
                Point crossControlPoint = ctl.PointToClient(crossScreenRect.Location);
                using (GraphicsPath path = new GraphicsPath())
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

        private IntPtr hitTestNCA(IntPtr hwnd, IntPtr wparam, IntPtr lparam)
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

            Point p = new Point(WinApiMembers.LoWord((int)lparam), WinApiMembers.HiWord((int)lparam));

            Rectangle topleft = RectangleToScreen(new Rectangle(0, 0, _glassMargins.cxLeftWidth, _glassMargins.cxLeftWidth));
            if (topleft.Contains(p))
            {
                return new IntPtr(HTTOPLEFT);
            }

            Rectangle topright = RectangleToScreen(new Rectangle(Width - _glassMargins.cxRightWidth, 0, _glassMargins.cxRightWidth, _glassMargins.cxRightWidth));
            if (topright.Contains(p))
            {
                return new IntPtr(HTTOPRIGHT);
            }

            Rectangle botleft = RectangleToScreen(new Rectangle(0, Height - _glassMargins.cyBottomHeight, _glassMargins.cxLeftWidth, _glassMargins.cyBottomHeight));
            if (botleft.Contains(p))
            {
                return new IntPtr(HTBOTTOMLEFT);
            }

            Rectangle botright = RectangleToScreen(new Rectangle(Width - _glassMargins.cxRightWidth, Height - _glassMargins.cyBottomHeight, _glassMargins.cxRightWidth, _glassMargins.cyBottomHeight));
            if (botright.Contains(p))
            {
                return new IntPtr(HTBOTTOMRIGHT);
            }

            Rectangle top = RectangleToScreen(new Rectangle(0, 0, Width, _glassMargins.cxLeftWidth));
            if (top.Contains(p))
            {
                return new IntPtr(HTTOP);
            }

            Rectangle left = RectangleToScreen(new Rectangle(0, 0, _glassMargins.cxLeftWidth, Height));
            if (left.Contains(p))
            {
                return new IntPtr(HTLEFT);
            }

            Rectangle right = RectangleToScreen(new Rectangle(Width - _glassMargins.cxRightWidth, 0, _glassMargins.cxRightWidth, Height));
            if (right.Contains(p))
            {
                return new IntPtr(HTRIGHT);
            }

            Rectangle bottom = RectangleToScreen(new Rectangle(0, Height - _glassMargins.cyBottomHeight, Width, _glassMargins.cyBottomHeight));
            if (bottom.Contains(p))
            {
                return new IntPtr(HTBOTTOM);
            }

            Rectangle cap = RectangleToScreen(new Rectangle(0, _glassMargins.cxLeftWidth, Width, _glassMargins.cyTopHeight - _glassMargins.cxLeftWidth));
            if (cap.Contains(p))
            {
                return new IntPtr(HTCAPTION);
            }

            return new IntPtr(HTCLIENT);
        }

        #endregion
    }
}
