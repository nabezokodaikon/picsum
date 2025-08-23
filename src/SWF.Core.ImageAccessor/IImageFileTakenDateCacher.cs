namespace SWF.Core.ImageAccessor
{
    public interface IImageFileTakenDateCacher
        : IDisposable
    {
        public ValueTask<DateTime> Get(string filePath);
        public ValueTask<DateTime> GetOrCreate(string filePath);
    }
}
