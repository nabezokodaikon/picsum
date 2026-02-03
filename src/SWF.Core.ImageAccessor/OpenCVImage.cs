using SkiaSharp;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    public sealed class OpenCVImage
        : IDisposable
    {
        public static readonly OpenCVImage EMPTY = new(SizeF.Empty);

        private static readonly SKSamplingOptions SAMPLING
            = new(SKFilterMode.Nearest, SKMipmapMode.None);

        private bool _disposed = false;
        private readonly string _filePath;
        private OpenCvSharp.Mat? _mat;
        private SKImage? _cache = null;
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

        public OpenCVImage(string filePath, OpenCvSharp.Mat mat)
            : this(filePath, mat, AppConstants.DEFAULT_ZOOM_VALUE)
        {

        }

        public OpenCVImage(string filePath, OpenCvSharp.Mat mat, float zoomValue)
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

        private OpenCVImage(SizeF size)
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
                this._cache?.Dispose();
                this._cache = null;
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void CacheResizeThumbnail(SKRectI destRect)
        {
            if (this._mat == null)
            {
                throw new InvalidOperationException("MatがNullです。");
            }

            using (Measuring.Time(false, "OpenCVImage.CacheResizeThumbnail"))
            {
                if (this._cache == null
                    || this._cache.Width != destRect.Width
                    || this._cache.Height != destRect.Height)
                {
                    this._cache?.Dispose();
                    using (var bmp = OpenCVUtil.Resize(this._mat, destRect.Width, destRect.Height))
                    {
                        this._cache = SkiaUtil.ToSKImage(bmp);
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

            using (Measuring.Time(false, "OpenCVImage.DrawResizeThumbnail"))
            {
                if (this._cache == null
                    || this._cache.Width != destRect.Width
                    || this._cache.Height != destRect.Height)
                {
                    return;
                }

                canvas.DrawImage(this._cache, destRect, SAMPLING, paint);
            }
        }
    }
}

