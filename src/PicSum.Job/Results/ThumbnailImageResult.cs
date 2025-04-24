using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Results
{
    /// <summary>
    /// サムネイルイメージエンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailImageResult
        : IJobResult
    {
        public static readonly ThumbnailImageResult EMPTY = new()
        {
            FilePath = string.Empty,
            ThumbnailImage = null,
            ThumbnailWidth = 0,
            ThumbnailHeight = 0,
            SourceWidth = 0,
            SourceHeight = 0,
            FileUpdatedate = FileUtil.EMPTY_DATETIME,
        };

        public string FilePath { get; internal set; } = string.Empty;
        public CvImage? ThumbnailImage { get; internal set; }
        public int ThumbnailWidth { get; internal set; }
        public int ThumbnailHeight { get; internal set; }
        public int SourceWidth { get; internal set; }
        public int SourceHeight { get; internal set; }
        public DateTime FileUpdatedate { get; internal set; }
    }
}
