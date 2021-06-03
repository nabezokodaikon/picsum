using System.Collections.Generic;
using System.Drawing;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// 画像読込パラメータエンティティ
    /// </summary>
    public class ReadImageFileParameterEntity : IEntity
    {
        public int CurrentIndex { get; set; }
        public IList<string> FilePathList { get; set; }
        public ImageDisplayMode ImageDisplayMode { get; set; }
        public ImageSizeMode ImageSizeMode { get; set; }
        public Size DrawSize { get; set; }
        public int ThumbnailSize { get; set; }
    }
}
