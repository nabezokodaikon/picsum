using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entity;
using System.Collections.Generic;

namespace PicSum.Task.Result
{
    public sealed class GetAddressInfoResult
        : ITaskResult
    {
        public string DirectoryPath { get; set; }
        public IList<FileShallowInfoEntity> DirectoryList { get; set; }
        public bool HasSubDirectory { get; set; }
    }
}
