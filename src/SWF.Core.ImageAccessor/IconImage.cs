using SkiaSharp;
using SWF.Core.Base;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public sealed class IconImage
    {
        private static readonly SKSamplingOptions SAMPLING
            = new(SKFilterMode.Nearest, SKMipmapMode.None);

        private static Bitmap? _bmpCache = null;
        private static SKImage? _skCache = null;
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

        public void Draw(
            SKCanvas canvas,
            SKPaint paint,
            SKRectI destRect)
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
                    using (var bmp = new Bitmap(
                        destRect.Width,
                        destRect.Height,
                        PixelFormat.Format32bppPArgb))
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

                        _skCache = SkiaImageUtil.ToSKImage(bmp);
                    }
                }

                canvas.DrawImage(_skCache, destRect, SAMPLING, paint);
            }
        }
    }
}
