using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;

namespace PicSum.Task.Paramters
{
    /// <summary>
    /// 次のコンテンツのパラメータを取得するエンティティ
    /// </summary>
    public sealed class GetNextPageParameter<T>
        : ITaskParameter
    {
        public ValueEntity<T> CurrentParameter { get; set; }
        public bool IsNext { get; set; }
    }
}
