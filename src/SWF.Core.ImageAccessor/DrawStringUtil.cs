namespace SWF.Core.ImageAccessor
{
    public static class DrawStringUtil
    {
        private const int MaxFileNameLength = 255;
        private const int MaxBufferSize = MaxFileNameLength + 3;

        [ThreadStatic]
        private static Dictionary<Font, FontMetrics>? _tlsFontCache;

        [ThreadStatic]
        private static char[]? _tlsBuffer;

        private sealed class FontMetrics
        {
            public readonly float[] CharWidths = new float[0x10000];
            public float LineHeight { get; init; }
            public float EllipsisWidth { get; init; }
        }

        private static readonly StringFormat FORMAT = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None,
            FormatFlags = StringFormatFlags.NoWrap
        };

        private static Dictionary<Font, FontMetrics> GetCache() => _tlsFontCache ??= new();
        private static char[] GetBuffer() => _tlsBuffer ??= new char[MaxBufferSize];

        public static void DrawText(Graphics g, string text, Font font, Rectangle rect, SolidBrush brush)
        {
            if (string.IsNullOrEmpty(text)) return;

            var cache = GetCache();
            if (!cache.TryGetValue(font, out var metrics))
            {
                metrics = BuildMetrics(g, font);
                cache[font] = metrics;
            }

            const int MaxLines = 2;
            const string Ellipsis = "...";
            var widths = metrics.CharWidths;
            var availWidth = (float)rect.Width;

            Span<int> starts = stackalloc int[MaxLines];
            Span<int> lengths = stackalloc int[MaxLines];
            Span<bool> needsEll = stackalloc bool[MaxLines];
            int lineCnt = 0, pos = 0;

            // 行分割ロジック - ギリギリまで詰める
            while (pos < text.Length && lineCnt < MaxLines)
            {
                bool isLast = lineCnt == MaxLines - 1;
                float ellipsisSpace = isLast ? metrics.EllipsisWidth : 0f;
                float max = availWidth - ellipsisSpace;

                int fit = 0;
                float currentSum = 0;
                ReadOnlySpan<char> rest = text.AsSpan(pos);

                // 幅がギリギリまで収まる文字数を計測
                for (; fit < rest.Length; fit++)
                {
                    char c = rest[fit];
                    float w = widths[c];
                    if (w <= 0)
                    {
                        w = g.MeasureString(c.ToString(), font).Width;
                        widths[c] = w;
                    }

                    // ギリギリまで詰める（超えない範囲で最大）
                    if (currentSum + w > max) break;
                    currentSum += w;
                }

                // 1文字も入らない場合でも最低1文字は表示
                if (fit == 0 && rest.Length > 0) fit = 1;

                starts[lineCnt] = pos;
                lengths[lineCnt] = fit;
                needsEll[lineCnt] = isLast && (pos + fit < text.Length);
                lineCnt++;
                pos += fit;
            }

            // 垂直方向もギリギリまで詰める
            float totalH = lineCnt * metrics.LineHeight;
            float startY = rect.Y + (rect.Height - totalH) / 2f;

            // 1行の場合
            if (lineCnt == 1)
            {
                int s = starts[0], len = lengths[0];

                if (!needsEll[0] && s == 0 && len == text.Length)
                {
                    g.DrawString(text, font, brush, new RectangleF(rect.X, startY, rect.Width, metrics.LineHeight), FORMAT);
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

                    g.DrawString(new string(buffer, 0, finalLen), font, brush,
                        new RectangleF(rect.X, startY, rect.Width, metrics.LineHeight), FORMAT);
                }
                return;
            }

            // 2行を改行文字で結合して1回のDrawStringで描画
            char[] multiLineBuffer = GetBuffer();
            int totalLen = 0;

            for (int i = 0; i < lineCnt; i++)
            {
                if (i > 0)
                {
                    multiLineBuffer[totalLen++] = '\n';
                }

                int s = starts[i], len = lengths[i];

                if (s >= text.Length) continue;
                int copyLen = Math.Min(len, text.Length - s);
                if (copyLen <= 0) continue;

                text.AsSpan(s, copyLen).CopyTo(multiLineBuffer.AsSpan(totalLen));
                totalLen += copyLen;

                if (needsEll[i] && totalLen + 3 <= multiLineBuffer.Length)
                {
                    Ellipsis.AsSpan().CopyTo(multiLineBuffer.AsSpan(totalLen));
                    totalLen += 3;
                }
            }

            // 改行を含む文字列を1回で描画
            using var multiLineFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };

            g.DrawString(new string(multiLineBuffer, 0, totalLen), font, brush,
                new RectangleF(rect.X, startY, rect.Width, totalH), multiLineFormat);
        }

        private static FontMetrics BuildMetrics(Graphics g, Font font)
        {
            SizeF sample = g.MeasureString("A", font);
            float ellW = g.MeasureString("...", font).Width;

            var metrics = new FontMetrics
            {
                LineHeight = sample.Height,
                EllipsisWidth = ellW
            };

            // ASCII事前キャッシュ
            for (char c = ' '; c <= '~'; c++)
            {
                metrics.CharWidths[c] = g.MeasureString(c.ToString(), font).Width;
            }

            // ひらがな事前キャッシュ
            for (char c = '\u3040'; c <= '\u309F'; c++)
            {
                metrics.CharWidths[c] = g.MeasureString(c.ToString(), font).Width;
            }

            // カタカナ事前キャッシュ
            for (char c = '\u30A0'; c <= '\u30FF'; c++)
            {
                metrics.CharWidths[c] = g.MeasureString(c.ToString(), font).Width;
            }

            return metrics;
        }
    }
}