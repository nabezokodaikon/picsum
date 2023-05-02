using PicSum.Core.Task.Base;
using System.Collections.Generic;

namespace PicSum.Task.Paramter
{
    public sealed class GetThumbnailParameter
        : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public int FirstIndex { get; set; }
        public int LastIndex { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
    }
}
