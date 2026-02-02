using SkiaSharp;
using SWF.Core.Base;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public static class SkiaImageUtil
    {
        public static Bitmap ToBitmap(SKImage src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            using (Measuring.Time(false, "SkiaImageUtil.ToBitmap"))
            {
                var bitmap = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppPArgb);
                var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                try
                {
                    var info = new SKImageInfo(
                        src.Width,
                        src.Height,
                        SKImageInfo.PlatformColorType,
                        SKAlphaType.Premul);

                    src.ReadPixels(info, data.Scan0, data.Stride, 0, 0);
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                return bitmap;
            }
        }

        public static SKImage ToSKImage(Bitmap src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            using (Measuring.Time(false, "SkiaImageUtil.ToSKImage"))
            {
                var data = src.LockBits(
                    new Rectangle(0, 0, src.Width, src.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppPArgb);

                try
                {
                    using (var tempBitmap = new SKBitmap())
                    {
                        var info = new SKImageInfo(
                            src.Width,
                            src.Height,
                            SKImageInfo.PlatformColorType,
                            SKAlphaType.Premul);

                        tempBitmap.InstallPixels(info, data.Scan0, data.Stride);

                        return SKImage.FromBitmap(tempBitmap);
                    }
                }
                finally
                {
                    src.UnlockBits(data);
                }
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
                var bitmap = new Bitmap(info.Width, info.Height, PixelFormat.Format32bppPArgb);

                var data = bitmap.LockBits(
                    new Rectangle(0, 0, info.Width, info.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppPArgb);

                var options = new SKCodecOptions(SKZeroInitialized.No);
                var decodeInfo = new SKImageInfo(info.Width, info.Height, SKColorType.Bgra8888, SKAlphaType.Premul);

                var result = codec.GetPixels(decodeInfo, data.Scan0);
                if (result != SKCodecResult.Success)
                {
                    bitmap.UnlockBits(data);
                    bitmap.Dispose();
                    throw new InvalidOperationException($"Decode failed: {result}");
                }

                bitmap.UnlockBits(data);
                return bitmap;
            }
        }

        public static SKImage Resize(SKImage srcImage, int targetWidth, int targetHeight)
        {
            ArgumentNullException.ThrowIfNull(srcImage, nameof(srcImage));

            using (Measuring.Time(true, "SkiaImageUtil.Resize"))
            {
                var info = new SKImageInfo(
                    targetWidth,
                    targetHeight,
                    srcImage.ColorType,
                    srcImage.AlphaType);

                using var bitmap = new SKBitmap(info);

                srcImage.ScalePixels(bitmap.PeekPixels(), SKSamplingOptions.Default);

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

        public static void DrawText(
            SKCanvas canvas,
            SKPaint paint,
            SKFont font,
            string text,
            SKRect bounds)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));
            ArgumentNullException.ThrowIfNull(font, nameof(font));
            ArgumentNullException.ThrowIfNullOrEmpty(text, nameof(text));

            var maxWidth = bounds.Width;
            font.GetFontMetrics(out var metrics);

            // 行の高さを計算
            var lineHeight = metrics.Descent - metrics.Ascent;

            // 1行目の切り出し
            float measuredWidthL1;
            var lengthL1 = (int)font.BreakText(text, maxWidth, out measuredWidthL1, paint);

            var line1 = text.Substring(0, lengthL1);
            var remainingText = text.Substring(lengthL1);

            var line2 = string.Empty;
            var measuredWidthL2 = 0f;

            // 2行目の処理
            if (!string.IsNullOrEmpty(remainingText))
            {
                measuredWidthL2 = font.MeasureText(remainingText, paint);

                if (measuredWidthL2 > maxWidth)
                {
                    // "..." の幅を取得
                    var ellipsisWidth = font.MeasureText("...", paint);
                    // 2行目の制限幅から三点リーダ分を引いて切り出し
                    var lengthL2 = (int)font.BreakText(remainingText, maxWidth - ellipsisWidth, out _, paint);

                    line2 = string.Concat(remainingText.AsSpan(0, lengthL2), "...");
                    measuredWidthL2 = font.MeasureText(line2, paint);
                }
                else
                {
                    line2 = remainingText;
                }
            }

            // 垂直位置の計算(1行か2行かで全体の高さを変える)
            var lineCount = string.IsNullOrEmpty(line2) ? 1 : 2;
            var totalTextHeight = lineHeight * lineCount;

            // boundsの中央から、全行の高さの半分を引いて、
            // さらにAscent（上方向への高さ、負の値）を引いてベースラインを特定
            var startBaselineY = bounds.MidY - (totalTextHeight / 2) - metrics.Ascent;

            // 描画実行
            DrawTextLine(canvas, paint, line1, font, bounds, startBaselineY, measuredWidthL1);

            if (line2 != null)
            {
                DrawTextLine(canvas, paint, line2, font, bounds, startBaselineY + lineHeight, measuredWidthL2);
            }
        }

        private static void DrawTextLine(
            SKCanvas canvas,
            SKPaint paint,
            string text,
            SKFont font,
            SKRect bounds,
            float y,
            float textWidth)
        {
            if (string.IsNullOrEmpty(text)) return;

            // 水平中央: (領域の幅 - テキストの幅) / 2
            var x = bounds.Left + (bounds.Width - textWidth) / 2;

            using var blob = SKTextBlob.Create(text, font);
            canvas.DrawText(blob, x, y, paint);
        }
    }
}
