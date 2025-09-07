using SWF.Core.Base;

namespace SWF.Core.FileAccessor
{
    public sealed class FilesAndDirectoriesCountEntity
    {
        public static readonly FilesAndDirectoriesCountEntity EMPTY = new();

        public string DirectoryPath { get; private set; } = string.Empty;
        public int FilesCount { get; private set; } = 0;
        public int DirectoriesCount { get; private set; } = 0;
        public DateTime UpdateDate { get; private set; } = DateTimeExtensions.EMPTY;

        public bool IsEmpty
        {
            get
            {
                return this == EMPTY;
            }
        }

        public FilesAndDirectoriesCountEntity(
            string directoryPath, int filesCount, int directoriesCount, DateTime updateDate)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            this.DirectoryPath = directoryPath;
            this.FilesCount = filesCount;
            this.DirectoriesCount = directoriesCount;
            this.UpdateDate = updateDate;
        }

        private FilesAndDirectoriesCountEntity()
        {

        }
    }
}
