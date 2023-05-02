using PicSum.Core.Task.Base;
using System;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// サムネイルバッファエンティティ
    /// </summary>
    public sealed class ThumbnailBufferEntity
        : IEntity
    {
        public string FilePath { get; set; }
        public byte[] ThumbnailBuffer { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int SourceWidth { get; set; }
        public int SourceHeight { get; set; }
        public DateTime FileUpdatedate { get; set; }
    }
}
