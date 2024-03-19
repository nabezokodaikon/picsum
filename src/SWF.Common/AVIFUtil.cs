using HeyRed.ImageSharp.Heif.Formats.Avif;
using HeyRed.ImageSharp.Heif.Formats.Heif;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;

namespace SWF.Common
{
    [SupportedOSPlatform("windows")]
    internal static class AVIFUtil
    {
        private static readonly DecoderOptions AVIF_DECODER_OPTIONS = new DecoderOptions()
        {
            Configuration = new Configuration(
                new AvifConfigurationModule(),
                new HeifConfigurationModule())
        };

        private static readonly BmpEncoder AVIF_ENCODER = new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder();

        public static Bitmap ReadImageFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var image = SixLabors.ImageSharp.Image.Load(AVIF_DECODER_OPTIONS, fs))
            using (var mem = new MemoryStream())
            {
                image.SaveAsBmp(mem, AVIF_ENCODER);
                mem.Position = 0;
                var bitmap = (Bitmap)System.Drawing.Image.FromStream(mem);
                return bitmap;
            }
        }

        public static System.Drawing.Size GetImageSize(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var image = SixLabors.ImageSharp.Image.Load(AVIF_DECODER_OPTIONS, fs))
            {
                return new System.Drawing.Size(image.Width, image.Height);
            }
        }
    }
}
