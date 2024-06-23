using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public sealed class ImageFileCache
        : IDisposable
    {
        private bool disposed = false;

        public static readonly ImageFileCache EMPTY = new();
        public Bitmap Image { get; private set; }
        public string FilePath { get; private set; }
        public DateTime Timestamp { get; private set; }

        private ImageFileCache()
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

        ~ImageFileCache()
        {
            this.Dispose(false);
        }

        public ImageFileCache(string filePath, Bitmap image, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(image, nameof(image));

            this.FilePath = filePath;
            this.Image = image;
            this.Timestamp = timestamp;
        }

        public ImageFileCache Clone()
        {
            if (this.FilePath == null)
            {
                throw new NullReferenceException("ファイルパスが設定されていません。");
            }

            if (this.Image == null)
            {
                throw new NullReferenceException("イメージが設定されていません。");
            }

            var cloneImage = this.Image.Clone(
                new Rectangle(0, 0, this.Image.Width, this.Image.Height), this.Image.PixelFormat);
            return new ImageFileCache(this.FilePath, cloneImage, this.Timestamp);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as ImageFileCache;
            if (other == null)
            {
                return false;
            }

            if (other.FilePath != this.FilePath)
            {
                return false;
            }

            if (other.Timestamp != this.Timestamp)
            {
                return false;
            }

            return true;
        }
    }
}
