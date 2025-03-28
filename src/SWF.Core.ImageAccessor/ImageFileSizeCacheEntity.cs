namespace SWF.Core.ImageAccessor
{
    public sealed class ImageFileSizeCacheEntity
        : IEquatable<ImageFileSizeCacheEntity>
    {
        public readonly string FilePath;
        public readonly Size Size;
        public readonly DateTime Timestamp;

        public ImageFileSizeCacheEntity(string filePath, Size size, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.Size = size;
            this.Timestamp = timestamp;
        }

        public bool Equals(ImageFileSizeCacheEntity? other)
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
            return HashCode.Combine(this.FilePath, this.Timestamp);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(ImageFileSizeCacheEntity))
            {
                return false;
            }

            return this.Equals((ImageFileSizeCacheEntity)obj);
        }
        public static bool operator ==(ImageFileSizeCacheEntity left, ImageFileSizeCacheEntity right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ImageFileSizeCacheEntity left, ImageFileSizeCacheEntity right)
        {
            return !(left == right);
        }
    }
}
