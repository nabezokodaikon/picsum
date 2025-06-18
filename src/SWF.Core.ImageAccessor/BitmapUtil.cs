using SWF.Core.ConsoleAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class BitmapUtil
    {
        public static Size GetImageSize(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (TimeMeasuring.Run(false, "BitmapUtil.GetImageSize"))
            {
                using (var reader = new BinaryReader(fs))
                {
                    var bmpSignature = reader.ReadBytes(2);
                    if (bmpSignature[0] != 'B' || bmpSignature[1] != 'M')
                    {
                        return ImageUtil.EMPTY_SIZE;
                    }

                    reader.BaseStream.Seek(18, SeekOrigin.Begin);

                    var width = reader.ReadInt32();
                    var height = reader.ReadInt32();
                    return new Size(width, Math.Abs(height));
                }
            }
        }
    }
}
