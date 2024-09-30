using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace SWF.Core.ImageAccessor
{
    internal static class OpenCVUtil
    {
        public static Bitmap Resize(Bitmap srcBMP, int newWidth, int newHeight)
        {
            ArgumentNullException.ThrowIfNull(srcBMP, nameof(srcBMP));

            var scale = newWidth / (float)srcBMP.Width;

            using (var srcMat = srcBMP.ToMat())
            using (var destMat = new Mat())
            {
                Cv2.Resize(srcMat, destMat, new OpenCvSharp.Size(newWidth, newHeight), 0, 0, InterpolationFlags.Area);
                return destMat.ToBitmap();
            }
        }

        public static Bitmap Clone(Bitmap src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            using (var m = src.ToMat())
            {
                return m.ToBitmap();
            }
        }
    }
}
