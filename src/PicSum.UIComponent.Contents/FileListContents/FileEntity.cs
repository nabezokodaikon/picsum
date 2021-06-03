using System;
using System.Drawing;
using PicSum.Task.Entity;

namespace PicSum.UIComponent.Contents.FileListContents
{
    internal class FileEntity
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public bool IsFile { get; set; }
        public bool IsImageFile { get; set; }
        public Image Icon { get; set; }
        public Image ThumbnailImage { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int SourceImageWidth { get; set; }
        public int SourceImageHeight { get; set; }
    }
}
