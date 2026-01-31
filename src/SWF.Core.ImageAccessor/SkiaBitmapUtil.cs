using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SWF.Core.Base;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public static class SkiaBitmapUtil
    {
        public static Bitmap ReadImageFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            using (Measuring.Time(false, "SkiaBitmapUtil.ReadImageFile"))
            {
                ArgumentNullException.ThrowIfNull(stream, nameof(stream));
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

        public static Bitmap ToBitmap(SKBitmap skBitmap)
        {
            using (Measuring.Time(false, "SkiaBitmapUtil.ToBitmap"))
            {
                if (skBitmap == null)
                {
                    throw new InvalidOperationException("Failed to decode image.");
                }

                return skBitmap.ToBitmap();
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
    }
}
