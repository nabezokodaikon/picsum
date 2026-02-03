using SkiaSharp;
using SWF.Core.ImageAccessor;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// 画像ファイルエンティティ
    /// </summary>

    public sealed class ImageFileEntity
    {
        public static readonly ImageFileEntity EMPTY = new()
        {
            FilePath = string.Empty,
            Image = SkiaImage.EMPTY,
            Thumbnail = null,
            IsEmpty = false,
            IsError = false,
        };

        public string FilePath { get; internal set; } = string.Empty;
        public SkiaImage Image { get; internal set; } = SkiaImage.EMPTY;
        public SKImage? Thumbnail { get; internal set; } = null;
        public bool IsEmpty { get; internal set; }
        public bool IsError { get; internal set; }
    }
}
