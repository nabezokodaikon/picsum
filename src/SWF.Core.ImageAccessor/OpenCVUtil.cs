using OpenCvSharp;
using OpenCvSharp.Extensions;
using SWF.Core.ConsoleAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class OpenCVUtil
    {
        public static Bitmap Resize(Mat srcMat, int newWidth, int newHeight, InterpolationFlags flag)
        {
            ArgumentNullException.ThrowIfNull(srcMat, nameof(srcMat));

            using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Mat"))
            {
                var size = new OpenCvSharp.Size(newWidth, newHeight);
                using (var destMat = new Mat(size, srcMat.Type()))
                {
                    Cv2.Resize(srcMat, destMat, new OpenCvSharp.Size(newWidth, newHeight), 0, 0, flag);
                    return destMat.ToBitmap();
                }
            }
        }

        public static Bitmap Resize(Bitmap srcBmp, int width, int height, InterpolationFlags flag)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Bitmap"))
            {
                var size = new OpenCvSharp.Size(width, height);
                using (var srcMat = srcBmp.ToMat())
                using (var destMat = new Mat(size, srcMat.Type()))
                {
                    Cv2.Resize(srcMat, destMat, size, 0, 0, flag);
                    return destMat.ToBitmap();
                }
            }
        }

        public static Bitmap ReadImageFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            using (TimeMeasuring.Run(false, "OpenCVUtil.ReadImageFile"))
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var mat = Mat.FromStream(stream, ImreadModes.Color))
                {
                    return mat.ToBitmap();
                }
            }
        }

        public static Mat ReadImageFileToMat(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            using (TimeMeasuring.Run(false, "OpenCVUtil.ReadImageFile"))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return Mat.FromStream(stream, ImreadModes.Color);
            }
        }
    }
}
