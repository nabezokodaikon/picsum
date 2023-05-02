using PicSum.Core.Task.Base;
using System;
using System.Drawing;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// ファイルの浅い情報エンティティ
    /// </summary>
    public sealed class FileShallowInfoEntity
        : IEntity
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<DateTime> RgistrationDate { get; set; }
        public Image LargeIcon { get; set; }
        public Image SmallIcon { get; set; }
        public bool IsFile { get; set; }
        public bool IsImageFile { get; set; }
    }
}
