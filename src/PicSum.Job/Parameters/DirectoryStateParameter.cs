using PicSum.Core.Job.AsyncJob;
using SWF.Common;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// フォルダ状態
    /// </summary>
    public sealed class DirectoryStateParameter
        : IJobParameter
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
    }
}
