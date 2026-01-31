using SkiaSharp;
using SWF.Core.Base;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public static class SkiaImageUtil
    {
        private const int WEBP_QUALITY = 80;

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
