using SWF.Core.FileAccessor;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルの浅い情報エンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public struct FileShallowInfoEntity
        : IEquatable<FileShallowInfoEntity>
    {
        public static readonly FileShallowInfoEntity EMPTY = new()
        {
            FilePath = string.Empty,
            FileName = string.Empty,
            UpdateDate = FileUtil.EMPTY_DATETIME,
            RgistrationDate = FileUtil.EMPTY_DATETIME,
            ExtraLargeIcon = null,
            SmallIcon = null,
            JumboIcon = null,
            ThumbnailImage = null,
            ThumbnailWidth = 0,
            ThumbnailHeight = 0,
            SourceWidth = 0,
            SourceHeight = 0,
            IsFile = false,
            IsImageFile = false,
        };

        public string FilePath { get; internal set; }
        public string FileName { get; internal set; }
        public DateTime UpdateDate { get; internal set; }
        public DateTime RgistrationDate { get; internal set; }
        public Image? ExtraLargeIcon { get; internal set; }
        public Image? SmallIcon { get; internal set; }
        public Image? JumboIcon { get; internal set; }
        public Bitmap? ThumbnailImage { get; internal set; }
        public int ThumbnailWidth { get; internal set; }
        public int ThumbnailHeight { get; internal set; }
        public int SourceWidth { get; internal set; }
        public int SourceHeight { get; internal set; }
        public bool IsFile { get; internal set; }
        public bool IsImageFile { get; internal set; }

        public bool Equals(FileShallowInfoEntity other)
        {
            if (this.FilePath != other.FilePath) { return false; }
            if (this.FileName != other.FileName) { return false; }
            if (this.UpdateDate != other.UpdateDate) { return false; }
            if (this.RgistrationDate != other.RgistrationDate) { return false; }
            if (this.ExtraLargeIcon != other.ExtraLargeIcon) { return false; }
            if (this.SmallIcon != other.SmallIcon) { return false; }
            if (this.JumboIcon != other.JumboIcon) { return false; }
            if (this.ThumbnailImage != other.ThumbnailImage) { return false; }
            if (this.ThumbnailWidth != other.ThumbnailWidth) { return false; }
            if (this.ThumbnailHeight != other.ThumbnailHeight) { return false; }
            if (this.SourceWidth != other.SourceWidth) { return false; }
            if (this.SourceHeight != other.SourceHeight) { return false; }
            if (this.IsFile != other.IsFile) { return false; }
            if (this.IsImageFile != other.IsImageFile) { return false; }

            return true;
        }
    }
}
