using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    internal sealed class ImageFileCache
        : IEquatable<ImageFileCache>
    {
        public string FilePath { get; private set; }
        public ImageFileBuffer Buffer { get; private set; }
        public DateTime Timestamp { get; private set; }

        public ImageFileCache(string filePath, ImageFileBuffer buffer, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));

            this.FilePath = filePath;
            this.Buffer = buffer;
            this.Timestamp = timestamp;
        }

        public bool Equals(ImageFileCache? other)
        {
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

        public override int GetHashCode()
        {
            return HashCode.Combine(this.FilePath, this.Timestamp);
        }

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as ImageFileCache);
        }
    }
}
