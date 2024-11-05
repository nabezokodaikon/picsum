namespace SWF.Core.ImageAccessor
{
    public interface IImageFileSizeCacher
        : IDisposable
    {
        public void Create(string filePath);
        public ImageFileSizeCacheEntity Get(string filePath);
        public void Set(string filePath, Size size);
    }
}
