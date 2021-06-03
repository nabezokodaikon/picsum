using System.Drawing;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// 画像サイズエンティティ
    /// </summary>
    public class ImageSizeEntity:IEntity
    {
        public string FilePath { get; set; }
        public Size ImageSize { get; set; }
    }
}
