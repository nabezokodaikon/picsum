using SkiaSharp;

namespace SWF.Core.ImageAccessor
{
    public static class SkiaUtil
    {
        // System.Drawing.BitmapからSKBitmapへ変換
        public static SKBitmap ConvertToSKBitmap(Bitmap gdiBitmap)
        {
            ArgumentNullException.ThrowIfNull(gdiBitmap, nameof(gdiBitmap));

            var info = new SKImageInfo(gdiBitmap.Width, gdiBitmap.Height,
                                       SKColorType.Bgra8888, SKAlphaType.Premul);
            var skBitmap = new SKBitmap(info);

            using (var pixmap = skBitmap.PeekPixels())
            {
                var bmpData = gdiBitmap.LockBits(
                    new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                unsafe
                {
                    Buffer.MemoryCopy(
                        bmpData.Scan0.ToPointer(),
                        pixmap.GetPixels().ToPointer(),
                        pixmap.RowBytes * pixmap.Height,
                        bmpData.Stride * bmpData.Height);
                }

                gdiBitmap.UnlockBits(bmpData);
            }

            return skBitmap;
        }
    }
}
