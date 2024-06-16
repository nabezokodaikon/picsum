using System.Drawing;

namespace PicSum.Job.Entities
{
    public sealed class ImageCacheEntity
        : IDisposable
    {
        private bool disposed = false;

        public static readonly ImageCacheEntity EMPTY = new();
        public Bitmap? Image { get; private set; }
        public string? FilePath { get; private set; }
        public DateTime Timestamp { get; private set; }

        private ImageCacheEntity()
        {
            this.FilePath = null;
            this.Image = null;
            this.Timestamp = DateTime.MinValue;
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Image?.Dispose();
            }

            this.Image = null;

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageCacheEntity()
        {
            this.Dispose(false);
        }

        public ImageCacheEntity(string filePath, Bitmap image, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(image, nameof(image));

            this.FilePath = filePath;
            this.Image = image;
            this.Timestamp = timestamp;
        }

        public ImageCacheEntity Clone()
        {
            if (this.FilePath == null)
            {
                throw new NullReferenceException("ファイルパスが設定されていません。");
            }

            if (this.Image == null)
            {
                throw new NullReferenceException("イメージが設定されていません。");
            }

            var img = this.Image.Clone(
                new Rectangle(0, 0, this.Image.Width, this.Image.Height),
                this.Image.PixelFormat);

            return new ImageCacheEntity(this.FilePath, img, this.Timestamp);
        }
    }
}
