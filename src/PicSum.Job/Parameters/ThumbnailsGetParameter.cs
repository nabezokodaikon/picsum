using PicSum.Core.Job.AsyncJob;
using System.Collections.Generic;

namespace PicSum.Job.Paramters
{
    public sealed class ThumbnailsGetParameter
        : IJobParameter
    {
        public IList<string>? FilePathList { get; set; }
        public int FirstIndex { get; set; }
        public int LastIndex { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
    }
}
