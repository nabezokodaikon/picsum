using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public sealed class FilesGetByDirectoryParameter
        : IJobParameter
    {
        public string DirectoryPath { get; set; } = string.Empty;
        public bool IsGetThumbnail { get; set; }
    }
}
