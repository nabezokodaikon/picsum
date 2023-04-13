using System.Collections.Generic;
using PicSum.Core.Task.Base;
using System.IO;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// フォルダ内検索結果
    /// </summary>
    public class SearchDirectoryResultEntity : IEntity
    {
        public string DirectoryPath { get; set; }
        public DirectoryStateEntity DirectoryState { get; set; }
        public IList<FileShallowInfoEntity> FileInfoList { get; set; }
        public DirectoryNotFoundException DirectoryNotFoundException { get; set; }
    }
}
