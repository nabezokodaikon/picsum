using PicSum.Job.Results;
using System.Drawing;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルの深い情報エンティティ
    /// </summary>
    public sealed class FileDeepInfoEntity
    {
        public string? FilePath { get; internal set; }
        public string? FileName { get; internal set; }
        public DateTime UpdateDate { get; internal set; }
        public bool IsFile { get; internal set; }
        public bool IsImageFile { get; internal set; }
        public string? FileType { get; internal set; }
        public Nullable<long> FileSize { get; internal set; }
        public Nullable<Size> ImageSize { get; internal set; }
        public Image? FileIcon { get; internal set; }
        public int Rating { get; internal set; }
        public ThumbnailImageResult? Thumbnail { get; internal set; }
    }
}
