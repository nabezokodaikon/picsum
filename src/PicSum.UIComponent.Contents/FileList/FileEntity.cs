using System;
using System.Drawing;

namespace PicSum.UIComponent.Contents.FileList
{
    internal sealed class FileEntity
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public Nullable<DateTime> RgistrationDate { get; set; }
        public bool IsFile { get; set; }
        public bool IsImageFile { get; set; }
        public Image SmallIcon { get; set; }
        public Image ExtraLargeIcon { get; set; }
        public Image JumboIcon { get; set; }
        public Image ThumbnailImage { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int SourceImageWidth { get; set; }
        public int SourceImageHeight { get; set; }
    }
}
