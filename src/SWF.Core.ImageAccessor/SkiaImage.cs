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

        public void DrawEmpty(SKCanvas canvas, SKPaint paint, SKRect destRect)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            canvas.DrawRect(destRect, paint);
        }

        public void DrawZoomThumbnail(
            GRContext context, SKCanvas canvas, SKPaint paint, SKRect destRect, SKRect srcRect)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            using (Measuring.Time(true, "SkiaImage.DrawZoomThumbnail"))
            {
                var zoomRect = this.GetZoomRectange(srcRect);
                using var gpuImg = this._src.ToTextureImage(context, false);
                canvas.DrawImage(gpuImg, zoomRect, destRect, paint);
            }
        }

        public void DrawResizeThumbnail(
            GRContext context, SKCanvas canvas, SKPaint paint, SKRect destRect)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            using (Measuring.Time(true, "SkiaImage.DrawResizeThumbnail"))
            {
                var srcRect = new SKRect(0, 0, destRect.Width, destRect.Height);
                using var gpuImg = this._src.ToTextureImage(context, false);
                canvas.DrawImage(gpuImg, destRect, paint);
            }
        }

        public SKImage CreateScaleImage(float scale)
        {
            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            var width = this.Width * scale;
            var height = this.Height * scale;

            try
            {
                using (Measuring.Time(false, "SkiaImage.CreateScaleImage"))
                {
                    using var paint = new SKPaint();
                    return SkiaImageUtil.Resize(this._src, (int)width, (int)height);
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
            GRContext context,
            SKCanvas canvas,
            SKPaint paint,
            SKRect destRect,
            SKRect srcRect)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            try
            {
                using (Measuring.Time(true, "SkiaImage.DrawZoomImage"))
                {
                    var zoomRect = this.GetZoomRectange(srcRect);

                    var sampling = this._src.Width > destRect.Width || this._src.Height > destRect.Height
                        ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                        : new SKSamplingOptions(SKCubicResampler.CatmullRom);

                    using var gpuImg = this._src.ToTextureImage(context, false);

                    canvas.DrawImage(
                        gpuImg,
                        zoomRect,
                        destRect,
                        sampling,
                        paint);
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

        public void DrawResizeImage(
            GRContext context,
            SKCanvas canvas,
            SKPaint paint,
            SKRect destRect)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            if (this._src == null)
            {
                throw new InvalidOperationException("SKImageがNullです。");
            }

            try
            {
                using (Measuring.Time(true, "SkiaImage.DrawResizeImage"))
                {
                    var sampling = this._src.Width > destRect.Width
                        ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                        : new SKSamplingOptions(SKCubicResampler.CatmullRom);

                    using var gpuImg = this._src.ToTextureImage(context, false);

                    canvas.DrawImage(
                        gpuImg,
                        destRect,
                        sampling,
                        paint);
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

        private SKRect GetZoomRectange(SKRect srcRect)
        {
            var x = srcRect.Left * this._scaleValue / this._zoomValue;
            var y = srcRect.Top * this._scaleValue / this._zoomValue;
            var w = srcRect.Width * this._scaleValue / this._zoomValue;
            var h = srcRect.Height * this._scaleValue / this._zoomValue;
            return new SKRect(x, y, x + w, y + h);
        }
    }
}

