using PicSum.Core.Task.AsyncTaskV2;
using System;

namespace PicSum.Task.Paramters
{
    public sealed class GetImageFileByDirectoryParameter
        : ITaskParameter
    {
        public string FilePath { get; private set; }

        public GetImageFileByDirectoryParameter(string filePath)
        {
            this.FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }
}
