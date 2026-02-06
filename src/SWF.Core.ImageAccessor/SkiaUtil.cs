using SkiaSharp;
using SWF.Core.Base;
using System.Collections.Concurrent;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace SWF.Core.ImageAccessor
{
    public static class SkiaUtil
    {
        public static Bitmap ToBitmap(SKImage src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            using (Measuring.Time(false, "SkiaImageUtil.ToBitmap"))
            {
                Bitmap? bitmap = null;
                BitmapData? data = null;

                try
                {
                    bitmap = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppPArgb);

                    data = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.WriteOnly,
                        bitmap.PixelFormat);

                    var info = new SKImageInfo(
                        src.Width,
                        src.Height,
                        SKImageInfo.PlatformColorType,
                        SKAlphaType.Premul);

                    if (!src.ReadPixels(info, data.Scan0, data.Stride, 0, 0))
                    {
                        throw new InvalidOperationException("Failed to read pixels from SKImage.");
                    }

                    bitmap.UnlockBits(data);
                    data = null;  // UnlockBits完了を記録

                    var result = bitmap;
                    bitmap = null;  // 所有権を移譲
                    return result;
                }
                finally
                {
                    // dataがnullでない = まだUnlockされていない
                    if (data != null && bitmap != null)
                    {
                        bitmap.UnlockBits(data);
                    }

                    bitmap?.Dispose();
                }
            }
        }

        public static unsafe SKImage ToSKImage(Bitmap src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            var data = src.LockBits(
                new Rectangle(0, 0, src.Width, src.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppPArgb);

            try
            {
                var info = new SKImageInfo(
                    src.Width,
                    src.Height,
                    SKImageInfo.PlatformColorType,
                    SKAlphaType.Premul);

                return SKImage.FromPixelCopy(info, data.Scan0, data.Stride);
            }
            finally
            {
                src.UnlockBits(data);
            }
        }

        public static Bitmap ReadImageFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            using (Measuring.Time(false, "SkiaBitmapUtil.ReadImageFile"))
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                using var managedStream = new SKManagedStream(stream);
                using var codec = SKCodec.Create(managedStream);

                var info = codec.Info;
                Bitmap? bitmap = null;
                BitmapData? data = null;

                try
                {
                    bitmap = new Bitmap(info.Width, info.Height, PixelFormat.Format32bppPArgb);

                    data = bitmap.LockBits(
                        new Rectangle(0, 0, info.Width, info.Height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppPArgb);

                    var decodeInfo = new SKImageInfo(
                        info.Width,
                        info.Height,
                        SKColorType.Bgra8888,
                        SKAlphaType.Premul);

                    var result = codec.GetPixels(decodeInfo, data.Scan0);

                    if (result != SKCodecResult.Success)
                    {
                        throw new InvalidOperationException($"Decode failed: {result}");
                    }

                    bitmap.UnlockBits(data);
                    data = null;  // UnlockBits完了を記録

                    var resultBitmap = bitmap;
                    bitmap = null;  // 所有権を移譲
                    return resultBitmap;
                }
                finally
                {
                    // dataがnullでない = まだUnlockされていない
                    if (data != null && bitmap != null)
                    {
                        bitmap.UnlockBits(data);
                    }

                    bitmap?.Dispose();
                }
            }
        }

        public static SKImage Resize(SKImage srcImage, int targetWidth, int targetHeight)
        {
            ArgumentNullException.ThrowIfNull(srcImage, nameof(srcImage));

            using (Measuring.Time(false, "SkiaImageUtil.Resize"))
            {
                var info = new SKImageInfo(
                    targetWidth,
                    targetHeight,
                    srcImage.ColorType,
                    srcImage.AlphaType);

                var sampling = srcImage.Width > targetWidth || srcImage.Height > targetHeight
                    ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                    : new SKSamplingOptions(SKCubicResampler.CatmullRom);

                using var bitmap = new SKBitmap(info);

                srcImage.ScalePixels(bitmap.PeekPixels(), sampling);

                return SKImage.FromBitmap(bitmap);
            }
        }

        public static SKImage ReadBuffer(byte[] bf)
        {
            using (Measuring.Time(false, "SkiaImageUtil.ReadImageFileToSKImage"))
            {
                if (bf == null || bf.Length == 0)
                {
                    throw new ArgumentException("Buffer is null/empty", nameof(bf));
                }

                var image = SKImage.FromEncodedData(bf);
                if (image == null)
                {
                    throw new InvalidOperationException("Failed to decode image");
                }

                return image;
            }
        }

        // フォントごとのキャッシュ（スレッドセーフ、グリッド描画で8並列対応）
        private static readonly ConcurrentDictionary<SKFont, ConcurrentDictionary<char, float>> _fontCharWidthCaches = new();

        // 省略記号 "..." の幅キャッシュ
        private static readonly ConcurrentDictionary<SKFont, float> _ellipsisWidthCache = new();

        /// <summary>
        /// サムネイルグリッド（120x120）向けテキスト描画：最大2行、溢れ時...、水平/垂直中央揃え。
        /// 30文字程度、10ms/frame目標で最適化（キャッシュ + Span + 非推奨API回避）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawText(
            SKCanvas canvas,
            SKPaint paint,
            SKFont font,
            string text,
            SKRect bounds)
        {
            ArgumentNullException.ThrowIfNull(canvas);
            ArgumentNullException.ThrowIfNull(paint);
            ArgumentNullException.ThrowIfNull(font);
            ArgumentNullException.ThrowIfNullOrEmpty(text);

            const int MaxChars = 255;
            if (text.Length > MaxChars)
                text = text[..MaxChars];  // Range演算子で高速カット

            ReadOnlySpan<char> textSpan = text.AsSpan();

            font.GetFontMetrics(out var metrics);
            float lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;  // 正確な行高（Leading含む）
            float maxWidth = bounds.Width;
            float ellipsisWidth = GetEllipsisWidth(font, paint);

            // 1行目折り返し（文字単位積算、キャッシュ活用）
            float currentWidth = 0f;
            int breakPos1 = 0;
            for (int i = 0; i < textSpan.Length; i++)
            {
                char c = textSpan[i];
                float cw = GetCharWidthInline(font, paint, c);  // インラインでJIT最適化
                if (currentWidth + cw > maxWidth) break;
                currentWidth += cw;
                breakPos1 = i + 1;
            }

            ReadOnlySpan<char> line1Span = textSpan[..breakPos1];
            ReadOnlySpan<char> remainingSpan = breakPos1 < textSpan.Length ? textSpan[breakPos1..] : [];

            string line2 = "";
            if (!remainingSpan.IsEmpty)
            {
                currentWidth = 0f;
                int breakPos2 = 0;
                for (int i = 0; i < remainingSpan.Length; i++)
                {
                    char c = remainingSpan[i];
                    float cw = GetCharWidthInline(font, paint, c);
                    if (currentWidth + cw + ellipsisWidth > maxWidth) break;
                    currentWidth += cw;
                    breakPos2 = i + 1;
                }

                // 最低1文字保証
                if (breakPos2 == 0) breakPos2 = 1;

                // Spanで...連結（効率的）
                ReadOnlySpan<char> line2Body = remainingSpan[..breakPos2];
                line2 = breakPos2 < remainingSpan.Length
                    ? string.Concat(line2Body, "...")
                    : line2Body.ToString();
            }

            int lineCount = string.IsNullOrEmpty(line2) ? 1 : 2;
            float totalHeight = lineHeight * lineCount;
            float baselineY = bounds.MidY - (totalHeight / 2f) - metrics.Ascent;  // 垂直中央（サムネイル最適）

            // DrawText で SKTextAlign.Center を直接指定（非推奨回避）
            canvas.DrawText(line1Span.ToString(), bounds.MidX, baselineY, SKTextAlign.Center, font, paint);

            if (!string.IsNullOrEmpty(line2))
            {
                canvas.DrawText(line2, bounds.MidX, baselineY + lineHeight, SKTextAlign.Center, font, paint);
            }
        }

        /// <summary>
        /// キャッシュクリア（メモリ節約、フォントDispose後推奨）
        /// </summary>
        public static void ClearAll()
        {
            _fontCharWidthCaches.Clear();
            _ellipsisWidthCache.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetCharWidthInline(SKFont font, SKPaint paint, char c)
        {
            var charCache = _fontCharWidthCaches.GetOrAdd(font, _ => new ConcurrentDictionary<char, float>());
            if (charCache.TryGetValue(c, out float width)) return width;

            width = font.MeasureText(c.ToString(), paint);
            charCache[c] = width;
            return width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetEllipsisWidth(SKFont font, SKPaint paint)
        {
            return _ellipsisWidthCache.GetOrAdd(font, f => f.MeasureText("...", paint));
        }
    }
}
