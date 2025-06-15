using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class ThumbnailsGetParameter
        : IJobParameter
    {
        public string[]? FilePathList { get; set; }
        public int FirstIndex { get; set; }
        public int LastIndex { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public bool IsExecuteCallback { get; set; }
    }
}
