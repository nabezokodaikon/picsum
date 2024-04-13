using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using PicSum.Task.Parameters;
using System.Collections.Generic;

namespace PicSum.Task.Results
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    public sealed class DirectoryGetResult
        : ITaskResult
    {
        public string DirectoryPath { get; set; }
        public DirectoryStateParameter DirectoryState { get; set; }
        public IList<FileShallowInfoEntity> FileInfoList { get; set; }
    }
}
