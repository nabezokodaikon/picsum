using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTaskV2;
using System.Collections.Generic;

namespace PicSum.Task.Paramters
{
    /// <summary>
    /// 画像読込パラメータエンティティ
    /// </summary>
    public sealed class ImageFileReadParameter
        : ITaskParameter
    {
        public int CurrentIndex { get; set; }
        public IList<string> FilePathList { get; set; }
        public ImageDisplayMode ImageDisplayMode { get; set; }
        public ImageSizeMode ImageSizeMode { get; set; }
        public int ThumbnailSize { get; set; }
    }
}
