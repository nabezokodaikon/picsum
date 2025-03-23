using PicSum.Job.Entities;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Results
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public struct AddressInfoGetResult
        : IJobResult, IEquatable<AddressInfoGetResult>
    {
        public string DirectoryPath { get; internal set; }
        public List<FileShallowInfoEntity>? DirectoryList { get; internal set; }
        public bool HasSubDirectory { get; internal set; }

        public readonly bool Equals(AddressInfoGetResult other)
        {
            if (this.DirectoryPath != other.DirectoryPath) { return false; }
            if (this.DirectoryList != other.DirectoryList) { return false; }
            if (this.HasSubDirectory != other.HasSubDirectory) { return false; }

            return true;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(AddressInfoGetResult))
            {
                return false;
            }

            return this.Equals((AddressInfoGetResult)obj);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.DirectoryPath, this.DirectoryList, this.HasSubDirectory);
        }

        public static bool operator ==(AddressInfoGetResult left, AddressInfoGetResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AddressInfoGetResult left, AddressInfoGetResult right)
        {
            return !(left == right);
        }
    }
}
