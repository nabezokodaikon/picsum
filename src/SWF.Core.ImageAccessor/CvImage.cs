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

        public Bitmap Bitmap { get; private set; }


        public System.Drawing.Size Size
        {
            get
            {
                return this.Bitmap.Size;
            }
        }

        public int Width
        {
            get
            {
                return this.Bitmap.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.Bitmap.Height;
            }
        }

        public CvImage(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            this.Bitmap = bitmap;
            this.mat = this.Bitmap.ToMat();
        }

        private CvImage(Mat mat)
        {
            this.mat = mat;
            this.Bitmap = this.mat.ToBitmap();
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Bitmap.Dispose();
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
    }
}

