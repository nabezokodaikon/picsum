using System.Drawing;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルの浅い情報エンティティ
    /// </summary>
    public sealed class FileShallowInfoEntity
    {
        public string? FilePath { get; internal set; }
        public string? FileName { get; internal set; }
        public DateTime? UpdateDate { get; internal set; }
        public DateTime? RgistrationDate { get; internal set; }
        public Image? LargeIcon { get; internal set; }
        public Image? SmallIcon { get; internal set; }
        public Bitmap? ThumbnailImage { get; internal set; }
        public int ThumbnailWidth { get; internal set; }
        public int ThumbnailHeight { get; internal set; }
        public int SourceWidth { get; internal set; }
        public int SourceHeight { get; internal set; }
        public bool IsFile { get; internal set; }
        public bool IsImageFile { get; internal set; }
    }
}
