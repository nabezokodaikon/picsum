using System;

namespace SWF.Common
{
    internal class ImageFileReadedTimeCache
    {
        public string FilePath { get; private set; }
        public long ReadedMilliseconds { get; private set; }

        public ImageFileReadedTimeCache(string filePath, long readedMilliseconds)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.ReadedMilliseconds = readedMilliseconds;
        }
    }
}
