namespace SWF.Core.ImageAccessor
{
    public sealed class ImageFileSizeCacheEntity
        : IEquatable<ImageFileSizeCacheEntity>
    {
        private readonly int _hashCode;
        public readonly string FilePath;
        public readonly Size Size;
        public readonly DateTime UpdateDate;

        public ImageFileSizeCacheEntity(string filePath, Size size, DateTime updateDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this._hashCode = HashCode.Combine(this.FilePath, this.UpdateDate);
            this.FilePath = filePath;
            this.Size = size;
            this.UpdateDate = updateDate;
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
