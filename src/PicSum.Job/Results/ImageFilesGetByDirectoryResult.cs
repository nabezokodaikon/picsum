using SWF.Core.Job;

namespace PicSum.Job.Results
{
    public struct ImageFilesGetByDirectoryResult
        : IJobResult, IEquatable<ImageFilesGetByDirectoryResult>
    {
        public string? DirectoryPath { get; internal set; }
        public List<string>? FilePathList { get; internal set; }
        public string? SelectedFilePath { get; internal set; }

        public bool Equals(ImageFilesGetByDirectoryResult other)
        {
            if (this.DirectoryPath != other.DirectoryPath) { return false; }
            if (this.FilePathList != other.FilePathList) { return false; }
            if (this.SelectedFilePath != other.SelectedFilePath) { return false; }

            return true;
        }
    }
}
