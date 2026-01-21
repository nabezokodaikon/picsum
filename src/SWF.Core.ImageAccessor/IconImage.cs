using SWF.Core.Base;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public sealed class IconImage
    {
        private static Bitmap? _cache = null;
        private static IconImage? _iconImage = null;

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
                if (_cache == null
                    || _iconImage == null
                    || _iconImage._icon != this._icon
                    || _cache.Width != destRect.Width
                    || _cache.Height != destRect.Height)
                {
                    _iconImage = this;
                    _cache?.Dispose();
                    _cache = new Bitmap(destRect.Width, destRect.Height, PixelFormat.Format32bppPArgb);
                    using (var innerGraphics = Graphics.FromImage(_cache))
                    {
                        innerGraphics.SmoothingMode = SmoothingMode.None;
                        innerGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        innerGraphics.CompositingQuality = CompositingQuality.HighSpeed;
                        innerGraphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                        innerGraphics.CompositingMode = CompositingMode.SourceOver;

                        innerGraphics.DrawImage(
                            this._icon,
                            new Rectangle(0, 0, _cache.Width, _cache.Height),
                            new Rectangle(0, 0, this._icon.Width, this._icon.Height),
                            GraphicsUnit.Pixel);
                    }
                }

                graphics.DrawImageUnscaled(_cache, destRect);
            }
        }
    }
}
