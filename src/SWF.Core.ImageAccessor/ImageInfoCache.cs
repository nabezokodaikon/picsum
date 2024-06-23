namespace SWF.Core.ImageAccessor
{
    public class ImageInfoCache
    {
        public string FilePath { get; private set; }
        public Size Size { get; private set; }
        public DateTime Timestamp { get; private set; }

        public ImageInfoCache(string filePath, Size size, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.Size = size;
            this.Timestamp = timestamp;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as ImageInfoCache;
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
    }
}
