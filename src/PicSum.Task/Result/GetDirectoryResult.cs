using PicSum.Core.Task.Base;
using PicSum.Task.Entity;
using System.Collections.Generic;
using System.IO;

namespace PicSum.Task.Result
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    public sealed class GetDirectoryResult
        : IEntity
    {
        public string DirectoryPath { get; set; }
        public DirectoryStateEntity DirectoryState { get; set; }
        public IList<FileShallowInfoEntity> FileInfoList { get; set; }
        public DirectoryNotFoundException DirectoryNotFoundException { get; set; }
    }
}
