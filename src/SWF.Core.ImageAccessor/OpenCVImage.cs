using SkiaSharp;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{
    public sealed class OpenCVImage
        : IDisposable
    {
        public static readonly OpenCVImage EMPTY = new();

        private static readonly SKSamplingOptions SAMPLING
            = new(SKFilterMode.Nearest);

        private bool _disposed = false;
        private OpenCvSharp.Mat? _mat;
        private SKImage? _cache = null;

        public bool IsEmpry
        {
            get
            {
                return this == EMPTY;
            }
        }

        public OpenCVImage(OpenCvSharp.Mat mat)
        {
            ArgumentNullException.ThrowIfNull(mat, nameof(mat));

            this._mat = mat;
        }

        private OpenCVImage()
        {
            this._mat = null;
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

