using SkiaSharp;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public static class SkiaUtil
    {
        public static SKImage ToSKImage(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(
                rect,
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppPArgb);

            try
            {
                var info = new SKImageInfo(
                    bitmap.Width,
                    bitmap.Height,
                    SKColorType.Bgra8888,
                    SKAlphaType.Premul);

                return SKImage.FromPixelCopy(info, data.Scan0, data.Stride);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }
    }
}
