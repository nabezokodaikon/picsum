﻿using PicSum.Core.Task.Base;
using PicSum.Task.Entity;
using System.Collections.Generic;

namespace PicSum.Task.Result
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>
    public sealed class GetFileDeepInfoResult
        : IEntity
    {
        public IList<string> FilePathList;
        public FileDeepInfoEntity FileInfo;
        public ListEntity<FileTagInfoEntity> TagInfoList;
    }
}
