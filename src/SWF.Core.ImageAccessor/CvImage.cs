using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace SWF.Core.ImageAccessor
{
    public sealed class CvImage
        : IDisposable
    {
        public static readonly CvImage EMPTY = new CvImage(System.Drawing.Size.Empty);

        private bool disposed = false;
        private readonly object lockObject = new object();
        private Mat? mat = null;
        private readonly Bitmap? bitmap;

        public readonly System.Drawing.Size Size;
        public readonly int Width;
        public readonly int Height;
        public readonly bool IsEmpty;

        public CvImage(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            this.bitmap = bitmap;
            this.Width = bitmap.Width;
            this.Height = bitmap.Height;
            this.Size = bitmap.Size;
            this.IsEmpty = false;
        }

        public CvImage(System.Drawing.Size size)
        {
            this.bitmap = null;
            this.Width = size.Width;
            this.Height = size.Height;
            this.Size = size;
            this.IsEmpty = true;
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.bitmap?.Dispose();

                if (this.mat != null)
                {
                    this.mat.Dispose();
                    this.mat = null;
                }
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

        private Bitmap Resize(int newWidth, int newHeight)
        {
            if (this.bitmap == null)
            {
                throw new NullReferenceException("BitmapがNullです。");
            }

            lock (this.lockObject)
            {
                if (this.mat == null)
                {
                    this.mat = this.bitmap.ToMat();
                }

                return OpenCVUtil.Resize(this.mat, newWidth, newHeight);
            }
        }

        public void CreateMat()
        {
            var sw = Stopwatch.StartNew();

            if (this.bitmap == null)
            {
                return;
            }

            lock (this.lockObject)
            {
                if (this.mat == null)
                {
                    this.mat = this.bitmap.ToMat();
                }
            }

            sw.Stop();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] CvImage.CreateMat: {sw.ElapsedMilliseconds} ms");
        }

        public CvImage ShallowCopy()
        {
            if (this.bitmap == null)
            {
                return new CvImage(this.Size);
            }

            lock (this.lockObject)
            {
                var sw = Stopwatch.StartNew();

                using (var mat = this.bitmap.ToMat())
                {
                    var bmp = mat.ToBitmap();
                    var clone = new CvImage(bmp);

                    sw.Stop();
                    Console.WriteLine($"[{Thread.CurrentThread.Name}] CvImage.ShallowCopy: {sw.ElapsedMilliseconds} ms");
                    return clone;
                }
            }
        }

        public CvImage DeepCopy()
        {
            if (this.bitmap == null)
            {
                return new CvImage(this.Size);
            }

            lock (this.lockObject)
            {
                var sw = Stopwatch.StartNew();

                if (this.mat == null)
                {
                    this.mat = this.bitmap.ToMat();
                }

                var bmp = this.mat.ToBitmap();
                var clone = new CvImage(bmp);

                sw.Stop();
                Console.WriteLine($"[{Thread.CurrentThread.Name}] CvImage.DeepCopy: {sw.ElapsedMilliseconds} ms");
                return clone;
            }
        }

        public void DrawEmptyImage(Graphics g, Brush brushe, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(brushe, nameof(brushe));

            g.CompositingMode = CompositingMode.SourceCopy;
            g.FillRectangle(brushe, destRect);
        }

        public void DrawSourceImage(Graphics g, RectangleF destRect, RectangleF srcRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this.bitmap == null)
            {
                throw new NullReferenceException("BitmapがNullです。");
            }

            g.CompositingMode = CompositingMode.SourceOver;
            g.DrawImage(this.bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        public void DrawResizeImage(Graphics g, RectangleF destRect, RectangleF srcRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this.bitmap == null)
            {
                throw new NullReferenceException("BitmapがNullです。");
            }

            using (var resizeImage = this.Resize((int)destRect.Width, (int)destRect.Height))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                g.DrawImage(resizeImage, destRect,
                    new RectangleF(0, 0, destRect.Width, destRect.Height), GraphicsUnit.Pixel);
            }
        }
    }
}

