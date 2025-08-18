using SWF.Core.Base;
using SWF.UIComponent.Base;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.Form
{

    public partial class GrassForm
        : BaseForm
    {
        private const int TOP_OFFSET = 37;
        private const int RESIZE_MARGIN = 8;

        private static readonly Version OS_VERSION = GetWindowsVersion();
        private static readonly Color ACTIVE_WINDOW_COLOR = Color.FromArgb(64, 68, 71);
        private static readonly Color DEACTIVATE_WINDOWCOLOR = Color.FromArgb(107, 109, 111);

        private static Version GetWindowsVersion()
        {
            var osVersionInfo = new WinApiMembers.OSVERSIONINFOEX
            {
                dwOSVersionInfoSize = Marshal.SizeOf(typeof(WinApiMembers.OSVERSIONINFOEX))
            };
            var _ = WinApiMembers.RtlGetVersion(ref osVersionInfo);
            return new Version(osVersionInfo.dwMajorVersion, osVersionInfo.dwMinorVersion, osVersionInfo.dwBuildNumber);
        }

        private static void SetDefaultCrusor(Control control)
        {
            if (control.Cursor == Cursors.SizeNS
                || control.Cursor == Cursors.SizeWE
                || control.Cursor == Cursors.SizeNWSE
                || control.Cursor == Cursors.SizeNESW)
            {
                control.Cursor = Cursors.Default;
            }
        }

        private static WinApiMembers.MARGINS GetGrassMargins(float scale)
        {
            return new()
            {
                cyTopHeight = (int)(TOP_OFFSET * scale),
                cxLeftWidth = 0,
                cxRightWidth = 0,
                cyBottomHeight = 0,
            };
        }

        public event EventHandler<ScaleChangedEventArgs> ScaleChanged;

        private float _currentScale = 0;
        private bool _isInit = true;
        private Size _initSize = Size.Empty;
        private FormWindowState _initWindowState = FormWindowState.Normal;
        private FormWindowState _currentWindowState = FormWindowState.Normal;
        private Rectangle _currentWindowBounds = Rectangle.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size Size
        {
            get
            {
                if (this._isInit)
                {
                    return this._initSize;
                }
                else
                {
                    return base.Size;
                }
            }
            set
            {
                if (this._isInit)
                {
                    this._initSize = value;
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
                if (this._isInit)
                {
                    return this._initWindowState;
                }
                else
                {
                    return base.WindowState;
                }
            }
            set
            {
                if (this._isInit)
                {
                    this._initWindowState = value;
                }
                else
                {
                    base.WindowState = value;
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= WinApiMembers.WS_EX_COMPOSITED;
                return cp;
            }
        }

        public GrassForm()
        {
            //this.SetWindowColor(ACTIVE_WINDOW_COLOR);

            this.Activated += this.GrassForm_Activated;
            this.Deactivate += this.GrassForm_Deactivate;
            this.Resize += this.GrassForm_Resize;
            this.LocationChanged += this.GrassForm_LocationChanged;
            this.Load += this.GrassForm_Load;
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
            this.Bounds = this._currentWindowBounds;
            this.WindowState = this._currentWindowState;
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
                return;
            }
            else if (m.Msg == WinApiMembers.WM_DPICHANGED)
            {
                var scale = WindowUtil.GetCurrentWindowScale(this);
                if (this._currentScale == 0 || this._currentScale != scale)
                {
                    this._currentScale = scale;
                    var glassMargins = GetGrassMargins(this._currentScale);
                    WinApiMembers.DwmExtendFrameIntoClientArea(this.Handle, glassMargins);
                    this.ScaleChanged?.Invoke(this, new ScaleChangedEventArgs(this._currentScale));
                    return;
                }
            }
            else if (m.Msg == WinApiMembers.WM_GETMINMAXINFO)
            {
                var minMaxInfo = (WinApiMembers.MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(WinApiMembers.MINMAXINFO));
                minMaxInfo.ptMinTrackSize.x = 480;
                minMaxInfo.ptMinTrackSize.y = 360;
                Marshal.StructureToPtr(minMaxInfo, m.LParam, true);
            }

            if (m.Msg == WinApiMembers.WM_NCPAINT ||
                m.Msg == WinApiMembers.WM_NCACTIVATE ||
                m.Msg == WinApiMembers.WM_NCHITTEST ||
                m.Msg == WinApiMembers.WM_SETREDRAW ||
                m.Msg == WinApiMembers.WM_DPICHANGED ||
                m.Msg >= WinApiMembers.WM_DWM_FIRST && m.Msg <= WinApiMembers.WM_DWM_LAST)
            {
                var dwmHandled = WinApiMembers.DwmDefWindowProc(
                    m.HWnd, m.Msg, m.WParam, m.LParam, out var result);
                if (dwmHandled == 1)
                {
                    m.Result = result;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        private void GrassForm_Activated(object sender, EventArgs e)
        {
            this.SetWindowColor(ACTIVE_WINDOW_COLOR);
        }

        private void GrassForm_Deactivate(object sender, EventArgs e)
        {
            //this.SetWindowColor(DEACTIVATE_WINDOWCOLOR);
        }

        private void GrassForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {

            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this._currentWindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this._currentWindowState = FormWindowState.Normal;
                this._currentWindowBounds = this.Bounds;
            }

            this.SetControlRegion();
        }

        private void GrassForm_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {

            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this._currentWindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this._currentWindowState = FormWindowState.Normal;
                this._currentWindowBounds = this.Bounds;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        private void GrassForm_Load(object sender, EventArgs e)
        {
            if (this._isInit)
            {
                base.Size = this._initSize;
                base.WindowState = this._initWindowState;

                this._currentScale = WindowUtil.GetCurrentWindowScale(this);
                var glassMargins = GetGrassMargins(this._currentScale);
                WinApiMembers.DwmExtendFrameIntoClientArea(this.Handle, glassMargins);

                this._isInit = false;
            }
        }

        protected void SetControlRegion()
        {
            var size = WindowUtil.GetControlBoxSize(this.Handle);
            var p = new Point(this.Right - size.Width, this.Top);
            var captionButtonRect = new Rectangle(p.X, p.Y, size.Width, size.Height);
            foreach (Control ctl in this.Controls)
            {
                ctl.Region = this.GetControlRegion(ctl, captionButtonRect);
            }
        }

        protected void AttachResizeEvents(Control current)
        {
            ArgumentNullException.ThrowIfNull(current, nameof(current));

            current.MouseMove += this.ChildMouseMoveHandler;
            current.MouseDown += this.ChildMouseDownHandler;

            foreach (Control child in current.Controls)
            {
                this.AttachResizeEvents(child);
            }
        }

        protected void DetachResizeEvents(Control current)
        {
            ArgumentNullException.ThrowIfNull(current, nameof(current));

            current.MouseMove -= this.ChildMouseMoveHandler;
            current.MouseDown -= this.ChildMouseDownHandler;

            foreach (Control child in current.Controls)
            {
                this.DetachResizeEvents(child);
            }
        }

        protected virtual bool CanDragOperation()
        {
            throw new NotImplementedException();
        }

        private void SetWindowColor(Color color)
        {
            using (TimeMeasuring.Run(true, "GrassForm.SetWindowColor"))
            {
                if (OS_VERSION.Major >= 10 && OS_VERSION.Build >= 22000)
                {
                    var colorValue = color.R | (color.G << 8) | (color.B << 16);
                    var _ = WinApiMembers.DwmSetWindowAttribute(
                        this.Handle, WinApiMembers.DWMWA_CAPTION_COLOR, ref colorValue, sizeof(int));
                    _ = WinApiMembers.DwmSetWindowAttribute(
                        this.Handle, WinApiMembers.DWMWA_BORDER_COLOR, ref colorValue, Marshal.SizeOf<int>());
                }
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
            var control = sender as Control;
            SetDefaultCrusor(this);
            SetDefaultCrusor(control);

            if (!this.CanDragOperation())
            {
                return;
            }

            if (this.WindowState != FormWindowState.Normal)
            {
                return;
            }

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
            if (!this.CanDragOperation())
            {
                return;
            }

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
                _ = WinApi.WinApiMembers.SendMessage(this.Handle, 0xA1, hitTest, 0);
            }
        }
    }
}
