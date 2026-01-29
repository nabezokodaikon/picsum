using SkiaSharp;
using SWF.Core.Base;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public static class SkiaUtil
    {
        private const int WEBP_QUALITY = 80;

        /// <summary>
        /// SKImageをGDI+のBitmapに変換します（高品質・乱れ防止版）
        /// </summary>
        public static Bitmap ToBitmap(SKImage src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            using (Measuring.Time(false, "SkiaUtil.ToBitmap"))
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

            using (Measuring.Time(false, "SkiaUtil.ToSKImage"))
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

        public static Bitmap ReadImageFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            using (Measuring.Time(false, "SkiaUtil.ReadImageFile"))
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

        public static Bitmap Resize(SKImage srcImage, float newWidth, float newHeight)
        {
            using (Measuring.Time(false, "SkiaUtil.Resize"))
            {
                ArgumentNullException.ThrowIfNull(srcImage, nameof(srcImage));

                var targetWidth = (int)newWidth;
                var targetHeight = (int)newHeight;

                if (srcImage.Width == targetWidth && srcImage.Height == targetHeight)
                {
                    return ToBitmap(srcImage);
                }

                var sampling = srcImage.Width > targetWidth || srcImage.Height > targetHeight
                    ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                    : new SKSamplingOptions(SKCubicResampler.CatmullRom);

                using var destBitmap = new SKBitmap(targetWidth, targetHeight);
                using (var canvas = new SKCanvas(destBitmap))
                using (var paint = new SKPaint { IsAntialias = true })
                {
                    canvas.DrawImage(srcImage, new SKRect(0, 0, targetWidth, targetHeight), sampling, paint);
                }

                using (var resultImage = SKImage.FromBitmap(destBitmap))
                {
                    return ToBitmap(resultImage);
                }
            }
        }

        public static Bitmap Resize(SKImage src, SKRectI roi, float targetWidth, float targetHeight)
        {
            using (Measuring.Time(false, "SkiaUtil.Resize"))
            {
                ArgumentNullException.ThrowIfNull(src, nameof(src));

                var tw = (int)targetWidth;
                var th = (int)targetHeight;

                var sampling = roi.Width > tw || roi.Height > th
                    ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                    : new SKSamplingOptions(SKCubicResampler.CatmullRom);

                using var destSkia = new SKBitmap(tw, th);
                using (var canvas = new SKCanvas(destSkia))
                using (var paint = new SKPaint { IsAntialias = true })
                {
                    canvas.DrawImage(src, (SKRect)roi, new SKRect(0, 0, tw, th), sampling, paint);
                }

                using (var resultImage = SKImage.FromBitmap(destSkia))
                {
                    return ToBitmap(resultImage);
                }
            }
        }

        public static SKImage ReadImageFileToSKImage(byte[] bf)
        {
            using (Measuring.Time(false, "SkiaUtil.ReadImageFileToSKImage"))
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
            using (Measuring.Time(false, "SkiaUtil.ToCompressionBinary"))
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
