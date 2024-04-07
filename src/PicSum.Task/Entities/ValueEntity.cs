namespace PicSum.Task.Entities
{
    /// <summary>
    /// 単一値エンティティ
    /// </summary>
    public sealed class ValueEntity<T>
    {
        // TODO: コンストラクタで値を設定できるようにする。
        public T Value { get; set; }
    }
}
