using HeyRed.ImageSharp.Heif.Formats.Avif;
using HeyRed.ImageSharp.Heif.Formats.Heif;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Qoi;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
                new BmpConfigurationModule(),
                new GifConfigurationModule(),
                new HeifConfigurationModule(),
                new JpegConfigurationModule(),
                new PbmConfigurationModule(),
                new PngConfigurationModule(),
                new QoiConfigurationModule(),
                new TgaConfigurationModule(),
                new TiffConfigurationModule(),
                new WebpConfigurationModule()),
            SkipMetadata = true,
        };

        public static System.Drawing.Size GetImageSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var imageInfo = SixLabors.ImageSharp.Image.Identify(filePath);
            return new System.Drawing.Size(imageInfo.Width, imageInfo.Height);
        }

        public static IImageFormat DetectFormat(FileStream fs)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return SixLabors.ImageSharp.Image.DetectFormat(DECODER_OPTIONS, fs);
            }
            finally
            {
                sw.Stop();
                //Console.WriteLine($"[{Thread.CurrentThread.Name}] SixLaborsUtil.DetectFormat: {sw.ElapsedMilliseconds} ms");
            }
        }

        public static Bitmap ReadImageFile(FileStream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (var image = SixLabors.ImageSharp.Image.Load(DECODER_OPTIONS, fs))
            {
                return ImageSharpeToBitmap((Image<Rgba32>)image);
            }
        }

        private static Bitmap ImageSharpeToBitmap(Image<Rgba32> image)
        {
            var width = image.Width;
            var height = image.Height;
            var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            System.Drawing.Imaging.BitmapData? bitmapData = null;

            try
            {
                bitmapData = bitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, width, height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    bitmap.PixelFormat);

                var ptr = bitmapData.Scan0;
                var bytes = Math.Abs(bitmapData.Stride) * height;
                var rgbValues = new byte[bytes];

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var pixel = image[x, y];
                        var position = (y * bitmapData.Stride) + (x * 4);

                        rgbValues[position] = pixel.B;
                        rgbValues[position + 1] = pixel.G;
                        rgbValues[position + 2] = pixel.R;
                        rgbValues[position + 3] = pixel.A;
                    }
                }

                Marshal.Copy(rgbValues, 0, ptr, bytes);

                return bitmap;
            }
            finally
            {
                if (bitmapData != null)
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }
        }
    }
}
