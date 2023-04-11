using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    // TODO: コンストラクタで値を設定できるようにする。

    /// <summary>
    /// リストエンティティ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListEntity<T> : List<T>, IEntity
    {
        public ListEntity() { }

        public ListEntity(IEnumerable<T> collection) : base(collection) { }
    }
}
