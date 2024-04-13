using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using System.Collections.Generic;

namespace PicSum.Task.Results
{
    public sealed class AddressInfoGetResult
        : ITaskResult
    {
        public string DirectoryPath { get; set; }
        public IList<FileShallowInfoEntity> DirectoryList { get; set; }
        public bool HasSubDirectory { get; set; }
    }
}
