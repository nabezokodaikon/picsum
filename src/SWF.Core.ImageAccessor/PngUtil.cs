using SWF.Core.ConsoleAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class PngUtil
    {
        private static readonly byte[] EXPECTED_PNG_SIGNATURE
            = [137, 80, 78, 71, 13, 10, 26, 10];

        public static Size GetImageSize(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (TimeMeasuring.Run(true, "PngUtil.GetImageSize"))
            {
                using (var reader = new BinaryReader(fs))
                {
                    var pngSignature = reader.ReadBytes(8);
                    if (!CompareByteArrays(pngSignature, EXPECTED_PNG_SIGNATURE))
                    {
                        return ImageUtil.EMPTY_SIZE;
                    }

                    reader.ReadInt32();
                    var ihdrChunkType = reader.ReadBytes(4);
                    var ihdrChunkTypeStr = System.Text.Encoding.ASCII.GetString(ihdrChunkType);
                    if (ihdrChunkTypeStr != "IHDR")
                    {
                        return ImageUtil.EMPTY_SIZE;
                    }

                    var width = ReadBigEndianInt32(reader);
                    var height = ReadBigEndianInt32(reader);
                    return new Size(width, height);
                }
            }
        }

        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static int ReadBigEndianInt32(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
