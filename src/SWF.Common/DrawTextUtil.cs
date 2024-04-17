using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using WinApi;

namespace SWF.Common
{
    [SupportedOSPlatform("windows")]
    public static class DrawTextUtil
    {
        public enum TextStyle
        {
            Normal,
            Glowing
        }

        private readonly static bool IS_SUPPORTED_DRAW_THEME_TEXT_EX_WINDOWS_VERSION = (6 <= Environment.OSVersion.Version.Major);

        public static void DrawText(Graphics srcDc, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags, TextStyle style)
        {
            if (IS_SUPPORTED_DRAW_THEME_TEXT_EX_WINDOWS_VERSION && IsSupportedTheme())
            {
                DrawGrassText(srcDc, text, font, bounds, color, flags, style);
            }
            else
            {
                DrawClassicText(srcDc, text, font, bounds, color, flags);
            }
        }

        private static void DrawClassicText(Graphics srcDc, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags)
        {
            TextRenderer.DrawText(srcDc, text, font, bounds, color, flags);
        }

        private static void DrawGrassText(Graphics srcDc, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags, TextStyle style)
        {
            var srcHdc = srcDc.GetHdc();

            // Create a memory DC so we can work offscreen
            var memoryHdc = WinApiMembers.CreateCompatibleDC(srcHdc);

            // Create a device-independent bitmap and select it into our DC
            var bi = new WinApiMembers.BITMAPINFO();
            bi.biSize = Marshal.SizeOf(bi);
            bi.biWidth = bounds.Width;
            bi.biHeight = -bounds.Height;
            bi.biPlanes = 1;
            bi.biBitCount = 32;
            bi.biCompression = 0; // BI_RGB
            var dib = WinApiMembers.CreateDIBSection(srcHdc, bi, 0, 0, IntPtr.Zero, 0);
            WinApiMembers.SelectObject(memoryHdc, dib);

            // Create and select font
            var fontHandle = font.ToHfont();
            WinApiMembers.SelectObject(memoryHdc, fontHandle);

            // Draw glowing text
            var renderer = new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);
            var dttOpts = new WinApiMembers.DTTOPTS
            {
                dwSize = Marshal.SizeOf(typeof(WinApiMembers.DTTOPTS)),
                dwFlags = GetDwFlags(style),

                crText = ColorTranslator.ToWin32(color),
                iGlowSize = 8 // This is about the size Microsoft Word 2007 uses
            };
            var textBounds = new WinApiMembers.RECT(0, 0, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);

            WinApiMembers.BitBlt(memoryHdc, 0, 0, bounds.Width, bounds.Height, srcHdc, bounds.Left, bounds.Top, WinApiMembers.SRCCOPY);

            var _ = WinApiMembers.DrawThemeTextEx(renderer.Handle, memoryHdc, 0, 0, text, -1, (int)flags, ref textBounds, ref dttOpts);

            // Copy to foreground
            WinApiMembers.BitBlt(srcHdc, bounds.Left, bounds.Top, bounds.Width, bounds.Height, memoryHdc, 0, 0, WinApiMembers.SRCCOPY);

            // Clean up
            WinApiMembers.DeleteObject(fontHandle);
            WinApiMembers.DeleteObject(dib);
            WinApiMembers.DeleteDC(memoryHdc);

            srcDc.ReleaseHdc(srcHdc);
        }

        private static bool IsSupportedTheme()
        {
            if (VisualStyleInformation.IsSupportedByOS &&
                VisualStyleInformation.IsEnabledByUser)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static int GetDwFlags(TextStyle style)
        {
            if (style == TextStyle.Glowing)
            {
                return WinApiMembers.DTT_COMPOSITED |
                       WinApiMembers.DTT_GLOWSIZE |
                       WinApiMembers.DTT_TEXTCOLOR;
            }
            else
            {
                return WinApiMembers.DTT_COMPOSITED |
                       WinApiMembers.DTT_TEXTCOLOR;
            }
        }
    }
}
