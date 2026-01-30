using SkiaSharp;
using SWF.Core.Base;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public static class SkiaImageUtil
    {
        private const int WEBP_QUALITY = 80;

        /// <summary>
        /// SKImageをGDI+のBitmapに変換します（高品質・乱れ防止版）
        /// </summary>
        public static Bitmap ToBitmap(SKImage src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            using (Measuring.Time(false, "SkiaImageUtil.ToBitmap"))
            {
                var bitmap = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppPArgb);
                var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                try
                {
                    // PlatformColorTypeとPremulの指定が画像乱れ防止の鍵です
                    var info = new SKImageInfo(src.Width, src.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                    src.ReadPixels(info, data.Scan0, data.Stride, 0, 0);
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
                return bitmap;
            }
        }

        /// <summary>
        /// GDI+のBitmapをSKImageに変換します
        /// </summary>
        public static SKImage ToSKImage(Bitmap src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            using (Measuring.Time(false, "SkiaImageUtil.ToSKImage"))
            {
                var data = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
                try
                {
                    var info = new SKImageInfo(src.Width, src.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                    using (var tempBitmap = new SKBitmap())
                    {
                        tempBitmap.InstallPixels(info, data.Scan0, data.Stride);
                        // 独立したSKImageとしてコピーを作成
                        return SKImage.FromBitmap(tempBitmap);
                    }
                }
                finally
                {
                    src.UnlockBits(data);
                }
            }
        }

        public static SKImage ToSKImageFast(Bitmap src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            using (Measuring.Time(false, "SkiaImageUtil.ToSKImageFast"))
            {
                // 1. LockBits (GDI+ 側で 1~3ms 程度消費)
                var data = src.LockBits(
                    new Rectangle(0, 0, src.Width, src.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppPArgb);

                try
                {
                    // 2. ピクセル情報の定義
                    var info = new SKImageInfo(src.Width, src.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

                    // 3. SKPixmap を作成 (ピクセルポインタを包むだけの超軽量オブジェクト)
                    using var pixmap = new SKPixmap(info, data.Scan0, data.Stride);

                    // 4. FromPixels (SKPixmap, releaseProc, context) を使用
                    // これが SkiaSharp における「ポインタ参照のみ」の最速ルートの一つです
                    var skImage = SKImage.FromPixels(pixmap, (address, context) =>
                    {
                        var bmp = (Bitmap)context;
                        bmp.UnlockBits(data);
                    }, src);

                    if (skImage == null)
                    {
                        src.UnlockBits(data);
                        throw new InvalidOperationException("Failed to create SKImage from pixmap.");
                    }

                    return skImage;
                }
                catch
                {
                    src.UnlockBits(data);
                    throw;
                }
            }
        }

        public static Bitmap ReadImageFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            using (Measuring.Time(false, "SkiaImageUtil.ReadImageFile"))
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                using (var skImage = SKImage.FromEncodedData(stream))
                {
                    if (skImage == null)
                    {
                        throw new InvalidOperationException("Failed to decode image from stream.");
                    }

                    return ToBitmap(skImage);
                }
            }
        }

        public static SKImage Resize(SKImage srcImage, float newWidth, float newHeight)
        {
            using (Measuring.Time(true, "SkiaImageUtil.Resize"))
            {
                ArgumentNullException.ThrowIfNull(srcImage, nameof(srcImage));

                var targetWidth = (int)newWidth;
                var targetHeight = (int)newHeight;

                if (srcImage.Width == targetWidth && srcImage.Height == targetHeight)
                {
                    return srcImage;
                }

                var sampling = srcImage.Width > targetWidth || srcImage.Height > targetHeight
                    ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                    : new SKSamplingOptions(SKCubicResampler.CatmullRom);

                using var destBitmap = new SKBitmap(targetWidth, targetHeight);
                using (var canvas = new SKCanvas(destBitmap))
                using (var paint = new SKPaint { IsAntialias = true })
                {
                    using (Measuring.Time(true, "SkiaImageUtil.Resize"))
                        canvas.DrawImage(srcImage, new SKRect(0, 0, targetWidth, targetHeight), sampling, paint);
                }

                var resultImage = SKImage.FromBitmap(destBitmap);
                return resultImage;

                //var sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None);
                //var info = new SKImageInfo(targetWidth, targetHeight);
                //using var surface = SKSurface.Create(info);
                //var canvas = surface.Canvas;
                //using var paint = new SKPaint { IsAntialias = true };
                //canvas.DrawImage(srcImage, new SKRect(0, 0, targetWidth, targetHeight), sampling, paint);
                //return surface.Snapshot();
            }
        }

        public static SKImage Resize(
            SKPaint paint, SKImage srcImage, float newWidth, float newHeight)
        {
            using (Measuring.Time(true, "SkiaImageUtil.Resize"))
            {
                ArgumentNullException.ThrowIfNull(srcImage, nameof(srcImage));

                var targetWidth = (int)newWidth;
                var targetHeight = (int)newHeight;

                if (srcImage.Width == targetWidth && srcImage.Height == targetHeight)
                {
                    return srcImage;
                }

                var sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

                var info = new SKImageInfo(
                    targetWidth,
                    targetHeight,
                    srcImage.ColorType,
                    srcImage.AlphaType);

                using var surface = SKSurface.Create(info);

                var canvas = surface.Canvas;
                canvas.DrawImage(
                    srcImage,
                    new SKRect(0, 0, targetWidth, targetHeight),
                    sampling,
                    paint);

                return surface.Snapshot();
            }
        }

        public static SKImage Resize(SKImage srcImage, int targetWidth, int targetHeight)
        {
            using (Measuring.Time(true, "SkiaImageUtil.Resize"))
            {
                // 変換先のピクセル情報を定義
                var info = new SKImageInfo(targetWidth, targetHeight, srcImage.ColorType, srcImage.AlphaType);

                // 新しいメモリ領域を確保（SKBitmap経由が扱いやすい）
                using var bitmap = new SKBitmap(info);

                srcImage.ScalePixels(bitmap.PeekPixels(), SKSamplingOptions.Default);
                using (Measuring.Time(true, "SkiaImageUtil.Resize SKImage.FromBitmap"))
                {
                    return SKImage.FromBitmap(bitmap);
                }
            }
        }

        public static SKImage Resize(
            SKPaint paint, SKImage src, SKRectI roi)
        {
            using (Measuring.Time(false, "SkiaBitmapUtil.Resize"))
            {
                ArgumentNullException.ThrowIfNull(src, nameof(src));

                var sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

                var info = new SKImageInfo(
                    roi.Width,
                    roi.Height,
                    src.ColorType,
                    src.AlphaType);

                using var surface = SKSurface.Create(info);

                var canvas = surface.Canvas;
                canvas.DrawImage(
                    src,
                    roi,
                    new SKRect(0, 0, roi.Width, roi.Height),
                    sampling,
                    paint);

                return surface.Snapshot();
            }
        }

        public static SKImage ReadImageFileToSKImage(byte[] bf)
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

        public static byte[] ToCompressionBinary(SKImage image)
        {
            using (Measuring.Time(false, "SkiaImageUtil.ToCompressionBinary"))
            {
                ArgumentNullException.ThrowIfNull(image, nameof(image));

                using (var data = image.Encode(SKEncodedImageFormat.Webp, WEBP_QUALITY))
                {
                    if (data == null)
                    {
                        throw new InvalidOperationException("Failed to encode to WebP");
                    }

                    return data.ToArray();
                }
            }
        }
    }
}
