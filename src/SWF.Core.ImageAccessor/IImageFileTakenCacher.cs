namespace SWF.Core.ImageAccessor
{
    public interface IImageFileTakenCacher
        : IDisposable
    {
        public DateTime Get(string filePath);
        public DateTime GetOrCreate(string filePath);
    }
}
