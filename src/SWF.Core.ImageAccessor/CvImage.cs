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
        private readonly object lockObject = new object();
        private Mat? mat = null;

        public readonly Bitmap Bitmap;
        public readonly System.Drawing.Size Size;
        public readonly int Width;
        public readonly int Height;

        public CvImage(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            this.Bitmap = bitmap;
            this.Width = bitmap.Width;
            this.Height = bitmap.Height;
            this.Size = bitmap.Size;
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

                if (this.mat != null)
                {
                    this.mat.Dispose();
                    this.mat = null;
                }

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
            lock (this.lockObject)
            {
                if (this.mat == null)
                {
                    this.mat = this.Bitmap.ToMat();
                }

                return OpenCVUtil.Resize(this.mat, newWidth, newHeight);
            }
        }

        public CvImage Clone()
        {
            lock (this.lockObject)
            {
                var sw = Stopwatch.StartNew();

                if (this.mat == null)
                {
                    this.mat = this.Bitmap.ToMat();
                }

                var bmp = this.mat.ToBitmap();
                var clone = new CvImage(bmp);

                sw.Stop();
                Console.WriteLine($"[{Thread.CurrentThread.Name}] CvImage.Clone: {sw.ElapsedMilliseconds} ms");
                return clone;
            }
        }
    }
}

