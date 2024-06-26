using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Results
{
    public sealed class ImageFileGetByDirectoryResult
        : IJobResult
    {
        public string? DirectoryPath { get; set; }
        public IList<string>? FilePathList { get; set; }
        public string? SelectedFilePath { get; set; }
    }
}
