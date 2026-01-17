using SWF.Core.Base;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public sealed class IconImage
        : IDisposable
    {
        private bool _disposed = false;
        private readonly Bitmap _icon;
        private Bitmap? _cache = null!;

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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._cache?.Dispose();
                this._cache = null;
            }

            this._disposed = true;
        }

        public void DoCache(int width, int height)
        {
            if (this._cache == null
                || this._cache.Width != width
                || this._cache.Height != height)
            {
                this._cache?.Dispose();
                this._cache = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                using (var g = Graphics.FromImage(this._cache))
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.CompositingQuality = CompositingQuality.HighSpeed;
                    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    g.CompositingMode = CompositingMode.SourceOver;

                    g.DrawImage(
                        this._icon,
                        new Rectangle(0, 0, this._cache.Width, this._cache.Height),
                        new Rectangle(0, 0, this._icon.Width, this._icon.Height),
                        GraphicsUnit.Pixel);
                }
            }
        }

        public void Draw(Graphics g, Rectangle destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._cache == null
                || this._cache.Width != destRect.Width
                || this._cache.Height != destRect.Height)
            {
                return;
            }

            g.DrawImageUnscaled(this._cache, destRect);
        }
    }
}
