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
    }
}
