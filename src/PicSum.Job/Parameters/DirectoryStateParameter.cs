using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// フォルダ状態
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
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
