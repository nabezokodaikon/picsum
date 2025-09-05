using Svg;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    internal static class SvgUtil
    {
        public static Size GetImageSize(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            var doc = SvgDocument.Open<SvgDocument>(stream);
            return new Size((int)doc.Width.Value, (int)doc.Height.Value);
        }

        public static Bitmap ReadImageFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            var doc = SvgDocument.Open<SvgDocument>(stream);

            var (width, height) = ((int)doc.Width.Value, (int)doc.Height.Value);
            var bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            {
                using (Measuring.Time(false, "SVGUtil.ReadImageFile Draw"))
                {
                    doc.Draw(g, new SizeF(width, height));
                    return bitmap;
                }
            }
        }
    }
}
