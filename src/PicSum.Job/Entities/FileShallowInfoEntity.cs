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
        public Nullable<DateTime> UpdateDate { get; internal set; }
        public Nullable<DateTime> RgistrationDate { get; internal set; }
        public Image? LargeIcon { get; internal set; }
        public Image? SmallIcon { get; internal set; }
        public Bitmap? ThumbnailImage { get; internal set; }
        public bool IsFile { get; internal set; }
        public bool IsImageFile { get; internal set; }
    }
}
