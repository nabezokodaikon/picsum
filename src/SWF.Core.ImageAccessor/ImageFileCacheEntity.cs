using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class ImageFileCacheEntity
        : IDisposable, IEquatable<ImageFileCacheEntity>
    {
        private bool disposed = false;

        public string FilePath { get; private set; }
        public Bitmap Bitmap { get; private set; }
        public DateTime Timestamp { get; private set; }

        public ImageFileCacheEntity(string filePath, Bitmap bitmap, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            this.FilePath = filePath;
            this.Bitmap = bitmap;
            this.Timestamp = timestamp;
        }

        ~ImageFileCacheEntity()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Bitmap.Dispose();
            }

            this.disposed = true;
        }

        public bool Equals(ImageFileCacheEntity? other)
        {
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

        public override int GetHashCode()
        {
            return HashCode.Combine(this.FilePath, this.Timestamp);
        }

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as ImageFileCacheEntity);
        }
    }
}
