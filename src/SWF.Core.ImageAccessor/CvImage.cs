using SkiaSharp;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    public sealed class CvImage
        : IDisposable
    {
        public static readonly CvImage EMPTY = new(SizeF.Empty);

        private bool _disposed = false;
        private readonly string _filePath;
        private OpenCvSharp.Mat? _mat;
        private Bitmap? _bitmapCache = null;
        private SKImage? _skCache = null;
        private readonly float _zoomValue;
        private readonly float _scaleValue;

        public readonly SizeF Size;
        public readonly float Width;
        public readonly float Height;
        public readonly bool IsLoadingImage;
        public readonly bool IsThumbnailImage;

        public bool IsEmpry
        {
            get
            {
                return this == EMPTY;
            }
        }

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
                this._bitmapCache?.Dispose();
                this._bitmapCache = null;
                this._skCache?.Dispose();
                this._skCache = null;
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

            using (Measuring.Time(false, "CvImage.DrawZoomThumbnailImage"))
            {
                if (this._bitmapCache == null
                    || this._bitmapCache.Width != (int)destRect.Width
                    || this._bitmapCache.Height != (int)destRect.Height)
                {
                    this._bitmapCache?.Dispose();
                    this._bitmapCache = OpenCVUtil.ToBitmap(this._mat);
                }

                var zoomRect = this.GetZoomRectange(srcRect);
                g.DrawImage(this._bitmapCache, destRect, zoomRect, GraphicsUnit.Pixel);
            }
        }

        public void DrawResizeThumbnail(
            Graphics g, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            using (Measuring.Time(false, "CvImage.DrawResizeThumbnailImage"))
            {
                if (this._bitmapCache == null
                    || this._bitmapCache.Width != (int)destRect.Width
                    || this._bitmapCache.Height != (int)destRect.Height)
                {
                    this._bitmapCache?.Dispose();
                    this._bitmapCache = OpenCVUtil.ToBitmap(this._mat);
                }

                var srcRect = new Rectangle(0, 0, this._bitmapCache.Width, this._bitmapCache.Height);
                g.DrawImage(this._bitmapCache, destRect, srcRect, GraphicsUnit.Pixel);
            }
        }

        public void CacheResizeThumbnail(SKRectI destRect)
        {
            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            using (Measuring.Time(false, "CvImage.DrawResizeThumbnailImage"))
            {
                if (this._skCache == null
                    || this._skCache.Width != destRect.Width
                    || this._skCache.Height != destRect.Height)
                {
                    this._skCache?.Dispose();
                    using (var bmp = OpenCVUtil.Resize(this._mat, destRect.Width, destRect.Height))
                    {
                        this._skCache = SkiaUtil.ToSKImage(bmp);
                    }
                }
            }
        }

        public void DrawResizeThumbnail(
            SKCanvas canvas, SKPaint paint, SKRectI destRect)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            using (Measuring.Time(false, "CvImage.DrawResizeThumbnailImage"))
            {
                if (this._skCache == null
                    || this._skCache.Width != destRect.Width
                    || this._skCache.Height != destRect.Height)
                {
                    return;
                }

                var x = destRect.Left + (destRect.Width - this._skCache.Width) / 2f;
                var y = destRect.Top + (destRect.Height - this._skCache.Height) / 2f;
                var r = x + this._skCache.Width;
                var b = y + this._skCache.Height;

                canvas.DrawImage(this._skCache, new SKRectI((int)x, (int)y, (int)r, (int)b), paint);
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
                using (Measuring.Time(false, "CvImage.CreateScaleImage"))
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
                throw new ImageUtilException($"スケールイメージの作成に失敗しました。", this._filePath, ex);
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
                using (Measuring.Time(false, "CvImage.DrawZoomImage"))
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
                throw new ImageUtilException($"ズームイメージの描画に失敗しました。", this._filePath, ex);
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
                var width = (int)destRect.Width;
                var height = (int)destRect.Height;

                using (Measuring.Time(false, "CvImage.DrawResizeImage"))
                {
                    if (this._bitmapCache == null
                        || this._bitmapCache.Width != width
                        || this._bitmapCache.Height != height)
                    {
                        this._bitmapCache?.Dispose();
                        this._bitmapCache = OpenCVUtil.Resize(this._mat, width, height);
                    }

                    g.DrawImageUnscaled(this._bitmapCache, (int)destRect.X, (int)destRect.Y);
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
                throw new ImageUtilException($"リサイズイメージの描画に失敗しました。", this._filePath, ex);
            }
        }

        public void DrawResizeThumbnail(Graphics g, Rectangle destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            try
            {
                using (Measuring.Time(false, "CvImage.DrawResizeImage"))
                {
                    if (this._bitmapCache == null
                        || this._bitmapCache.Width != destRect.Width
                        || this._bitmapCache.Height != destRect.Height)
                    {
                        this._bitmapCache?.Dispose();
                        this._bitmapCache = OpenCVUtil.Resize(this._mat, destRect.Width, destRect.Height);
                    }

                    g.DrawImageUnscaled(this._bitmapCache, destRect);
                }
            }
            catch (Exception ex) when (
                ex is NotSupportedException ||
                ex is ArgumentNullException ||
                ex is ArgumentException ||
                ex is ObjectDisposedException ||
                ex is NotImplementedException)
            {
                throw new ImageUtilException($"リサイズイメージの描画に失敗しました。", this._filePath, ex);
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

