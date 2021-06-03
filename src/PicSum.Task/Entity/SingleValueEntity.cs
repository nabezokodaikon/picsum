using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// 単一値エンティティ
    /// </summary>
    public class SingleValueEntity<T> : IEntity
    {
        public T Value { get; set; }
    }
}
