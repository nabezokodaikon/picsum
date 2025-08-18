using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    internal static class BitmapUtil
    {
        public static Size GetImageSize(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (TimeMeasuring.Run(false, "BitmapUtil.GetImageSize"))
            {
                using (var reader = new BinaryReader(fs, System.Text.Encoding.UTF8, true))
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
