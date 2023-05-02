using PicSum.Core.Task.Base;
using PicSum.Task.Entity;
using SWF.Common;

namespace PicSum.Task.Result
{
    /// <summary>
    /// 画像読込結果エンティティ
    /// </summary>
    public sealed class GetImageFileResult
        : IEntity
    {
        public ImageFileEntity Image1 { get; set; }
        public ImageFileEntity Image2 { get; set; }
        public ImageUtilException ReadImageFileException { get; set; }
    }
}
