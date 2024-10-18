using OpenCvSharp;
using OpenCvSharp.Extensions;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{
    internal static class OpenCVUtil
    {
        public static Bitmap Resize(Mat srcMat, int newWidth, int newHeight)
        {
            ArgumentNullException.ThrowIfNull(srcMat, nameof(srcMat));

            using (TimeMeasuring.Run(true, "OpenCVUtil.Resize By Mat"))
            using (var destMat = new Mat())
            {
                Cv2.Resize(srcMat, destMat, new OpenCvSharp.Size(newWidth, newHeight), 0, 0, InterpolationFlags.Area);
                return destMat.ToBitmap();
            }
        }

        public static Bitmap Resize(Bitmap srcBmp, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            using (TimeMeasuring.Run(true, "OpenCVUtil.Resize By Bitmap"))
            {
                try
                {
                    var size = new OpenCvSharp.Size(width, height);
                    using (var srcMat = srcBmp.ToMat())
                    using (var destMat = new Mat(size, srcMat.Type()))
                    {
                        Cv2.Resize(srcMat, destMat, size, 0, 0, InterpolationFlags.Area);
                        return destMat.ToBitmap();
                    }
                }
                catch (NotImplementedException)
                {
                    var destBmp = new Bitmap(width, height);
                    using (var g = Graphics.FromImage(destBmp))
                    {
                        g.DrawImage(
                            srcBmp,
                            new Rectangle(0, 0, width, height),
                            new Rectangle(0, 0, srcBmp.Width, srcBmp.Height),
                            GraphicsUnit.Pixel);
                    }
                    return destBmp;
                }
            }
        }

        public static Bitmap ReadImageFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            using (TimeMeasuring.Run(true, "OpenCVUtil.Convert"))
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var mat = Mat.FromStream(stream, ImreadModes.Color))
                {
                    return mat.ToBitmap();
                }
            }
        }
    }
}
