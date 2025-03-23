using SWF.Core.Job;

namespace PicSum.Job.Results
{
    public struct ImageFilesGetByDirectoryResult
        : IJobResult
    {
        public string? DirectoryPath { get; internal set; }
        public List<string>? FilePathList { get; internal set; }
        public string? SelectedFilePath { get; internal set; }
    }
}
