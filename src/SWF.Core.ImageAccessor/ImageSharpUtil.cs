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
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    internal static class ImageSharpUtil
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

        public static System.Drawing.Size GetImageSize(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            var imageInfo = SixLabors.ImageSharp.Image.Identify(fs);
            return new System.Drawing.Size(imageInfo.Width, imageInfo.Height);
        }

        public static string DetectFormat(Stream fs)
        {
            using (TimeMeasuring.Run(false, "SixLaborsUtil.DetectFormat"))
            {
                var format = SixLabors.ImageSharp.Image.DetectFormat(DECODER_OPTIONS, fs);
                return $".{format.Name}";
            }
        }

        public static Bitmap ReadImageFile(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (var image = SixLabors.ImageSharp.Image.Load(DECODER_OPTIONS, fs))
            {
                return ImageSharpToBitmap((Image<Rgba32>)image);
            }
        }

        private static Bitmap ImageSharpToBitmap(Image<Rgba32> image)
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

                unsafe
                {
                    var ptr = (byte*)bitmapData.Scan0;
                    var stride = bitmapData.Stride;

                    for (var y = 0; y < height; y++)
                    {
                        var row = ptr + (y * stride);
                        for (var x = 0; x < width; x++)
                        {
                            var pixel = image[x, y];
                            var pixelPtr = row + (x * 4);

                            pixelPtr[0] = pixel.B; // Blue
                            pixelPtr[1] = pixel.G; // Green
                            pixelPtr[2] = pixel.R; // Red
                            pixelPtr[3] = pixel.A; // Alpha
                        }
                    }
                }

                return bitmap;
            }
            finally
            {
                if (bitmapData != null)
                {
                    bitmap.UnlockBits(bitmapData);
                    bitmapData = null;
                }
            }
        }
    }
}
