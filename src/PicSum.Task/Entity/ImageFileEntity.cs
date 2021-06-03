using System.Drawing;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// 画像ファイルエンティティ
    /// </summary>
    public class ImageFileEntity : IEntity
    {
        public string FilePath { get; set; }
        public Image Image { get; set; }
        public Image Thumbnail { get; set; }
    }
}
