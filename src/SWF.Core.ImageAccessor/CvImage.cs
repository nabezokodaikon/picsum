using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public struct CvImage
        : IDisposable, IEquatable<CvImage>
    {
        public static readonly CvImage EMPTY = new(System.Drawing.Size.Empty);

        private static string CreateErrorMessage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return $"描画に失敗しました。";
            }
            else
            {
                return $"'{filePath}'の描画に失敗しました。";
            }
        }

        private bool disposed = false;
        private readonly string filePath;
        private readonly PixelFormat pixelFormat;
        private Mat? mat;

        public readonly System.Drawing.Size Size;
        public readonly int Width;
        public readonly int Height;
        public readonly bool IsEmpty;

        public CvImage(string filePath, Mat mat, PixelFormat pixelFormat)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(mat, nameof(mat));

            this.filePath = filePath;
            this.pixelFormat = pixelFormat;
            this.mat = mat;
            this.Width = mat.Width;
            this.Height = mat.Height;
            this.Size = new System.Drawing.Size(this.Width, this.Height);
            this.IsEmpty = false;
        }

        public CvImage(string filePath, System.Drawing.Size size)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.filePath = filePath;
            this.pixelFormat = PixelFormat.DontCare;
            this.mat = null;
            this.Width = size.Width;
            this.Height = size.Height;
            this.Size = size;
            this.IsEmpty = true;
        }

        private CvImage(System.Drawing.Size size)
        {
            this.filePath = string.Empty;
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

        public readonly void DrawEmptyImage(Graphics g, Brush brush, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(brush, nameof(brush));

            g.CompositingMode = CompositingMode.SourceCopy;
            g.FillRectangle(brush, destRect);
        }

        public readonly Bitmap GetResizeImage(System.Drawing.Size size)
        {
            if (this.mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            try
            {
                return OpenCVUtil.Resize(this.mat, size.Width, size.Height);
            }
            catch (NotSupportedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (ArgumentException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (NotImplementedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
        }

        public readonly void DrawSourceImage(Graphics g, RectangleF destRect, RectangleF srcRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this.mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            g.CompositingMode = CompositingMode.SourceOver;

            try
            {
                using (var bmp = this.mat.ToBitmap(this.pixelFormat))
                {
                    g.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
                }
            }
            catch (NotSupportedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (ArgumentException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (NotImplementedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
        }

        public readonly void DrawResizeImage(Graphics g, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this.mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            g.CompositingMode = CompositingMode.SourceOver;

            try
            {
                using (var bmp = OpenCVUtil.Resize(this.mat, (int)destRect.Width, (int)destRect.Height))
                {
                    g.DrawImage(bmp, destRect,
                        new RectangleF(0, 0, destRect.Width, destRect.Height), GraphicsUnit.Pixel);
                }
            }
            catch (NotSupportedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (ArgumentException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
            catch (NotImplementedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this.filePath), ex);
            }
        }

        public readonly bool Equals(CvImage other)
        {
            if (this.filePath != other.filePath) { return false; }
            if (this.pixelFormat != other.pixelFormat) { return false; }
            if (this.mat != other.mat) { return false; }
            if (this.Width != other.Width) { return false; }
            if (this.Height != other.Height) { return false; }
            if (this.Size != other.Size) { return false; }
            if (this.IsEmpty != other.IsEmpty) { return false; }

            return true;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(CvImage))
            {
                return false;
            }

            return this.Equals((CvImage)obj);
        }
        public static bool operator ==(CvImage left, CvImage right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CvImage left, CvImage right)
        {
            return !(left == right);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(
                this.filePath,
                this.pixelFormat,
                this.mat,
                this.Width,
                this.Height,
                this.Size,
                this.IsEmpty);
        }
    }
}

