using PicSum.Core.Task.Base;
using System.Collections.Generic;

namespace PicSum.Task.Entity
{
    // TODO: コンストラクタで値を設定できるようにする。

    /// <summary>
    /// リストエンティティ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ListEntity<T> :
        List<T>, IEntity
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
