using SkiaSharp;
using SWF.Core.Base;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public sealed class IconImage
    {
        private static Bitmap? _bmpCache = null;
        private static SKBitmap? _skCache = null;
        private static IconImage? _iconCache = null;

        private readonly Bitmap _icon;

        public int Width
        {
            get
            {
                return this._icon.Width;
            }
        }

        public int Height
        {
            get
            {
                return this._icon.Height;
            }
        }

        public IconImage(Bitmap icon)
        {
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            this._icon = icon;
        }

        public void Draw(Graphics graphics, Rectangle destRect)
        {
            ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));

            using (Measuring.Time(false, "IconImage.Draw"))
            {
                if (_bmpCache == null
                    || _iconCache == null
                    || _iconCache._icon != this._icon
                    || _bmpCache.Width != destRect.Width
                    || _bmpCache.Height != destRect.Height)
                {
                    _iconCache = this;
                    _bmpCache?.Dispose();
                    _bmpCache = new Bitmap(destRect.Width, destRect.Height, PixelFormat.Format32bppPArgb);
                    using (var innerGraphics = Graphics.FromImage(_bmpCache))
                    {
                        innerGraphics.SmoothingMode = SmoothingMode.None;
                        innerGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        innerGraphics.CompositingQuality = CompositingQuality.HighSpeed;
                        innerGraphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                        innerGraphics.CompositingMode = CompositingMode.SourceOver;

                        innerGraphics.DrawImage(
                            this._icon,
                            new Rectangle(0, 0, _bmpCache.Width, _bmpCache.Height),
                            new Rectangle(0, 0, this._icon.Width, this._icon.Height),
                            GraphicsUnit.Pixel);
                    }
                }

                graphics.DrawImageUnscaled(_bmpCache, destRect);
            }
        }

        public void Draw(
            SKCanvas canvas,
            SKPaint paint,
            SKRect destRect)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));

            using (Measuring.Time(false, "IconImage.Draw"))
            {
                if (_skCache == null
                    || _iconCache == null
                    || _iconCache._icon != this._icon
                    || _skCache.Width != destRect.Width
                    || _skCache.Height != destRect.Height)
                {
                    _iconCache = this;
                    _skCache?.Dispose();
                    using (var bmp = new Bitmap((int)destRect.Width, (int)destRect.Height, PixelFormat.Format32bppPArgb))
                    {
                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.SmoothingMode = SmoothingMode.None;
                            g.InterpolationMode = InterpolationMode.NearestNeighbor;
                            g.CompositingQuality = CompositingQuality.HighSpeed;
                            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                            g.CompositingMode = CompositingMode.SourceOver;

                            g.DrawImage(
                                this._icon,
                                new Rectangle(0, 0, bmp.Width, bmp.Height),
                                new Rectangle(0, 0, this._icon.Width, this._icon.Height),
                                GraphicsUnit.Pixel);
                        }

                        _skCache = SkiaUtil.ConvertToSKBitmap(bmp);
                    }
                }

                var x = destRect.Left + (destRect.Width - _skCache.Width) / 2f;
                var y = destRect.Top + (destRect.Height - _skCache.Height) / 2f;
                var r = x + _skCache.Width;
                var b = y + _skCache.Height;

                canvas.DrawBitmap(_skCache, new SKRect(x, y, r, b), paint);
            }
        }
    }
}
