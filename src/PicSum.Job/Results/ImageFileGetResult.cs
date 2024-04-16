using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;

namespace PicSum.Job.Results
{
    /// <summary>
    /// 画像読込結果エンティティ
    /// </summary>
    public sealed class ImageFileGetResult
        : IJobResult
    {
        public ImageFileEntity? Image1 { get; set; }
        public ImageFileEntity? Image2 { get; set; }
    }
}
