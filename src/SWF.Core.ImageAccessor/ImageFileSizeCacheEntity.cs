namespace SWF.Core.ImageAccessor
{
    public readonly struct ImageFileSizeCacheEntity
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

        public readonly bool Equals(ImageFileSizeCacheEntity other)
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

            if (obj.GetType() != typeof(ImageFileSizeCacheEntity))
            {
                return false;
            }

            return this.Equals((ImageFileSizeCacheEntity)obj);
        }
        public static bool operator ==(ImageFileSizeCacheEntity left, ImageFileSizeCacheEntity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ImageFileSizeCacheEntity left, ImageFileSizeCacheEntity right)
        {
            return !(left == right);
        }
    }
}
