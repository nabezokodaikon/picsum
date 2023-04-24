using System.Collections.Generic;
using PicSum.Core.Task.Base;
using System.IO;
using PicSum.Task.Entity;

namespace PicSum.Task.Result
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    public class GetDirectoryResult : IEntity
    {
        public string DirectoryPath { get; set; }
        public DirectoryStateEntity DirectoryState { get; set; }
        public IList<FileShallowInfoEntity> FileInfoList { get; set; }
        public DirectoryNotFoundException DirectoryNotFoundException { get; set; }
    }
}
