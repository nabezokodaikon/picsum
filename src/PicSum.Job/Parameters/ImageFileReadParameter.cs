using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// 画像読込パラメータエンティティ
    /// </summary>
    public sealed class ImageFileReadParameter
        : IJobParameter
    {
        public int CurrentIndex { get; set; }
        public IList<string>? FilePathList { get; set; }
        public ImageDisplayMode ImageDisplayMode { get; set; }
        public ImageSizeMode ImageSizeMode { get; set; }
        public int ThumbnailSize { get; set; }
    }
}
