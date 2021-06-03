using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// 次のコンテンツのパラメータを取得するエンティティ
    /// </summary>
    public class GetNextContentsParameterEntity<T> : IEntity
    {
        public SingleValueEntity<T> CurrentParameter { get; set; }
        public bool IsNext { get; set; }
    }
}
