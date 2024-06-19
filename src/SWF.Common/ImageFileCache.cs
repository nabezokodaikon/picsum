using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.Common
{
    [SupportedOSPlatform("windows")]
    internal sealed class ImageFileCache
        : IDisposable
    {
        private bool disposed = false;

        private readonly int width;
        private readonly int height;
        private readonly PixelFormat pixelFormat;

        public static readonly ImageFileCache EMPTY = new();
        public byte[] ImageBuffer { get; private set; }
        public string FilePath { get; private set; }
        public DateTime Timestamp { get; private set; }

        private ImageFileCache()
        {
            this.width = 0;
            this.height = 0;

            this.FilePath = null;
            this.ImageBuffer = null;
            this.Timestamp = DateTime.MinValue;
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                
            }

            this.ImageBuffer = null;

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageFileCache()
        {
            this.Dispose(false);
        }

        public ImageFileCache(string filePath, Bitmap image, DateTime timestamp)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentNullException.ThrowIfNull(image, nameof(image));

            this.width = image.Width;
            this.height = image.Height;
            this.pixelFormat = image.PixelFormat;

            this.FilePath = filePath;
            this.ImageBuffer = ImageUtil.ToBinary(image);
            this.Timestamp = timestamp;
        }

        public Bitmap Clone()
        {
            if (this.FilePath == null)
            {
                throw new NullReferenceException("ファイルパスが設定されていません。");
            }

            if (this.ImageBuffer == null)
            {
                throw new NullReferenceException("イメージが設定されていません。");
            }

            return (Bitmap)ImageUtil.ToImage(this.ImageBuffer, this.width, this.height, this.pixelFormat);
        }
    }
}
