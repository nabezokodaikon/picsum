using PicSum.Job.Results;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルの深い情報エンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public struct FileDeepInfoEntity
        : IEquatable<FileDeepInfoEntity>
    {
        public static readonly FileDeepInfoEntity EMPTY = new()
        {
            FilePath = string.Empty,
            FileName = string.Empty,
            UpdateDate = FileUtil.EMPTY_DATETIME,
            IsFile = false,
            IsImageFile = false,
            FileType = string.Empty,
            FileSize = 0,
            ImageSize = ImageUtil.EMPTY_SIZE,
            FileIcon = null,
            Rating = 0,
            Thumbnail = ThumbnailImageResult.EMPTY,
            IsError = false,
        };

        public static readonly FileDeepInfoEntity ERROR = new()
        {
            FilePath = string.Empty,
            FileName = string.Empty,
            UpdateDate = FileUtil.EMPTY_DATETIME,
            IsFile = false,
            IsImageFile = false,
            FileType = string.Empty,
            FileSize = 0,
            ImageSize = ImageUtil.EMPTY_SIZE,
            FileIcon = null,
            Rating = 0,
            Thumbnail = ThumbnailImageResult.EMPTY,
            IsError = true,
        };

        public string FilePath { get; internal set; }
        public string FileName { get; internal set; }
        public DateTime UpdateDate { get; internal set; }
        public bool IsFile { get; internal set; }
        public bool IsImageFile { get; internal set; }
        public string FileType { get; internal set; }
        public long FileSize { get; internal set; }
        public Size ImageSize { get; internal set; }
        public Image? FileIcon { get; internal set; }
        public int Rating { get; internal set; }
        public ThumbnailImageResult Thumbnail { get; internal set; }
        public bool IsError { get; private set; }

        public readonly bool Equals(FileDeepInfoEntity other)
        {
            if (this.FilePath != other.FilePath) { return false; }
            if (this.FileName != other.FileName) { return false; }
            if (this.UpdateDate != other.UpdateDate) { return false; }
            if (this.IsFile != other.IsFile) { return false; }
            if (this.IsImageFile != other.IsImageFile) { return false; }
            if (this.FileType != other.FileType) { return false; }
            if (this.FileSize != other.FileSize) { return false; }
            if (this.ImageSize != other.ImageSize) { return false; }
            if (this.FileIcon != other.FileIcon) { return false; }
            if (this.Rating != other.Rating) { return false; }
            if (!this.Thumbnail.Equals(other.Thumbnail)) { return false; }
            if (this.IsError != other.IsError) { return false; }

            return true;
        }
    }
}
