using PicSum.Core.Job.AsyncJob;
using System;

namespace PicSum.Job.Paramters
{
    public sealed class ImageFileGetByDirectoryParameter
        : IJobParameter
    {
        public string FilePath { get; private set; }

        public ImageFileGetByDirectoryParameter(string filePath)
        {
            this.FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }
}
