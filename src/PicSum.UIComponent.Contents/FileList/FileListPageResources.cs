using SkiaSharp;

namespace PicSum.UIComponent.Contents.FileList
{
    // TODO: 終了時に破棄する。
    public static class FileListPageResources
    {
        public static readonly SKPaint IMAGE_PAINT = new()
        {
            IsAntialias = false,
            BlendMode = SKBlendMode.SrcOver,
        };
    }
}
