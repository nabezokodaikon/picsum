using PicSum.Core.Task.AsyncTaskV2;
using System.Collections.Generic;

namespace PicSum.Task.Paramter
{
    public sealed class GetThumbnailParameter
        : ITaskParameter
    {
        public IList<string> FilePathList { get; set; }
        public int FirstIndex { get; set; }
        public int LastIndex { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
    }
}
