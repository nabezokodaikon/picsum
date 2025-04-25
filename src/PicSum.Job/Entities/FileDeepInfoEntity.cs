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
    public sealed class FileDeepInfoEntity
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

        public string FilePath { get; internal set; } = string.Empty;
        public string FileName { get; internal set; } = string.Empty;
        public DateTime UpdateDate { get; internal set; } = FileUtil.EMPTY_DATETIME;
        public bool IsFile { get; internal set; } = false;
        public bool IsImageFile { get; internal set; } = false;
        public string FileType { get; internal set; } = string.Empty;
        public long FileSize { get; internal set; } = 0;
        public Size ImageSize { get; internal set; } = ImageUtil.EMPTY_SIZE;
        public Image? FileIcon { get; internal set; } = null;
        public int Rating { get; internal set; } = 0;
        public ThumbnailImageResult Thumbnail { get; internal set; } = ThumbnailImageResult.EMPTY;
        public bool IsError { get; private set; } = false;
    }
}
