using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms.VisualStyles;
using WinApi;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class DrawTextUtil
    {
        public static void DrawText(Graphics srcDc, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags)
        {
            DrawGrassText(srcDc, text, font, bounds, color, flags);
        }

        private static void DrawGrassText(Graphics srcDc, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags)
        {
            var srcHdc = IntPtr.Zero;
            var memoryHdc = IntPtr.Zero;
            var dib = IntPtr.Zero;
            var oldDib = IntPtr.Zero;
            var fontHandle = IntPtr.Zero;
            var oldFont = IntPtr.Zero;

            try
            {
                srcHdc = srcDc.GetHdc();

                // Create a memory DC so we can work offscreen
                memoryHdc = WinApiMembers.CreateCompatibleDC(srcHdc);

                // Create a device-independent bitmap and select it into our DC
                var bi = new WinApiMembers.BITMAPINFO();
                bi.biSize = Marshal.SizeOf(bi);
                bi.biWidth = bounds.Width;
                bi.biHeight = -bounds.Height;
                bi.biPlanes = 1;
                bi.biBitCount = 32;
                bi.biCompression = 0; // BI_RGB
                dib = WinApiMembers.CreateDIBSection(srcHdc, bi, 0, 0, IntPtr.Zero, 0);
                WinApiMembers.SelectObject(memoryHdc, dib);

                // Create and select font
                fontHandle = font.ToHfont();
                WinApiMembers.SelectObject(memoryHdc, fontHandle);

                // Draw glowing text
                var renderer = new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);
                var dttOpts = new WinApiMembers.DTTOPTS
                {
                    dwSize = Marshal.SizeOf(typeof(WinApiMembers.DTTOPTS)),
                    dwFlags = GetDwFlags(),

                    crText = ColorTranslator.ToWin32(color),
                    iGlowSize = 8 // This is about the size Microsoft Word 2007 uses
                };
                var textBounds = new WinApiMembers.RECT(0, 0, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);

                WinApiMembers.BitBlt(memoryHdc, 0, 0, bounds.Width, bounds.Height, srcHdc, bounds.Left, bounds.Top, WinApiMembers.SRCCOPY);

                var _ = WinApiMembers.DrawThemeTextEx(renderer.Handle, memoryHdc, 0, 0, text, -1, (int)flags, ref textBounds, ref dttOpts);

                // Copy to foreground
                WinApiMembers.BitBlt(srcHdc, bounds.Left, bounds.Top, bounds.Width, bounds.Height, memoryHdc, 0, 0, WinApiMembers.SRCCOPY);
            }
            finally
            {
                if (memoryHdc != IntPtr.Zero)
                {
                    if (oldFont != IntPtr.Zero)
                    {
                        WinApiMembers.SelectObject(memoryHdc, oldFont);
                    }

                    if (oldDib != IntPtr.Zero)
                    {
                        WinApiMembers.SelectObject(memoryHdc, oldDib);
                    }

                    WinApiMembers.DeleteDC(memoryHdc);
                }

                if (fontHandle != IntPtr.Zero)
                {
                    WinApiMembers.DeleteObject(fontHandle);
                }

                if (dib != IntPtr.Zero)
                {
                    WinApiMembers.DeleteObject(dib);
                }

                if (srcHdc != IntPtr.Zero)
                {
                    srcDc.ReleaseHdc(srcHdc);
                }
            }
        }

        private static int GetDwFlags()
        {
            return WinApiMembers.DTT_COMPOSITED |
                   WinApiMembers.DTT_GLOWSIZE |
                   WinApiMembers.DTT_TEXTCOLOR;
        }
    }
}
