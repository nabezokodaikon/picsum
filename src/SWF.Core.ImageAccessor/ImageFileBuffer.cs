using System.Drawing.Imaging;

namespace SWF.Core.ImageAccessor
{
    public sealed class ImageFileBuffer
    {
        public byte[] Buffer { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Size Size { get; private set; }
        public PixelFormat PixelFormat { get; private set; }

        public ImageFileBuffer(Bitmap bmp)
        {
            ArgumentNullException.ThrowIfNull(bmp, nameof(bmp));

            this.Buffer = ImageUtil.BitmapToBuffer(bmp);
            this.Width = bmp.Width;
            this.Height = bmp.Height;
            this.Size = bmp.Size;
            this.PixelFormat = bmp.PixelFormat;
        }

        public Bitmap ToBitmap()
        {
            return ImageUtil.BufferToBitmap(this.Buffer, this.Width, this.Height, this.PixelFormat);
        }
    }
}
