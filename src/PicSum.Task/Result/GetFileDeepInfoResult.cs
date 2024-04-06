using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entity;
using System.Collections.Generic;

namespace PicSum.Task.Result
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>
    public sealed class GetFileDeepInfoResult
        : ITaskResult
    {
        public IList<string> FilePathList;
        public FileDeepInfoEntity FileInfo;
        public ListEntity<FileTagInfoEntity> TagInfoList;
    }
}
