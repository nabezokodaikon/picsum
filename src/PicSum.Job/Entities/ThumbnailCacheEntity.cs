using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// サムネイルバッファエンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailCacheEntity
    {
        public static readonly ThumbnailCacheEntity EMPTY = new()
        {
            FilePath = string.Empty,
            ThumbnailWidth = 0,
            ThumbnailHeight = 0,
            SourceWidth = 0,
            SourceHeight = 0,
            FileUpdateDate = DateTime.MinValue,
            ThumbnailBuffer = null,
        };

        public string FilePath { get; internal set; } = string.Empty;
        public int ThumbnailWidth { get; internal set; } = 0;
        public int ThumbnailHeight { get; internal set; } = 0;
        public int SourceWidth { get; internal set; } = 0;
        public int SourceHeight { get; internal set; } = 0;
        public DateTime FileUpdateDate { get; internal set; } = FileUtil.EMPTY_DATETIME;
        public byte[]? ThumbnailBuffer { get; internal set; } = null;
    }
}
