using SWF.Core.FileAccessor;
using System.Buffers;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class ImageFileCacheEntity
        : IDisposable, IEquatable<ImageFileCacheEntity>
    {
        public static readonly ImageFileCacheEntity EMPTY = new();

        private const int MINIMUM_LENGTH = 1 * 1024 * 1024;

        private bool _disposed = false;
        private readonly int _hashCode;

        public string FilePath { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; }
        public Size Size { get; private set; }
        public byte[] Bytes { get; private set; }

        public ImageFileCacheEntity(
            string filePath, Bitmap bmp, DateTime timestamp, Size size)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(bmp, nameof(bmp));

            this.FilePath = filePath;
            this.Timestamp = timestamp;
            this.Size = size;
            this._hashCode = HashCode.Combine(this.FilePath, this.Timestamp);

            this.Bytes = OpenCVUtil.BitmapToBytes(bmp);
        }

        private ImageFileCacheEntity()
        {
            this.FilePath = string.Empty;
            this.Timestamp = FileUtil.EMPTY_DATETIME;
            this.Size = ImageUtil.EMPTY_SIZE;
            this.Bytes = [];
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
                if (this.Bytes.Length > 0)
                {
                    ArrayPool<byte>.Shared.Return(this.Bytes);
                }
            }

            this.FilePath = string.Empty;

            this._disposed = true;
        }

        public bool Equals(ImageFileCacheEntity? other)
        {
            if (other is null)
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
            return this._hashCode;
        }

        public override bool Equals(object? obj)
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
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ImageFileCacheEntity left, ImageFileCacheEntity right)
        {
            return !(left == right);
        }
    }
}
