using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    internal sealed class ImageFileCache
        : IEquatable<ImageFileCache>
    {
        private readonly byte[] buffer;
        private readonly PixelFormat pixelFormat;

        public string FilePath { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Size Size { get; private set; }

        public ImageFileCache(string filePath, Bitmap image, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(image, nameof(image));

            this.FilePath = filePath;
            this.Timestamp = timestamp;

            this.Size = image.Size;
            this.pixelFormat = image.PixelFormat;

            var sw = Stopwatch.StartNew();
            try
            {
                this.buffer = ImageUtil.ToBinary(image);
            }
            finally
            {
                sw.Stop();
                Console.WriteLine($"ToBinary: {sw.ElapsedMilliseconds}");
            }
        }

        public Bitmap ToImage()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return ImageUtil.ToImage(
                    this.buffer, this.Size.Width, this.Size.Height, this.pixelFormat);
            }
            finally
            {
                sw.Stop();
                Console.WriteLine($"ToImage: {sw.ElapsedMilliseconds}");
            }
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
            return (this.FilePath, this.Timestamp).GetHashCode();
        }
    }
}
