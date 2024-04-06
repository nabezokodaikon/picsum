using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTaskV2;

namespace PicSum.Task.Parameter
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
