using PicSum.Job.Results;
using System;
using System.Drawing;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルの深い情報エンティティ
    /// </summary>
    public sealed class FileDeepInfoEntity
    {
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsFile { get; set; }
        public bool IsImageFile { get; set; }
        public string? FileType { get; set; }
        public Nullable<long> FileSize { get; set; }
        public Nullable<Size> ImageSize { get; set; }
        public Image? FileIcon { get; set; }
        public int Rating { get; set; }
        public ThumbnailImageResult? Thumbnail { get; set; }
    }
}
