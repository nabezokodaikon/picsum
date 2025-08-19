using SWF.Core.Base;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// サムネイルバッファエンティティ
    /// </summary>

    public sealed class ThumbnailCacheEntity
    {
        public static readonly ThumbnailCacheEntity EMPTY = new()
        {
            FilePath = string.Empty,
            ThumbnailWidth = 0,
            ThumbnailHeight = 0,
            SourceWidth = 0,
            SourceHeight = 0,
            FileUpdateDate = DateTimeExtensions.EMPTY,
            ThumbnailBuffer = null,
        };

        public bool IsEmpry
        {
            get
            {
                return this == EMPTY;
            }
        }

        public string FilePath { get; internal set; } = string.Empty;
        public int ThumbnailWidth { get; internal set; } = 0;
        public int ThumbnailHeight { get; internal set; } = 0;
        public int SourceWidth { get; internal set; } = 0;
        public int SourceHeight { get; internal set; } = 0;
        public DateTime FileUpdateDate { get; internal set; } = DateTimeExtensions.EMPTY;
        public byte[]? ThumbnailBuffer { get; internal set; } = null;
    }
}
