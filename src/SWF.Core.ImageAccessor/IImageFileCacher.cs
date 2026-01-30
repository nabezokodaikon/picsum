namespace SWF.Core.ImageAccessor
{
    public interface IImageFileCacher
        : IDisposable
    {
        public ValueTask<Size> GetSize(string filePath);
        public ValueTask<CvImage> GetCache(string filePath, float zoomValue);
        public ValueTask<SkiaImage> GetSKCache(string filePath, float zoomValue);
        public ValueTask Create(string filePath);
    }
}
