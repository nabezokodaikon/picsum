using SWF.Core.ImageAccessor;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// 画像ファイルエンティティ
    /// </summary>
    public sealed class ImageFileEntity
    {
        public string? FilePath { get; internal set; }
        public CvImage? Image { get; internal set; }
        public bool IsEmpty { get; internal set; }
        public bool IsError { get; internal set; }
    }
}
