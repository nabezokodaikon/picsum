using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace SWF.Core.ImageAccessor
{
    internal static class OpenCVUtil
    {
        private static readonly OpenCvSharp.Size GAUSSIAN_BLUR_SIZE = new(5, 5);

        public static System.Drawing.Bitmap Resize(System.Drawing.Bitmap srcBMP, int newWidth, int newHeight)
        {
            ArgumentNullException.ThrowIfNull(srcBMP, nameof(srcBMP));

            using (var srcMat = srcBMP.ToMat())
            using (var destMat = new Mat())
            {
                Cv2.GaussianBlur(srcMat, srcMat, GAUSSIAN_BLUR_SIZE, 0);
                Cv2.Resize(srcMat, destMat, new OpenCvSharp.Size(newWidth, newHeight), 0, 0, InterpolationFlags.Linear);
                return destMat.ToBitmap();
            }
        }

        public static System.Drawing.Bitmap Clone(System.Drawing.Bitmap src)
        {
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            using (var m = src.ToMat())
            {
                return m.ToBitmap();
            }
        }
    }
}
