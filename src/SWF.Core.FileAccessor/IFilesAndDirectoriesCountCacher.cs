namespace SWF.Core.FileAccessor
{
    public interface IFilesAndDirectoriesCountCacher
        : IDisposable
    {
        public ValueTask<FilesAndDirectoriesCountEntity> GetOrCreate(string directoryPath);
    }
}
