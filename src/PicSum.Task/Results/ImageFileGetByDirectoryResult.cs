using PicSum.Core.Task.AsyncTaskV2;
using System.Collections.Generic;

namespace PicSum.Task.Results
{
    public sealed class ImageFileGetByDirectoryResult
        : ITaskResult
    {
        public string DirectoryPath { get; set; }
        public IList<string> FilePathList { get; set; }
        public string SelectedFilePath { get; set; }
    }
}
