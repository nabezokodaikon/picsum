using System;
using System.Drawing;

namespace SWF.Common
{
    public class ImageInfoCache
    {
        public string FilePath { get; private set; }
        public Size Size { get; private set; }
        public bool IsAlpha { get; private set; }
        public DateTime Timestamp { get; private set; }

        public ImageInfoCache(string filePath, Size size, bool isAlpha, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.Size = size;
            this.IsAlpha = isAlpha;
            this.Timestamp = timestamp;
        }
    }
}
