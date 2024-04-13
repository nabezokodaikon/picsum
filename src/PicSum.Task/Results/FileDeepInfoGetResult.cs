using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using System.Collections.Generic;

namespace PicSum.Task.Results
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>
    public sealed class FileDeepInfoGetResult
        : ITaskResult
    {
        public IList<string> FilePathList;
        public FileDeepInfoEntity FileInfo;
        public ListEntity<FileTagInfoEntity> TagInfoList;
    }
}
