namespace PicSum.Job.Entities
{
    // TODO: コンストラクタで値を設定できるようにする。

    /// <summary>
    /// リストエンティティ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ListEntity<T>
        : List<T>
    {
        public ListEntity()
        {

        }

        public ListEntity(IEnumerable<T> collection)
            : base(collection)
        {

        }
    }
}
