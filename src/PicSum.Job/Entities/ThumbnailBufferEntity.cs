using PicSum.Data.DatabaseAccessor.Dto;
using System;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// サムネイルバッファエンティティ
    /// </summary>
    public sealed class ThumbnailBufferEntity
    {
        public static readonly ThumbnailBufferEntity EMPTY = new()
        {
            FilePath = string.Empty,
            ThumbnailWidth = 0,
            ThumbnailHeight = 0,
            SourceWidth = 0,
            SourceHeight = 0,
            FileUpdatedate = DateTime.MinValue,
            ThumbnailBuffer = []
        };

        public string? FilePath { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int SourceWidth { get; set; }
        public int SourceHeight { get; set; }
        public DateTime FileUpdatedate { get; set; }
        public byte[]? ThumbnailBuffer { get; set; }
    }
}
