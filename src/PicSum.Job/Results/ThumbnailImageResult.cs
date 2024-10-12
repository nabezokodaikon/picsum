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
        public string? FilePath { get; set; }
        public Image? ThumbnailImage { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int SourceWidth { get; set; }
        public int SourceHeight { get; set; }
        public DateTime FileUpdatedate { get; set; }
    }
}
