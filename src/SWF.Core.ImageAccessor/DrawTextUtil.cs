using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using WinApi;

namespace SWF.Core.ImageAccessor
{

    public static class DrawTextUtil
    {
        public static void DrawText(
            Graphics g,
            string text,
            Font font,
            Rectangle rect,
            Color color)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNullOrEmpty(text, nameof(text));

            const int maxLines = 2;
            const string ellipsis = "...";
            var lines = new List<string>();

            var startIndex = 0;
            var measureFlags = TextFormatFlags.NoPadding;

            while (startIndex < text.Length && lines.Count < maxLines)
            {
                var remainingText = text.Substring(startIndex);

                // 2分探索で最適な切断位置を見つける
                var left = 1;
                var right = remainingText.Length;
                var bestFit = 0;

                while (left <= right)
                {
                    var mid = (left + right) / 2;
                    var testLine = remainingText.Substring(0, mid);
                    var size = TextRenderer.MeasureText(
                        g, testLine, font, rect.Size, measureFlags);

                    if (size.Width <= rect.Width)
                    {
                        bestFit = mid;
                        left = mid + 1;
                    }
                    else
                    {
                        right = mid - 1;
                    }
                }

                if (bestFit == 0)
                    bestFit = 1; // 最低1文字

                var currentLine = remainingText.Substring(0, bestFit);

                // 最後の行で、まだテキストが残っている場合
                if (lines.Count == maxLines - 1 && startIndex + bestFit < text.Length)
                {
                    // 三点リーダ用のスペース確保（再度2分探索）
                    left = 1;
                    right = bestFit;
                    bestFit = 0;

                    while (left <= right)
                    {
                        var mid = (left + right) / 2;
                        var testLine = string.Concat(
                            remainingText.AsSpan(0, mid),
                            ellipsis);
                        var size = TextRenderer.MeasureText(
                            g,
                            testLine,
                            font,
                            rect.Size,
                            measureFlags);

                        if (size.Width <= rect.Width)
                        {
                            bestFit = mid;
                            left = mid + 1;
                        }
                        else
                        {
                            right = mid - 1;
                        }
                    }

                    currentLine = string.Concat(
                        remainingText.AsSpan(0, Math.Max(1, bestFit)),
                        ellipsis);
                }

                lines.Add(currentLine);
                startIndex += bestFit;
            }

            // 描画
            var lineHeight = TextRenderer.MeasureText(
                g, "A", font, rect.Size, measureFlags).Height;
            var totalHeight = lines.Count * lineHeight;
            var startY = rect.Y + (rect.Height - totalHeight) / 2;

            for (var i = 0; i < lines.Count; i++)
            {
                var lineRect = new Rectangle(
                    rect.X,
                    startY + i * lineHeight,
                    rect.Width,
                    lineHeight);

                TextRenderer.DrawText(
                    g,
                    lines[i],
                    font,
                    lineRect,
                    color,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.NoPadding);
            }
        }

        public static void DrawText(Graphics srcDc, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags)
        {
            ArgumentNullException.ThrowIfNull(srcDc, nameof(srcDc));
            ArgumentNullException.ThrowIfNull(font, nameof(font));

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
                    dwSize = Marshal.SizeOf<WinApiMembers.DTTOPTS>(),
                    dwFlags = GetDwFlags() | WinApiMembers.DTT_COMPOSITED,

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
