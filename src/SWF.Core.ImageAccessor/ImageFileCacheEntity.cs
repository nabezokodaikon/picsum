using System.Buffers;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class ImageFileCacheEntity
        : IDisposable, IEquatable<ImageFileCacheEntity>
    {
        private bool disposed = false;

        private readonly byte[] buffer;
        private readonly int bufferSize;

        public string FilePath { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Size Size { get; private set; }
        public PixelFormat PixelFormat { get; private set; }

        public ImageFileCacheEntity(string filePath, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var bmp = ImageUtil.ReadImageFile(filePath))
            {
                this.FilePath = filePath;
                this.Timestamp = timestamp;
                this.Size = bmp.Size;
                this.PixelFormat = bmp.PixelFormat;
                (this.buffer, this.bufferSize) = ImageUtil.BitmapToByteArray(bmp);
            }
        }

        ~ImageFileCacheEntity()
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
                ArrayPool<byte>.Shared.Return(this.buffer);
            }

            this.disposed = true;
        }

        public bool Equals(ImageFileCacheEntity? other)
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
            return this.Equals(obj as ImageFileCacheEntity);
        }

        public Bitmap CreateBitmap()
        {
            return ImageUtil.ByteArrayToBitmap(
                this.buffer, this.bufferSize, this.Size.Width, this.Size.Height, this.PixelFormat);
        }
    }
}
