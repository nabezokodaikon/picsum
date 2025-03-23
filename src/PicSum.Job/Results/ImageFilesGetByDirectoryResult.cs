using SWF.Core.Job;

namespace PicSum.Job.Results
{
    public struct ImageFilesGetByDirectoryResult
        : IJobResult, IEquatable<ImageFilesGetByDirectoryResult>
    {
        public string? DirectoryPath { get; internal set; }
        public List<string>? FilePathList { get; internal set; }
        public string? SelectedFilePath { get; internal set; }

        public readonly bool Equals(ImageFilesGetByDirectoryResult other)
        {
            if (this.DirectoryPath != other.DirectoryPath) { return false; }
            if (this.FilePathList != other.FilePathList) { return false; }
            if (this.SelectedFilePath != other.SelectedFilePath) { return false; }

            return true;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(ImageFilesGetByDirectoryResult))
            {
                return false;
            }

            return this.Equals((ImageFilesGetByDirectoryResult)obj);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.DirectoryPath, this.FilePathList, this.SelectedFilePath);
        }

        public static bool operator ==(ImageFilesGetByDirectoryResult left, ImageFilesGetByDirectoryResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ImageFilesGetByDirectoryResult left, ImageFilesGetByDirectoryResult right)
        {
            return !(left == right);
        }
    }
}
