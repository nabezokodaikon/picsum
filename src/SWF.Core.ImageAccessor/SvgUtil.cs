using Svg;

namespace SWF.Core.ImageAccessor
{
    internal static class SvgUtil
    {
        public static Bitmap ReadImageFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var svgDocument = SvgDocument.Open(filePath);
            return svgDocument.Draw();
        }
    }
}
