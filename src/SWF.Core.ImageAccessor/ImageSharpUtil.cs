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
using SWF.Core.Job;

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

        public static async ValueTask<System.Drawing.Size> GetImageSize(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            var imageInfo = await SixLabors.ImageSharp.Image.IdentifyAsync(fs).False();
            return new System.Drawing.Size(imageInfo.Width, imageInfo.Height);
        }

        public static async ValueTask<string> DetectFormat(Stream fs)
        {
            using (Measuring.Time(false, "SixLaborsUtil.DetectFormat"))
            {
                var format = await SixLabors.ImageSharp.Image.DetectFormatAsync(DECODER_OPTIONS, fs).False();
                return $".{format.Name}";
            }
        }

        public static async ValueTask<Bitmap> ReadImageFile(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(DECODER_OPTIONS, fs).False())
            {
                return ImageSharpToBitmap((Image<Rgba32>)image);
            }
        }

        private static Bitmap ImageSharpToBitmap(Image<Rgba32> image)
        {
            var width = image.Width;
            var height = image.Height;
            Bitmap? bitmap = null;
            System.Drawing.Imaging.BitmapData? bitmapData = null;

            try
            {
                bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                bitmapData = bitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, width, height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    bitmap.PixelFormat);

                unsafe
                {
                    var ptr = (byte*)bitmapData.Scan0;
                    var stride = bitmapData.Stride;

                    image.ProcessPixelRows(accessor =>
                    {
                        for (var y = 0; y < height; y++)
                        {
                            var pixelRow = accessor.GetRowSpan(y);
                            var row = ptr + (y * stride);

                            for (var x = 0; x < width; x++)
                            {
                                var pixel = pixelRow[x];
                                var pixelPtr = row + (x * 4);
                                pixelPtr[0] = pixel.B;
                                pixelPtr[1] = pixel.G;
                                pixelPtr[2] = pixel.R;
                                pixelPtr[3] = pixel.A;
                            }
                        }
                    });
                }

                bitmap.UnlockBits(bitmapData);
                bitmapData = null;  // UnlockBitsŠ®—¹‚ð‹L˜^

                var result = bitmap;
                bitmap = null;  // Š—LŒ ‚ðˆÚ÷
                return result;
            }
            finally
            {
                // bitmapData‚ªnull‚Å‚È‚¢ = ‚Ü‚¾Unlock‚³‚ê‚Ä‚¢‚È‚¢
                if (bitmapData != null && bitmap != null)
                {
                    bitmap.UnlockBits(bitmapData);
                }

                bitmap?.Dispose();
            }
        }
    }
}
