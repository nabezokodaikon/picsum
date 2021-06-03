﻿using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// ファイルタグ情報エンティティ
    /// </summary>
    public class FileTagInfoEntity : IEntity
    {
        public string Tag { get; set; }
        public bool IsAll { get; set; }
    }
}
