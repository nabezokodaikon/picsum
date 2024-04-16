using System;
using System.Drawing;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルの浅い情報エンティティ
    /// </summary>
    public sealed class FileShallowInfoEntity
    {
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public Nullable<DateTime> RgistrationDate { get; set; }
        public Image? LargeIcon { get; set; }
        public Image? SmallIcon { get; set; }
        public bool IsFile { get; set; }
        public bool IsImageFile { get; set; }
    }
}
