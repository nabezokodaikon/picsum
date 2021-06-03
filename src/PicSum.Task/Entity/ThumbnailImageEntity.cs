using System;
using System.Drawing;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// サムネイルイメージエンティティ
    /// </summary>
    public class ThumbnailImageEntity : IEntity
    {
        public string FilePath { get; set; }
        public Image ThumbnailImage { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int SourceWidth { get; set; }
        public int SourceHeight { get; set; }
        public DateTime FileUpdatedate { get; set; }
    }
}
