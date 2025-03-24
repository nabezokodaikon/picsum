using SWF.Core.Job;

namespace PicSum.Job.Results
{
    public sealed class ImageFilesGetByDirectoryResult
        : IJobResult
    {
        public string DirectoryPath { get; internal set; } = string.Empty;
        public List<string>? FilePathList { get; internal set; }
        public string SelectedFilePath { get; internal set; } = string.Empty;
    }
}
