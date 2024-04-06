﻿using PicSum.Core.Task.Base;

namespace PicSum.Task.Entities
{
    /// <summary>
    /// 単一値エンティティ
    /// </summary>
    public sealed class SingleValueEntity<T>
        : IEntity
    {
        // TODO: コンストラクタで値を設定できるようにする。
        public T Value { get; set; }
    }
}