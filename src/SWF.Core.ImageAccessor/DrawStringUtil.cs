namespace SWF.Core.ImageAccessor
{
    public static class DrawStringUtil
    {
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
        private static char[] GetBuffer() => _tlsBuffer ??= new char[128];

        public static void DrawText(Graphics g, string text, Font font, Rectangle rect, SolidBrush brush)
        {
            if (string.IsNullOrEmpty(text)) return;

            // 1. キャッシュ取得（ロックなし）
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

            // 2. 行分割ロジック
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
                        // キャッシュになければ計測して保存
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

            // 3. 描画位置の計算
            float totalH = lineCnt * metrics.LineHeight;
            float startY = rect.Y + (rect.Height - totalH) / 2f;

            char[] buffer = GetBuffer();

            // 4. 描画
            for (int i = 0; i < lineCnt; i++)
            {
                int s = starts[i], len = lengths[i];
                var r = new RectangleF(rect.X, startY + i * metrics.LineHeight,
                                       rect.Width, metrics.LineHeight);

                string lineStr;
                if (needsEll[i])
                {
                    text.AsSpan(s, len).CopyTo(buffer);
                    Ellipsis.AsSpan().CopyTo(buffer.AsSpan(len));
                    lineStr = new string(buffer, 0, len + 3);
                }
                else
                {
                    lineStr = (s == 0 && len == text.Length) ? text : text.Substring(s, len);
                }

                g.DrawString(lineStr, font, brush, r, FORMAT);
            }
        }

        private static FontMetrics BuildMetrics(Graphics g, Font font)
        {
            // 初期計測
            SizeF sample = g.MeasureString("A", font);
            float ellW = g.MeasureString("...", font).Width;

            return new FontMetrics
            {
                LineHeight = sample.Height,
                EllipsisWidth = ellW
            };
        }
    }
}