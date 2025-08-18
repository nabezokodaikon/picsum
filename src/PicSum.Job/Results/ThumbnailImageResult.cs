using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    /// <summary>
    /// サムネイルイメージエンティティ
    /// </summary>

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
            FileUpdateDate = DateTimeExtensions.EMPTY,
        };

        public string FilePath { get; internal set; } = string.Empty;
        public CvImage? ThumbnailImage { get; internal set; }
        public int ThumbnailWidth { get; internal set; }
        public int ThumbnailHeight { get; internal set; }
        public int SourceWidth { get; internal set; }
        public int SourceHeight { get; internal set; }
        public DateTime FileUpdateDate { get; internal set; }
    }
}
