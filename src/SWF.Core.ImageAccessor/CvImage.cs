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
        private PixelFormat pixelFormat;
        private Mat? mat;

        public readonly System.Drawing.Size Size;
        public readonly int Width;
        public readonly int Height;
        public readonly bool IsEmpty;

        public CvImage(ImageFileBuffer buffer)
        {
            ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));

            if (buffer.Buffer == null)
            {
                throw new NullReferenceException("バッファがNullです。");
            }

            this.pixelFormat = buffer.PixelFormat;

            var matType = this.pixelFormat switch
            {
                PixelFormat.Format8bppIndexed => MatType.CV_8UC1,
                PixelFormat.Format16bppGrayScale => MatType.CV_16UC1,
                PixelFormat.Format24bppRgb => MatType.CV_8UC3,
                PixelFormat.Format32bppRgb => MatType.CV_8UC4,
                PixelFormat.Format32bppArgb => MatType.CV_8UC4,
                PixelFormat.Format32bppPArgb => MatType.CV_8UC4,
                PixelFormat.Format48bppRgb => MatType.CV_16UC3,
                PixelFormat.Format64bppArgb => MatType.CV_16UC4,
                PixelFormat.Format1bppIndexed => MatType.CV_8UC1,
                _ => throw new NotImplementedException($"対応していないピクセルフォーマットです。{this.pixelFormat}"),
            };

            this.mat = Mat.FromPixelData(buffer.Height, buffer.Width, matType, buffer.Buffer, buffer.Stride);
            this.Width = buffer.Width;
            this.Height = buffer.Height;
            this.Size = buffer.Size;
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
                throw new NullReferenceException("BitmapがNullです。");
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
                throw new NullReferenceException("バッファがNullです。");
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

