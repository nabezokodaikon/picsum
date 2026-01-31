using SkiaSharp;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    public sealed class CvImage
        : IDisposable
    {
        public static readonly CvImage EMPTY = new(SizeF.Empty);

        private static readonly SKSamplingOptions SAMPLING
            = new(SKFilterMode.Nearest, SKMipmapMode.None);

        private bool _disposed = false;
        private readonly string _filePath;
        private OpenCvSharp.Mat? _mat;
        private Bitmap? _gdiCache = null;
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
                this._gdiCache?.Dispose();
                this._gdiCache = null;
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

                canvas.DrawImage(this._skCache, destRect, SAMPLING, paint);
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
                    if (this._gdiCache == null
                        || this._gdiCache.Width != destRect.Width
                        || this._gdiCache.Height != destRect.Height)
                    {
                        this._gdiCache?.Dispose();
                        this._gdiCache = OpenCVUtil.Resize(this._mat, destRect.Width, destRect.Height);
                    }

                    g.DrawImageUnscaled(this._gdiCache, destRect);
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
    }
}

