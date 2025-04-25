using ImageMagick;
using SWF.Core.ConsoleAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class MagickUtil
    {
        public static Bitmap ReadImageFile(Stream stream, MagickFormat format)
        {
            using (var image = new MagickImage(stream, format))
            {
                return image.ToBitmap();
            }
        }

        public static Bitmap ReadImageFile(string filePath)
        {
            using (var image = new MagickImage(filePath))
            {
                return image.ToBitmap();
            }
        }

        public static MagickFormat DetectFormat(string filePath)
        {
            using (TimeMeasuring.Run(false, "MagickUtil.DetectFormatFromFilePath"))
            {
                var info = new MagickImageInfo(filePath);
                return info.Format;
            }
        }
    }
}
