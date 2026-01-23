using ImageMagick;
using ImageMagick.Factories;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using SWF.Core.Base;
using System.Globalization;

namespace SWF.Core.ImageAccessor
{

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

        public static Bitmap ReadImageFileForHEIC(Stream stream)
        {
            using (var image = new MagickImage(stream))
            {
                // 1. カラープロファイルが存在する場合のみ sRGB へ変換
                // (最新の Magick.NET では ColorProfile.sRGB を使用)
                var profile = image.GetColorProfile();
                if (profile != null && profile.Name != "sRGB")
                {
                    image.TransformColorSpace(ColorProfiles.SRGB);
                }

                // 2. 不要なEXIF情報などを破棄してBitmap変換を高速化
                image.Strip();

                // 3. iPhoneの回転情報を反映
                image.AutoOrient();

                // 4. Bitmapへ変換 (Magick.NET.SystemDrawingパッケージが必要)
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
            using (Measuring.Time(false, "MagickUtil.DetectFormatFromFilePath"))
            {
                var info = new MagickImageInfo(filePath);
                return info.Format;
            }
        }

        public static byte[] ToCompressionBinary(Mat mat)
        {
            using (Measuring.Time(false, "MagickUtil.ToCompressionBinary"))
            {
                var bytes = mat.ToBytes();

                using (var magickImage = MAGICK_FACTORY.Image.Create(bytes))
                {
                    magickImage.Format = MagickFormat.Avif;
                    magickImage.Quality = 50;
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "encoder", "rav1e");
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "speed", "9");
                    magickImage.Settings.SetDefine(MagickFormat.Avif, "threads", Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture));
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

            using (Measuring.Time(false, "MagickUtil.ReadImageFileToMat"))
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
