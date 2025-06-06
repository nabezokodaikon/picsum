using OpenCvSharp.Extensions;
using SWF.Core.ConsoleAccessor;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class CvImage
        : IDisposable
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

        private bool _disposed = false;
        private readonly string _filePath;
        private readonly PixelFormat _pixelFormat;
        private OpenCvSharp.Mat? _mat;

        public readonly System.Drawing.Size Size;
        public readonly int Width;
        public readonly int Height;
        public readonly bool IsEmpty;

        public CvImage(string filePath, OpenCvSharp.Mat mat, PixelFormat pixelFormat)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(mat, nameof(mat));

            this._filePath = filePath;
            this._pixelFormat = pixelFormat;
            this._mat = mat;
            this.Width = mat.Width;
            this.Height = mat.Height;
            this.Size = new System.Drawing.Size(this.Width, this.Height);
            this.IsEmpty = false;
        }

        public CvImage(string filePath, System.Drawing.Size size)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this._filePath = filePath;
            this._pixelFormat = PixelFormat.DontCare;
            this._mat = null;
            this.Width = size.Width;
            this.Height = size.Height;
            this.Size = size;
            this.IsEmpty = true;
        }

        private CvImage(System.Drawing.Size size)
        {
            this._filePath = string.Empty;
            this._pixelFormat = PixelFormat.DontCare;
            this._mat = null;
            this.Width = size.Width;
            this.Height = size.Height;
            this.Size = size;
            this.IsEmpty = true;
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._mat?.Dispose();
                this._mat = null;
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void DrawEmptyImage(Graphics g, Brush brush, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(brush, nameof(brush));

            g.CompositingMode = CompositingMode.SourceCopy;
            g.FillRectangle(brush, destRect);
        }

        public Bitmap ToBitmap()
        {
            if (this._mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            return this._mat.ToBitmap();
        }

        public Bitmap GetHighQualityResizeImage(System.Drawing.Size size)
        {
            return this.GetResizeImage(size, OpenCvSharp.InterpolationFlags.Area);
        }

        public Bitmap GetLowQualityResizeImage(System.Drawing.Size size)
        {
            return this.GetResizeImage(size, OpenCvSharp.InterpolationFlags.Linear);
        }

        private Bitmap GetResizeImage(System.Drawing.Size size, OpenCvSharp.InterpolationFlags flag)
        {
            if (this._mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            try
            {
                return OpenCVUtil.Resize(this._mat, size.Width, size.Height, flag);
            }
            catch (NotSupportedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (ArgumentException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (NotImplementedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
        }

        public void DrawSourceImage(Graphics g, RectangleF destRect, RectangleF srcRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            g.CompositingMode = CompositingMode.SourceOver;

            try
            {
                using (TimeMeasuring.Run(false, "CvImage.DrawSourceImage ToBitmap"))
                {
                    var roi = new OpenCvSharp.Rect(
                        (int)srcRect.X,
                        (int)srcRect.Y,
                        (int)srcRect.Width,
                        (int)srcRect.Height);
                    using (var cropped = new OpenCvSharp.Mat(this._mat, roi))
                    using (var bmp = cropped.ToBitmap(this._pixelFormat))
                    {
                        g.DrawImage(bmp, destRect, new Rectangle(0, 0, cropped.Width, cropped.Height), GraphicsUnit.Pixel);
                    }
                }
            }
            catch (NotSupportedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (ArgumentException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (NotImplementedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
        }

        public void DrawHighQualityResizeImage(Graphics g, RectangleF destRect)
        {
            this.DrawResizeImage(g, destRect, OpenCvSharp.InterpolationFlags.Area);
        }

        public void DrawLowQualityResizeImage(Graphics g, RectangleF destRect)
        {
            this.DrawResizeImage(g, destRect, OpenCvSharp.InterpolationFlags.Linear);
        }

        public void DrawResizeImage(Graphics g, RectangleF destRect, OpenCvSharp.InterpolationFlags flag)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            g.CompositingMode = CompositingMode.SourceOver;

            try
            {
                using (var bmp = OpenCVUtil.Resize(this._mat, (int)destRect.Width, (int)destRect.Height, flag))
                {
                    g.DrawImage(bmp, destRect,
                        new RectangleF(0, 0, destRect.Width, destRect.Height), GraphicsUnit.Pixel);
                }
            }
            catch (NotSupportedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (ArgumentException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
            catch (NotImplementedException ex)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
        }
    }
}

