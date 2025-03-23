using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// 画像読込パラメータエンティティ
    /// </summary>
    public struct ImageFileReadParameter
        : IJobParameter
    {
        public int CurrentIndex { get; set; }
        public string[]? FilePathList { get; set; }
        public ImageDisplayMode ImageDisplayMode { get; set; }
    }
}
