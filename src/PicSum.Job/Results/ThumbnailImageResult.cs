using SWF.Core.Job;
using System.Drawing;

namespace PicSum.Job.Results
{
    /// <summary>
    /// サムネイルイメージエンティティ
    /// </summary>
    public sealed class ThumbnailImageResult
        : IJobResult
    {
        public string? FilePath { get; internal set; }
        public Image? ThumbnailImage { get; internal set; }
        public int ThumbnailWidth { get; internal set; }
        public int ThumbnailHeight { get; internal set; }
        public int SourceWidth { get; internal set; }
        public int SourceHeight { get; internal set; }
        public DateTime FileUpdatedate { get; internal set; }
    }
}
