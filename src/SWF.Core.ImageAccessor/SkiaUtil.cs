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

                using var surface = SKSurface.Create(info);
                using var canvas = surface.Canvas;
                using var paint = new SKPaint
                {
                    IsAntialias = false
                };

                var sampling = srcImage.Width > targetWidth || srcImage.Height > targetHeight
                    ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                    : new SKSamplingOptions(SKCubicResampler.CatmullRom);

                canvas.DrawImage(srcImage,
                    SKRectI.Create(0, 0, srcImage.Width, srcImage.Height),
                    SKRectI.Create(0, 0, targetWidth, targetHeight),
                    sampling,
                    paint);

                return surface.Snapshot();
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
            SKRect bounds,
            SKTextAlign textAlign,
            int maxLines)
        {
            ArgumentNullException.ThrowIfNull(canvas);
            ArgumentNullException.ThrowIfNull(paint);
            ArgumentNullException.ThrowIfNull(font);
            ArgumentNullException.ThrowIfNullOrEmpty(text);

            if (maxLines < 1) maxLines = 1;
            if (maxLines > 2) maxLines = 2;

            const int MaxChars = 255;
            if (text.Length > MaxChars)
                text = text[..MaxChars];

            ReadOnlySpan<char> textSpan = text.AsSpan();
            font.GetFontMetrics(out var metrics);

            float lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;
            float maxWidth = bounds.Width;
            float ellipsisWidth = GetEllipsisWidth(font, paint);

            string line1 = "";
            string line2 = "";

            // ── 1行目の処理 ────────────────────────────────────────
            float currentWidth = 0f;
            int breakPos1 = 0;
            for (int i = 0; i < textSpan.Length; i++)
            {
                char c = textSpan[i];
                float cw = GetCharWidthInline(font, paint, c);

                bool isLastLine = (maxLines == 1) || (i == textSpan.Length - 1);
                if (currentWidth + cw + (isLastLine ? ellipsisWidth : 0) > maxWidth)
                    break;

                currentWidth += cw;
                breakPos1 = i + 1;
            }

            ReadOnlySpan<char> line1Span = textSpan[..breakPos1];
            line1 = line1Span.ToString();

            ReadOnlySpan<char> remaining = breakPos1 < textSpan.Length
                ? textSpan[breakPos1..]
                : ReadOnlySpan<char>.Empty;

            // ── 2行目が必要な場合のみ ───────────────────────────────
            if (maxLines >= 2 && !remaining.IsEmpty)
            {
                currentWidth = 0f;
                int breakPos2 = 0;
                for (int i = 0; i < remaining.Length; i++)
                {
                    char c = remaining[i];
                    float cw = GetCharWidthInline(font, paint, c);

                    if (currentWidth + cw + ellipsisWidth > maxWidth)
                        break;

                    currentWidth += cw;
                    breakPos2 = i + 1;
                }

                if (breakPos2 == 0 && remaining.Length > 0)
                    breakPos2 = 1;

                ReadOnlySpan<char> line2Body = remaining[..breakPos2];
                line2 = (breakPos2 < remaining.Length)
                    ? string.Concat(line2Body.ToString(), "...")
                    : line2Body.ToString();
            }
            else if (maxLines == 1 && breakPos1 < textSpan.Length)
            {
                if (line1.Length > 0)
                {
                    // 1行モードでの簡易 ... 処理（より正確にしたい場合は breakPos1 を調整）
                    line1 = line1.AsSpan()[..^1].ToString() + "...";
                }
            }

            int actualLines = string.IsNullOrEmpty(line2) ? 1 : 2;
            float totalHeight = lineHeight * actualLines;
            float baselineY = bounds.MidY - (totalHeight / 2f) - metrics.Ascent;

            // ── 揃えに応じたX座標を決定 ───────────────────────────────
            float x = textAlign switch
            {
                SKTextAlign.Center => bounds.MidX,
                SKTextAlign.Left => bounds.Left,
                SKTextAlign.Right => bounds.Right,   // 右寄せも使えるようにしておく（便利）
                _ => bounds.MidX     // 不正値なら中央に戻す
            };

            // 描画
            canvas.DrawText(line1, x, baselineY, textAlign, font, paint);

            if (!string.IsNullOrEmpty(line2))
            {
                canvas.DrawText(line2, x, baselineY + lineHeight, textAlign, font, paint);
            }
        }

        // TODO: 終了時に破棄
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
