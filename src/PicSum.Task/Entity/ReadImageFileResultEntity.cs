using PicSum.Core.Task.Base;
using SWF.Common;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// 画像読込結果エンティティ
    /// </summary>
    public class ReadImageFileResultEntity : IEntity
    {
        public ImageFileEntity Image1 { get; set; }
        public ImageFileEntity Image2 { get; set; }
        public ImageUtilException ReadImageFileException { get; set; }
    }
}
