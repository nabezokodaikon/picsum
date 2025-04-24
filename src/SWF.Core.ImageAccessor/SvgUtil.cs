using Svg;
using SWF.Core.Base;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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
                using (TimeMeasuring.Run(false, "SVGUtil.ReadImageFile Draw"))
                {
                    doc.Draw(g, new SizeF(width, height));
                    return bitmap;
                }
            }
        }
    }
}
