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
                Console.WriteLine($"DetectFormat: {sw.ElapsedMilliseconds} ms");
            }
        }

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

        public static Bitmap ReadImageFileWithDecoder(FileStream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            var sw = Stopwatch.StartNew();
            using (var image = SixLabors.ImageSharp.Image.Load(DECODER_OPTIONS, fs))
            {
                sw.Stop();
                Console.WriteLine($"ReadImageFileWithDecoder Image.Load: {sw.ElapsedMilliseconds} ms");
                using (var mem = new MemoryStream())
                {
                    sw.Restart();
                    var img = ConvertImageSharpImageToBitmap((Image<Rgba32>)image);
                    sw.Stop();
                    Console.WriteLine($"ReadImageFileWithDecoder ConvertImageSharpImageToBitmap: {sw.ElapsedMilliseconds} ms");
                    return img;
                }
            }
        }

        public static System.Drawing.Size GetImageSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var fs = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read, 8, FileOptions.SequentialScan))
            using (var image = SixLabors.ImageSharp.Image.Load(DECODER_OPTIONS, fs))
            {
                return new System.Drawing.Size(image.Width, image.Height);
            }
        }

        private static Bitmap ConvertImageSharpImageToBitmap(Image<Rgba32> image)
        {
            var width = image.Width;
            var height = image.Height;
            var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Bitmapをロックしてピクセルデータにアクセスする
            System.Drawing.Imaging.BitmapData bitmapData = null;

            try
            {
                bitmapData = bitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, width, height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    bitmap.PixelFormat);

                // ピクセルデータへのポインタを取得する
                var ptr = bitmapData.Scan0;
                var bytes = Math.Abs(bitmapData.Stride) * height;
                var rgbValues = new byte[bytes];

                // ImageSharpの画像データをピクセルごとにコピーする
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

                // ピクセルデータをアンマネージメモリにコピーする
                Marshal.Copy(rgbValues, 0, ptr, bytes);

                return bitmap;
            }
            finally
            {
                // ビットマップのロックを解除する
                if (bitmapData != null)
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }
        }
    }
}
