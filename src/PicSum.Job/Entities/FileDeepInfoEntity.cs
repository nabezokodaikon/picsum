using PicSum.Job.Results;
using System.Drawing;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルの深い情報エンティティ
    /// </summary>
    public sealed class FileDeepInfoEntity
    {
        public string? FilePath { get; internal set; } = null;
        public string? FileName { get; internal set; } = null;
        public DateTime UpdateDate { get; internal set; }
        public bool IsFile { get; internal set; }
        public bool IsImageFile { get; internal set; }
        public string? FileType { get; internal set; } = null;
        public long? FileSize { get; internal set; }
        public Size? ImageSize { get; internal set; }
        public Image? FileIcon { get; internal set; } = null;
        public int Rating { get; internal set; }
        public ThumbnailImageResult? Thumbnail { get; internal set; } = null;
    }
}
