namespace SWF.Core.ImageAccessor
{
    public interface IImageFileCacher
        : IDisposable
    {
        public Size GetSize(string filePath);
        public CvImage GetCache(string filePath, float zoomValue);
        public void Create(string filePath);
    }
}
