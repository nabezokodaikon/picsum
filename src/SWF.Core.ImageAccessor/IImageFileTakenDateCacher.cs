namespace SWF.Core.ImageAccessor
{
    public interface IImageFileTakenDateCacher
        : IDisposable
    {
        public DateTime Get(string filePath);
        public DateTime GetOrCreate(string filePath);
    }
}
