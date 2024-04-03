using PicSum.Core.Base.Conf;
using PicSum.Core.Task.Base;
using System.Collections.Generic;
using System.Drawing;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// 画像読込パラメータエンティティ
    /// </summary>
    public sealed class GetImageFileParameter
        : IEntity
    {
        public int CurrentIndex { get; set; }
        public IList<string> FilePathList { get; set; }
        public ImageDisplayMode ImageDisplayMode { get; set; }
        public ImageSizeMode ImageSizeMode { get; set; }
        public int ThumbnailSize { get; set; }
    }
}
