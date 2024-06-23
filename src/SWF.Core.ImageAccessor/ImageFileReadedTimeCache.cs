namespace SWF.Core.ImageAccessor
{
    internal class ImageFileReadedTimeCache
    {
        public string FilePath { get; private set; }
        public long ReadedMilliseconds { get; private set; }

        public ImageFileReadedTimeCache(string filePath, long readedMilliseconds)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.ReadedMilliseconds = readedMilliseconds;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as ImageFileReadedTimeCache;
            if (other == null)
            {
                return false;
            }

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
