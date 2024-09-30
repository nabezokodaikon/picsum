using Svg;
using System.Diagnostics;

namespace SWF.Core.ImageAccessor
{
    internal static class SvgUtil
    {
        public static Bitmap ReadImageFile(string filePath)
        {
            var sw = Stopwatch.StartNew();

            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var svgDocument = SvgDocument.Open(filePath);
                return svgDocument.Draw();
            }
            finally
            {
                sw.Stop();
                Console.WriteLine($"SvgUtil.ReadImageFile: {sw.ElapsedMilliseconds} ms");
            }

        }
    }
}
