using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Results
{
    /// <summary>
    /// サムネイルイメージエンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public struct ThumbnailImageResult
        : IJobResult, IEquatable<ThumbnailImageResult>
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

        public string FilePath { get; internal set; }
        public Image? ThumbnailImage { get; internal set; }
        public int ThumbnailWidth { get; internal set; }
        public int ThumbnailHeight { get; internal set; }
        public int SourceWidth { get; internal set; }
        public int SourceHeight { get; internal set; }
        public DateTime FileUpdatedate { get; internal set; }

        public readonly bool Equals(ThumbnailImageResult other)
        {
            if (this.FilePath != other.FilePath) { return false; }
            if (this.ThumbnailImage != other.ThumbnailImage) { return false; }
            if (this.ThumbnailWidth != other.ThumbnailWidth) { return false; }
            if (this.ThumbnailHeight != other.ThumbnailHeight) { return false; }
            if (this.SourceWidth != other.SourceWidth) { return false; }
            if (this.SourceHeight != other.SourceHeight) { return false; }
            if (this.FileUpdatedate != other.FileUpdatedate) { return false; }

            return true;
        }
    }
}
