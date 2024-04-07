using System.Drawing;

namespace PicSum.Task.Entities
{
    /// <summary>
    /// 画像ファイルエンティティ
    /// </summary>
    public sealed class ImageFileEntity
    {
        public string FilePath { get; set; }
        public Bitmap Image { get; set; }
        public Bitmap Thumbnail { get; set; }
    }
}
