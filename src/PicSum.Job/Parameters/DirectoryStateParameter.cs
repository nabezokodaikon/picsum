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
        public static readonly DirectoryStateParameter EMPTY
            = new(string.Empty, FileSortMode.FilePath, false, string.Empty);

        public string DirectoryPath { get; private set; }
        public FileSortMode SortMode { get; private set; }
        public bool IsAscending { get; private set; }
        public string SelectedFilePath { get; private set; }

        public DirectoryStateParameter(
            string directoryPath,
            FileSortMode sortMode,
            bool isAscending,
            string selectedFilePath)
        {
            ArgumentNullException.ThrowIfNull(directoryPath, nameof(directoryPath));
            ArgumentNullException.ThrowIfNull(selectedFilePath, nameof(selectedFilePath));

            this.DirectoryPath = directoryPath;
            this.SortMode = sortMode;
            this.IsAscending = isAscending;
            this.SelectedFilePath = selectedFilePath;
        }
    }
}
