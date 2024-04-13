using PicSum.Core.Task.AsyncTaskV2;
using SWF.Common;

namespace PicSum.Task.Parameters
{
    /// <summary>
    /// フォルダ状態
    /// </summary>
    public sealed class DirectoryStateParameter
        : ITaskParameter
    {
        public string DirectoryPath { get; set; }
        public SortTypeID SortTypeID { get; set; }
        public bool IsAscending { get; set; }
        public string SelectedFilePath { get; set; }
    }
}
