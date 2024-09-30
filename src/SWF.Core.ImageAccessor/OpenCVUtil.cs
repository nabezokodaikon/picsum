using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;

namespace SWF.Core.ImageAccessor
{
    internal static class OpenCVUtil
    {
        public static Bitmap Resize(Mat srcMat, int newWidth, int newHeight)
        {
            ArgumentNullException.ThrowIfNull(srcMat, nameof(srcMat));

            using (var destMat = new Mat())
            {
                Cv2.Resize(srcMat, destMat, new OpenCvSharp.Size(newWidth, newHeight), 0, 0, InterpolationFlags.Area);
                var destBMP = destMat.ToBitmap();
                return destBMP;
            }
        }
    }
}
