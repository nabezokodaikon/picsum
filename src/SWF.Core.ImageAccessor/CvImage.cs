using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;

namespace SWF.Core.ImageAccessor
{
    public sealed class CvImage
        : IDisposable
    {
        public static readonly CvImage EMPTY = new CvImage(ImageUtil.EMPTY_IMAGE);

        private bool disposed = false;
        private readonly Mat mat;

        public System.Drawing.Size Size
        {
            get
            {
                return new System.Drawing.Size(this.Width, this.Height);
            }
        }

        public int Width
        {
            get
            {
                return this.mat.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.mat.Height;
            }
        }

        public CvImage(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            this.mat = bitmap.ToMat();
        }

        private CvImage(Mat mat)
        {
            this.mat = mat;
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.mat.Dispose();

                var sw = Stopwatch.StartNew();
                GC.Collect();
                sw.Stop();
                Console.WriteLine($"[{Thread.CurrentThread.Name}] GC.Collect: {sw.ElapsedMilliseconds} ms");
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CvImage()
        {
            this.Dispose(false);
        }

        public Bitmap Resize(int newWidth, int newHeight)
        {
            return OpenCVUtil.Resize(this.mat, newWidth, newHeight);
        }

        public CvImage Clone()
        {
            var sw = Stopwatch.StartNew();
            var clone = new CvImage(this.mat.Clone());
            sw.Stop();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] CvImage.Clone: {sw.ElapsedMilliseconds} ms");
            return clone;
        }

        public Bitmap CreateBitmap()
        {
            var sw = Stopwatch.StartNew();
            var bmp = this.mat.ToBitmap();
            sw.Stop();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] CvImage.CreateBitmap: {sw.ElapsedMilliseconds} ms");
            return bmp;
        }
    }
}

