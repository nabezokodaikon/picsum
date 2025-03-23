using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal struct ImageFileCacheEntity
        : IDisposable, IEquatable<ImageFileCacheEntity>
    {
        public static readonly ImageFileCacheEntity EMPTY = new()
        {
            FilePath = string.Empty,
            Bitmap = null,
            Timestamp = FileUtil.EMPTY_DATETIME,
        };

        private bool disposed = false;

        public string FilePath { get; private set; }
        public Bitmap? Bitmap { get; private set; }
        public DateTime Timestamp { get; private set; }

        public ImageFileCacheEntity(string filePath, Bitmap bitmap, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            this.FilePath = filePath;
            this.Bitmap = bitmap;
            this.Timestamp = timestamp;
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
                this.Bitmap?.Dispose();
            }

            this.disposed = true;
        }

        public readonly bool Equals(ImageFileCacheEntity other)
        {
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

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.FilePath, this.Timestamp);
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(ImageFileCacheEntity))
            {
                return false;
            }

            return this.Equals((ImageFileCacheEntity)obj);
        }
        public static bool operator ==(ImageFileCacheEntity left, ImageFileCacheEntity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ImageFileCacheEntity left, ImageFileCacheEntity right)
        {
            return !(left == right);
        }
    }
}
