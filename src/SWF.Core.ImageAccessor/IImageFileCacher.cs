namespace SWF.Core.ImageAccessor
{
    public interface IImageFileCacher
        : IDisposable
    {
        public Size GetSize(string filePath);
        public CvImage GetCvImage(string filePath);
        public void Create(string filePath);
    }
}
