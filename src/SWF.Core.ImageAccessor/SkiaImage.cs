using SkiaSharp;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{
    public sealed class SkiaImage
        : IDisposable
    {
        public static readonly SkiaImage EMPTY = new(SizeF.Empty);

        private bool _disposed = false;
        private readonly string _filePath;
        private SKImage? _src;
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

        public SkiaImage(string filePath, SKImage src, SizeF size, float zoomValue)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            this._filePath = filePath;
            this._src = src;
            this._zoomValue = zoomValue;
            this.Width = size.Width * zoomValue;
            this.Height = size.Height * zoomValue;
            this.Size = new SizeF(this.Width, this.Height);
            this.IsLoadingImage = false;
            this.IsThumbnailImage = true;
            this._scaleValue = this._src.Width / (this.Width / this._zoomValue);
        }

        public SkiaImage(string filePath, SKImage src, float zoomValue)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(src, nameof(src));

            this._filePath = filePath;
            this._src = src;
            this._zoomValue = zoomValue;
            this.Width = src.Width * zoomValue;
            this.Height = src.Height * zoomValue;
            this.Size = new SizeF(this.Width, this.Height);
            this.IsLoadingImage = false;
            this.IsThumbnailImage = false;
            this._scaleValue = this._src.Width / (this.Width / this._zoomValue);
        }

        public SkiaImage(string filePath, SKImage src)
            : this(filePath, src, AppConstants.DEFAULT_ZOOM_VALUE)
        {

        }

        public SkiaImage(string filePath, SizeF size, float zoomValue)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this._filePath = filePath;
            this._src = null;
            this._zoomValue = zoomValue;
            this.Width = size.Width * zoomValue;
            this.Height = size.Height * zoomValue;
            this.Size = new SizeF(this.Width, this.Height);
            this.IsLoadingImage = true;
            this.IsThumbnailImage = false;
            this._scaleValue = 1f;
        }

        private SkiaImage(SizeF size)
        {
            this._filePath = string.Empty;
            this._src = null;
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
                this._src?.Dispose();
                this._src = null;
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

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            using (Measuring.Time(false, "SkiaImage.DrawZoomThumbnailImage"))
            {
                if (this._bitmapCache == null
                    || this._bitmapCache.Width != (int)destRect.Width
                    || this._bitmapCache.Height != (int)destRect.Height)
                {
                    this._bitmapCache?.Dispose();
                    this._bitmapCache = SkiaImageUtil.ToBitmap(this._src);
                }

                var zoomRect = this.GetZoomRectange(srcRect);
                g.DrawImage(this._bitmapCache, destRect, zoomRect, GraphicsUnit.Pixel);
            }
        }

        public void DrawResizeThumbnail(
            Graphics g, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullですがNullです。");
            }

            using (Measuring.Time(false, "SkiaImage.DrawResizeThumbnail"))
            {
                if (this._bitmapCache == null
                    || this._bitmapCache.Width != (int)destRect.Width
                    || this._bitmapCache.Height != (int)destRect.Height)
                {
                    this._bitmapCache?.Dispose();
                    this._bitmapCache = SkiaImageUtil.ToBitmap(this._src);
                }

                var srcRect = new Rectangle(0, 0, this._bitmapCache.Width, this._bitmapCache.Height);
                g.DrawImage(this._bitmapCache, destRect, srcRect, GraphicsUnit.Pixel);
            }
        }

        public void CacheResizeThumbnail(SKRectI destRect)
        {
            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullですがNullです。");
            }

            using (Measuring.Time(false, "SkiaImage.CacheResizeThumbnail"))
            {
                if (this._skCache == null
                    || this._skCache.Width != destRect.Width
                    || this._skCache.Height != destRect.Height)
                {
                    this._skCache?.Dispose();
                    using (var bmp = SkiaImageUtil.Resize(this._src, destRect.Width, destRect.Height))
                    {
                        this._skCache = SkiaImageUtil.ToSKImage(bmp);
                    }
                }
            }
        }

        public void DrawResizeThumbnail(
            SKCanvas canvas, SKPaint paint, SKRectI destRect)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullですがNullです。");
            }

            using (Measuring.Time(false, "SkiaImage.DrawResizeThumbnail"))
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
            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullですがNullです。");
            }

            var width = this.Width * scale;
            var height = this.Height * scale;

            try
            {
                using (Measuring.Time(false, "SkiaImage.CreateScaleImage"))
                {
                    return SkiaImageUtil.Resize(this._src, width, height);
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

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullですがNullです。");
            }

            try
            {
                using (Measuring.Time(false, "SkiaImage.DrawZoomImage"))
                {
                    var zoomRect = this.GetZoomRectange(srcRect);

                    var roi = new SKRectI(
                        (int)zoomRect.X,
                        (int)zoomRect.Y,
                        (int)zoomRect.Right,
                        (int)zoomRect.Bottom);

                    using (var resizedBmp = SkiaImageUtil.Resize(this._src, roi, destRect.Width, destRect.Height))
                    {
                        g.DrawImage(resizedBmp,
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
                ex is NotImplementedException)
            {
                throw new ImageUtilException($"ズームイメージの描画に失敗しました。", this._filePath, ex);
            }
        }

        public void DrawResizeImage(Graphics g, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            try
            {
                var width = (int)destRect.Width;
                var height = (int)destRect.Height;

                using (Measuring.Time(false, "SkiaImage.DrawResizeImage"))
                {
                    if (this._bitmapCache == null
                        || this._bitmapCache.Width != width
                        || this._bitmapCache.Height != height)
                    {
                        this._bitmapCache?.Dispose();
                        this._bitmapCache = SkiaImageUtil.Resize(this._src, width, height);
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

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            try
            {
                using (Measuring.Time(false, "SkiaImage.DrawResizeThumbnail"))
                {
                    if (this._bitmapCache == null
                        || this._bitmapCache.Width != destRect.Width
                        || this._bitmapCache.Height != destRect.Height)
                    {
                        this._bitmapCache?.Dispose();
                        this._bitmapCache = SkiaImageUtil.Resize(this._src, destRect.Width, destRect.Height);
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

