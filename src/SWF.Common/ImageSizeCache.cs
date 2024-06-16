using System;
using System.Drawing;

namespace SWF.Common
{
    internal class ImageSizeCache
    {
        public string? FilePath { get; private set; }
        public Size Size { get; private set; }
        public DateTime Timestamp { get; private set; }

        public ImageSizeCache(string filePath, Size size, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.Size = size;
            this.Timestamp = timestamp;
        }
    }
}
