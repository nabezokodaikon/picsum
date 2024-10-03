using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace SWF.Core.ImageAccessor
{
    internal static class SkiaSharpUtil
    {
        public static Bitmap ReadImageFile(FileStream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (var skBitmap = SKBitmap.Decode(fs))
            {
                return skBitmap.ToBitmap();
            }
        }
    }
}
