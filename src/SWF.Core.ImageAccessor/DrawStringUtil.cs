namespace SWF.Core.ImageAccessor
{
    public static class DrawStringUtil
    {
        [ThreadStatic]
        private static Dictionary<Font, FontMetrics>? _tlsFontCache;

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

            // 行分割ロジック
            while (pos < text.Length && lineCnt < MaxLines)
            {
                bool isLast = lineCnt == MaxLines - 1;
                float max = availWidth - (isLast ? metrics.EllipsisWidth : 0f);

                int fit = 0;
                float currentSum = 0;
                ReadOnlySpan<char> rest = text.AsSpan(pos);

                for (; fit < rest.Length; fit++)
                {
                    char c = rest[fit];
                    float w = widths[c];
                    if (w <= 0)
                    {
                        w = g.MeasureString(c.ToString(), font).Width;
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

            float totalH = lineCnt * metrics.LineHeight;
            float startY = rect.Y + (rect.Height - totalH) / 2f;

            // **最適化: 1行の場合**
            if (lineCnt == 1)
            {
                if (!needsEll[0] && starts[0] == 0 && lengths[0] == text.Length)
                {
                    // 省略なし・全体表示
                    g.DrawString(text, font, brush, new RectangleF(rect.X, startY, rect.Width, metrics.LineHeight), FORMAT);
                }
                else
                {
                    // 省略ありまたは部分表示
                    Span<char> buffer = stackalloc char[23];
                    int s = starts[0], len = lengths[0];
                    text.AsSpan(s, len).CopyTo(buffer);
                    if (needsEll[0])
                    {
                        Ellipsis.AsSpan().CopyTo(buffer.Slice(len));
                        len += 3;
                    }
                    g.DrawString(new string(buffer.Slice(0, len)), font, brush,
                        new RectangleF(rect.X, startY, rect.Width, metrics.LineHeight), FORMAT);
                }
                return;
            }

            // **最適化: 2行を改行文字で結合して1回のDrawStringで描画**
            Span<char> multiLineBuffer = stackalloc char[50]; // 20文字×2行 + 省略記号×2 + 改行1 = 最大45文字
            int totalLen = 0;

            for (int i = 0; i < lineCnt; i++)
            {
                if (i > 0)
                {
                    multiLineBuffer[totalLen++] = '\n';
                }

                int s = starts[i], len = lengths[i];
                text.AsSpan(s, len).CopyTo(multiLineBuffer.Slice(totalLen));
                totalLen += len;

                if (needsEll[i])
                {
                    Ellipsis.AsSpan().CopyTo(multiLineBuffer.Slice(totalLen));
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

            g.DrawString(new string(multiLineBuffer.Slice(0, totalLen)), font, brush,
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