namespace SWF.Core.ImageAccessor
{
    public static class TextRenderUtil
    {
        // スレッドごとに独立したキャッシュを持つことで、ロックを完全に排除
        [ThreadStatic]
        private static Dictionary<Font, FontMetrics>? _tlsFontCache;

        // 文字列生成用の再利用バッファ
        [ThreadStatic]
        private static char[]? _tlsBuffer;

        private sealed class FontMetrics
        {
            public readonly float[] CharWidths = new float[0x10000];
            public int LineHeight { get; init; }
            public float EllipsisWidth { get; init; }
        }

        private static Dictionary<Font, FontMetrics> GetCache() => _tlsFontCache ??= new();
        private static char[] GetBuffer() => _tlsBuffer ??= new char[128];

        public static void DrawText(Graphics g, string text, Font font, Rectangle rect, Color color)
        {
            if (string.IsNullOrEmpty(text)) return;

            // 1. キャッシュ取得（ロックなし）
            var cache = GetCache();
            if (!cache.TryGetValue(font, out var metrics))
            {
                metrics = BuildMetrics(font);
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

            // 2. 行分割ロジック（計測もロックなし）
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

            // 3. 描画位置の計算
            int totalH = lineCnt * metrics.LineHeight;
            int startY = rect.Y + (rect.Height - totalH) / 2;
            const TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;

            char[] buffer = GetBuffer();

            // 4. 描画（String生成を最小化）
            for (int i = 0; i < lineCnt; i++)
            {
                int s = starts[i], len = lengths[i];
                var r = new Rectangle(rect.X, startY + i * metrics.LineHeight, rect.Width, metrics.LineHeight);

                string lineStr;
                if (needsEll[i])
                {
                    text.AsSpan(s, len).CopyTo(buffer);
                    Ellipsis.AsSpan().CopyTo(buffer.AsSpan(len));
                    lineStr = new string(buffer, 0, len + 3);
                }
                else
                {
                    // 全体表示なら元のインスタンスを使い回す
                    lineStr = (s == 0 && len == text.Length) ? text : text.Substring(s, len);
                }

                TextRenderer.DrawText(g, lineStr, font, r, color, flags);
            }
        }

        private static FontMetrics BuildMetrics(Font font)
        {
            // 初期計測
            Size sample = TextRenderer.MeasureText("A", font, Size.Empty, TextFormatFlags.NoPadding);
            float ellW = TextRenderer.MeasureText("...", font, Size.Empty, TextFormatFlags.NoPadding).Width;

            return new FontMetrics
            {
                LineHeight = sample.Height,
                EllipsisWidth = ellW
            };
        }
    }
}