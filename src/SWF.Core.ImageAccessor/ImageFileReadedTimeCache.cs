namespace SWF.Core.ImageAccessor
{
    internal struct ImageFileReadedTimeCache
        : IEquatable<ImageFileReadedTimeCache>
    {
        public readonly string FilePath;
        public readonly long ReadedMilliseconds;

        public ImageFileReadedTimeCache(string filePath, long readedMilliseconds)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.ReadedMilliseconds = readedMilliseconds;
        }

        public bool Equals(ImageFileReadedTimeCache other)
        {
            if (other.FilePath != this.FilePath)
            {
                return false;
            }

            if (other.ReadedMilliseconds != this.ReadedMilliseconds)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return (this.FilePath, this.ReadedMilliseconds).GetHashCode();
        }
    }
}
