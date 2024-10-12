using SWF.Core.Job;
using SWF.Core.Base;

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

        public string? DirectoryPath { get; set; }
        public SortTypeID SortTypeID { get; set; }
        public bool IsAscending { get; set; }
        public string? SelectedFilePath { get; set; }

        public bool Equals(DirectoryStateParameter other)
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
    }
}
