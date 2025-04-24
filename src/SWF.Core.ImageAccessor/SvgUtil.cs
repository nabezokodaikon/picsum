using Svg;

namespace SWF.Core.ImageAccessor
{
    internal static class SVGUtil
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
            return doc.Draw();
        }
    }
}
