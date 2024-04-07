using System;

namespace PicSum.Task.Entities
{
    /// <summary>
    /// サムネイルバッファエンティティ
    /// </summary>
    public sealed class ThumbnailBufferEntity
    {
        public string FilePath { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int SourceWidth { get; set; }
        public int SourceHeight { get; set; }
        public DateTime FileUpdatedate { get; set; }
        public byte[] ThumbnailBuffer { get; set; }
    }
}
