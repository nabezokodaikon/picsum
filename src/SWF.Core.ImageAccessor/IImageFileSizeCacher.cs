namespace SWF.Core.ImageAccessor
{
    public interface IImageFileSizeCacher
        : IDisposable
    {
        public ValueTask Create(string filePath);
        public ValueTask<ImageFileSizeCacheEntity> GetOrCreate(string filePath);
        public ValueTask Set(string filePath, Size size, DateTime updateDate);
        public ValueTask Set(string filePath, Size size);
    }
}
