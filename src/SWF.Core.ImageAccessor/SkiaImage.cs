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
        private SKImage? _src = null;
        private readonly float _zoomValue;
        private readonly float _scaleValue;

        public SizeF Size { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public bool IsLoadingImage { get; private set; }
        public bool IsThumbnailImage { get; private set; }

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

        public SkiaImage(string filePath, SKImage src)
            : this(filePath, src, AppConstants.DEFAULT_ZOOM_VALUE)
        {

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
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void DrawEmpty(SKCanvas canvas, SKPaint paint, SKRect destRect)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            canvas.DrawRect(destRect, paint);
        }

        public void DrawZoomThumbnail(
            SKCanvas canvas, SKPaint paint, SKRect destRect, SKRect srcRect)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            using (Measuring.Time(false, "SkiaImage.DrawZoomThumbnail"))
            {
                var zoomRect = this.GetZoomRectange(srcRect);
                canvas.DrawImage(this._src, zoomRect, destRect, paint);
            }
        }

        public void DrawResizeThumbnail(
            SKCanvas canvas, SKPaint paint, SKRect destRect)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            using (Measuring.Time(false, "SkiaImage.DrawResizeThumbnail"))
            {
                var srcRect = SKRect.Create(0, 0, destRect.Width, destRect.Height);
                canvas.DrawImage(this._src, destRect, paint);
            }
        }

        public SKImage GetScaleImage(float scale)
        {
            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            using (Measuring.Time(false, "SkiaImage.CreateScaleImage"))
            {
                var width = this.Width * scale;
                var height = this.Height * scale;
                return SkiaUtil.Resize(this._src, (int)width, (int)height);
            }
        }

        public void DrawZoomImage(
            SKCanvas canvas,
            SKPaint paint,
            SKRect destRect,
            SKRect srcRect)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            using (Measuring.Time(false, "SkiaImage.DrawZoomImage"))
            {
                var zoomRect = this.GetZoomRectange(srcRect);

                var sampling = this._src.Width > destRect.Width || this._src.Height > destRect.Height
                    ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                    : new SKSamplingOptions(SKCubicResampler.CatmullRom);

                canvas.DrawImage(
                    this._src,
                    zoomRect,
                    destRect,
                    sampling,
                    paint);
            }
        }

        public void DrawResizeImage(
            SKCanvas canvas,
            SKPaint paint,
            SKRect destRect)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            using (Measuring.Time(false, "SkiaImage.DrawResizeImage"))
            {
                var sampling = this._src.Width > destRect.Width
                    ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                    : new SKSamplingOptions(SKCubicResampler.CatmullRom);

                canvas.DrawImage(
                    this._src,
                    destRect,
                    sampling,
                    paint);
            }
        }

        private SKRect GetZoomRectange(SKRect srcRect)
        {
            var x = srcRect.Left * this._scaleValue / this._zoomValue;
            var y = srcRect.Top * this._scaleValue / this._zoomValue;
            var w = srcRect.Width * this._scaleValue / this._zoomValue;
            var h = srcRect.Height * this._scaleValue / this._zoomValue;
            return SKRect.Create(x, y, w, h);
        }
    }
}

