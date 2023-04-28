using PicSum.Core.Task.Base;
using System;

namespace PicSum.Task.Paramter
{
    public sealed class GetImageFileByDirectoryParameter
        : IEntity
    {
        public string FilePath { get; private set; }

        public GetImageFileByDirectoryParameter(string filePath)
        {
            this.FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }
}
