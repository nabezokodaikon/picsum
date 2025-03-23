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
        };

        public string? FilePath { get; internal set; }
        public string? FileName { get; internal set; }
        public DateTime UpdateDate { get; internal set; }
        public bool IsFile { get; internal set; }
        public bool IsImageFile { get; internal set; }
        public string? FileType { get; internal set; }
        public long? FileSize { get; internal set; }
        public Size? ImageSize { get; internal set; }
        public Image? FileIcon { get; internal set; }
        public int Rating { get; internal set; }
        public ThumbnailImageResult Thumbnail { get; internal set; }
    }
}
