using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinApi;

namespace SWF.Core.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class WindowUtil
    {
        private const float BASE_DPI = 96f;

        public static float GetCursorWindowScale()
        {
            using (TimeMeasuring.Run(false, "WindowUtil.GetCursorWindowScale"))
            {
                var hwnd = WinApiMembers.WindowFromPoint(
                    new WinApiMembers.POINT(Cursor.Position.X, Cursor.Position.Y));
                var dpi = WinApiMembers.GetDpiForWindow(hwnd);
                var scale = dpi / BASE_DPI;
                return scale;
            }
        }

        public static float GetCurrentWindowScale(Control control)
        {
            ArgumentNullException.ThrowIfNull(control, nameof(control));

            using (TimeMeasuring.Run(false, "WindowUtil.GetCurrentWindowScale"))
            {
                var dpi = WinApiMembers.GetDpiForWindow(control.Handle);
                var scale = dpi / BASE_DPI;
                return scale;
            }
        }

        public static Size GetControlBoxSize(IntPtr window)
        {
            if (WinApiMembers.DwmGetWindowAttribute(
                window,
                WinApiMembers.DWMWA_CAPTION_BUTTON_BOUNDS,
                out WinApiMembers.RECT rect,
                Marshal.SizeOf<WinApiMembers.RECT>()) == 0)
            {
                return new Size(
                    rect.right - rect.left,
                    rect.bottom - rect.top);
            }

            return Size.Empty;
        }
    }
}
