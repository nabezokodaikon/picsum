using LibHeifSharp;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class LibHeifSharpUtil
    {
        public static Size GetImageSize(FileStream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (var context = new HeifContext(fs))
            using (var primaryImageHandle = context.GetPrimaryImageHandle())
            {
                return new Size(primaryImageHandle.Width, primaryImageHandle.Height);
            }
        }

        public static unsafe Bitmap ReadImageFile(Stream fs)
        {
            try
            {
                using (var context = new HeifContext(fs))
                using (var handle = context.GetPrimaryImageHandle())
                using (var heifImage = handle.Decode(HeifColorspace.Undefined, HeifChroma.InterleavedRgba32))
                {
                    var width = heifImage.Width;
                    var height = heifImage.Height;
                    var stride = width * 4;

                    var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    var bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb);

                    try
                    {
                        var plane = heifImage.GetPlane(HeifChannel.Interleaved);
                        var srcPtr = (byte*)plane.Scan0;
                        var dstPtr = (byte*)bitmapData.Scan0;

                        Parallel.For(0, height, y =>
                        {
                            var srcRow = srcPtr + y * stride;
                            var dstRow = dstPtr + y * bitmapData.Stride;

                            for (int x = 0; x < width; x++)
                            {
                                dstRow[0] = srcRow[2]; // B
                                dstRow[1] = srcRow[1]; // G
                                dstRow[2] = srcRow[0]; // R
                                dstRow[3] = srcRow[3]; // A

                                srcRow += 4;
                                dstRow += 4;
                            }
                        });

                        return bitmap;
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                }
            }
            catch (HeifException ex)
            {
                throw new Exception($"HEIFデコードエラー: {ex.Message}", ex);
            }
        }
    }
}

