namespace SWF.Core.ImageAccessor
{
    public static class TextRenderUtil
    {
        private const int MaxFileNameLength = 255;
        private const int MaxBufferSize = MaxFileNameLength + 3;
        private const int HorizontalMargin = 4; // 左右の余白（ピクセル）
        private const int VerticalMargin = 2;   // 上下の余白（ピクセル）

        [ThreadStatic]
        private static Dictionary<Font, FontMetrics>? _tlsFontCache;

        [ThreadStatic]
        private static char[]? _tlsBuffer;

        private sealed class FontMetrics
        {
            public readonly float[] CharWidths = new float[0x10000];
            public int LineHeight { get; init; }
            public float EllipsisWidth { get; init; }
        }

        private static Dictionary<Font, FontMetrics> GetCache() => _tlsFontCache ??= [];
        private static char[] GetBuffer() => _tlsBuffer ??= new char[MaxBufferSize];

        public static void DrawText(Graphics g, string text, Font font, Rectangle rect, Color color)
        {
            if (string.IsNullOrEmpty(text)) return;

            var cache = GetCache();
            if (!cache.TryGetValue(font, out var metrics))
            {
                metrics = BuildMetrics(font);
                cache[font] = metrics;
            }

            const int MaxLines = 2;
            const string Ellipsis = "...";
            var widths = metrics.CharWidths;

            // 余白を考慮した利用可能幅
            var availWidth = rect.Width - (HorizontalMargin * 2);

            Span<int> starts = stackalloc int[MaxLines];
            Span<int> lengths = stackalloc int[MaxLines];
            Span<bool> needsEll = stackalloc bool[MaxLines];
            int lineCnt = 0, pos = 0;

            // 行分割ロジック
            while (pos < text.Length && lineCnt < MaxLines)
            {
                bool isLast = lineCnt == MaxLines - 1;
                float ellipsisSpace = isLast ? metrics.EllipsisWidth : 0f;
                float max = availWidth - ellipsisSpace;

                int fit = 0;
                float currentSum = 0;
                ReadOnlySpan<char> rest = text.AsSpan(pos);

                for (; fit < rest.Length; fit++)
                {
                    char c = rest[fit];
                    float w = widths[c];
                    if (w <= 0)
                    {
                        w = TextRenderer.MeasureText(c.ToString(), font, Size.Empty, TextFormatFlags.NoPadding).Width;
                        widths[c] = w;
                    }

                    if (currentSum + w > max) break;
                    currentSum += w;
                }

                if (fit == 0 && rest.Length > 0) fit = 1;

                starts[lineCnt] = pos;
                lengths[lineCnt] = fit;
                needsEll[lineCnt] = isLast && (pos + fit < text.Length);
                lineCnt++;
                pos += fit;
            }

            // 垂直方向も余白を考慮
            int totalH = lineCnt * metrics.LineHeight;
            int availableHeight = rect.Height - (VerticalMargin * 2);
            int startY = rect.Y + VerticalMargin + (availableHeight - totalH) / 2;

            const TextFormatFlags flags = TextFormatFlags.HorizontalCenter |
                                          TextFormatFlags.VerticalCenter |
                                          TextFormatFlags.NoPadding;

            // 1行の場合
            if (lineCnt == 1)
            {
                int s = starts[0], len = lengths[0];

                // 余白を考慮した描画領域
                var drawRect = new Rectangle(
                    rect.X + HorizontalMargin,
                    rect.Y + VerticalMargin,
                    rect.Width - (HorizontalMargin * 2),
                    rect.Height - (VerticalMargin * 2)
                );

                if (!needsEll[0] && s == 0 && len == text.Length)
                {
                    TextRenderer.DrawText(g, text, font, drawRect, color, flags);
                }
                else
                {
                    char[] buffer = GetBuffer();

                    int copyLen = Math.Min(len, text.Length - s);
                    if (copyLen > 0)
                    {
                        text.AsSpan(s, copyLen).CopyTo(buffer);
                    }

                    int finalLen = copyLen;
                    if (needsEll[0] && finalLen + 3 <= buffer.Length)
                    {
                        Ellipsis.AsSpan().CopyTo(buffer.AsSpan(finalLen));
                        finalLen += 3;
                    }

                    TextRenderer.DrawText(g, new string(buffer, 0, finalLen), font, drawRect, color, flags);
                }
                return;
            }

            // 2行の場合
            char[] lineBuffer = GetBuffer();

            for (int i = 0; i < lineCnt; i++)
            {
                int s = starts[i], len = lengths[i];

                // 余白を考慮した描画領域
                var r = new Rectangle(
                    rect.X + HorizontalMargin,
                    startY + i * metrics.LineHeight,
                    rect.Width - (HorizontalMargin * 2),
                    metrics.LineHeight
                );

                if (s >= text.Length) continue;
                int copyLen = Math.Min(len, text.Length - s);
                if (copyLen <= 0) continue;

                text.AsSpan(s, copyLen).CopyTo(lineBuffer);
                int lineLen = copyLen;

                if (needsEll[i] && lineLen + 3 <= lineBuffer.Length)
                {
                    Ellipsis.AsSpan().CopyTo(lineBuffer.AsSpan(lineLen));
                    lineLen += 3;
                }

                TextRenderer.DrawText(g, new string(lineBuffer, 0, lineLen), font, r, color, flags);
            }
        }

        private static FontMetrics BuildMetrics(Font font)
        {
            Size sample = TextRenderer.MeasureText("A", font, Size.Empty, TextFormatFlags.NoPadding);
            float ellW = TextRenderer.MeasureText("...", font, Size.Empty, TextFormatFlags.NoPadding).Width;

            var metrics = new FontMetrics
            {
                LineHeight = sample.Height,
                EllipsisWidth = ellW
            };

            // ASCII事前キャッシュ
            for (char c = ' '; c <= '~'; c++)
            {
                metrics.CharWidths[c] = TextRenderer.MeasureText(c.ToString(), font,
                    Size.Empty, TextFormatFlags.NoPadding).Width;
            }

            // ひらがな事前キャッシュ
            for (char c = '\u3040'; c <= '\u309F'; c++)
            {
                metrics.CharWidths[c] = TextRenderer.MeasureText(c.ToString(), font,
                    Size.Empty, TextFormatFlags.NoPadding).Width;
            }

            // カタカナ事前キャッシュ
            for (char c = '\u30A0'; c <= '\u30FF'; c++)
            {
                metrics.CharWidths[c] = TextRenderer.MeasureText(c.ToString(), font,
                    Size.Empty, TextFormatFlags.NoPadding).Width;
            }

            return metrics;
        }
    }
}