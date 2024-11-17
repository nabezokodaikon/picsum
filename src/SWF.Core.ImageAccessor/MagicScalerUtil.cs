using PhotoSauce.MagicScaler;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class MagicScalerUtil
    {
        private static readonly ProcessImageSettings settings = new()
        {
            Width = 0,
            Height = 0,
            ResizeMode = CropScaleMode.Max,
            EncoderOptions = new JpegEncoderOptions(85, ChromaSubsampleMode.Subsample420, false),
        };

        public static Bitmap ReadImageFile(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (var ms = new MemoryStream())
            {
                var settings = new ProcessImageSettings
                {
                    Width = 0,
                    Height = 0,
                    ResizeMode = CropScaleMode.Max,
                    EncoderOptions = new JpegEncoderOptions(85, ChromaSubsampleMode.Subsample420, false),
                };

                var _ = MagicImageProcessor.ProcessImage(fs, ms, settings);
                ms.Seek(0, SeekOrigin.Begin);
                return (Bitmap)Bitmap.FromStream(ms, false, true);
            }
        }
    }
}
