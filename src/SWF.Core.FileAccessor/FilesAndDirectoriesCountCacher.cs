using SWF.Core.Job;

namespace SWF.Core.FileAccessor
{
    public sealed class FilesAndDirectoriesCountCacher
        : IFilesAndDirectoriesCountCacher
    {
        private const int CACHE_CAPACITY = 10000;

        private bool _disposed = false;
        private readonly Dictionary<string, FilesAndDirectoriesCountEntity> _cacheDictionary = new(CACHE_CAPACITY);
        private readonly SemaphoreSlim _cacheLock = new(1, 1);

        public FilesAndDirectoriesCountCacher()
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._cacheLock.Dispose();
            }

            this._disposed = true;
        }

        public async ValueTask<FilesAndDirectoriesCountEntity> GetOrCreate(string directoryPath)
        {
            if (FileUtil.IsExistsFile(directoryPath))
            {
                return FilesAndDirectoriesCountEntity.EMPTY;
            }

            await this._cacheLock.WaitAsync().False();
            try
            {
                if (this._cacheDictionary.TryGetValue(directoryPath, out var cache))
                {
                    var updateDate = FileUtil.GetUpdateDate(directoryPath);
                    if (cache.UpdateDate == updateDate)
                    {
                        return cache;
                    }

                    this._cacheDictionary.Remove(directoryPath);
                }
            }
            finally
            {
                this._cacheLock.Release();
            }

            var (filesCount, directoriesCount)
                = FileUtil.GetFilesAndDirectoriesCount(directoryPath);

            var newCache = new FilesAndDirectoriesCountEntity(
                directoryPath,
                filesCount,
                directoriesCount,
                FileUtil.GetUpdateDate(directoryPath));

            await this._cacheLock.WaitAsync().False();
            try
            {
                if (this._cacheDictionary.TryGetValue(directoryPath, out var cache))
                {
                    var updateDate = FileUtil.GetUpdateDate(directoryPath);
                    if (cache.UpdateDate == updateDate)
                    {
                        return cache;
                    }

                    this._cacheDictionary.Remove(directoryPath);
                }

                this._cacheDictionary.Add(directoryPath, newCache);

                return newCache;
            }
            finally
            {
                this._cacheLock.Release();
            }
        }
    }
}
