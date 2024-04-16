namespace PicSum.Job.Entities
{
    /// <summary>
    /// 単一値エンティティ
    /// </summary>
    public sealed class ValueEntity<T>(T value)
    {
        public T Value { get; private set; } = value;
    }
}
