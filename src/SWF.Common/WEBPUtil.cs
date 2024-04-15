using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;

namespace SWF.Common
{
    [SupportedOSPlatform("windows")]
    internal static class WEBPUtil
    {
        public static Bitmap ReadImageFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var webpImage = SixLabors.ImageSharp.Image.Load<Rgba32>(fs))
            using (var mem = new MemoryStream())
            {
                webpImage.SaveAsBmp(mem);
                mem.Position = 0;
                var bitmap = (Bitmap)System.Drawing.Image.FromStream(mem);
                return bitmap;
            }
        }

        public static System.Drawing.Size GetImageSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var webpImage = SixLabors.ImageSharp.Image.Load<Rgba32>(fs))
            {
                return new System.Drawing.Size(webpImage.Width, webpImage.Height);
            }
        }
    }
}
