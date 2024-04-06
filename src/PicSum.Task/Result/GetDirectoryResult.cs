using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entity;
using PicSum.Task.Parameter;
using System.Collections.Generic;

namespace PicSum.Task.Result
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    public sealed class GetDirectoryResult
        : ITaskResult
    {
        public string DirectoryPath { get; set; }
        public DirectoryStateParameter DirectoryState { get; set; }
        public IList<FileShallowInfoEntity> FileInfoList { get; set; }
    }
}
