namespace SWF.Core.ImageAccessor
{
    public interface IImageFileSizeCacher
        : IDisposable
    {
        public void Create(string filePath);
        public ImageFileSizeCacheEntity GetOrCreate(string filePath);
        public void Set(string filePath, Size size, DateTime updateDate);
        public void Set(string filePath, Size size);
    }
}
