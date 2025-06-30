using OpenCvSharp;
using OpenCvSharp.Extensions;
using SWF.Core.ConsoleAccessor;
using System.Buffers;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class OpenCVUtil
    {
        private static readonly ImageEncodingParam _webPQuality
            = new(ImwriteFlags.WebPQuality, 70);

        public static Mat ToMat(Bitmap bmp)
        {
            using (TimeMeasuring.Run(false, "OpenCVUtil.ToMat"))
            {
                return bmp.ToMat();
            }
        }

        public static Bitmap ToBitmap(Mat mat)
        {
            using (TimeMeasuring.Run(false, "OpenCVUtil.ToBitmap"))
            {
                return mat.ToBitmap();
            }
        }

        public static Bitmap Resize(Mat srcMat, float newWidth, float newHeight, InterpolationFlags flag)
        {
            ArgumentNullException.ThrowIfNull(srcMat, nameof(srcMat));

            using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Mat"))
            {
                var size = new OpenCvSharp.Size(newWidth, newHeight);
                using (var destMat = new Mat(size, srcMat.Type()))
                {
                    Cv2.Resize(srcMat, destMat, size, 0, 0, flag);
                    return ToBitmap(destMat);
                }
            }
        }

        public static Mat Resize(Bitmap srcBmp, float width, float height, InterpolationFlags flag)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Bitmap"))
            {
                var size = new OpenCvSharp.Size(width, height);
                using (var srcMat = ToMat(srcBmp))
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
            Cv2.ImEncode(".jpeg", mat, out var bf, _webPQuality);
            return bf;
        }

        public static byte[] BitmapToBytes(Bitmap bmp)
        {
            ArgumentNullException.ThrowIfNull(bmp, nameof(bmp));

            using (TimeMeasuring.Run(true, "OpenCVUtil.BitmapToBytes"))
            {
                using (var mat = ToMat(bmp))
                {
                    var bf = MatToBytes(mat);
                    var length = bf.Length;
                    var ret = ArrayPool<byte>.Shared.Rent(length);
                    ret.AsMemory(0, length).Span.Clear();
                    bf.CopyTo(ret, 0);
                    return ret;
                }
            }
        }

        public static byte[] MatToBytes(Mat mat)
        {
            ArgumentNullException.ThrowIfNull(mat, nameof(mat));

            using (TimeMeasuring.Run(true, "OpenCVUtil.MatToBytes"))
            {
                Cv2.ImEncode(".jpeg", mat, out var bf);
                return bf;
            }
        }

        public static Mat BytesToMat(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (TimeMeasuring.Run(true, "OpenCVUtil.BytesToMat"))
            {
                return Cv2.ImDecode(bf, ImreadModes.Unchanged);
            }
        }
    }
}
