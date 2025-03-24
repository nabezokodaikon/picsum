using SWF.Core.Base;
using SWF.Core.Job;

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
            IsAscending = false,
            SelectedFilePath = string.Empty,
        };

        public string DirectoryPath { get; set; } = string.Empty;
        public SortTypeID SortTypeID { get; set; } = SortTypeID.FilePath;
        public bool IsAscending { get; set; } = false;
        public string SelectedFilePath { get; set; } = string.Empty;
    }
}
