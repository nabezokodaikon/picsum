using HeyRed.ImageSharp.Heif.Formats.Avif;
using HeyRed.ImageSharp.Heif.Formats.Heif;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    internal static class SixLaborsUtil
    {
        private static readonly DecoderOptions DECODER_OPTIONS = new()
        {
            Configuration = new Configuration(
                new AvifConfigurationModule(),
                new HeifConfigurationModule(),
                new WebpConfigurationModule(),
                new JpegConfigurationModule(),
                new PngConfigurationModule(),
                new GifConfigurationModule(),
                new TiffConfigurationModule(),
                new TgaConfigurationModule()),
            SkipMetadata = true,
        };

        private static readonly BmpEncoder ENCODER = new();

        public static Bitmap ReadImageFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var imageData = File.ReadAllBytes(filePath);
            using (var loadMS = new MemoryStream(imageData))
            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(loadMS))
            using (var ms = new MemoryStream())
            {
                image.SaveAsBmp(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return new Bitmap(ms);
            }
        }

        public static Bitmap ReadImageFileWithDecoder(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var fs = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read, 8192, FileOptions.SequentialScan))
            using (var image = SixLabors.ImageSharp.Image.Load(DECODER_OPTIONS, fs))
            using (var mem = new MemoryStream())
            {
                image.SaveAsBmp(mem, ENCODER);
                mem.Position = 0;
                var bitmap = (Bitmap)System.Drawing.Image.FromStream(mem);
                return bitmap;
            }
        }

        public static System.Drawing.Size GetImageSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var image = SixLabors.ImageSharp.Image.Load(DECODER_OPTIONS, fs))
            {
                return new System.Drawing.Size(image.Width, image.Height);
            }
        }
    }
}
