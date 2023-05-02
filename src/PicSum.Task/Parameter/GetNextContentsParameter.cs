using PicSum.Core.Task.Base;
using PicSum.Task.Entity;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// 次のコンテンツのパラメータを取得するエンティティ
    /// </summary>
    public sealed class GetNextContentsParameter<T>
        : IEntity
    {
        public SingleValueEntity<T> CurrentParameter { get; set; }
        public bool IsNext { get; set; }
    }
}
