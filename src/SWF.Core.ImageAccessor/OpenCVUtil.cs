using OpenCvSharp;
using OpenCvSharp.Extensions;
using SWF.Core.ConsoleAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class OpenCVUtil
    {
        private static readonly ImageEncodingParam _webPQuality
            = new(ImwriteFlags.WebPQuality, 70);

        public static Bitmap Resize(Mat srcMat, float newWidth, float newHeight, InterpolationFlags flag)
        {
            ArgumentNullException.ThrowIfNull(srcMat, nameof(srcMat));

            using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Mat"))
            {
                var size = new OpenCvSharp.Size(newWidth, newHeight);
                using (var destMat = new Mat(size, srcMat.Type()))
                {
                    Cv2.Resize(srcMat, destMat, size, 0, 0, flag);
                    return destMat.ToBitmap();
                }
            }
        }

        public static Mat Resize(Bitmap srcBmp, float width, float height, InterpolationFlags flag)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Bitmap"))
            {
                var size = new OpenCvSharp.Size(width, height);
                using (var srcMat = srcBmp.ToMat())
                {
                    var destMat = new Mat(size, srcMat.Type());
                    Cv2.Resize(srcMat, destMat, size, 0, 0, flag);
                    return destMat;
                }
            }
        }

        public static Bitmap ReadImageFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            using (TimeMeasuring.Run(false, "OpenCVUtil.ReadImageFile"))
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var mat = Mat.FromStream(stream, ImreadModes.Unchanged))
                {
                    return mat.ToBitmap();
                }
            }
        }

        public static Mat ReadImageFileToMat(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (TimeMeasuring.Run(false, "OpenCVUtil.ReadImageFileToMat"))
            {
                return Cv2.ImDecode(bf, ImreadModes.Unchanged);
            }
        }

        public static byte[] ToCompressionBinary(Mat mat)
        {
            Cv2.ImEncode(".webp", mat, out var bf, _webPQuality);
            return bf;
        }
    }
}
