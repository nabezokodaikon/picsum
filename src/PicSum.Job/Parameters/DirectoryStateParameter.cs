using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// フォルダ状態
    /// </summary>
    public struct DirectoryStateParameter
        : IJobParameter, IEquatable<DirectoryStateParameter>
    {
        public static readonly DirectoryStateParameter EMPTY = new()
        {
            DirectoryPath = string.Empty,
            SortTypeID = SortTypeID.FilePath,
            IsAscending = true,
            SelectedFilePath = string.Empty,
        };

        public string DirectoryPath { get; set; }
        public SortTypeID SortTypeID { get; set; }
        public bool IsAscending { get; set; }
        public string SelectedFilePath { get; set; }

        public readonly bool Equals(DirectoryStateParameter other)
        {
            if (other.DirectoryPath != this.DirectoryPath)
            {
                return false;
            }

            if (other.SortTypeID != this.SortTypeID)
            {
                return false;
            }

            if (other.IsAscending != this.IsAscending)
            {
                return false;
            }

            if (other.SelectedFilePath != this.SelectedFilePath)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                this.DirectoryPath, this.SortTypeID, this.IsAscending, this.SelectedFilePath);
        }

        public override bool Equals(object? obj)
        {
            return obj is DirectoryStateParameter parameter && this.Equals(parameter);
        }

        public static bool operator ==(DirectoryStateParameter left, DirectoryStateParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DirectoryStateParameter left, DirectoryStateParameter right)
        {
            return !(left == right);
        }
    }
}
