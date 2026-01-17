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

        public void Draw(Graphics g, Rectangle destRect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            using (Measuring.Time(false, "IconImage.Draw"))
            {
                if (this._cache == null
                    || this._cache.Width != destRect.Width
                    || this._cache.Height != destRect.Height)
                {
                    this._cache?.Dispose();
                    this._cache = new Bitmap(destRect.Width, destRect.Height, PixelFormat.Format32bppPArgb);
                    using (var gr = Graphics.FromImage(this._cache))
                    {
                        gr.SmoothingMode = SmoothingMode.None;
                        gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                        gr.CompositingQuality = CompositingQuality.HighSpeed;
                        gr.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                        gr.CompositingMode = CompositingMode.SourceOver;

                        gr.DrawImage(
                            this._icon,
                            new Rectangle(0, 0, this._cache.Width, this._cache.Height),
                            new Rectangle(0, 0, this._icon.Width, this._icon.Height),
                            GraphicsUnit.Pixel);
                    }
                }

                g.DrawImageUnscaled(this._cache, destRect);
            }
        }
    }
}
