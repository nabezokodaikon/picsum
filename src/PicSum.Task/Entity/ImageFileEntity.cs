using PicSum.Core.Task.Base;
using System.Drawing;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// 画像ファイルエンティティ
    /// </summary>
    public sealed class ImageFileEntity
        : IEntity
    {
        public string FilePath { get; set; }
        public Bitmap Image { get; set; }
        public Bitmap Thumbnail { get; set; }
    }
}
