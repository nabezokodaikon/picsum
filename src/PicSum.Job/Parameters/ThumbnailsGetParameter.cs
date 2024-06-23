using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Parameters
{
    public struct ThumbnailsGetParameter
        : IJobParameter
    {
        public IList<string>? FilePathList { get; set; }
        public int FirstIndex { get; set; }
        public int LastIndex { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
    }
}
