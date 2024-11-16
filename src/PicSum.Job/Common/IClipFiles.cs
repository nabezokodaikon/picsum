using PicSum.Job.Entities;

namespace PicSum.Job.Common
{
    public interface IClipFiles
        : IDisposable
    {
        public void AddFiles(string[] files);
        public void RemoveFiles(string[] files);
        public ClipFileEntity[] GetFiles();
    }
}
