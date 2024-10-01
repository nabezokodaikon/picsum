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

            var sw = Stopwatch.StartNew();
            using (var destMat = new Mat())
            {
                Cv2.Resize(srcMat, destMat, new OpenCvSharp.Size(newWidth, newHeight), 0, 0, InterpolationFlags.Area);
                var destBMP = destMat.ToBitmap();
                sw.Stop();
                Console.WriteLine($"[{Thread.CurrentThread.Name}] OpenCVUtil.Resize: {sw.ElapsedMilliseconds} ms");
                return destBMP;
            }
        }

        public static Bitmap Convert(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            var sw = Stopwatch.StartNew();
            stream.Seek(0, SeekOrigin.Begin);
            using (var mat = Mat.FromStream(stream, ImreadModes.Color))
            {
                var bmp = mat.ToBitmap();
                sw.Stop();
                Console.WriteLine($"[{Thread.CurrentThread.Name}] OpenCVUtil.Convert: {sw.ElapsedMilliseconds} ms");
                return bmp;
            }
        }
    }
}
