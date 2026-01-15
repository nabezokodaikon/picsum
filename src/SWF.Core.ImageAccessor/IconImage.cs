using SWF.Core.Base;

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

        public void Draw(Graphics g, RectangleF destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            using (Measuring.Time(false, "IconImage.Draw"))
            {
                if (this._cache == null
                    || this._cache.Width != (int)destRect.Width
                    || this._cache.Height != (int)destRect.Height)
                {
                    this._cache?.Dispose();
                    this._cache = new Bitmap((int)destRect.Width, (int)destRect.Height);
                    using (var gr = Graphics.FromImage(this._cache))
                    {
                        gr.DrawImage(
                            this._icon,
                            new RectangleF(0, 0, destRect.Width, destRect.Height),
                            new RectangleF(0, 0, this.Width, this.Height),
                            GraphicsUnit.Pixel);
                    }
                }

                g.DrawImage(this._cache, destRect);
            }
        }
    }
}
