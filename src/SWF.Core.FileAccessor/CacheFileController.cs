using SWF.Core.Base;
using System.IO.MemoryMappedFiles;

namespace SWF.Core.FileAccessor
{

    public sealed class CacheFileController
        : IDisposable
    {
        private const string MAP_NAME = "thumbnail.cache";

        private static MemoryMappedFile CreateMappedFile(string cacheFilePath, int capacity)
        {
            return MemoryMappedFile.CreateFromFile(
                cacheFilePath,
                FileMode.Open,
                MAP_NAME,
                capacity,
                MemoryMappedFileAccess.ReadWrite);
        }

        private static MemoryMappedViewAccessor CreateViewAccessor(
            MemoryMappedFile mmf, int offset, int capacity)
        {
            return mmf.CreateViewAccessor(offset, capacity);
        }

        private bool _disposed = false;

        private readonly string _cacheFilePath;
        private readonly int _capacity;
        private int _position;
        private MemoryMappedFile _memoryMappedFile;
        private MemoryMappedViewAccessor _accessor;

        public CacheFileController(string cacheFilePath, int capacity, int position)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(cacheFilePath, nameof(cacheFilePath));

            this._cacheFilePath = cacheFilePath;
            this._capacity = capacity;
            this._position = position;

            if (!FileUtil.IsExistsFile(this._cacheFilePath))
            {
                var buffer = new byte[this._capacity];

                using (TimeMeasuring.Run(true, "CacheFileController.New Create cache file"))
                {
                    using (var fs = new FileStream(
                        this._cacheFilePath, FileMode.CreateNew, FileAccess.Write))
                    using (var bs = new BufferedStream(fs))
                    {
                        bs.Write(buffer, 0, buffer.Length);
                    }
                }
            }

            this._memoryMappedFile = CreateMappedFile(
                this._cacheFilePath, this._capacity);
            this._accessor = CreateViewAccessor(
                this._memoryMappedFile, 0, this._capacity);
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
                this._accessor.Dispose();
                this._memoryMappedFile.Dispose();
            }

            this._disposed = true;
        }

        public int Write(byte[] buffer)
        {
            ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));

            using (TimeMeasuring.Run(false, "CacheFileController.Write"))
            {
                var length = buffer.Length;

                if (this._position + length > this._capacity)
                {
                    this._accessor.Dispose();
                    this._memoryMappedFile.Dispose();

                    this._position = 0;

                    this._memoryMappedFile = CreateMappedFile(
                        this._cacheFilePath, this._capacity);
                    this._accessor = CreateViewAccessor(
                        this._memoryMappedFile, this._position, this._capacity);
                }

                this._accessor.WriteArray(this._position, buffer, 0, length);
                this._position += length;
                return this._position;
            }
        }

        public byte[] Read(int position, int length)
        {
            using (TimeMeasuring.Run(false, "CacheFileController.Read"))
            {
                var buffer = new byte[length];
                this._accessor.ReadArray(position, buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
