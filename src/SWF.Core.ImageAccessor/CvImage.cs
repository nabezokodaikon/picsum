using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class CvImage
        : IDisposable
    {
        public static readonly CvImage EMPTY = new(SizeF.Empty);

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
        private OpenCvSharp.Mat? _mat;
        private readonly float _zoomValue;

        public readonly SizeF Size;
        public readonly float Width;
        public readonly float Height;
        public readonly bool IsEmpty;

        public CvImage(string filePath, OpenCvSharp.Mat mat)
            : this(filePath, mat, AppConstants.DEFAULT_ZOOM_VALUE)
        {

        }

        public CvImage(string filePath, OpenCvSharp.Mat mat, float zoomValue)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(mat, nameof(mat));

            this._filePath = filePath;
            this._mat = mat;
            this._zoomValue = zoomValue;
            this.Width = mat.Width * zoomValue;
            this.Height = mat.Height * zoomValue;
            this.Size = new SizeF(this.Width, this.Height);
            this.IsEmpty = false;
        }

        public CvImage(string filePath, SizeF size, float zoomValue)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this._filePath = filePath;
            this._mat = null;
            this._zoomValue = zoomValue;
            this.Width = size.Width * zoomValue;
            this.Height = size.Height * zoomValue;
            this.Size = new SizeF(this.Width, this.Height);
            this.IsEmpty = true;
        }

        private CvImage(SizeF size)
        {
            this._filePath = string.Empty;
            this._mat = null;
            this._zoomValue = AppConstants.DEFAULT_ZOOM_VALUE;
            this.Width = size.Width * AppConstants.DEFAULT_ZOOM_VALUE;
            this.Height = size.Height * AppConstants.DEFAULT_ZOOM_VALUE;
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

        public Bitmap CreateScaleImage(float scale)
        {
            if (this._mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            var width = this.Width * scale;
            var height = this.Height * scale;

            try
            {
                using (TimeMeasuring.Run(false, "CvImage.CreateScaleImage"))
                {
                    return OpenCVUtil.Resize(
                        this._mat, width, height, OpenCvSharp.InterpolationFlags.Area);
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

        public void DrawZoomImage(Graphics g, RectangleF destRect, RectangleF srcRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._mat == null)
            {
                throw new NullReferenceException("MatがNullです。");
            }

            g.CompositingMode = CompositingMode.SourceOver;

            try
            {
                using (TimeMeasuring.Run(false, "CvImage.DrawZoomImage"))
                {
                    var roi = new OpenCvSharp.Rect(
                        new OpenCvSharp.Point(srcRect.X / this._zoomValue, srcRect.Y / this._zoomValue),
                        new OpenCvSharp.Size(srcRect.Width / this._zoomValue, srcRect.Height / this._zoomValue));
                    using (var cropped = new OpenCvSharp.Mat(this._mat, roi))
                    using (var bmp = OpenCVUtil.Resize(
                        cropped,
                        destRect.Width,
                        destRect.Height,
                        OpenCvSharp.InterpolationFlags.Area))
                    {
                        g.DrawImage(bmp,
                            destRect,
                            new RectangleF(0, 0, destRect.Width, destRect.Height),
                            GraphicsUnit.Pixel);
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
                using (var bmp = OpenCVUtil.Resize(this._mat, destRect.Width, destRect.Height, flag))
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

