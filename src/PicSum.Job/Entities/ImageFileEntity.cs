using System.Drawing;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// 画像ファイルエンティティ
    /// </summary>
    public sealed class ImageFileEntity
    {
        public string? FilePath { get; set; }
        public Bitmap? Image { get; set; }
        public Bitmap? Thumbnail { get; set; }
        public bool IsError { get; set; }
    }
}
