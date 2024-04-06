using PicSum.Core.Task.AsyncTaskV2;
using System.Collections.Generic;

namespace PicSum.Task.Result
{
    public sealed class GetImageFileByDirectoryResult
        : ITaskResult
    {
        public string DirectoryPath { get; set; }
        public IList<string> FilePathList { get; set; }
        public string SelectedFilePath { get; set; }
    }
}
