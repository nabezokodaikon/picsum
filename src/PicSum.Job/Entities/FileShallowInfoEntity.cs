using SWF.Core.Base;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルの浅い情報エンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileShallowInfoEntity
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

        public string FilePath { get; internal set; } = string.Empty;
        public string FileName { get; internal set; } = string.Empty;
        public DateTime UpdateDate { get; internal set; } = FileUtil.EMPTY_DATETIME;
        public DateTime RgistrationDate { get; internal set; } = FileUtil.EMPTY_DATETIME;
        public Image? ExtraLargeIcon { get; internal set; } = null;
        public Image? SmallIcon { get; internal set; } = null;
        public Image? JumboIcon { get; internal set; } = null;
        public Bitmap? ThumbnailImage { get; internal set; } = null;
        public int ThumbnailWidth { get; internal set; } = 0;
        public int ThumbnailHeight { get; internal set; } = 0;
        public int SourceWidth { get; internal set; } = 0;
        public int SourceHeight { get; internal set; } = 0;
        public bool IsFile { get; internal set; } = false;
        public bool IsImageFile { get; internal set; } = false;
    }
}
