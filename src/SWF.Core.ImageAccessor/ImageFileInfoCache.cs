namespace SWF.Core.ImageAccessor
{
    public class ImageFileInfoCache
        : IEquatable<ImageFileInfoCache>
    {
        public readonly string FilePath;
        public readonly Size Size;
        public readonly DateTime Timestamp;

        public ImageFileInfoCache(string filePath, Size size, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.Size = size;
            this.Timestamp = timestamp;
        }

        public bool Equals(ImageFileInfoCache? other)
        {
            ArgumentNullException.ThrowIfNull(other, nameof(other));

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
            return (this.FilePath, this.Timestamp).GetHashCode();
        }
    }
}
