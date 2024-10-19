using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    internal sealed partial class ImageFileBuffer
        : IDisposable
    {
        private bool disposed = false;

        public byte[]? Buffer { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Size Size { get; private set; }
        public PixelFormat PixelFormat { get; private set; }

        public ImageFileBuffer(Bitmap bmp)
        {
            ArgumentNullException.ThrowIfNull(bmp, nameof(bmp));

            if (bmp.PixelFormat == PixelFormat.Format4bppIndexed)
            {
                this.Buffer = ImageUtil.BitmapToBufferFor4bpp(bmp);
            }
            else if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                this.Buffer = ImageUtil.BitmapToBufferFor8bpp(bmp);
            }
            else
            {
                this.Buffer = ImageUtil.BitmapToBuffer(bmp);
            }

            this.Width = bmp.Width;
            this.Height = bmp.Height;
            this.Size = bmp.Size;
            this.PixelFormat = bmp.PixelFormat;
        }

        ~ImageFileBuffer()
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
                this.Buffer = null;
            }

            this.disposed = true;
        }

        public Bitmap ToBitmap()
        {
            if (this.Buffer == null)
            {
                throw new NullReferenceException("バッファがNullです。");
            }

            if (this.PixelFormat == PixelFormat.Format4bppIndexed)
            {
                return ImageUtil.BufferToBitmapFor4bpp(this.Buffer, this.Width, this.Height);
            }
            else if (this.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                return ImageUtil.BufferToBitmapFor8bpp(this.Buffer, this.Width, this.Height);
            }
            else
            {
                return ImageUtil.BufferToBitmap(this.Buffer, this.Width, this.Height, this.PixelFormat);
            }
        }
    }
}
