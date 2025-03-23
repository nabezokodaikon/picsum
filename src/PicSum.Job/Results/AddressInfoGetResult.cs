using PicSum.Job.Entities;
using SWF.Core.Job;

namespace PicSum.Job.Results
{
    public struct AddressInfoGetResult
        : IJobResult, IEquatable<AddressInfoGetResult>
    {
        public string? DirectoryPath { get; internal set; }
        public List<FileShallowInfoEntity>? DirectoryList { get; internal set; }
        public bool HasSubDirectory { get; internal set; }

        public readonly bool Equals(AddressInfoGetResult other)
        {
            if (this.DirectoryPath != other.DirectoryPath) { return false; }
            if (this.DirectoryList != other.DirectoryList) { return false; }
            if (this.HasSubDirectory != other.HasSubDirectory) { return false; }

            return true;
        }
    }
}
