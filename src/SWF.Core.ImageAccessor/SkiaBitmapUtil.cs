using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{
    public static class SkiaBitmapUtil
    {
        private const int WEBP_QUALITY = 80;

        public static Bitmap ToBitmap(SKBitmap src)
        {
            using (Measuring.Time(false, "SkiaBitmapUtil.ToBitmap"))
            {
                return src.ToBitmap();
            }
        }

        public static SKBitmap ToSKBitmap(Bitmap src)
        {
            using (Measuring.Time(true, "SkiaBitmapUtil.ToSKBitmap"))
            {
                ArgumentNullException.ThrowIfNull(src, nameof(src));

                return src.ToSKBitmap();
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

                using (var skBitmap = SKBitmap.Decode(stream))
                {
                    if (skBitmap == null)
                    {
                        throw new InvalidOperationException("Failed to decode image from stream.");
                    }

                    return ToBitmap(skBitmap);
                }
            }
        }

        public static Bitmap Resize(SKBitmap srcBitmap, float newWidth, float newHeight)
        {
            using (Measuring.Time(false, "SkiaBitmapUtil.Resize"))
            {
                ArgumentNullException.ThrowIfNull(srcBitmap, nameof(srcBitmap));

                var targetWidth = (int)newWidth;
                var targetHeight = (int)newHeight;

                if (srcBitmap.Width == targetWidth && srcBitmap.Height == targetHeight)
                {
                    return ToBitmap(srcBitmap);
                }

                SKSamplingOptions sampling;

                if (srcBitmap.Width > targetWidth || srcBitmap.Height > targetHeight)
                {
                    sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
                }
                else
                {
                    sampling = new SKSamplingOptions(SKCubicResampler.CatmullRom);
                }

                using var destBitmap = new SKBitmap(targetWidth, targetHeight);

                using (var image = SKImage.FromBitmap(srcBitmap))
                using (var canvas = new SKCanvas(destBitmap))
                using (var paint = new SKPaint { IsAntialias = true })
                {
                    canvas.DrawImage(image, new SKRect(0, 0, targetWidth, targetHeight), sampling, paint);
                }

                return ToBitmap(destBitmap);
            }
        }

        public static Bitmap Resize(
            SKBitmap src, SKRectI roi, float targetWidth, float targetHeight)
        {
            using (Measuring.Time(false, "SkiaBitmapUtil.Resize"))
            {
                ArgumentNullException.ThrowIfNull(src, nameof(src));

                var tw = (int)targetWidth;
                var th = (int)targetHeight;

                SKSamplingOptions sampling;
                if (roi.Width > tw || roi.Height > th)
                {
                    sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
                }
                else
                {
                    sampling = new SKSamplingOptions(SKCubicResampler.CatmullRom);
                }

                using var destSkia = new SKBitmap(tw, th);

                using (var canvas = new SKCanvas(destSkia))
                using (var paint = new SKPaint { IsAntialias = true })
                {
                    canvas.DrawBitmap(src, roi, new SKRect(0, 0, tw, th), paint);
                }

                return ToBitmap(destSkia);
            }
        }

        public static SKBitmap ReadImageFileToSKBitmap(byte[] bf)
        {
            using (Measuring.Time(false, "SkiaBitmapUtil.ReadImageFileToSKBitmap"))
            {
                if (bf == null || bf.Length == 0)
                {
                    throw new ArgumentException("Image data buffer is null or empty.", nameof(bf));
                }

                var bitmap = SKBitmap.Decode(bf);
                if (bitmap == null)
                {
                    throw new InvalidOperationException("Failed to decode image from the provided byte array.");
                }

                return bitmap;
            }
        }

        public static byte[] ToCompressionBinary(SKImage image)
        {
            using (Measuring.Time(false, "SkiaBitmapUtil.ToCompressionBinary"))
            {
                ArgumentNullException.ThrowIfNull(image, nameof(image));

                using (var data = image.Encode(SKEncodedImageFormat.Webp, WEBP_QUALITY))
                {
                    if (data == null)
                    {
                        throw new InvalidOperationException("Failed to encode image to WebP format.");
                    }

                    return data.ToArray();
                }
            }
        }
    }
}
