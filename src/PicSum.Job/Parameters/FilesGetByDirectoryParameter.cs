using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    public struct FilesGetByDirectoryParameter
        : IJobParameter
    {
        public string DirectoryPath { get; set; }
        public bool IsGetThumbnail { get; set; }
    }
}
