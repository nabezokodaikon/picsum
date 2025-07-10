using ImageMagick;
using ImageMagick.Factories;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using SWF.Core.Base;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class MagickUtil
    {
        private static readonly MagickFactory MAGICK_FACTORY = new();

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

        public static byte[] ToCompressionBinary(Mat mat)
        {
            using (TimeMeasuring.Run(false, "MagickUtil.ToCompressionBinary"))
            {
                var bytes = mat.ToBytes();

                using (var magickImage = MAGICK_FACTORY.Image.Create(bytes))
                {
                    magickImage.Format = MagickFormat.Avif;
                    magickImage.Quality = 50;
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "encoder", "rav1e");
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "speed", "9");
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "threads", Environment.ProcessorCount.ToString());
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "chroma-subsampling", "4:2:0");
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "lossless", "false");
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "tile-rows", "2");
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "tile-cols", "2");

                    return magickImage.ToByteArray();
                }
            }
        }

        public static Mat ReadImageFileToMat(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (TimeMeasuring.Run(false, "MagickUtil.ReadImageFileToMat"))
            {
                using (var magickImage = new MagickImage(bf))
                using (var bmp = magickImage.ToBitmap())
                {
                    return bmp.ToMat();
                }
            }
        }
    }
}
