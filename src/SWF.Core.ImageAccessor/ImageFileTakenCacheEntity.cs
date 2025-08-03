using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class ImageFileTakenCacheEntity
    {
        public static readonly ImageFileTakenCacheEntity EMPTY = new();

        public string FilePath { get; private set; }
        public DateTime TakenDate { get; private set; }
        public DateTime UpdateDate { get; private set; }

        private ImageFileTakenCacheEntity()
        {
            this.FilePath = string.Empty;
            this.TakenDate = FileUtil.EMPTY_DATETIME;
            this.UpdateDate = FileUtil.EMPTY_DATETIME;
        }

        public ImageFileTakenCacheEntity(string filePath, DateTime takenDate, DateTime updateDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.TakenDate = takenDate;
            this.UpdateDate = updateDate;
        }
    }
}
