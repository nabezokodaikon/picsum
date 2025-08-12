using OpenCvSharp;
using OpenCvSharp.Extensions;
using SWF.Core.Base;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class OpenCVUtil
    {
        private static readonly ImageEncodingParam WEBP_QUALITY
            = new(ImwriteFlags.WebPQuality, 70);

        public static Bitmap ToBitmap(Mat mat)
        {
            using (TimeMeasuring.Run(false, "OpenCVUtil.ToBitmap"))
            {
                return mat.ToBitmap();
            }
        }

        public static Mat ToMat(Bitmap bmp)
        {
            using (TimeMeasuring.Run(false, "OpenCVUtil.ToMat"))
            {
                return bmp.ToMat();
            }
        }

        public static Bitmap Resize(Mat srcMat, float newWidth, float newHeight)
        {
            ArgumentNullException.ThrowIfNull(srcMat, nameof(srcMat));

            var size = new OpenCvSharp.Size(newWidth, newHeight);
            using (var destMat = new Mat(size, srcMat.Type()))
            {
                if (srcMat.Width > newWidth || srcMat.Height > newHeight)
                {
                    using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Mat: Area"))
                    {
                        Cv2.Resize(srcMat, destMat, size, 0, 0, InterpolationFlags.Area);
                    }
                }
                else if (srcMat.Width < newWidth || srcMat.Height < newHeight)
                {
                    using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Mat: Cubic"))
                    {
                        Cv2.Resize(srcMat, destMat, size, 0, 0, InterpolationFlags.Cubic);
                    }
                }
                else
                {
                    using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Mat: Nearest"))
                    {
                        Cv2.Resize(srcMat, destMat, size, 0, 0, InterpolationFlags.Nearest);
                    }
                }

                return ToBitmap(destMat);
            }
        }

        public static Mat Resize(Bitmap srcBmp, float width, float height)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            var size = new OpenCvSharp.Size(width, height);
            using (var srcMat = ToMat(srcBmp))
            {
                var destMat = new Mat(size, srcMat.Type());

                if (srcMat.Width > width || srcMat.Height > height)
                {
                    using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Bitmap: Area"))
                    {
                        Cv2.Resize(srcMat, destMat, size, 0, 0, InterpolationFlags.Area);
                    }
                }
                else if (srcMat.Width < width || srcMat.Height < height)
                {
                    using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Bitmap: Cubic"))
                    {
                        Cv2.Resize(srcMat, destMat, size, 0, 0, InterpolationFlags.Cubic);
                    }
                }
                else
                {
                    using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Bitmap: Nearest"))
                    {
                        Cv2.Resize(srcMat, destMat, size, 0, 0, InterpolationFlags.Nearest);
                    }
                }

                return destMat;
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
                    return ToBitmap(mat);
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
            Cv2.ImEncode(".webp", mat, out var bf, WEBP_QUALITY);
            return bf;
        }
    }
}
