using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public sealed partial class CvImage
        : IDisposable
    {
        public static readonly CvImage EMPTY = new(System.Drawing.Size.Empty);

        private bool disposed = false;
        private readonly object lockObject = new();
        private readonly PixelFormat pixelFormat;
        private Mat? mat;

        public readonly System.Drawing.Size Size;
        public readonly int Width;
        public readonly int Height;
        public readonly bool IsEmpty;

        public CvImage(Mat mat, PixelFormat pixelFormat)
        {
            ArgumentNullException.ThrowIfNull(mat, nameof(mat));

            this.pixelFormat = pixelFormat;
            this.mat = mat;
            this.Width = mat.Width;
            this.Height = mat.Height;
            this.Size = new System.Drawing.Size(this.Width, this.Height);
            this.IsEmpty = false;
        }

        public CvImage(System.Drawing.Size size)
        {
            this.pixelFormat = PixelFormat.DontCare;
            this.mat = null;
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
                this.mat?.Dispose();
                this.mat = null;
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

            if (this.mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            g.CompositingMode = CompositingMode.SourceOver;

            lock (this.lockObject)
            {
                using (var bmp = this.mat.ToBitmap(this.pixelFormat))
                {
                    g.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
                }
            }
        }

        public void DrawResizeImage(Graphics g, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this.mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            g.CompositingMode = CompositingMode.SourceOver;

            lock (this.lockObject)
            {
                using (var bmp = OpenCVUtil.Resize(this.mat, (int)destRect.Width, (int)destRect.Height))
                {
                    g.DrawImage(bmp, destRect,
                        new RectangleF(0, 0, destRect.Width, destRect.Height), GraphicsUnit.Pixel);
                }
            }
        }
    }
}

