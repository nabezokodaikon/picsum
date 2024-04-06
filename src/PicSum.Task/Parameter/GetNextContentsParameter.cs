using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entity;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// 次のコンテンツのパラメータを取得するエンティティ
    /// </summary>
    public sealed class GetNextContentsParameter<T>
        : ITaskParameter
    {
        public SingleValueEntity<T> CurrentParameter { get; set; }
        public bool IsNext { get; set; }
    }
}
