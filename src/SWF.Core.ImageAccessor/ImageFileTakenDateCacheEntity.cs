using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    internal sealed class ImageFileTakenDateCacheEntity
    {
        public static readonly ImageFileTakenDateCacheEntity EMPTY = new();

        public string FilePath { get; private set; }
        public DateTime TakenDate { get; private set; }
        public DateTime UpdateDate { get; private set; }

        private ImageFileTakenDateCacheEntity()
        {
            this.FilePath = string.Empty;
            this.TakenDate = DateTimeExtensions.EMPTY;
            this.UpdateDate = DateTimeExtensions.EMPTY;
        }

        public ImageFileTakenDateCacheEntity(string filePath, DateTime takenDate, DateTime updateDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.TakenDate = takenDate;
            this.UpdateDate = updateDate;
        }
    }
}
