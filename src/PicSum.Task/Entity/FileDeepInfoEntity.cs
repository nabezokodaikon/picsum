using PicSum.Core.Task.Base;
using PicSum.Task.Result;
using System;
using System.Drawing;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// ファイルの深い情報エンティティ
    /// </summary>
    public sealed class FileDeepInfoEntity
        : IEntity
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public bool IsFile { get; set; }
        public bool IsImageFile { get; set; }
        public string FileType { get; set; }
        public Nullable<long> FileSize { get; set; }
        public Nullable<Size> ImageSize { get; set; }
        public Image FileIcon { get; set; }
        public int Rating { get; set; }
        public ThumbnailImageResult Thumbnail { get; set; }
    }
}
