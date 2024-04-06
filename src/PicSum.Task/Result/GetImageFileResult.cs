using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entity;

namespace PicSum.Task.Result
{
    /// <summary>
    /// 画像読込結果エンティティ
    /// </summary>
    public sealed class GetImageFileResult
        : AbstractTaskResult
    {
        public ImageFileEntity Image1 { get; set; }
        public ImageFileEntity Image2 { get; set; }
    }
}
