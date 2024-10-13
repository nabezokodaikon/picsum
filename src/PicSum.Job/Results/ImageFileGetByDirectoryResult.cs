using SWF.Core.Job;

namespace PicSum.Job.Results
{
    public sealed class ImageFileGetByDirectoryResult
        : IJobResult
    {
        public string? DirectoryPath { get; internal set; }
        public IList<string>? FilePathList { get; internal set; }
        public string? SelectedFilePath { get; internal set; }
    }
}
