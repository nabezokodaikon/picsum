using SkiaSharp;

namespace PicSum.UIComponent.Contents.FileList
{
    public static class FileListPageResources
    {
        public static readonly SKPaint IMAGE_PAINT = new()
        {
            IsAntialias = false,
            BlendMode = SKBlendMode.SrcOver,
        };

        public static void Dispose()
        {
            IMAGE_PAINT.Dispose();
        }
    }
}
