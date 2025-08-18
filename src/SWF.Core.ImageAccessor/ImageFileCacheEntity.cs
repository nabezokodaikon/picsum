using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    internal sealed class ImageFileCacheEntity
        : IDisposable, IEquatable<ImageFileCacheEntity>
    {
        public static readonly ImageFileCacheEntity EMPTY = new()
        {
            FilePath = string.Empty,
            Bitmap = null,
            UpdateDate = DateTimeExtensions.EMPTY,
        };

        private bool _disposed = false;
        private readonly int _hashCode;

        public string FilePath { get; private set; } = string.Empty;
        public Bitmap? Bitmap { get; private set; }
        public DateTime UpdateDate { get; private set; }

        public ImageFileCacheEntity(string filePath, Bitmap bitmap, DateTime updateDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            this.FilePath = filePath;
            this.Bitmap = bitmap;
            this.UpdateDate = updateDate;
            this._hashCode = HashCode.Combine(this.FilePath, this.UpdateDate);
        }

        private ImageFileCacheEntity()
        {

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
                this.Bitmap?.Dispose();
            }

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

            if (other.UpdateDate != this.UpdateDate)
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
