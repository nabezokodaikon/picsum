namespace PicSum.Job.Entities
{
    /// <summary>
    /// サムネイルバッファエンティティ
    /// </summary>
    public struct ThumbnailCacheEntity
    {
        public static readonly ThumbnailCacheEntity EMPTY = new()
        {
            FilePath = string.Empty,
            ThumbnailWidth = 0,
            ThumbnailHeight = 0,
            SourceWidth = 0,
            SourceHeight = 0,
            FileUpdatedate = DateTime.MinValue,
            ThumbnailBuffer = []
        };

        public string? FilePath { get; internal set; }
        public int ThumbnailWidth { get; internal set; }
        public int ThumbnailHeight { get; internal set; }
        public int SourceWidth { get; internal set; }
        public int SourceHeight { get; internal set; }
        public DateTime FileUpdatedate { get; internal set; }
        public byte[]? ThumbnailBuffer { get; internal set; }
    }
}
