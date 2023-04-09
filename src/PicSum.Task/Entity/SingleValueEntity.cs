using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// 単一値エンティティ
    /// </summary>
    public class SingleValueEntity<T> : IEntity
    {
        // TODO: コンストラクタで値を設定できるようにする。
        public T Value { get; set; }
    }
}