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
        public string? DirectoryPath { get; set; }
        public SortTypeID SortTypeID { get; set; }
        public bool IsAscending { get; set; }
        public string? SelectedFilePath { get; set; }
    }
}
