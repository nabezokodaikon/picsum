using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

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
        private readonly float _scaleValue;

        public readonly SizeF Size;
        public readonly float Width;
        public readonly float Height;
        public readonly bool IsLoadingImage;
        public readonly bool IsThumbnailImage;

        public CvImage(string filePath, OpenCvSharp.Mat mat, SizeF size, float zoomValue)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(mat, nameof(mat));

            this._filePath = filePath;
            this._mat = mat;
            this._zoomValue = zoomValue;
            this.Width = size.Width * zoomValue;
            this.Height = size.Height * zoomValue;
            this.Size = new SizeF(this.Width, this.Height);
            this.IsLoadingImage = false;
            this.IsThumbnailImage = true;
            this._scaleValue = this._mat.Width / (this.Width / this._zoomValue);
        }

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
            this.IsLoadingImage = false;
            this.IsThumbnailImage = false;
            this._scaleValue = this._mat.Width / (this.Width / this._zoomValue);
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
            this.IsLoadingImage = true;
            this.IsThumbnailImage = false;
            this._scaleValue = 1f;
        }

        private CvImage(SizeF size)
        {
            this._filePath = string.Empty;
            this._mat = null;
            this._zoomValue = AppConstants.DEFAULT_ZOOM_VALUE;
            this.Width = size.Width * AppConstants.DEFAULT_ZOOM_VALUE;
            this.Height = size.Height * AppConstants.DEFAULT_ZOOM_VALUE;
            this.Size = size;
            this.IsLoadingImage = true;
            this.IsThumbnailImage = false;
            this._scaleValue = 1f;
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

            g.FillRectangle(brush, destRect);
        }

        public void DrawZoomThumbnailImage(
            Graphics g, RectangleF destRect, RectangleF srcRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            using (TimeMeasuring.Run(false, "CvImage.DrawZoomThumbnailImage"))
            {
                using (var bmp = OpenCVUtil.ToBitmap(this._mat))
                {
                    var zoomRect = this.GetZoomRectange(srcRect);
                    g.DrawImage(bmp, destRect, zoomRect, GraphicsUnit.Pixel);
                }
            }
        }

        public void DrawResizeThumbnailImage(
            Graphics g, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            using (TimeMeasuring.Run(false, "CvImage.DrawResizeThumbnailImage"))
            {
                using (var bmp = OpenCVUtil.ToBitmap(this._mat))
                {
                    var srcRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    g.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
                }
            }
        }

        public Bitmap CreateScaleImage(float scale)
        {
            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            var width = this.Width * scale;
            var height = this.Height * scale;

            try
            {
                using (TimeMeasuring.Run(false, "CvImage.CreateScaleImage"))
                {
                    return OpenCVUtil.Resize(this._mat, width, height);
                }
            }
            catch (Exception ex) when (
                ex is NotSupportedException ||
                ex is ArgumentNullException ||
                ex is ArgumentException ||
                ex is ObjectDisposedException ||
                ex is NotImplementedException ||
                ex is OpenCvSharp.OpenCVException)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
        }

        public void DrawZoomImage(
            Graphics g,
            RectangleF destRect,
            RectangleF srcRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            try
            {
                using (TimeMeasuring.Run(false, "CvImage.DrawZoomImage"))
                {
                    var zoomRect = this.GetZoomRectange(srcRect);
                    var point = new OpenCvSharp.Point(zoomRect.X, zoomRect.Y);
                    var size = new OpenCvSharp.Size(zoomRect.Width, zoomRect.Height);
                    var roi = new OpenCvSharp.Rect(point, size);

                    using (var cropped = new OpenCvSharp.Mat(this._mat, roi))
                    using (var bmp = OpenCVUtil.Resize(cropped, destRect.Width, destRect.Height))
                    {
                        g.DrawImage(bmp,
                            destRect,
                            new RectangleF(0, 0, destRect.Width, destRect.Height),
                            GraphicsUnit.Pixel);
                    }
                }
            }
            catch (Exception ex) when (
                ex is NotSupportedException ||
                ex is ArgumentNullException ||
                ex is ArgumentException ||
                ex is ObjectDisposedException ||
                ex is NotImplementedException ||
                ex is OpenCvSharp.OpenCVException)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
        }

        public void DrawResizeImage(Graphics g, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            try
            {
                var width = destRect.Width;
                var height = destRect.Height;

                using (TimeMeasuring.Run(false, "CvImage.DrawResizeImage"))
                {
                    using (var bmp = OpenCVUtil.Resize(this._mat, width, height))
                    {
                        g.DrawImage(bmp, destRect,
                            new RectangleF(0, 0, width, height), GraphicsUnit.Pixel);
                    }
                }
            }
            catch (Exception ex) when (
                ex is NotSupportedException ||
                ex is ArgumentNullException ||
                ex is ArgumentException ||
                ex is ObjectDisposedException ||
                ex is NotImplementedException ||
                ex is OpenCvSharp.OpenCVException)
            {
                throw new ImageUtilException(CreateErrorMessage(this._filePath), ex);
            }
        }

        private RectangleF GetZoomRectange(RectangleF srcRect)
        {
            return new RectangleF(
                srcRect.X * this._scaleValue / this._zoomValue,
                srcRect.Y * this._scaleValue / this._zoomValue,
                srcRect.Width * this._scaleValue / this._zoomValue,
                srcRect.Height * this._scaleValue / this._zoomValue);
        }
    }
}

