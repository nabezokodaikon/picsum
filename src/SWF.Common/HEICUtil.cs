using ImageMagick;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;

namespace SWF.Common
{
    [SupportedOSPlatform("windows")]
    internal class HEICUtil
    {
        public static Bitmap ReadImageFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var randomAccessStream = fs.AsRandomAccessStream())
            using (var stream = randomAccessStream.AsStreamForRead())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                ms.Position = 0;
                using (var image = new MagickImage(ms, MagickFormat.Heic))
                using (MemoryStream convertedStream = new MemoryStream())
                {
                    image.Write(convertedStream, MagickFormat.Bmp);
                    convertedStream.Position = 0;
                    return new Bitmap(convertedStream);
                }
            }
        }

        public static Size GetImageSize(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var randomAccessStream = fs.AsRandomAccessStream())
            using (var stream = randomAccessStream.AsStreamForRead())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                ms.Position = 0;
                using (var image = new MagickImage(ms, MagickFormat.Heic))
                {
                    return new Size(image.Width, image.Height);
                }
            }
        }
    }
}
