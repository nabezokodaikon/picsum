using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;

namespace PicSum.Task.Results
{
    /// <summary>
    /// 画像読込結果エンティティ
    /// </summary>
    public sealed class GetImageFileResult
        : ITaskResult
    {
        public ImageFileEntity Image1 { get; set; }
        public ImageFileEntity Image2 { get; set; }
    }
}
