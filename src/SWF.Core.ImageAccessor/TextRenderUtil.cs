namespace SWF.Core.ImageAccessor
{
    public static class TextRenderUtil
    {
        [ThreadStatic]
        private static Dictionary<Font, FontMetrics>? _tlsFontCache;

        private sealed class FontMetrics
        {
            public readonly float[] CharWidths = new float[0x10000];
            public int LineHeight { get; init; }
            public float EllipsisWidth { get; init; }
        }

        private static Dictionary<Font, FontMetrics> GetCache() => _tlsFontCache ??= new();

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

            const TextFormatFlags flags = TextFormatFlags.HorizontalCenter |
                                          TextFormatFlags.VerticalCenter |
                                          TextFormatFlags.NoPadding;

            // **最適化: 1行の場合は1回のDrawTextで完結**
            if (lineCnt == 1)
            {
                if (!needsEll[0] && starts[0] == 0 && lengths[0] == text.Length)
                {
                    // 省略なし・全体表示
                    TextRenderer.DrawText(g, text, font, rect, color, flags);
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
                    TextRenderer.DrawText(g, new string(buffer.Slice(0, len)), font, rect, color, flags);
                }
                return;
            }

            // **最適化: 2行の場合も1回のDrawTextで描画（改行を使用）**
            Span<char> multiLineBuffer = stackalloc char[50]; // 最大: 20文字×2行 + 省略記号×2 + 改行1 = 45文字
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
            // ただし、TextRendererは改行で自動的に行分けしてくれるが、
            // 垂直位置の制御が難しいため、手動で位置調整が必要

            // **方法1: 改行文字で1回描画（簡易版）**
            TextRenderer.DrawText(g, new string(multiLineBuffer.Slice(0, totalLen)), font, rect, color, flags);

            //// **方法2: 各行の位置を正確に制御（推奨）**
            //int totalH = lineCnt * metrics.LineHeight;
            //int startY = rect.Y + (rect.Height - totalH) / 2;

            //totalLen = 0;
            //Span<char> lineBuffer = stackalloc char[23];

            //for (int i = 0; i < lineCnt; i++)
            //{
            //    int s = starts[i], len = lengths[i];
            //    var r = new Rectangle(rect.X, startY + i * metrics.LineHeight,
            //                         rect.Width, metrics.LineHeight);

            //    text.AsSpan(s, len).CopyTo(lineBuffer);
            //    int lineLen = len;

            //    if (needsEll[i])
            //    {
            //        Ellipsis.AsSpan().CopyTo(lineBuffer.Slice(len));
            //        lineLen += 3;
            //    }

            //    TextRenderer.DrawText(g, new string(lineBuffer.Slice(0, lineLen)), font, r, color, flags);
            //}
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

            // ASCII
            for (char c = ' '; c <= '~'; c++)
            {
                metrics.CharWidths[c] = TextRenderer.MeasureText(c.ToString(), font,
                    Size.Empty, TextFormatFlags.NoPadding).Width;
            }

            // ひらがな
            for (char c = '\u3040'; c <= '\u309F'; c++)
            {
                metrics.CharWidths[c] = TextRenderer.MeasureText(c.ToString(), font,
                    Size.Empty, TextFormatFlags.NoPadding).Width;
            }

            // カタカナ
            for (char c = '\u30A0'; c <= '\u30FF'; c++)
            {
                metrics.CharWidths[c] = TextRenderer.MeasureText(c.ToString(), font,
                    Size.Empty, TextFormatFlags.NoPadding).Width;
            }

            return metrics;
        }
    }
}