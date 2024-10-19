using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    internal sealed partial class ImageFileCache
        : IDisposable, IEquatable<ImageFileCache>
    {
        private bool disposed = false;

        public string FilePath { get; private set; }
        public ImageFileBuffer? Buffer { get; private set; }
        public DateTime Timestamp { get; private set; }

        public ImageFileCache(string filePath, ImageFileBuffer buffer, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));

            this.FilePath = filePath;
            this.Buffer = buffer;
            this.Timestamp = timestamp;
        }

        ~ImageFileCache()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Buffer?.Dispose();
                this.Buffer = null;
            }

            this.disposed = true;
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
